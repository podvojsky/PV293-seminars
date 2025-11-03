using System.Text.RegularExpressions;

namespace Library.Domain.ValueObjects;

public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{7,14}$", RegexOptions.Compiled);

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public bool Equals(PhoneNumber? other)
    {
        if (other is null)
            return false;

        // Compare normalized values (case-insensitive though not needed)
        return string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty.", nameof(value));

        // Normalize: remove whitespace
        value = value.Trim();

        // Validate format (E.164-like: + followed by 8–15 digits)
        if (!PhoneRegex.IsMatch(value))
            throw new ArgumentException($"Invalid phone number format: '{value}'.", nameof(value));

        return new PhoneNumber(value);
    }

    public override string ToString()
    {
        return Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PhoneNumber);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.Ordinal);
    }

    public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
    {
        return !Equals(left, right);
    }
}
