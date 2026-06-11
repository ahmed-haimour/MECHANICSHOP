using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Commands.SettleInvoice;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Contracts.Common;
using MechanicShop.Tests.Common.Billing;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepaireTasks;
using MechanicShop.Tests.Common.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Commands.SettleInvoice;

[Collection(WebAppFactoryCollection.CollectionName)]
public class SettleInvoiceCommandHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    [Fact]
    public async Task Handle_WithMissingInvoice_ShouldFail()
    {
        var result = await _mediator.Send(new SettleInvoiceCommand(Guid.NewGuid()));

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WithValidInvoice_ShouldSucceed()
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

        var invoice = InvoiceFactory.CreateInvoice(
            workOrderId: workOrder.Id).Value;

        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync(default);

        var command = new SettleInvoiceCommand(invoice.Id);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var settledInvoice = await _context.Invoices
            .FirstOrDefaultAsync(x => x.Id == invoice.Id);

        Assert.NotNull(settledInvoice);

        // add more assertions here depending on your domain:
        // Assert.Equal(InvoiceStatus.Settled, settledInvoice.State);
    }
}
