using System.Net;
using System.Net.Http.Json;

using MechanicShop.Api.IntegrationTests.Common;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Contracts.Requests.RepairTasks;
using MechanicShop.Tests.Common.Security;

using Microsoft.EntityFrameworkCore;

using Xunit;

using RepairDurationInMinutes = MechanicShop.Contracts.Common.RepairDurationInMinutes;

namespace MechanicShop.Api.IntegrationTests.Controllers;

[Collection(WebAppFactoryCollection.CollectionName)]
public class RepairTasksControllerTests(WebAppFactory webAppFactory)
{
    private readonly AppHttpClient _client = webAppFactory.CreateAppHttpClient();
    private readonly IAppDbContext _context = webAppFactory.CreateAppDbContext();

    [Fact]
    public async Task GetRepairTasks_WithAuthentication_ShouldReturnRepairTasks()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var response = await _client.GetAsync("/api/v1.0/repair-tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<RepairTaskDto>>();

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetRepairTasks_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1.0/repair-tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRepairTaskById_WithValidId_ShouldReturnRepairTask()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var repairTask = RepairTaskTestDataBuilder.Create().Build();

        _context.RepairTasks.Add(repairTask);
        await _context.SaveChangesAsync(default);

        try
        {
            var response = await _client.GetAsync($"/api/v1.0/repair-tasks/{repairTask.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<RepairTaskDto>();

            Assert.NotNull(result);
            Assert.Equal(repairTask.Id, result!.RepairTaskId);
            Assert.Equal(repairTask.Name, result.Name);
        }
        finally
        {
            await DeleteRepairTaskAsync(repairTask.Id);
        }
    }

    [Fact]
    public async Task GetRepairTaskById_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var response = await _client.GetAsync($"/api/v1.0/repair-tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetRepairTaskById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync($"/api/v1.0/repair-tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateRepairTask_WithValidRequest_ShouldCreateRepairTask()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var request = CreateValidRepairTaskRequest();

        RepairTaskDto? dto = null;

        try
        {
            var response = await _client.PostAsJsonAsync("/api/v1.0/repair-tasks", request);

            dto = await response.Content.ReadFromJsonAsync<RepairTaskDto>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(dto);
            Assert.Equal(request.Name, dto!.Name);
            Assert.Single(dto.Parts);
        }
        finally
        {
            if (dto is not null)
            {
                await DeleteRepairTaskAsync(dto.RepairTaskId);
            }
        }
    }

    [Fact]
    public async Task CreateRepairTask_WithInvalidRequest_ShouldReturnBadRequest()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var request = new CreateRepairTaskRequest
        {
            Name = string.Empty,
            LaborCost = 0,
            EstimatedDurationInMins = null,
            Parts = []
        };

        var response = await _client.PostAsJsonAsync("/api/v1.0/repair-tasks", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRepairTask_WithoutManagerRole_ShouldReturnForbidden()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Labor01);

        _client.SetAuthorizationHeader(token);

        var response = await _client.PostAsJsonAsync("/api/v1.0/repair-tasks", CreateValidRepairTaskRequest());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRepairTask_WithValidRequest_ShouldUpdateRepairTask()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var repairTask = RepairTaskTestDataBuilder.Create().Build();

        _context.RepairTasks.Add(repairTask);
        await _context.SaveChangesAsync(default);

        var part = repairTask.Parts.Single();
        var request = CreateValidUpdateRepairTaskRequest(part.Id);

        try
        {
            var response = await _client.PutAsJsonAsync($"/api/v1.0/repair-tasks/{repairTask.Id}", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedRepairTask = await _context.RepairTasks
                .AsNoTracking()
                .SingleAsync(rt => rt.Id == repairTask.Id);

            Assert.Equal(request.Name, updatedRepairTask.Name);
            Assert.Equal(request.LaborCost, updatedRepairTask.LaborCost);
        }
        finally
        {
            await DeleteRepairTaskAsync(repairTask.Id);
        }
    }

    [Fact]
    public async Task UpdateRepairTask_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var response = await _client.PutAsJsonAsync(
            $"/api/v1.0/repair-tasks/{Guid.NewGuid()}",
            CreateValidUpdateRepairTaskRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRepairTask_WithInvalidRequest_ShouldReturnBadRequest()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var repairTask = RepairTaskTestDataBuilder.Create().Build();

        _context.RepairTasks.Add(repairTask);
        await _context.SaveChangesAsync(default);

        var request = new UpdateRepairTaskRequest
        {
            Name = string.Empty,
            LaborCost = 0,
            EstimatedDurationInMins = RepairDurationInMinutes.Min30,
            Parts = []
        };

        try
        {
            var response = await _client.PutAsJsonAsync($"/api/v1.0/repair-tasks/{repairTask.Id}", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await DeleteRepairTaskAsync(repairTask.Id);
        }
    }

    [Fact]
    public async Task UpdateRepairTask_WithoutManagerRole_ShouldReturnForbidden()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Labor01);

        _client.SetAuthorizationHeader(token);

        var response = await _client.PutAsJsonAsync(
            $"/api/v1.0/repair-tasks/{Guid.NewGuid()}",
            CreateValidUpdateRepairTaskRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRepairTask_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.PutAsJsonAsync(
            $"/api/v1.0/repair-tasks/{Guid.NewGuid()}",
            CreateValidUpdateRepairTaskRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRepairTask_WithValidId_ShouldDeleteRepairTask()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var repairTask = RepairTaskTestDataBuilder.Create().Build();

        _context.RepairTasks.Add(repairTask);
        await _context.SaveChangesAsync(default);

        try
        {
            var response = await _client.DeleteAsync($"/api/v1.0/repair-tasks/{repairTask.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        finally
        {
            await DeleteRepairTaskAsync(repairTask.Id);
        }
    }

    [Fact]
    public async Task DeleteRepairTask_WithInvalidId_ShouldReturnNotFound()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Manager);

        _client.SetAuthorizationHeader(token);

        var response = await _client.DeleteAsync($"/api/v1.0/repair-tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRepairTask_WithoutManagerRole_ShouldReturnForbidden()
    {
        var token = await _client.GenerateTokenAsync(TestUsers.Labor01);

        _client.SetAuthorizationHeader(token);

        var response = await _client.DeleteAsync($"/api/v1.0/repair-tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRepairTask_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.DeleteAsync($"/api/v1.0/repair-tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task DeleteRepairTaskAsync(Guid repairTaskId)
    {
        await _context.RepairTasks
            .Where(rt => rt.Id == repairTaskId)
            .ExecuteDeleteAsync();
    }

    private static CreateRepairTaskRequest CreateValidRepairTaskRequest()
    {
        return new CreateRepairTaskRequest
        {
            Name = "Integration Repair Task",
            LaborCost = 85m,
            EstimatedDurationInMins = RepairDurationInMinutes.Min45,
            Parts =
            [
                new CreateRepairTaskPartRequest
                {
                    Name = "Brake Pad",
                    Cost = 50m,
                    Quantity = 2
                }
            ]
        };
    }

    private static UpdateRepairTaskRequest CreateValidUpdateRepairTaskRequest(Guid partId)
    {
        return new UpdateRepairTaskRequest
        {
            Name = "Updated Repair Task",
            LaborCost = 120m,
            EstimatedDurationInMins = RepairDurationInMinutes.Min60,
            Parts =
            [
                new UpdateRepairTaskPartRequest
                {
                    PartId = partId,
                    Name = "Updated Brake Pad",
                    Cost = 65m,
                    Quantity = 2
                }
            ]
        };
    }
}
