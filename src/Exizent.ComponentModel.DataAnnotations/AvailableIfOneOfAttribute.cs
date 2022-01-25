using System.Collections;
using System.Runtime.CompilerServices;

namespace Exizent.ComponentModel.DataAnnotations;

public class AvailableIfOneOfAttribute : DependantPropertyBaseAttribute
{
    public object[] PossibleDependantPropertyValues { get; }

    public AvailableIfOneOfAttribute(string dependentProperty, params object[] possibleDependantPropertyValues)
        : base(dependentProperty, "The field {0} must be one of {1} to assign {2} to {3}.")
    {
        PossibleDependantPropertyValues = possibleDependantPropertyValues;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue)
    {
        if (value == null)
            return true;

        if (dependentPropertyValue is IEnumerable enumerable)
        {
            throw new InvalidOperationException(
                $"The dependent property '{DependentProperty}' must not be of type IEnumerable");
        }


        if (!PossibleDependantPropertyValues.Any(val => val.Equals(dependentPropertyValue)))
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