using MechanicShop.Application.Features.Customers.Dtos;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

// IRequest mean This command will be sent through MediatR and returns a CustomerDto.

public sealed record CreateVehicleCommand(string Make, string Model, int Year, string LicensePlate) : IRequest<IResult<VehicleDto>>;