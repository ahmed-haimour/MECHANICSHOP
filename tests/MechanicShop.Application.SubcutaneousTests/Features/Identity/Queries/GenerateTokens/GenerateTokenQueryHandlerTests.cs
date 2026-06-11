using System.Security.Claims;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Application.Features.Identity.Queries.GenerateTokens;
using MechanicShop.Domain.Common.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Identity.Queries.GenerateTokens;

public class GenerateTokenQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ShouldSucceed()
    {
        var tokenResponse = new TokenResponse
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresOnUtc = DateTime.UtcNow.AddHours(1)
        };

        var handler = new GenerateTokenQueryHandler(
            NullLogger<GenerateTokenQueryHandler>.Instance,
            new IdentityServiceStub(new AppUserDto("user-id", "user@example.com", [], [])),
            new TokenProviderStub(tokenResponse));

        var result = await handler.Handle(new GenerateTokenQuery("user@example.com", "password"), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("access-token", result.Value.AccessToken);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ShouldFail()
    {
        var handler = new GenerateTokenQueryHandler(
            NullLogger<GenerateTokenQueryHandler>.Instance,
            new IdentityServiceStub(Error.Unauthorized("Identity_InvalidCredentials", "Invalid credentials.")),
            new TokenProviderStub(new TokenResponse()));

        var result = await handler.Handle(new GenerateTokenQuery("user@example.com", "bad-password"), default);

        Assert.True(result.IsError);
    }

    private sealed class IdentityServiceStub(Result<AppUserDto> authenticateResult) : IIdentityService
    {
        public Task<Result<AppUserDto>> AuthenticateAsync(string email, string password) => Task.FromResult(authenticateResult);
        public Task<bool> AuthorizeAsync(string userId, string? policyName) => Task.FromResult(false);
        public Task<Result<AppUserDto>> GetUserByIdAsync(string userId) => Task.FromResult(authenticateResult);
        public Task<string?> GetUserNameAsync(string userId) => Task.FromResult<string?>(null);
        public Task<bool> IsInRoleAsync(string userId, string role) => Task.FromResult(false);
    }

    private sealed class TokenProviderStub(Result<TokenResponse> tokenResult) : ITokenProvider
    {
        public Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default) => Task.FromResult(tokenResult);
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token) => null;
    }
}
