namespace MechanicShop.Application.Features.Customers.Dtos;

public sealed record class VehicleDto(Guid VehicleId, string Make, string Model, int Year, string LicensePlate);