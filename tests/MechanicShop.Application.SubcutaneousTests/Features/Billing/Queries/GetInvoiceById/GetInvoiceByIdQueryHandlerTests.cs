using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Commands.SettleInvoice;
using MechanicShop.Application.Features.Billing.Queries.GetInvoiceById;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Tests.Common.Billing;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepaireTasks;
using MechanicShop.Tests.Common.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Queries.GetInvoiceById;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetInvoiceByIdQueryHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();

    private readonly IAppDbContext _context = factory.CreateAppDbContext();
    [Fact]
    public async Task Handle_WithMissingInvoice_ShouldFail()
    {
        var result = await _mediator.Send(new GetInvoiceByIdQuery(Guid.NewGuid()));

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WithExistingInvoice_ShouldReturnInvoiceById()
    {
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

        var result = await _mediator.Send(new GetInvoiceByIdQuery(invoice.Id));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(invoice.Id, result.Value.InvoiceId);
        Assert.Equal(workOrder.Id, result.Value.WorkOrderId);
        Assert.Equal(invoice.Subtotal, result.Value.Subtotal);
        Assert.Equal(invoice.TaxAmount, result.Value.TaxAmount);
        Assert.Equal(invoice.Total, result.Value.Total);
        Assert.Equal(invoice.Status.ToString(), result.Value.PaymentStatus);
    }
}
