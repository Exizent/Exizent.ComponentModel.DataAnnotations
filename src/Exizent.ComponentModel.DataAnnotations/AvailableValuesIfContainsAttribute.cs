namespace Exizent.ComponentModel.DataAnnotations;

public class AvailableValuesIfContainsAttribute : AvailableIfContainsAttribute
{
    public object[] AvailableFieldValues { get; }

    public AvailableValuesIfContainsAttribute(string dependentProperty, object dependantPropertyValue, params object[] availableFieldValues)
        : base(dependentProperty, dependantPropertyValue)
    {
        ErrorMessage = "The field {0} must contain {1} when {2} is assign to {3}.";
        AvailableFieldValues = availableFieldValues;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue)
    {
        if(value == null)
            return true;
        
        var isValid = base.IsValid(value, dependentPropertyValue);
        if(isValid)
        {
            isValid = AvailableFieldValues.Any(availableFieldValue => Equals(availableFieldValue, value));
        }
        return isValid;
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