using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer Id is required.");
    }
}