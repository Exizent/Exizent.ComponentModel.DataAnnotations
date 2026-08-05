using System.Globalization;

namespace Exizent.ComponentModel.DataAnnotations;

// Expects the value to be a normalised percentage, e.g. 98.1234% is passed in as 0.981234,
// so minimum/maximum should be expressed in that normalised range too, e.g. 0 to 1 for 0%-100%.
public class PercentageFieldRangeAttribute : RangeAttribute
{
    private readonly decimal _minimumPercentage;
    private readonly decimal _maximumPercentage;

    public PercentageFieldRangeAttribute(string minimum, string maximum, int maxDecimalPlaces)
        : base(typeof(Decimal), minimum, maximum)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxDecimalPlaces);

        MaxDecimalPlaces = maxDecimalPlaces;
        ParseLimitsInInvariantCulture = true;
        _minimumPercentage = decimal.Parse(minimum, CultureInfo.InvariantCulture) * 100;
        _maximumPercentage = decimal.Parse(maximum, CultureInfo.InvariantCulture) * 100;

        ErrorMessage = "The field {0} must be a percentage between {1}% and {2}%.";
        DecimalPlacesErrorMessage =
            "The field {0} must be a percentage between {1}% and {2}% with up to {3} decimal places (passed in as a decimal fraction with a maximum of {4} decimal places, e.g. 98.1234% as 0.981234).";
    }

    public int MaxDecimalPlaces { get; }

    public string DecimalPlacesErrorMessage { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (!base.IsValid(value))
            return new ValidationResult(FormatRangeErrorMessage(validationContext.DisplayName), GetMemberNames(validationContext));

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

    private string FormatRangeErrorMessage(string name)
        => string.Format(ErrorMessageString, name, _minimumPercentage, _maximumPercentage);

    private string FormatDecimalPlacesErrorMessage(string name, int effectiveMaxDecimalPlaces)
        => string.Format(DecimalPlacesErrorMessage, name, _minimumPercentage, _maximumPercentage, MaxDecimalPlaces, effectiveMaxDecimalPlaces);

    private static string[]? GetMemberNames(ValidationContext validationContext)
        => validationContext.MemberName is null ? null : new[] { validationContext.MemberName };
}
