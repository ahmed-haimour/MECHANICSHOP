using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks;
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

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks;

[Collection(WebAppFactoryCollection.CollectionName)]
public class UpdateWorkOrderRepairTasksCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var oldRepairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min30).Value;
        var newRepairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(7).AddHours(10);
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            startAt: startAt,
            endAt: startAt.AddMinutes(30),
            laborId: employee.Id,
            spot: Spot.A,
            repairTasks: [oldRepairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddRangeAsync(oldRepairTask, newRepairTask);
        await _context.Employees.AddAsync(employee);
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderRepairTasksCommand(workOrder.Id, [newRepairTask.Id]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsSuccess);

        var updatedWorkOrder = await _context.WorkOrders
            .AsNoTracking()
            .Include(x => x.RepairTasks)
            .FirstAsync(x => x.Id == workOrder.Id);

        Assert.Equal(startAt.AddHours(1), updatedWorkOrder.EndAtUtc);
        Assert.Single(updatedWorkOrder.RepairTasks);
        Assert.Equal(newRepairTask.Id, updatedWorkOrder.RepairTasks.First().Id);
    }

    [Fact]
    public async Task Handle_WithMissingWorkOrder_ShouldFail()
    {
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;

        await _context.RepairTasks.AddAsync(repairTask);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderRepairTasksCommand(Guid.NewGuid(), [repairTask.Id]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WithMissingRepairTask_ShouldFail()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(8).AddHours(10);
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

        var command = new UpdateWorkOrderRepairTasksCommand(workOrder.Id, [Guid.NewGuid()]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WithOutsideOperatingHours_ShouldFail()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var oldRepairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min30).Value;
        var newRepairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(9).AddHours(17).AddMinutes(30);
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            startAt: startAt,
            endAt: startAt.AddMinutes(30),
            laborId: employee.Id,
            repairTasks: [oldRepairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddRangeAsync(oldRepairTask, newRepairTask);
        await _context.Employees.AddAsync(employee);
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderRepairTasksCommand(workOrder.Id, [newRepairTask.Id]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
        Assert.Equal("WorkOrder_Outside_OperatingHours", result.TopError.Code);
    }

    [Fact]
    public async Task Handle_WithSpotConflict_ShouldFail()
    {
        var vehicle1 = VehicleFactory.CreateVehicle().Value;
        var vehicle2 = VehicleFactory.CreateVehicle().Value;
        var customer = CustomerFactory.CreateCustomer(vehicles: [vehicle1, vehicle2]).Value;
        var oldRepairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min30).Value;
        var newRepairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee1 = EmployeeFactory.CreateEmployee().Value;
        var employee2 = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(10).AddHours(10);
        var occupyingStartAt = startAt.AddMinutes(45);
        var workOrder = WorkOrderFactory.CreateWorkOrder(vehicleId: vehicle1.Id, startAt: startAt, endAt: startAt.AddMinutes(30), laborId: employee1.Id, spot: Spot.A, repairTasks: [oldRepairTask]).Value;
        var occupyingWorkOrder = WorkOrderFactory.CreateWorkOrder(vehicleId: vehicle2.Id, startAt: occupyingStartAt, endAt: occupyingStartAt.AddHours(1), laborId: employee2.Id, spot: Spot.A, repairTasks: [oldRepairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddRangeAsync(vehicle1, vehicle2);
        await _context.RepairTasks.AddRangeAsync(oldRepairTask, newRepairTask);
        await _context.Employees.AddRangeAsync(employee1, employee2);
        await _context.WorkOrders.AddRangeAsync(workOrder, occupyingWorkOrder);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderRepairTasksCommand(workOrder.Id, [newRepairTask.Id]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
        Assert.Equal("MechanicShop_Spot_Full", result.TopError.Code);
    }

    [Fact]
    public async Task Handle_WithLaborConflict_ShouldFail()
    {
        var vehicle1 = VehicleFactory.CreateVehicle().Value;
        var vehicle2 = VehicleFactory.CreateVehicle().Value;
        var customer = CustomerFactory.CreateCustomer(vehicles: [vehicle1, vehicle2]).Value;
        var oldRepairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min30).Value;
        var newRepairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var startAt = DateTimeOffset.UtcNow.Date.AddDays(11).AddHours(10);
        var occupyingStartAt = startAt.AddMinutes(45);
        var workOrder = WorkOrderFactory.CreateWorkOrder(vehicleId: vehicle1.Id, startAt: startAt, endAt: startAt.AddMinutes(30), laborId: employee.Id, spot: Spot.A, repairTasks: [oldRepairTask]).Value;
        var occupyingWorkOrder = WorkOrderFactory.CreateWorkOrder(vehicleId: vehicle2.Id, startAt: occupyingStartAt, endAt: occupyingStartAt.AddHours(1), laborId: employee.Id, spot: Spot.B, repairTasks: [oldRepairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddRangeAsync(vehicle1, vehicle2);
        await _context.RepairTasks.AddRangeAsync(oldRepairTask, newRepairTask);
        await _context.Employees.AddAsync(employee);
        await _context.WorkOrders.AddRangeAsync(workOrder, occupyingWorkOrder);
        await _context.SaveChangesAsync(default);

        var command = new UpdateWorkOrderRepairTasksCommand(workOrder.Id, [newRepairTask.Id]);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
        Assert.Equal("Labor_Occupied", result.TopError.Code);
    }
}
