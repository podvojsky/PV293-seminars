using FluentValidation;

namespace Library.Application.Loans.Commands;

public class ReportDamageCommandValidator : AbstractValidator<ReportDamageCommand>
{
    public ReportDamageCommandValidator()
    {
        RuleFor(x => x.LoanId).NotEmpty().WithMessage("Loan ID is required");

        RuleFor(x => x.Description).NotEmpty().WithMessage("Damage description is required");

        RuleFor(x => x.Cost).NotEmpty().WithMessage("Damage cost is required");

        RuleFor(x => x.Cost.Amount).GreaterThan(0).WithMessage("Damage cost needs to be positive number");
    }
}
