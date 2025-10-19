using Library.Domain.Entities;

namespace Library.Application.RepositoryInterfaces;

public interface ILoanRepository : IRepository<Loan>
{
    Task<Loan?> GetActiveLoanByBookIdAsync(Guid bookId);
    Task<bool> HasActiveLoanAsync(Guid borrowerId, Guid bookId);
}
