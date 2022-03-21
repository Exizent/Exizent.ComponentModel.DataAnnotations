namespace Exizent.ComponentModel.DataAnnotations;

public class MaxPrecisionAttribute : ValidationAttribute
{
    public int MaxPrecision { get; }

    public MaxPrecisionAttribute(int maxPrecision) : base($"The field {{0}} must have a max precision of {maxPrecision} decimal places.")
    {
        MaxPrecision = maxPrecision;
    }

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

        return decimal.Round(typedValue, MaxPrecision) == typedValue;
    }
}