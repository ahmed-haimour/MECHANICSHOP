using MechanicShop.Application.Features.Billing.Queries.GetInvoiceById;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Queries.GetInvoiceById;

public class GetInvoiceByIdQueryValidatorTests
{
    private readonly GetInvoiceByIdQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_InvoiceId_Is_Empty()
    {
        var result = _validator.Validate(new GetInvoiceByIdQuery(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "InvoiceId");
    }

    [Fact]
    public void Should_Pass_When_InvoiceId_Is_Valid()
    {
        var result = _validator.Validate(new GetInvoiceByIdQuery(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }
}
