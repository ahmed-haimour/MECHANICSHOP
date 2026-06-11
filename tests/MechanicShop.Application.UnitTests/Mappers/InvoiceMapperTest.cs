using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Tests.Common.Billing;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.WorkOrders;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;

public class InvoiceMapperTest
{
    [Fact]
    public void ToDto_ShouldMapCorrectly()
    {
        var customer = CustomerFactory.CreateCustomer(
            name: "Jane Customer",
            phoneNumber: "5551234567",
            email: "jane@localhost").Value;

        var vehicle = customer.Vehicles.First();
        SetVehicleCustomer(vehicle, customer);

        var labor = EmployeeFactory.CreateLabor().Value;
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            laborId: labor.Id).Value;
        workOrder.Vehicle = vehicle;
        workOrder.Labor = labor;

        var lineItem = InvoiceLineItemFactory.CreateInvoiceLineItem(
            lineNumber: 1,
            description: "Oil Change",
            quantity: 2,
            unitPrice: 50m).Value;

        var invoice = InvoiceFactory.CreateInvoice(
            workOrderId: workOrder.Id,
            items: [lineItem],
            discount: 10m,
            taxAmount: 5m).Value;
        invoice.WorkOrder = workOrder;

        var dto = invoice.ToDto();

        Assert.Equal(invoice.Id, dto.InvoiceId);
        Assert.Equal(invoice.WorkOrderId, dto.WorkOrderId);
        Assert.Equal(invoice.IssuedAtUtc, dto.IssuedAtUtc);
        Assert.Equal(invoice.Subtotal, dto.Subtotal);
        Assert.Equal(invoice.TaxAmount, dto.TaxAmount);
        Assert.Equal(invoice.DiscountAmount, dto.DiscountAmount);
        Assert.Equal(invoice.Total, dto.Total);
        Assert.Equal(invoice.Status.ToString(), dto.PaymentStatus);

        Assert.NotNull(dto.Customer);
        Assert.Equal(customer.Id, dto.Customer!.CustomerId);
        Assert.Equal(customer.Name, dto.Customer.Name);

        Assert.NotNull(dto.Vehicle);
        Assert.Equal(vehicle.Id, dto.Vehicle!.VehicleId);
        Assert.Equal(vehicle.Make, dto.Vehicle.Make);
        Assert.Equal(vehicle.Model, dto.Vehicle.Model);
        Assert.Equal(vehicle.Year, dto.Vehicle.Year);
        Assert.Equal(vehicle.LicensePlate, dto.Vehicle.LicensePlate);

        Assert.Single(dto.Items);
        Assert.Equal(lineItem.InvoiceId, dto.Items[0].InvoiceId);
        Assert.Equal(lineItem.LineNumber, dto.Items[0].LineNumber);
        Assert.Equal(lineItem.Description, dto.Items[0].Description);
        Assert.Equal(lineItem.Quantity, dto.Items[0].Quantity);
        Assert.Equal(lineItem.UnitPrice, dto.Items[0].UnitPrice);
        Assert.Equal(lineItem.LineTotal, dto.Items[0].LineTotal);
    }

    [Fact]
    public void ToDtos_ShouldMapListCorrectly()
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        SetVehicleCustomer(vehicle, customer);

        var labor = EmployeeFactory.CreateLabor().Value;
        var workOrder = WorkOrderFactory.CreateWorkOrder(
            vehicleId: vehicle.Id,
            laborId: labor.Id).Value;
        workOrder.Vehicle = vehicle;
        workOrder.Labor = labor;

        var invoice = InvoiceFactory.CreateInvoice(
            workOrderId: workOrder.Id,
            items:
            [
                InvoiceLineItemFactory.CreateInvoiceLineItem(
                    description: "Brake Inspection",
                    quantity: 1,
                    unitPrice: 100m).Value
            ]).Value;
        invoice.WorkOrder = workOrder;

        var invoices = new List<Invoice> { invoice };

        var dtos = invoices.ToDtos();

        Assert.Single(dtos);
        var dto = dtos[0];

        Assert.Equal(invoice.Id, dto.InvoiceId);
        Assert.Equal(invoice.WorkOrderId, dto.WorkOrderId);
        Assert.NotNull(dto.Customer);
        Assert.NotNull(dto.Vehicle);
        Assert.Single(dto.Items);
        Assert.Equal(invoice.Total, dto.Total);
        Assert.Equal(invoice.Status.ToString(), dto.PaymentStatus);
    }

    private static void SetVehicleCustomer(Vehicle vehicle, Customer customer)
    {
        typeof(Vehicle)
            .GetProperty(nameof(Vehicle.Customer))!
            .SetValue(vehicle, customer);
    }
}
