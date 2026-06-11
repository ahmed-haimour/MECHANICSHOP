using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepaireTasks;
using MechanicShop.Tests.Common.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.AssignLabor;

[Collection(WebAppFactoryCollection.CollectionName)]
public class AssignLaborCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var oldLabor = EmployeeFactory.CreateLabor().Value;
        var newLabor = EmployeeFactory.CreateLabor().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(12).AddHours(10);
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            startAt: startAt,
            endAt: startAt.AddHours(1),
            laborId: oldLabor.Id,
            repairTasks: [repairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddRangeAsync(oldLabor, newLabor);
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var result = await _mediator.Send(new AssignLaborCommand(workOrder.Id, newLabor.Id));

        Assert.True(result.IsSuccess);

        var updatedWorkOrder = await _context.WorkOrders.AsNoTracking().FirstAsync(x => x.Id == workOrder.Id);
        Assert.Equal(newLabor.Id, updatedWorkOrder.LaborId);
    }

    [Fact]
    public async Task Handle_WithMissingWorkOrder_ShouldFail()
    {
        var labor = EmployeeFactory.CreateLabor().Value;

        await _context.Employees.AddAsync(labor);
        await _context.SaveChangesAsync(default);

        var result = await _mediator.Send(new AssignLaborCommand(Guid.NewGuid(), labor.Id));

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WithMissingLabor_ShouldFail()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var labor = EmployeeFactory.CreateLabor().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(13).AddHours(10);
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            startAt: startAt,
            endAt: startAt.AddHours(1),
            laborId: labor.Id,
            repairTasks: [repairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddAsync(labor);
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var result = await _mediator.Send(new AssignLaborCommand(workOrder.Id, Guid.NewGuid()));

        Assert.True(result.IsError);
    }
}
