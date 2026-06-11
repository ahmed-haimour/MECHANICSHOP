using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;

// This query:
//     - is a MediatR request
//     - returns Result<List<CustomerDto>>
//     - supports caching

public sealed record GetCustomersQuery : ICachedQuery<Result<List<CustomerDto>>>
{
    public string CacheKey => "customers";

    public string[] Tags => ["customer"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}