namespace Exizent.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class AvailableIfAttribute : DependantPropertyBaseAttribute
{
    public object? DependantPropertyRequiredValue { get; }

    public AvailableIfAttribute(string dependentProperty, object? dependantPropertyRequiredValue)
        :base(dependentProperty, "{0} must be set to {1} to assign {2} to {3}")
    {
        DependantPropertyRequiredValue = dependantPropertyRequiredValue;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue) =>
        value is null
        || Equals(dependentPropertyValue, DependantPropertyRequiredValue);

    protected override string FormatErrorMessage(object? value, object? dependentPropertyValue,
        DependentPropertyValidationContext validationContext)
    {
        return string.Format(ErrorMessageString,
            validationContext.GetDependentPropertyDisplayName(),
            FormatDependantPropertyRequiredValue(),
            value,
            validationContext.ValidationContext.DisplayName
        );
    }

    private string FormatDependantPropertyRequiredValue()
        => DependantPropertyRequiredValue is null ? "null" : DependantPropertyRequiredValue.ToString();
}