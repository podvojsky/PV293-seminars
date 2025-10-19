using Library.Application.CQRS;
using Library.Application.RepositoryInterfaces;
using Library.Domain.ValueObjects;
using MediatR;

namespace Library.Application.Loans.Commands;

public class ReportDamageCommand : ICommand<Unit>
{
    public Guid LoanId { get; set; }

    public string Description { get; set; }

    public Money Cost { get; set; }
}

public class ReportDamageCommandHandler(ILoanRepository loanRepository) : IRequestHandler<ReportDamageCommand, Unit>
{
    public async Task<Unit> Handle(ReportDamageCommand command, CancellationToken cancellationToken)
    {
        var loan = await loanRepository.GetByIdAsync(command.LoanId);
        if (loan == null)
            throw new InvalidOperationException($"Loan with ID {command.LoanId} not found");

        loan.ReportDamage(command.Description, command.Cost);
        return Unit.Value;
    }
}
