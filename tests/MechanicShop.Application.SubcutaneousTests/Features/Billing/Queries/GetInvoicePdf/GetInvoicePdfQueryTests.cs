using MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Queries.GetInvoicePdf;

public class GetInvoicePdfQueryTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var invoiceId = Guid.NewGuid();

        var query = new GetInvoicePdfQuery(invoiceId);

        Assert.Equal(invoiceId, query.InvoiceId);
    }
}
