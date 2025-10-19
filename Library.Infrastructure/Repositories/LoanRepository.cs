using Library.Application.RepositoryInterfaces;
using Library.DataAccess.Data;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class LoanRepository : Repository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext context) : base(context)
    {
    }

    private ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)Context;

    public async Task<Loan?> GetActiveLoanByBookIdAsync(Guid bookId)
    {
        return await Entities
            .FirstOrDefaultAsync(l => l.BookId == bookId && l.Status == LoanStatus.Active);
    }

    public async Task<bool> HasActiveLoanAsync(Guid borrowerId, Guid bookId)
    {
        return await Entities
            .AnyAsync(l => l.BorrowerId == borrowerId && l.BookId == bookId && l.Status == LoanStatus.Active);
    }
}
