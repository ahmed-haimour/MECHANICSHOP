using MechanicShop.Application.Features.Billing.Commands.SettleInvoice;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Commands.SettleInvoice;

public class SettleInvoiceCommandValidatorTests
{
    private readonly SettleInvoiceCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_InvoiceId_Is_Empty()
    {
        var result = _validator.Validate(new SettleInvoiceCommand(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "InvoiceId");
    }

    [Fact]
    public void Should_Pass_When_InvoiceId_Is_Valid()
    {
        var result = _validator.Validate(new SettleInvoiceCommand(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
