using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepaireTasks;
using MechanicShop.Tests.Common.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.UpdateOrderState;

[Collection(WebAppFactoryCollection.CollectionName)]
public class UpdateWorkOrderStateCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithValidTransitionToInProgress_ShouldSucceed()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(-1).AddHours(10);
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            startAt: startAt,
            endAt: startAt.AddHours(1),
            laborId: employee.Id,
            repairTasks: [repairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddAsync(employee);
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderStateCommand(workOrder.Id, WorkOrderState.InProgress);

        var result = await _mediator.Send(command);

        Assert.True(result.IsSuccess);

        var updatedWorkOrder = await _context.WorkOrders.AsNoTracking().FirstAsync(x => x.Id == workOrder.Id);
        Assert.Equal(WorkOrderState.InProgress, updatedWorkOrder.State);
    }

    [Fact]
    public async Task Handle_WithValidTransitionToCompleted_ShouldSucceed()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(-2).AddHours(11);
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            startAt: startAt,
            endAt: startAt.AddHours(1),
            laborId: employee.Id,
            repairTasks: [repairTask]).Value;

        workOrder.UpdateState(WorkOrderState.InProgress);

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddAsync(employee);
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderStateCommand(workOrder.Id, WorkOrderState.Completed);

        var result = await _mediator.Send(command);

        Assert.True(result.IsSuccess);

        var updatedWorkOrder = await _context.WorkOrders.AsNoTracking().FirstAsync(x => x.Id == workOrder.Id);
        Assert.Equal(WorkOrderState.Completed, updatedWorkOrder.State);
    }

    [Fact]
    public async Task Handle_WithMissingWorkOrder_ShouldFail()
    {
        var command = new UpdateWorkOrderStateCommand(Guid.NewGuid(), WorkOrderState.InProgress);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WithFutureWorkOrder_ShouldFail()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(6).AddHours(10);
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            startAt: startAt,
            endAt: startAt.AddHours(1),
            laborId: employee.Id,
            repairTasks: [repairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddAsync(employee);
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderStateCommand(workOrder.Id, WorkOrderState.InProgress);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
        Assert.Equal("WorkOrderErrors.StateTransitionNotAllowed", result.TopError.Code);
    }

    [Fact]
    public async Task Handle_WithInvalidStateTransition_ShouldFail()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(-3).AddHours(12);
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            startAt: startAt,
            endAt: startAt.AddHours(1),
            laborId: employee.Id,
            repairTasks: [repairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddAsync(employee);
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderStateCommand(workOrder.Id, WorkOrderState.Completed);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
        Assert.Equal("WorkOrderErrors.InvalidStateTransition", result.TopError.Code);
    }
}
