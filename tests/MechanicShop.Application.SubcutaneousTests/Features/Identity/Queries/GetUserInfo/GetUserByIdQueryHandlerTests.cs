using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Application.Features.Identity.Queries.GetUserInfo;
using MechanicShop.Domain.Common.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Identity.Queries.GetUserInfo;

public class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingUser_ShouldSucceed()
    {
        var user = new AppUserDto("user-id", "user@example.com", [], []);
        var handler = new GetUserByIdQueryHanlder(
            NullLogger<GetUserByIdQueryHanlder>.Instance,
            new IdentityServiceStub(user));

        var result = await handler.Handle(new GetUserByIdQuery("user-id"), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("user-id", result.Value.UserId);
    }

    [Fact]
    public async Task Handle_WithMissingUser_ShouldFail()
    {
        var handler = new GetUserByIdQueryHanlder(
            NullLogger<GetUserByIdQueryHanlder>.Instance,
            new IdentityServiceStub(Error.NotFound("User_NotFound", "User was not found.")));

        var result = await handler.Handle(new GetUserByIdQuery("missing-user-id"), default);

        Assert.True(result.IsError);
    }

    private sealed class IdentityServiceStub(Result<AppUserDto> getUserResult) : IIdentityService
    {
        public Task<Result<AppUserDto>> AuthenticateAsync(string email, string password) => Task.FromResult(getUserResult);
        public Task<bool> AuthorizeAsync(string userId, string? policyName) => Task.FromResult(false);
        public Task<Result<AppUserDto>> GetUserByIdAsync(string userId) => Task.FromResult(getUserResult);
        public Task<string?> GetUserNameAsync(string userId) => Task.FromResult<string?>(null);
        public Task<bool> IsInRoleAsync(string userId, string role) => Task.FromResult(false);
    }
}
