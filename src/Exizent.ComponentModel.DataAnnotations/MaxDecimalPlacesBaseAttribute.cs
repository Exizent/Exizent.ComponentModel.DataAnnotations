namespace Exizent.ComponentModel.DataAnnotations;

public abstract class MaxDecimalPlacesBaseAttribute : ValidationAttribute
{
    protected MaxDecimalPlacesBaseAttribute(int maxDecimalPlaces, string errorMessage)
        : base(errorMessage)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxDecimalPlaces);

        MaxDecimalPlaces = maxDecimalPlaces;
    }

    public int MaxDecimalPlaces { get; }

    protected abstract int EffectiveMaxDecimalPlaces { get; }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        decimal typedValue;
        try
        {
            typedValue = Convert.ToDecimal(value);
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidOperationException(
                "Field must be convertible to decimal", ex);
        }

        return decimal.Round(typedValue, EffectiveMaxDecimalPlaces) == typedValue;
    }
}
