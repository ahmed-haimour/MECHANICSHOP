using System.Net;
using System.Net.Http.Json;

using MechanicShop.Api.IntegrationTests.Common;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Contracts.Requests.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Tests.Common.Security;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace MechanicShop.Api.IntegrationTests.Controllers;

[Collection(WebAppFactoryCollection.CollectionName)]
public class CustomerVehiclesControllerTests(WebAppFactory webAppFactory)
{
    private readonly AppHttpClient _client = webAppFactory.CreateAppHttpClient();
    private readonly IAppDbContext _context = webAppFactory.CreateAppDbContext();

    [Fact]
    public async Task CreateCustomer_WithVehicle_ShouldReturnCreatedVehicle()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var vehicle = VehicleTestDataBuilder.Create()
            .WithMake("Honda")
            .WithModel("Accord")
            .WithYear(2021)
            .Build();

        var request = CreateCustomerRequest(vehicle);

        CustomerDto? dto = null;

        try
        {
            var response = await _client.PostAsJsonAsync("/api/v1.0/customers", request);

            dto = await response.Content.ReadFromJsonAsync<CustomerDto>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(dto);

            var createdVehicle = Assert.Single(dto!.Vehicles);
            Assert.Equal(vehicle.Make, createdVehicle.Make);
            Assert.Equal(vehicle.Model, createdVehicle.Model);
            Assert.Equal(vehicle.Year, createdVehicle.Year);
            Assert.Equal(vehicle.LicensePlate, createdVehicle.LicensePlate);
        }
        finally
        {
            if (dto is not null)
            {
                await DeleteCustomerAsync(dto.CustomerId);
            }
        }
    }

    [Fact]
    public async Task UpdateCustomer_WithExistingVehicle_ShouldUpdateVehicle()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var customer = CustomerTestDataBuilder.Create()
            .WithVehicles(VehicleTestDataBuilder.Create().Build())
            .Build();

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(default);

        var vehicle = customer.Vehicles.Single();

        var request = CreateUpdateCustomerRequest(
            new UpdateVehicleRequest
            {
                VehicleId = vehicle.Id,
                Make = "Mazda",
                Model = "CX-5",
                Year = 2022,
                LicensePlate = $"UPD-{Guid.NewGuid():N}"[..10]
            });

        try
        {
            var response = await _client.PutAsJsonAsync($"/api/v1.0/customers/{customer.Id}", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedVehicle = await _context.Vehicles
                .AsNoTracking()
                .SingleAsync(v => v.Id == vehicle.Id);

            Assert.Equal(request.Vehicles[0].Make, updatedVehicle.Make);
            Assert.Equal(request.Vehicles[0].Model, updatedVehicle.Model);
            Assert.Equal(request.Vehicles[0].Year, updatedVehicle.Year);
            Assert.Equal(request.Vehicles[0].LicensePlate, updatedVehicle.LicensePlate);
        }
        finally
        {
            await DeleteCustomerAsync(customer.Id);
        }
    }

    [Fact]
    public async Task UpdateCustomer_WithNewVehicle_ShouldAddVehicle()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var customer = CustomerTestDataBuilder.Create()
            .WithVehicles(VehicleTestDataBuilder.Create().Build())
            .Build();

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(default);

        var existingVehicle = customer.Vehicles.Single();

        var request = CreateUpdateCustomerRequest(
            new UpdateVehicleRequest
            {
                VehicleId = existingVehicle.Id,
                Make = existingVehicle.Make!,
                Model = existingVehicle.Model!,
                Year = existingVehicle.Year,
                LicensePlate = existingVehicle.LicensePlate!
            },
            new UpdateVehicleRequest
            {
                Make = "Ford",
                Model = "Focus",
                Year = 2023,
                LicensePlate = $"NEW-{Guid.NewGuid():N}"[..10]
            });

        try
        {
            var response = await _client.PutAsJsonAsync($"/api/v1.0/customers/{customer.Id}", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var vehicleCount = await _context.Vehicles.CountAsync(v => v.CustomerId == customer.Id);

            Assert.Equal(2, vehicleCount);
        }
        finally
        {
            await DeleteCustomerAsync(customer.Id);
        }
    }

    [Fact]
    public async Task UpdateCustomer_WithRemovedVehicle_ShouldDeleteVehicle()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var vehicleToKeep = VehicleTestDataBuilder.Create().Build();
        var vehicleToRemove = VehicleTestDataBuilder.Create()
            .WithLicensePlate($"REM-{Guid.NewGuid():N}"[..10])
            .Build();

        var customer = CustomerTestDataBuilder.Create()
            .WithVehicles(vehicleToKeep, vehicleToRemove)
            .Build();

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(default);

        var request = CreateUpdateCustomerRequest(
            new UpdateVehicleRequest
            {
                VehicleId = vehicleToKeep.Id,
                Make = vehicleToKeep.Make!,
                Model = vehicleToKeep.Model!,
                Year = vehicleToKeep.Year,
                LicensePlate = vehicleToKeep.LicensePlate!
            });

        try
        {
            var response = await _client.PutAsJsonAsync($"/api/v1.0/customers/{customer.Id}", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var vehicles = await _context.Vehicles
                .AsNoTracking()
                .Where(v => v.CustomerId == customer.Id)
                .ToListAsync();

            var remainingVehicle = Assert.Single(vehicles);
            Assert.Equal(vehicleToKeep.Id, remainingVehicle.Id);
        }
        finally
        {
            await DeleteCustomerAsync(customer.Id);
        }
    }

    [Fact]
    public async Task UpdateCustomer_WithInvalidVehicle_ShouldReturnBadRequest()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var customer = CustomerTestDataBuilder.Create().Build();

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(default);

        var vehicle = customer.Vehicles.Single();

        var request = CreateUpdateCustomerRequest(
            new UpdateVehicleRequest
            {
                VehicleId = vehicle.Id,
                Make = string.Empty,
                Model = string.Empty,
                Year = 1800,
                LicensePlate = string.Empty
            });

        try
        {
            var response = await _client.PutAsJsonAsync($"/api/v1.0/customers/{customer.Id}", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await DeleteCustomerAsync(customer.Id);
        }
    }

    private static CreateCustomerRequest CreateCustomerRequest(Vehicle vehicle)
    {
        return new CreateCustomerRequest
        {
            Name = "Vehicle Customer",
            PhoneNumber = "15551234567",
            Email = $"vehicle-customer-{Guid.NewGuid():N}@example.com",
            Vehicles =
            [
                new CreateVehicleRequest
                {
                    Make = vehicle.Make!,
                    Model = vehicle.Model!,
                    Year = vehicle.Year,
                    LicensePlate = vehicle.LicensePlate!
                }
            ]
        };
    }

    private static UpdateCustomerRequest CreateUpdateCustomerRequest(params UpdateVehicleRequest[] vehicles)
    {
        return new UpdateCustomerRequest
        {
            Name = "Updated Vehicle Customer",
            PhoneNumber = "15557654321",
            Email = $"updated-vehicle-customer-{Guid.NewGuid():N}@example.com",
            Vehicles = [.. vehicles]
        };
    }

    private async Task DeleteCustomerAsync(Guid customerId)
    {
        await _context.Vehicles
            .Where(v => v.CustomerId == customerId)
            .ExecuteDeleteAsync();

        await _context.Customers
            .Where(c => c.Id == customerId)
            .ExecuteDeleteAsync();
    }
}
