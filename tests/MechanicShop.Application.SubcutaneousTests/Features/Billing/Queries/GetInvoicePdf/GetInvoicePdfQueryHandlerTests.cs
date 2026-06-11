using MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf;
using MechanicShop.Application.SubcutaneousTests.Common;
using MediatR;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.Billing.Queries.GetInvoicePdf;

[Collection(WebAppFactoryCollection.CollectionName)]
public class GetInvoicePdfQueryHandlerTests(WebAppFactory factory)
{
    private readonly IMediator _mediator = factory.CreateMediator();

    [Fact]
    public async Task Handle_WithMissingInvoice_ShouldFail()
    {
        var result = await _mediator.Send(new GetInvoicePdfQuery(Guid.NewGuid()));

        Assert.True(result.IsError);
    }
}
