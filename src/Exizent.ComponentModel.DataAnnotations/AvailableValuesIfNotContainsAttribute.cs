namespace Exizent.ComponentModel.DataAnnotations;

public class AvailableValuesIfNotContainsAttribute : AvailableIfNotContainsAttribute
{
    public object[] AvailableFieldValues { get; }

    public AvailableValuesIfNotContainsAttribute(string dependentProperty,
        object dependantPropertyValue, params object[] availableFieldValues)
        : base(dependentProperty, dependantPropertyValue)
    {
        ErrorMessage = "The field {0} must contain {1} when {2} is not assigned to {3}.";
        AvailableFieldValues = availableFieldValues;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue)
    {
        if (value == null)
            return true;

        var isValid = base.IsValid(value, dependentPropertyValue);
        if (isValid)
        {
            return AvailableFieldValues.Any(availableFieldValue
                => Equals(availableFieldValue, value));
        }

        return true;
    }

    protected override string FormatErrorMessage(object? value, object? dependentPropertyValue,
        DependentPropertyValidationContext validationContext)
    {
        return string.Format(ErrorMessageString,
            validationContext.ValidationContext.DisplayName,
            FormatAvailableFieldValues(),
            validationContext.GetDependentPropertyDisplayName(),
            PossibleDependantPropertyValues.Single());
    }

    private string FormatAvailableFieldValues()
        => ValueFormatter.FormatOrValues(AvailableFieldValues);
}