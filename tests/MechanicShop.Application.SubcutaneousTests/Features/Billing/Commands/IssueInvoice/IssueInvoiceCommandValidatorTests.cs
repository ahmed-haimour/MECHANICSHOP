using MechanicShop.Application.Features.Billing.Commands.IssueInvoice;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Commands.IssueInvoice;

public class IssueInvoiceCommandValidatorTests
{
    private readonly IssueInvoiceCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_WorkOrderId_Is_Empty()
    {
        var result = _validator.Validate(new IssueInvoiceCommand(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WorkOrderId");
    }

    [Fact]
    public void Should_Pass_When_WorkOrderId_Is_Valid()
    {
        var result = _validator.Validate(new IssueInvoiceCommand(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
