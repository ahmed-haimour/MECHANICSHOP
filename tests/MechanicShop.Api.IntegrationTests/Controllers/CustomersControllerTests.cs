using System.Net;
using System.Net.Http.Json;

using MechanicShop.Api.IntegrationTests.Common;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Contracts.Requests.Customers;
using MechanicShop.Tests.Common.Security;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace MechanicShop.Api.IntegrationTests.Controllers;

[Collection(WebAppFactoryCollection.CollectionName)]
public class CustomersControllerTests(WebAppFactory webAppFactory)
{
    private readonly AppHttpClient _client = webAppFactory.CreateAppHttpClient();
    private readonly IAppDbContext _context = webAppFactory.CreateAppDbContext();

    [Fact]
    public async Task GetCustomers_WithAuthentication_ShouldReturnCustomers()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var response = await _client.GetAsync("/api/v1.0/customers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCustomers_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1.0/customers");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomerById_WithValidId_ShouldReturnCustomer()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var customer = CustomerTestDataBuilder.Create().Build();

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(default);

        try
        {
            var response = await _client.GetAsync($"/api/v1.0/customers/{customer.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<CustomerDto>();

            Assert.NotNull(result);
            Assert.Equal(customer.Id, result!.CustomerId);
            Assert.Equal(customer.Name, result.Name);
        }
        finally
        {
            await DeleteCustomerAsync(customer.Id);
        }
    }

    [Fact]
    public async Task GetCustomerById_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var response = await _client.GetAsync($"/api/v1.0/customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomerById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync($"/api/v1.0/customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithValidRequest_ShouldCreateCustomer()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var request = CreateValidCustomerRequest();

        CustomerDto? dto = null;

        try
        {
            var response = await _client.PostAsJsonAsync("/api/v1.0/customers", request);

            dto = await response.Content.ReadFromJsonAsync<CustomerDto>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(dto);
            Assert.Equal(request.Name, dto!.Name);
            Assert.Single(dto.Vehicles);
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
    public async Task CreateCustomer_WithoutManagerRole_ShouldReturnForbidden()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Labor01);

        _client.SetAuthorizationHeader(token);

        var response = await _client.PostAsJsonAsync("/api/v1.0/customers", CreateValidCustomerRequest());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidRequest_ShouldReturnBadRequest()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var request = new CreateCustomerRequest
        {
            Name = string.Empty,
            PhoneNumber = "invalid-phone",
            Email = "invalid-email",
            Vehicles = []
        };

        var response = await _client.PostAsJsonAsync("/api/v1.0/customers", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCustomer_WithValidRequest_ShouldUpdateCustomer()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var customer = CustomerTestDataBuilder.Create().Build();

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(default);

        var vehicle = customer.Vehicles.First();

        var request = CreateValidUpdateCustomerRequest(vehicle.Id);

        try
        {
            var response = await _client.PutAsJsonAsync($"/api/v1.0/customers/{customer.Id}", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            await DeleteCustomerAsync(customer.Id);
        }
    }

    [Fact]
    public async Task UpdateCustomer_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var response = await _client.PutAsJsonAsync(
            $"/api/v1.0/customers/{Guid.NewGuid()}",
            CreateValidUpdateCustomerRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCustomer_WithoutManagerRole_ShouldReturnForbidden()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Labor01);

        _client.SetAuthorizationHeader(token);

        var response = await _client.PutAsJsonAsync(
            $"/api/v1.0/customers/{Guid.NewGuid()}",
            CreateValidUpdateCustomerRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCustomer_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.PutAsJsonAsync(
            $"/api/v1.0/customers/{Guid.NewGuid()}",
            CreateValidUpdateCustomerRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCustomer_WithValidId_ShouldDeleteCustomer()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var customer = CustomerTestDataBuilder.Create().Build();

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(default);

        try
        {
            var response = await _client.DeleteAsync($"/api/v1.0/customers/{customer.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        finally
        {
            await DeleteCustomerAsync(customer.Id);
        }
    }

    [Fact]
    public async Task DeleteCustomer_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var response = await _client.DeleteAsync($"/api/v1.0/customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCustomer_WithoutManagerRole_ShouldReturnForbidden()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Labor01);

        _client.SetAuthorizationHeader(token);

        var response = await _client.DeleteAsync($"/api/v1.0/customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCustomer_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.DeleteAsync($"/api/v1.0/customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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

    private static CreateCustomerRequest CreateValidCustomerRequest()
    {
        return new CreateCustomerRequest
        {
            Name = "Integration Customer",
            PhoneNumber = "15551234567",
            Email = $"integration-{Guid.NewGuid():N}@example.com",
            Vehicles =
            [
                new CreateVehicleRequest
                {
                    Make = "Honda",
                    Model = "Civic",
                    Year = 2021,
                    LicensePlate = $"INT-{Guid.NewGuid():N}"[..10]
                }
            ]
        };
    }

    private static UpdateCustomerRequest CreateValidUpdateCustomerRequest(Guid vehicleId)
    {
        return new UpdateCustomerRequest
        {
            Name = "Updated Customer",
            PhoneNumber = "15557654321",
            Email = $"updated-{Guid.NewGuid():N}@example.com",
            Vehicles =
            [
                new UpdateVehicleRequest
                {
                    VehicleId = vehicleId,
                    Make = "Toyota",
                    Model = "Corolla",
                    Year = 2022,
                    LicensePlate = $"UPD-{Guid.NewGuid():N}"[..10]
                }
            ]
        };
    }
}
