using System.Collections;

namespace Exizent.ComponentModel.DataAnnotations;

public class RequiredIfContainsAttribute : DependantPropertyBaseAttribute
{
    public object[] DependantPropertyRequiredValues { get; }
    private readonly RequiredAttribute _innerAttribute = new();

    public RequiredIfContainsAttribute(string dependentProperty, params object[] dependantPropertyRequiredValues)
        : base(dependentProperty, "The field {0} is required when {1} contains {2}.")
    {
        DependantPropertyRequiredValues = dependantPropertyRequiredValues;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue)
    {
        if (dependentPropertyValue == null)
            return true;

        if (dependentPropertyValue is not IEnumerable enumerable)
        {
            throw new InvalidOperationException(
                $"The dependent property '{DependentProperty}' must be of type IEnumerable");
        }

        var dependentValues = enumerable.Cast<object>().ToArray();

        if (dependentValues.Length == 0)
            return true;

        if (DependantPropertyRequiredValues.All(x => dependentValues.Any(val => Equals(val, x))))
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