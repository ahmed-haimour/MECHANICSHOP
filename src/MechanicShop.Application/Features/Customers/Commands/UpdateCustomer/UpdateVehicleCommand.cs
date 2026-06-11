using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

// IRequest mean This command will be sent through MediatR and returns a CustomerDto.

public sealed record UpdateVehicleCommand(Guid? VehicleId, string Make, string Model, int Year, string LicensePlate) : IRequest<Result<Updated>>;