using MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Queries.GetInvoicePdf;

public class GetInvoicePdfQueryValidatorTests
{
    private readonly GetInvoicePdfQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_InvoiceId_Is_Empty()
    {
        var result = _validator.Validate(new GetInvoicePdfQuery(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "InvoiceId");
    }

    [Fact]
    public void Should_Pass_When_InvoiceId_Is_Valid()
    {
        var result = _validator.Validate(new GetInvoicePdfQuery(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
