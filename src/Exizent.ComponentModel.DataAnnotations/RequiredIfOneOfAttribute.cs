using System.Collections;

namespace Exizent.ComponentModel.DataAnnotations;

public class RequiredIfOneOfAttribute : DependantPropertyBaseAttribute
{
    public object[] DependantPropertyRequiredValues { get; }
    private readonly RequiredAttribute _innerAttribute = new();

    public RequiredIfOneOfAttribute(string dependentProperty, params object[] dependantPropertyRequiredValues)
        : base(dependentProperty, "The field {0} is required when {1} is one of {2}.")
    {
        DependantPropertyRequiredValues = dependantPropertyRequiredValues;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue)
    {
        if (dependentPropertyValue == null)
            return true;

        if (dependentPropertyValue is IEnumerable)
        {
            throw new InvalidOperationException(
                $"The dependent property '{DependentProperty}' must not be of type IEnumerable");
        }

        if (DependantPropertyRequiredValues.Any(x => Equals(dependentPropertyValue, x)))
        {
            return ValidateInnerAttribute(value);
        }

        return true;
    }

    private bool ValidateInnerAttribute(object? value)
    {
        return _innerAttribute.IsValid(value);
    }

    protected override string FormatErrorMessage(object? value, object? dependentPropertyValue,
        DependentPropertyValidationContext validationContext)
    {
        return string.Format(
            ErrorMessageString,
            validationContext.ValidationContext.DisplayName,
            DependentProperty,
            FormatPossibleDependantPropertyValues()
        );
    }

    private string FormatPossibleDependantPropertyValues()
        => ValueFormatter.FormatAndValues(DependantPropertyRequiredValues);
}