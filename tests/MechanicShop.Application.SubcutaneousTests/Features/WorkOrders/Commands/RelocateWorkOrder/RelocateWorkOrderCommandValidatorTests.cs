using MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;
using MechanicShop.Domain.WorkOrders.Enum;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.RelocateWorkOrder;

public class RelocateWorkOrderCommandValidatorTests
{
    private readonly RescheduleAppointmentCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_WorkOrderId_Is_Empty()
    {
        var command = new RelocateWorkOrderCommand(Guid.Empty, DateTimeOffset.UtcNow.AddHours(1), Spot.A);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WorkOrderId");
    }

    [Fact]
    public void Should_Have_Error_When_NewStartAt_Is_Not_In_Future()
    {
        var command = new RelocateWorkOrderCommand(Guid.NewGuid(), DateTimeOffset.UtcNow.AddSeconds(-1), Spot.A);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "NewStartAt");
    }

    [Fact]
    public void Should_Have_Error_When_NewSpot_Is_Invalid()
    {
        var command = new RelocateWorkOrderCommand(Guid.NewGuid(), DateTimeOffset.UtcNow.AddHours(1), (Spot)999);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "NewSpot");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = new RelocateWorkOrderCommand(Guid.NewGuid(), DateTimeOffset.UtcNow.AddHours(1), Spot.B);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
