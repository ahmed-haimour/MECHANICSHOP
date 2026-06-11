using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler(IAppDbContext context, ILogger<DeleteCustomerCommandHandler> logger, HybridCache cache)
: IRequestHandler<DeleteCustomerCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Deleted>> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await _context.Customers.FindAsync([request.CustomerId], ct);

        if (customer is null)
        {
            _logger.LogWarning("Customer with Id {CustomerId} not found for deletion", request.CustomerId);
            return ApplicationErrors.CustomerNotFound;
        }


        _context.Customers.Remove(customer);

        await _context.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}