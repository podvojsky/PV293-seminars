using System.Security.Claims;
using Library.Domain.Aggregates;
using Library.Domain.Aggregates.Loan;
using Library.Domain.Repositories;
using Library.Domain.Specifications;
using Library.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Library.Domain.Services;

/// <summary>
///     Domain service for loan-related business logic
///     Can depend on domain repository interfaces (Dependency Inversion Principle)
/// </summary>
public class LoanService(
    IRepository<Book> bookRepository,
    IRepository<Loan> loanRepository,
    UserManager<ApplicationUser> userManager) : ILoanService
{
    public async Task<Loan> CheckOutBookAsync(
        Guid bookId,
        Guid? requestedBorrowerId,
        ClaimsPrincipal user,
        int loanDurationDays,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Fetch required data using repositories (domain defines the interface)
        var book = await bookRepository.GetByIdAsync(bookId)
                   ?? throw new InvalidOperationException($"Book with ID {bookId} not found");

        var activeLoanSpec = new ActiveLoanByBookSpec(bookId);
        var existingActiveLoan = await loanRepository.FirstOrDefaultAsync(activeLoanSpec, cancellationToken);

        // Determine the actual borrower (respects authorization logic in Loan aggregate)
        var actualBorrowerId = Loan.DetermineBorrowerId(requestedBorrowerId, user);

        var borrower = await userManager.FindByIdAsync(actualBorrowerId.ToString())
                       ?? throw new InvalidOperationException($"User with ID {actualBorrowerId} not found");

        // Business Rule: Book must be available (no active loans)
        if (existingActiveLoan != null)
            throw new InvalidOperationException($"Book '{book.Title}' is already checked out");

        // Create the loan using the aggregate factory method
        // Note: Book availability will be updated via domain event handler
        var loan = Loan.Create(
            book.Id,
            actualBorrowerId,
            borrower.FullName,
            borrower.Email ?? string.Empty,
            PhoneNumber.Create(borrower.PhoneNumber ?? string.Empty),
            loanDurationDays
        );

        return loan;
    }
}
