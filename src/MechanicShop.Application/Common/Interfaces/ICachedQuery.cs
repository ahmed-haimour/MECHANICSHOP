using MediatR;

public interface ICachedQuery
{
    public string CacheKey { get; }

    public string[] Tags { get; } 

    TimeSpan Expiration { get; }
}

public interface ICachedQuery<TRequest> : IRequest<TRequest>, ICachedQuery;