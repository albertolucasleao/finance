using FluentValidation;
using Tce.Application.DTOs;

namespace Tce.Application.Validators;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}