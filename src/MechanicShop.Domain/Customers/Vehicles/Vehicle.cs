using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers.Vehicles;

public sealed class Vehicle : AuditableEntity
{
    public Guid CustomerId { get; }
    public Customer? Customer { get; private set; }
    public string? LicensePlate { get; private set; }
    public string? Make { get; private set; }
    public string? Model { get; private set; }
    public string? VehicleInfo { get; private set; }
    public int Year { get; private set; }

    private Vehicle(Guid Id, string? make, string? model, int year, string? licensePlate) : base(Id)
    {
        Make = make;
        Model = model;
        Year = year;
        LicensePlate = licensePlate;
    }

    public static Result<Vehicle> Create(Guid id, string? make, string? model, int year, string? licensePlate)
    {
        // if (customerId == Guid.Empty)
        //     return VehicleErrors.CustomerIdRequired;

        if (string.IsNullOrWhiteSpace(make))
            return VehicleErrors.MakeRequired;

        if (string.IsNullOrWhiteSpace(model))
            return VehicleErrors.ModelRequired;

        if (string.IsNullOrWhiteSpace(licensePlate))
            return VehicleErrors.LicensePlateRequired;

        if (year < 1886 || year > DateTime.UtcNow.Year)
            return VehicleErrors.InvalidYear;

        return new Vehicle(id, make, model, year, licensePlate);
    }

    public Result<Updated> Update(string? make, string? model, int year, string? licensePlate)
    {
        if (string.IsNullOrWhiteSpace(make))
            return VehicleErrors.MakeRequired;

        if (string.IsNullOrWhiteSpace(model))
            return VehicleErrors.ModelRequired;

        if (string.IsNullOrWhiteSpace(licensePlate))
            return VehicleErrors.LicensePlateRequired;

        if (year < 1886 || year > DateTime.UtcNow.Year)
            return VehicleErrors.InvalidYear;

        Make = make;
        Model = model;
        Year = year;
        LicensePlate = licensePlate;

        return Result.Updated;
    }

}