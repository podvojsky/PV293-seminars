using System.Reflection;
using Library.Domain.Aggregates.Loan;
using Library.Domain.Specifications;
using Library.Domain.ValueObjects;

namespace Library.Tests.Domain.Specifications;

public class LoanSpecificationsTests
{
    [Fact]
    public void OverdueLoansSpec_FindsOverdueActiveLoans()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var overdueLoan1 = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Alice", "alice@example.com",
            PhoneNumber.Create("+420123456789"));
        SetDueDate(overdueLoan1, now.AddDays(-5)); // 5 days overdue

        var overdueLoan2 = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Bob", "bob@example.com",
            PhoneNumber.Create("+420987654321"));
        SetDueDate(overdueLoan2, now.AddDays(-1)); // 1 day overdue

        var activeLoan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Carol", "carol@example.com",
            PhoneNumber.Create("+420132547698"));
        SetDueDate(activeLoan, now.AddDays(5)); // Still 5 days to go

        var returnedLoan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Dave", "dave@example.com",
            PhoneNumber.Create("+420111222333444"));
        SetDueDate(returnedLoan, now.AddDays(-3));
        returnedLoan.Return();

        var loans = new List<Loan> { overdueLoan1, overdueLoan2, activeLoan, returnedLoan };

        var spec = new OverdueLoansSpec();

        // Act
        var result = spec.Evaluate(loans).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, loan =>
        {
            Assert.Equal(LoanStatus.Active, loan.Status);
            Assert.True(loan.DueDate < now);
        });
        Assert.Contains(result, l => l.BorrowerName == "Alice");
        Assert.Contains(result, l => l.BorrowerName == "Bob");
    }

    [Fact]
    public void OverdueLoansSpec_OrdersByMostOverdueFirst()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var overdueLoan1 = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Alice", "alice@example.com",
            PhoneNumber.Create("+420123456789"));
        SetDueDate(overdueLoan1, now.AddDays(-10)); // Very overdue

        var overdueLoan2 = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Bob", "bob@example.com",
            PhoneNumber.Create("+420987654321"));
        SetDueDate(overdueLoan2, now.AddDays(-1)); // Just overdue

        var overdueLoan3 = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Carol", "carol@example.com",
            PhoneNumber.Create("+420132547698"));
        SetDueDate(overdueLoan3, now.AddDays(-5)); // Moderately overdue

        var loans = new List<Loan> { overdueLoan2, overdueLoan3, overdueLoan1 };

        var spec = new OverdueLoansSpec();

        // Act
        var result = spec.Evaluate(loans).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Alice", result[0].BorrowerName); // Most overdue
        Assert.Equal("Carol", result[1].BorrowerName); // Middle
        Assert.Equal("Bob", result[2].BorrowerName); // Least overdue
    }

    [Fact]
    public void OverdueLoansSpec_FiltersByMinimumDaysOverdue()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var veryOverdue = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Alice", "alice@example.com",
            PhoneNumber.Create("+420123456789"));
        SetDueDate(veryOverdue, now.AddDays(-10)); // 10 days overdue

        var moderatelyOverdue = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Bob", "bob@example.com",
            PhoneNumber.Create("+420987654321"));
        SetDueDate(moderatelyOverdue, now.AddDays(-5)); // 5 days overdue

        var slightlyOverdue = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Carol", "carol@example.com",
            PhoneNumber.Create("+420132547698"));
        SetDueDate(slightlyOverdue, now.AddDays(-1)); // 1 day overdue

        var loans = new List<Loan> { veryOverdue, moderatelyOverdue, slightlyOverdue };

        var spec = new OverdueLoansSpec(7);

        // Act
        var result = spec.Evaluate(loans).ToList();

        // Assert
        var loan = Assert.Single(result);
        Assert.Equal("Alice", loan.BorrowerName);
    }

    [Fact]
    public void OverdueLoansSpec_ReturnsEmptyWhenNoOverdueLoans()
    {
        // Arrange
        var activeLoan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Alice", "alice@example.com",
            PhoneNumber.Create("+420123456789"));
        var loans = new List<Loan> { activeLoan };

        var spec = new OverdueLoansSpec();

        // Act
        var result = spec.Evaluate(loans).ToList();

        // Assert
        Assert.Empty(result);
    }

    // Helper methods
    private static void SetDueDate(Loan loan, DateTime dueDate)
    {
        var prop = typeof(Loan).GetProperty(nameof(Loan.DueDate),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        prop!.SetValue(loan, dueDate);
    }

    private static void SetLoanDate(Loan loan, DateTime loanDate)
    {
        var prop = typeof(Loan).GetProperty(nameof(Loan.LoanDate),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        prop!.SetValue(loan, loanDate);
    }
}
