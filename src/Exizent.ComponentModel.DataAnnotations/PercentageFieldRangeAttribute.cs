namespace Exizent.ComponentModel.DataAnnotations;

// Expects the value to be a normalised percentage, e.g. 98.1234% is passed in as 0.981234,
// so minimum/maximum should be expressed in that normalised range too, e.g. 0 to 1 for 0%-100%.
public class PercentageFieldRangeAttribute : RangeAttribute
{
    public PercentageFieldRangeAttribute(double minimum, double maximum, int maxDecimalPlaces)
        : base(minimum, maximum)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxDecimalPlaces);

        MaxDecimalPlaces = maxDecimalPlaces;
        DecimalPlacesErrorMessage =
            "{0} must be passed as a decimal fraction with a maximum of {1} decimal places (a percentage with up to {2} decimal places, e.g. 98.1234% as 0.981234).";
    }

    public int MaxDecimalPlaces { get; }

    public string DecimalPlacesErrorMessage { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (!base.IsValid(value))
            return new ValidationResult(base.FormatErrorMessage(validationContext.DisplayName), GetMemberNames(validationContext));

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

        var effectiveMaxDecimalPlaces = MaxDecimalPlaces + 2;
        if (decimal.Round(typedValue, effectiveMaxDecimalPlaces) != typedValue)
            return new ValidationResult(
                FormatDecimalPlacesErrorMessage(validationContext.DisplayName, effectiveMaxDecimalPlaces),
                GetMemberNames(validationContext));

        return ValidationResult.Success;
    }

    private string FormatDecimalPlacesErrorMessage(string name, int effectiveMaxDecimalPlaces)
        => string.Format(DecimalPlacesErrorMessage, name, effectiveMaxDecimalPlaces, MaxDecimalPlaces);

    private static string[]? GetMemberNames(ValidationContext validationContext)
        => validationContext.MemberName is null ? null : new[] { validationContext.MemberName };
}
