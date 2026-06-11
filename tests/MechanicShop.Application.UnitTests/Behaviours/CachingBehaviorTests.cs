using MechanicShop.Application.Common.Behavior;
using MechanicShop.Application.Common.Behaviours;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace MechanicShop.Application.UnitTests.Behaviours;

public class CachingBehaviorTests
{
    private readonly TestHybridCache _cache = new();
    private readonly ILogger<CachingBehavior<CachedQuery, Result<string>>> _logger = Substitute.For<ILogger<CachingBehavior<CachedQuery, Result<string>>>>();

    private readonly CachingBehavior<CachedQuery, Result<string>> _sut;

    public CachingBehaviorTests()
    {
        _sut = new CachingBehavior<CachedQuery, Result<string>>(_cache, _logger);
    }

    [Fact]
    public async Task Handle_WhenNotCachedQuery_ShouldSkipCacheAndReturnResult()
    {
        // Arrange
        var uncachedRequest = new NonCachedQuery();
        var behavior = new CachingBehavior<NonCachedQuery, string>(_cache, Substitute.For<ILogger<CachingBehavior<NonCachedQuery, string>>>());

        // Act
        var result = await behavior.Handle(uncachedRequest, _ => Task.FromResult("OK"), CancellationToken.None);

        // Assert
        Assert.Equal("OK", result);
        Assert.False(_cache.GetOrCreateCalled);
    }

    [Fact]
    public async Task Handle_WhenCachedQueryAndResultIsSuccess_ShouldCacheResult()
    {
        // Arrange
        var request = new CachedQuery();
        var response = (Result<string>)"test-value";

        // Act
        var result = await _sut.Handle(request, _ => Task.FromResult(response), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(_cache.GetOrCreateCalled);
        Assert.Equal(request.CacheKey, _cache.Key);

        var typed = Assert.IsType<Result<string>>(_cache.CreatedValue);
        Assert.True(typed.IsSuccess);
        Assert.Equal("test-value", typed.Value);

        Assert.Equal(request.Expiration, _cache.Options!.Expiration);
        Assert.Equal(request.Tags, _cache.Tags);
    }

    [Fact]
    public async Task Handle_WhenCachedQueryAndResultIsError_ShouldNotCacheResult()
    {
        // Arrange
        var request = new CachedQuery();
        var errorResult = (Result<string>)Error.Validation("code", "message");

        // Act
        var result = await _sut.Handle(request, _ => Task.FromResult(errorResult), CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.True(_cache.GetOrCreateCalled);
        Assert.Null(_cache.CreatedValue);
    }

    public class NonCachedQuery;

    public class CachedQuery : ICachedQuery
    {
        public string CacheKey => "test-key";
        public TimeSpan Expiration => TimeSpan.FromMinutes(5);
        public string[] Tags => ["unit-test"];
    }

    private sealed class TestHybridCache : HybridCache
    {
        public bool GetOrCreateCalled { get; private set; }
        public string? Key { get; private set; }
        public object? CreatedValue { get; private set; }
        public HybridCacheEntryOptions? Options { get; private set; }
        public IEnumerable<string>? Tags { get; private set; }

        public override async ValueTask<T> GetOrCreateAsync<TState, T>(
            string key,
            TState state,
            Func<TState, CancellationToken, ValueTask<T>> factory,
            HybridCacheEntryOptions? options = null,
            IEnumerable<string>? tags = null,
            CancellationToken cancellationToken = default)
        {
            GetOrCreateCalled = true;
            Key = key;
            Options = options;
            Tags = tags;

            var result = await factory(state, cancellationToken);
            CreatedValue = result;

            return result;
        }

        public override ValueTask SetAsync<T>(
            string key,
            T value,
            HybridCacheEntryOptions? options = null,
            IEnumerable<string>? tags = null,
            CancellationToken cancellationToken = default)
        {
            Key = key;
            CreatedValue = value;
            Options = options;
            Tags = tags;

            return ValueTask.CompletedTask;
        }

        public override ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }

        public override ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }
}
