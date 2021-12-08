using System.Collections;

namespace Exizent.ComponentModel.DataAnnotations;

public class RequiredIfContainsAttribute : DependantPropertyBaseAttribute
{
    public object[] DependantPropertyRequiredValues { get; }
    private readonly RequiredAttribute _innerAttribute = new();

    public RequiredIfContainsAttribute(string dependentProperty, params object[] dependantPropertyRequiredValues)
        : base(dependentProperty, "The field {0} must contain {1} to assign {2} to {3}.")
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

        if(DependantPropertyRequiredValues.All(x => dependentValues.Any(val => Equals(val, x))))
        {
            return ValidateInnerAttribute(value);
        }

        return false;
    }

    private bool ValidateInnerAttribute(object? value)
    {
        return _innerAttribute.IsValid(value);
    }
    
    protected override string FormatErrorMessage(object? value, object? dependentPropertyValue,
        DependentPropertyValidationContext validationContext)
    {
        return string.Format(ErrorMessageString, DependentProperty, FormatPossibleDependantPropertyValues(),
            validationContext.ValidationContext.DisplayName, value);
    }

    private string FormatPossibleDependantPropertyValues()
    {
        if (DependantPropertyRequiredValues is { Length: 1 })
            return DependantPropertyRequiredValues[0].ToString();

        var formatted = string.Join(", ", DependantPropertyRequiredValues[..^1]);

        return formatted + " and " + DependantPropertyRequiredValues[^1];
    }
}