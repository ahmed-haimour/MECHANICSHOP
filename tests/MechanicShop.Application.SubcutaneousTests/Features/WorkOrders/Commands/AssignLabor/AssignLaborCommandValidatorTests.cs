using MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.AssignLabor;

public class AssignLaborCommandValidatorTests
{
    private readonly AssignLaborCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_WorkOrderId_Is_Empty()
    {
        var command = new AssignLaborCommand(Guid.Empty, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WorkOrderId");
    }

    [Fact]
    public void Should_Have_Error_When_LaborId_Is_Empty()
    {
        var command = new AssignLaborCommand(Guid.NewGuid(), Guid.Empty);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LaborId");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = new AssignLaborCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
