using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behavior;

// TRequest The incoming MediatR request/query/command
// TResponse The response from the request/query/command
public class CachingBehavior<TRequest, TResponse>(HybridCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
: IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly HybridCache _cache = cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICachedQuery cachedRequest) // If request DOES NOT implement ICachedQuery then skip caching
            return await next(cancellationToken);

        _logger.LogInformation("Checking cache for {RequestName}", typeof(TRequest).Name);

        // GetOrCreateAsync mean => Get cached value if exists, otherwise create it and cache it. 

        var hasUncachedResult = false;
        var uncachedResult = default(TResponse);

        var result = await _cache.GetOrCreateAsync(
            key: cachedRequest.CacheKey,
            // (factory) This function runs ONLY when cache does NOT exist.
            factory: async ct =>
            {
                var innerResult = await next(ct);
                if (innerResult is IResult r && r.IsSuccess)
                {
                    return innerResult;
                }

                // Don't cache failed results
                hasUncachedResult = true;
                uncachedResult = innerResult;
                return default!;
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = cachedRequest.Expiration
            },
            tags: cachedRequest.Tags,
            cancellationToken: cancellationToken
        );

        return hasUncachedResult ? uncachedResult! : result;
    }
}
