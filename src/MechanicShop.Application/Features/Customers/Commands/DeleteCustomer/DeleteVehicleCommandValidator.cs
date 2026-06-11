using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteVehicleCommandValidator : AbstractValidator<DeleteVehicleCommand>
{
    public DeleteVehicleCommandValidator()
    {
        RuleFor(x => x.VehicleId)
                    .NotEmpty().WithMessage("Vehicle Id is required.");
    }
}