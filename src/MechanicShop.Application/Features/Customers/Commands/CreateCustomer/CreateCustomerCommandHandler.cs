using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler(IAppDbContext context, ILogger<CreateCustomerCommandHandler> logger, HybridCache cache)
: IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;


    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLower();

        var exists = await _context.Customers.AnyAsync(c => c.Email!.ToLower() == email, ct);

        if (exists)
        {
            _logger.LogWarning("Customer creation aborted. Email already exists.");

            return CustomerErrors.CustomerExists;
        }

        List<Vehicle> vehicles = [];

        foreach (var v in request.Vehicles)
        {
            var VehicleResult = Vehicle.Create(Guid.NewGuid(), v.Make, v.Model, v.Year, v.LicensePlate);

            if (VehicleResult.IsError)
                return VehicleResult.Errors;

            vehicles.Add(VehicleResult.Value);
        }

        var createCustomerResult = Customer.Create(Guid.NewGuid(), request.Name, request.PhoneNumber, email, vehicles);

        if (createCustomerResult.IsError)
            return createCustomerResult.Errors;

        await _context.Customers.AddAsync(createCustomerResult.Value, ct);

        await _context.SaveChangesAsync(ct);

        var customer = createCustomerResult.Value;

        _logger.LogInformation("Customer created successfully with ID: {CustomerId}", customer.Id);

        await _cache.RemoveByTagAsync("customer", ct);

        return customer.ToDto();
    }

}