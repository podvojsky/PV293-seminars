using Library.Domain.Aggregates.Loan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Data.EntityConfigurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.BorrowerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.BorrowerEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Status)
            .HasConversion<string>();

        builder.OwnsOne(e => e.BorrowerPhoneNumber,
            borrowerPhoneBuilder => { borrowerPhoneBuilder.Property(p => p.Value).HasColumnName("Phone"); });

        // Configure Fines as owned entities
        builder.OwnsMany(e => e.Fines, finesBuilder =>
        {
            finesBuilder.ToTable("Fines");
            finesBuilder.WithOwner().HasForeignKey("LoanId");
            finesBuilder.HasKey("Id");
            finesBuilder.Property(f => f.Id).ValueGeneratedNever();
            finesBuilder.Property(f => f.Type).HasConversion<string>();
            finesBuilder.Property(f => f.Status).HasConversion<string>();
            finesBuilder.Property(f => f.Reason).IsRequired().HasMaxLength(500);
            finesBuilder.Property(f => f.PaymentReference).HasMaxLength(100);

            // Configure Money value object within Fine
            finesBuilder.OwnsOne(f => f.Amount, amountBuilder =>
            {
                amountBuilder.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2);
                amountBuilder.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3);
            });
        });

        // Indexes for querying
        builder.HasIndex(e => new { e.BookId, e.Status });
        builder.HasIndex(e => e.BorrowerId);
        builder.HasIndex(e => e.DueDate);
    }
}
