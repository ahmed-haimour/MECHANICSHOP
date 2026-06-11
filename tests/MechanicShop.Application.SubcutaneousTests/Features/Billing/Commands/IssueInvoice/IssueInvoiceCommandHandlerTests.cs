using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Commands.IssueInvoice;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepaireTasks;
using MechanicShop.Tests.Common.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Commands.IssueInvoice;

[Collection(WebAppFactoryCollection.CollectionName)]
public class IssueInvoiceCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();

    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithMissingWorkOrder_ShouldFail()
    {
        var result = await _mediator.Send(new IssueInvoiceCommand(Guid.NewGuid()));

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WithValidWorkOrder_ShouldSucceed()
    {
        // Arrange
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();

        var repairTask = RepairTaskFactory.CreateRepairTask(
            laborCost: 100m).Value;

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
        workOrder.UpdateState(MechanicShop.Domain.WorkOrders.Enum.WorkOrderState.InProgress);
        workOrder.UpdateState(MechanicShop.Domain.WorkOrders.Enum.WorkOrderState.Completed);

        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync(default);

        var command = new IssueInvoiceCommand(workOrder.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);
        Assert.Equal(workOrder.Id, result.Value.WorkOrderId);

        // Optional: verify invoice was actually saved
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.WorkOrderId == workOrder.Id);

        Assert.NotNull(invoice);
    }

}
