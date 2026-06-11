using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Customers;
using MechanicShop.Tests.Common.Customers;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;

public class CustomerMapperTest
{
    [Fact]
    public void ToDto_ShouldMapCorrectly()
    {
        var vehicle = VehicleFactory.CreateVehicle(
            make: "Toyota",
            model: "Camry",
            year: 2023,
            licensePlate: "XYZ 789").Value;

        var customer = CustomerFactory.CreateCustomer(
            name: "Jane Customer",
            phoneNumber: "5551234567",
            email: "jane@localhost",
            vehicles: [vehicle]).Value;

        var dto = customer.ToDto();

        Assert.Equal(customer.Id, dto.CustomerId);
        Assert.Equal(customer.Name, dto.Name);
        Assert.Equal(customer.PhoneNumber, dto.PhoneNumber);
        Assert.Equal(customer.Email, dto.Email);

        Assert.Single(dto.Vehicles);
        Assert.Equal(vehicle.Id, dto.Vehicles[0].VehicleId);
        Assert.Equal(vehicle.Make, dto.Vehicles[0].Make);
        Assert.Equal(vehicle.Model, dto.Vehicles[0].Model);
        Assert.Equal(vehicle.Year, dto.Vehicles[0].Year);
        Assert.Equal(vehicle.LicensePlate, dto.Vehicles[0].LicensePlate);
    }

    [Fact]
    public void ToDtos_ShouldMapListCorrectly()
    {
        var customer = CustomerFactory.CreateCustomer(
            name: "Jane Customer",
            phoneNumber: "5551234567",
            email: "jane@localhost").Value;

        var customers = new List<Customer> { customer };

        var dtos = customers.ToDtos();

        Assert.Single(dtos);
        var dto = dtos[0];

        Assert.Equal(customer.Id, dto.CustomerId);
        Assert.Equal(customer.Name, dto.Name);
        Assert.Equal(customer.PhoneNumber, dto.PhoneNumber);
        Assert.Equal(customer.Email, dto.Email);
        Assert.Equal(customer.Vehicles.Count(), dto.Vehicles.Count);
    }
}
