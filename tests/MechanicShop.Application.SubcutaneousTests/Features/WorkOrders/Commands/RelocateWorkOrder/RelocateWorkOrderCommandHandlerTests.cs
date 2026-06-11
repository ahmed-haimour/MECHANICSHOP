using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepaireTasks;
using MechanicShop.Tests.Common.WorkOrders;
using MediatR;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.RelocateWorkOrder;

[Collection(WebAppFactoryCollection.CollectionName)]
public class RelocateWorkOrderCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithMissingWorkOrder_ShouldFail()
    {
        var command = new RelocateWorkOrderCommand(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(10),
            Spot.B);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WithUnavailableCurrentSpot_ShouldFail()
    {
        var vehicle1 = VehicleFactory.CreateVehicle().Value;
        var vehicle2 = VehicleFactory.CreateVehicle().Value;
        var customer = CustomerFactory.CreateCustomer(vehicles: [vehicle1, vehicle2]).Value;
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee1 = EmployeeFactory.CreateEmployee().Value;
        var employee2 = EmployeeFactory.CreateEmployee().Value;
        var originalStartAt = DateTimeOffset.UtcNow.Date.AddDays(2).AddHours(10);
        var newStartAt = DateTimeOffset.UtcNow.Date.AddDays(3).AddHours(11);

        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle1.Id,
            startAt: originalStartAt,
            endAt: originalStartAt.AddHours(1),
            laborId: employee1.Id,
            spot: Spot.A,
            repairTasks: [repairTask]).Value;

        var occupyingWorkOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle2.Id,
            startAt: newStartAt,
            endAt: newStartAt.AddHours(1),
            laborId: employee2.Id,
            spot: Spot.A,
            repairTasks: [repairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddRangeAsync(vehicle1, vehicle2);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddRangeAsync(employee1, employee2);
        await _context.WorkOrders.AddRangeAsync(workOrder, occupyingWorkOrder);
        await _context.SaveChangesAsync(default);

        var command = new RelocateWorkOrderCommand(workOrder.Id, newStartAt, Spot.B);

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
        var repairTask = RepairTaskFactory.CreateRepairTask(repairDurationInMinutes: RepairDurationInMinutes.Min60).Value;
        var employee = EmployeeFactory.CreateEmployee().Value;
        var originalStartAt = DateTimeOffset.UtcNow.Date.AddDays(4).AddHours(10);
        var newStartAt = DateTimeOffset.UtcNow.Date.AddDays(5).AddHours(11);

        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle1.Id,
            startAt: originalStartAt,
            endAt: originalStartAt.AddHours(1),
            laborId: employee.Id,
            spot: Spot.A,
            repairTasks: [repairTask]).Value;

        var occupyingWorkOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle2.Id,
            startAt: newStartAt,
            endAt: newStartAt.AddHours(1),
            laborId: employee.Id,
            spot: Spot.C,
            repairTasks: [repairTask]).Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddRangeAsync(vehicle1, vehicle2);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddAsync(employee);
        await _context.WorkOrders.AddRangeAsync(workOrder, occupyingWorkOrder);
        await _context.SaveChangesAsync(default);

        var command = new RelocateWorkOrderCommand(workOrder.Id, newStartAt, Spot.B);

        var result = await _mediator.Send(command);

        Assert.True(result.IsError);
        Assert.Equal("Labor_Occupied", result.TopError.Code);
    }
}
