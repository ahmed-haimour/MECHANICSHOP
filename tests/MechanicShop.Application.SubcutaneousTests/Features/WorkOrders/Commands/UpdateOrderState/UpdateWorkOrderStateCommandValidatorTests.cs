using MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState;
using MechanicShop.Domain.WorkOrders.Enum;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.UpdateOrderState;

public class UpdateWorkOrderStateCommandValidatorTests
{
    private readonly UpdateWorkOrderStateCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_State_Is_Invalid()
    {
        var command = new UpdateWorkOrderStateCommand(Guid.NewGuid(), (WorkOrderState)999);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "State");
    }

    [Fact]
    public void Should_Pass_When_State_Is_Valid()
    {
        var command = new UpdateWorkOrderStateCommand(Guid.NewGuid(), WorkOrderState.InProgress);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
