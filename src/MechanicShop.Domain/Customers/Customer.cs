namespace MechanicShop.Domain.Customers;

using System.Text.RegularExpressions;
using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;

public sealed class Customer : AuditableEntity
{

    public string? Name { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }

    private readonly List<Vehicle> _vehicles = [];
    public IEnumerable<Vehicle> Vehicles => _vehicles.AsReadOnly();

    private Customer() { }

    private Customer(Guid id, string name, string phoneNumber, string? email, List<Vehicle> vehicles) : base(id)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        _vehicles = vehicles;
    }

    public static Result<Customer> Create(Guid id, string name, string phoneNumber, string? email, List<Vehicle> vehicles)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CustomerErrors.NameRequired;

        if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\+?\d{7,15}$"))
            return CustomerErrors.PhoneNumberRequired;

        if (string.IsNullOrWhiteSpace(email))
            return CustomerErrors.EmailRequired;

        // try
        // {
        //     _ = new MailAddress(email);
        // }
        // catch (FormatException)
        // {
        //     return CustomerErrors.EmailInvalid;
        // }

        return new Customer(id, name, phoneNumber, email, vehicles);
    }

    public Result<Updated> Update(string name, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CustomerErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return CustomerErrors.EmailRequired;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\+?\d{7,15}$"))
        {
            return CustomerErrors.InvalidPhoneNumber;
        }

        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;

        return Result.Updated;
    }

    //     Current vehicles: [Car1, Car2, Car3]
    // Incoming vehicles: [Car1, Car3, Car4]

    // Checking Car1:
    //   - Is Car1 != Car1? NO
    //   - Is Car3 != Car1? YES  
    //   - Is Car4 != Car1? YES
    //   → NOT all different → KEEP Car1 ✓ this return false because not all much so will not deleted  car 1

    // Checking Car2:
    //   - Is Car1 != Car2? YES
    //   - Is Car3 != Car2? YES
    //   - Is Car4 != Car2? YES
    //   → ALL different (no match found) → REMOVE Car2 ✗ ** this return true because all much so car 2 will deleted

    // Checking Car3:
    //   - Is Car1 != Car3? YES
    //   - Is Car3 != Car3? NO
    //   - Is Car4 != Car3? YES
    //   → NOT all different → KEEP Car3 ✓

    //Result: Car2 is deleted, [Car1, Car3] remain
    public Result<Updated> UpsertParts(List<Vehicle> incomingVehicle)
    {
        _vehicles.RemoveAll(existing => incomingVehicle.All(v => v.Id != existing.Id));

        foreach (var incoming in incomingVehicle)
        {
            var existing = _vehicles.FirstOrDefault(v => v.Id == incoming.Id);
            if (existing is null)
            {
                _vehicles.Add(incoming);
            }
            else
            {
                var updateVehicleResult = existing.Update(incoming.Make, incoming.Model, incoming.Year, incoming.LicensePlate);

                if (updateVehicleResult.IsError)
                {
                    return updateVehicleResult.Errors;
                }
            }
        }

        return Result.Updated;
    }
}
