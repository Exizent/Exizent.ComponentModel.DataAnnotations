namespace Exizent.ComponentModel.DataAnnotations;

public class MaxPrecisionAttribute : ValidationAttribute
{
    public int MaxPrecision { get; }

    public MaxPrecisionAttribute(int maxPrecision)
    {
        MaxPrecision = maxPrecision;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        decimal typedValue;
        try
        {
            typedValue = Convert.ToDecimal(value);
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidOperationException(
                $"Field '{validationContext.MemberName}' must be convertible to decimal", ex);
        }

        return decimal.Round(typedValue, MaxPrecision) == typedValue
            ? ValidationResult.Success
            : new ValidationResult($"The field {validationContext.MemberName} must have a max precision of {MaxPrecision} decimal places.", validationContext.MemberName is not null ? new[] {validationContext.MemberName} : null);
    }
}