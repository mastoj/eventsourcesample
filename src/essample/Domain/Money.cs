using System.Linq;

namespace essample.Domain
{

public record Money {
    public float  Amount   { get; }
    public string Currency { get; }
    public static Money Default => new Money(0, "EUR");

    static readonly string[] SupportedCurrencies = {"USD", "GPB", "EUR", "NOK"};

    internal Money(float amount, string currency) {
        Amount   = amount;
        Currency = currency;
    }

    public static Money FromCurrency(float amount, string currency) {
        if (!SupportedCurrencies.Contains(currency)) throw new DomainException($"Unsupported currency: {currency}");

        return new Money(amount, currency);
    }

    public bool IsSameCurrency(Money another) => Currency == another.Currency;

    public static Money operator -(Money one, Money another) {
        if (!one.IsSameCurrency(another)) throw new DomainException("Cannot operate on different currencies");

        return new Money(one.Amount - another.Amount, one.Currency);
    }

    public static implicit operator double(Money money) => money.Amount;

    public void Deconstruct(out float amount, out string currency) {
        amount = Amount;
        currency = Currency;
    }
}
}
