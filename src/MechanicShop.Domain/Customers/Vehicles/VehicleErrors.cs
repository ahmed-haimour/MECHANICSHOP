using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers.Vehicles;

public static class VehicleErrors
{
    public static Error CustomerIdRequired => Error.Validation("Vehicle_CustomerId_Required", "Customer Id is required.");

    public static Error LicensePlateRequired => Error.Validation("Vehicle_LicensePlate_Required", "License plate is required.");

    public static Error MakeRequired => Error.Validation("Vehicle_Make_Required", "Vehicle make is required.");

    public static Error ModelRequired => Error.Validation("Vehicle_Model_Required", "Vehicle model is required.");

    public static Error InvalidYear => Error.Validation("Vehicle_Invalid_Year", "Vehicle year is invalid.");

}