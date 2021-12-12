using System.Collections;

namespace Exizent.ComponentModel.DataAnnotations;

public class AvailableIfContainsAttribute : DependantPropertyBaseAttribute
{
    public object[] PossibleDependantPropertyValues { get; }

    public AvailableIfContainsAttribute(string dependentProperty, params object[] possibleDependantPropertyValues) :
        base(dependentProperty, "The field {0} must contain {1} to assign {2} to {3}.")
    {
        PossibleDependantPropertyValues = possibleDependantPropertyValues;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue)
    {
        if(value == null)
            return true;
        
        if (dependentPropertyValue is not IEnumerable enumerable)
        {
            throw new InvalidOperationException(
                $"The dependent property '{DependentProperty}' must be of type IEnumerable");
        }

        var dependentValues = enumerable.Cast<object>().ToArray();

        if (dependentValues.All(x => !PossibleDependantPropertyValues.Any(val => val.Equals(x))))
        {
            return false;
        }

        return true;
    }

    protected override string FormatErrorMessage(object? value, object? dependentPropertyValue,
        DependentPropertyValidationContext validationContext)
    {
        return string.Format(ErrorMessageString, DependentProperty, FormatPossibleDependantPropertyValues(),
            validationContext.ValidationContext.DisplayName, value);
    }

    private string FormatPossibleDependantPropertyValues()
        => ValueFormatter.FormatOrValues(PossibleDependantPropertyValues);
}
