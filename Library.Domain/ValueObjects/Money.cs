using System.Text.Json.Serialization;

namespace Library.Domain.ValueObjects;

public class Money : IEquatable<Money>
{
    [JsonConstructor]
    internal Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public bool Equals(Money? other)
    {
        if (other is null)
            return false;

        return Amount == other.Amount && Currency == other.Currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter ISO code", nameof(currency));

        return new Money(amount, currency.ToUpperInvariant());
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot add money with different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot subtract money with different currencies: {Currency} and {other.Currency}");

        if (Amount < other.Amount)
            throw new InvalidOperationException("Cannot subtract to a negative amount");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money MultiplyBy(decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));

        return new Money(Amount * multiplier, Currency);
    }

    public override string ToString()
    {
        return $"{Currency} {Amount:F2}";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Money);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public static bool operator ==(Money? left, Money? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !Equals(left, right);
    }
}
