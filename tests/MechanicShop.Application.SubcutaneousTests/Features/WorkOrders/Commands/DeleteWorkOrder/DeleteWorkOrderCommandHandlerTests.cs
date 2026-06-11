using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepaireTasks;
using MechanicShop.Tests.Common.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.DeleteWorkOrder;

[Collection(WebAppFactoryCollection.CollectionName)]
public class DeleteWorkOrderCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();


    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        // Arrange
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var repairTask = RepairTaskFactory.CreateRepairTask().Value;
        var employee = EmployeeFactory.CreateEmployee().Value;

        await _context.Customers.AddAsync(customer);
        await _context.Vehicles.AddAsync(vehicle);
        await _context.RepairTasks.AddAsync(repairTask);
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync(default);

        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            laborId: employee.Id,
            repairTasks: [repairTask]).Value;

        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var command = new DeleteWorkOrderCommand(workOrder.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var deletedWorkOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.Id == workOrder.Id);

        Assert.Null(deletedWorkOrder);
    }

}