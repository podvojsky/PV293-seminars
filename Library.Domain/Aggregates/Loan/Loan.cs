using System.Security.Claims;
using Library.Domain.Common;
using Library.Domain.Constants;
using Library.Domain.ValueObjects;

namespace Library.Domain.Aggregates.Loan;

public class Loan : AggregateRoot
{
    private readonly List<Fine> _fines = new();

    // Private constructor for EF Core
    private Loan()
    {
        BorrowerPhoneNumber = PhoneNumber.Create("+420000000000");
    }

    // Constructor with ID for seeding
    private Loan(Guid id) : base(id)
    {
    }

    public Guid BookId { get; private set; }
    public Guid BorrowerId { get; private set; }
    public string BorrowerName { get; private set; } = string.Empty;
    public string BorrowerEmail { get; private set; } = string.Empty;
    public PhoneNumber BorrowerPhoneNumber { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }

    public IReadOnlyCollection<Fine> Fines => _fines.AsReadOnly();

    public static Guid DetermineBorrowerId(Guid? requestedBorrowerId, ClaimsPrincipal currentUser)
    {
        if (currentUser == null)
            throw new ArgumentException("User context is required", nameof(currentUser));

        var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var currentUserId))
            throw new InvalidOperationException("Invalid user ID in claims");

        var isLibrarian = currentUser.IsInRole(UserRoles.Librarian);
        var isAdmin = currentUser.IsInRole(UserRoles.Admin);
        var canLoanForOthers = isLibrarian || isAdmin;

        if (requestedBorrowerId.HasValue)
        {
            if (!canLoanForOthers && requestedBorrowerId.Value != currentUserId)
                throw new InvalidOperationException("Members can only create loans for themselves");

            return requestedBorrowerId.Value;
        }

        return currentUserId;
    }

    // Factory method for checking out a book
    public static Loan Create(
        Guid bookId,
        Guid borrowerId,
        string borrowerName,
        string borrowerEmail,
        PhoneNumber borrowerPhoneNumber,
        int loanDurationDays = 14)
    {
        if (bookId == Guid.Empty)
            throw new ArgumentException("Book ID cannot be empty", nameof(bookId));

        if (borrowerId == Guid.Empty)
            throw new ArgumentException("Borrower ID cannot be empty", nameof(borrowerId));

        if (string.IsNullOrWhiteSpace(borrowerName))
            throw new ArgumentException("Borrower name cannot be empty", nameof(borrowerName));

        if (string.IsNullOrWhiteSpace(borrowerEmail))
            throw new ArgumentException("Borrower email cannot be empty", nameof(borrowerEmail));

        if (string.IsNullOrWhiteSpace(borrowerPhoneNumber.ToString()))
            throw new ArgumentException("Borrower phone cannot be empty", nameof(borrowerPhoneNumber));

        if (loanDurationDays <= 0)
            throw new ArgumentException("Loan duration must be positive", nameof(loanDurationDays));

        if (loanDurationDays > 30)
            throw new ArgumentException("Loan duration cannot exceed 30 days", nameof(loanDurationDays));

        var loan = new Loan
        {
            BookId = bookId,
            BorrowerId = borrowerId,
            BorrowerName = borrowerName,
            BorrowerEmail = borrowerEmail,
            BorrowerPhoneNumber = borrowerPhoneNumber,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanDurationDays),
            Status = LoanStatus.Active
        };

        return loan;
    }

    public void Return()
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException($"Cannot return a loan with status: {Status}");

        ReturnDate = DateTime.UtcNow;

        if (ReturnDate.Value > DueDate)
        {
            var daysLate = (ReturnDate.Value - DueDate).Days;
            var lateFeeAmount = Money.Create(daysLate * 1.50m, "EUR"); // $1.50 per day late fee
            AddFine(FineType.LateFee, lateFeeAmount, $"Late return fee: {daysLate} days overdue");
        }

        Status = LoanStatus.Returned;

        if (!HasOutstandingFines()) Complete();
    }

    public void ExtendDueDate(int additionalDays)
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException(
                $"Cannot extend a loan with status: {Status}. Only active loans can be extended.");

        if (additionalDays <= 0)
            throw new ArgumentException("Additional days must be positive", nameof(additionalDays));

        if (IsOverdue())
            throw new InvalidOperationException("Cannot extend an overdue loan.");

        // Calculate total loan duration after extension
        var currentDuration = (DueDate - LoanDate).Days;
        var newTotalDuration = currentDuration + additionalDays;

        if (newTotalDuration > 90)
            throw new InvalidOperationException("Total loan duration cannot exceed 90 days.");

        // Update due date
        DueDate = DueDate.AddDays(additionalDays);
    }


    public void MarkAsLost(Money? replacementCost = null)
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException($"Cannot mark as lost a loan with status: {Status}");

        Status = LoanStatus.Lost;

        // Add replacement fee
        var replacementFee = replacementCost ?? Money.Create(20.00m, "EUR");
        AddFine(FineType.LostBookFee, replacementFee, "Book replacement fee");
    }

    public void ReportDamage(string damageDescription, Money damageCost)
    {
        if (Status == LoanStatus.Lost || Status == LoanStatus.Completed)
            throw new InvalidOperationException($"Cannot report damage on a loan with status: {Status}");

        if (string.IsNullOrWhiteSpace(damageDescription))
            throw new ArgumentException("Damage description cannot be empty", nameof(damageDescription));

        if (damageCost == null)
            throw new ArgumentNullException(nameof(damageCost), "Damage cost cannot be null");

        if (damageCost.Amount <= 0)
            throw new ArgumentException("Damage cost must be positive", nameof(damageCost));

        // Add fine with damage description in the reason
        AddFine(FineType.DamageFee, damageCost, $"Book damage fee: {damageDescription.Trim()}");
    }


    private void AddFine(FineType type, Money amount, string reason)
    {
        var fine = new Fine(type, amount, reason);
        _fines.Add(fine);
    }

    public void Complete()
    {
        if (Status != LoanStatus.Returned)
            throw new InvalidOperationException(
                $"Cannot complete a loan with status: {Status}. Book must be returned first.");

        if (HasOutstandingFines())
            throw new InvalidOperationException(
                "Cannot complete loan with outstanding fines. All fines must be paid or waived.");

        Status = LoanStatus.Completed;
    }

    public void PayFine(Guid fineId, string paymentReference)
    {
        var fine = _fines.FirstOrDefault(f => f.Id == fineId);
        if (fine == null)
            throw new ArgumentException($"Fine with ID {fineId} not found", nameof(fineId));

        fine.Pay(paymentReference);

        // Check if loan can be completed after paying this fine
        if (Status == LoanStatus.Returned && !HasOutstandingFines()) Complete();
    }

    public void WaiveFine(Guid fineId, string waiveReason)
    {
        var fine = _fines.FirstOrDefault(f => f.Id == fineId)
                   ?? throw new ArgumentException($"Fine with ID {fineId} not found", nameof(fineId));

        fine.Waive(waiveReason);

        if (Status == LoanStatus.Returned && !HasOutstandingFines()) Complete();
    }

    public bool IsActive()
    {
        return Status == LoanStatus.Active;
    }

    public bool IsReturned()
    {
        return Status == LoanStatus.Returned || Status == LoanStatus.Completed;
    }

    public bool IsCompleted()
    {
        return Status == LoanStatus.Completed;
    }

    public bool IsOverdue()
    {
        return Status == LoanStatus.Active && DateTime.UtcNow > DueDate;
    }

    public int GetDaysOverdue()
    {
        return IsOverdue() ? (DateTime.UtcNow - DueDate).Days : 0;
    }

    public int GetLoanDuration()
    {
        return (ReturnDate ?? DateTime.UtcNow).Subtract(LoanDate).Days;
    }


    public bool HasOutstandingFines()
    {
        return _fines.Any(f => f.IsPending);
    }
}

public enum LoanStatus
{
    Active, // Book is currently borrowed
    Returned, // Book has been returned but may have outstanding fines
    Lost, // Book has been marked as lost
    Completed // Book returned and all fines settled - loan fully resolved
}
