﻿using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Exizent.ComponentModel.DataAnnotations;

public class AvailableIfContainsAttribute : ValidationAttribute
{
    public string DependentProperty { get; }
    public object[] PossibleDependantPropertyValues { get; }

    public AvailableIfContainsAttribute(string dependentProperty, params object[] possibleDependantPropertyValues)
        : base("The {0} field must contain {1} to assign {2} to {3}.")
    {
        DependentProperty = dependentProperty;
        PossibleDependantPropertyValues = possibleDependantPropertyValues;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var result = ValidationResult.Success!;
        if (value == null)
            return result;

        if (validationContext.ObjectType.GetProperty(DependentProperty) is not { } dependentPropertyInfo)
        {
            throw new InvalidOperationException($"The dependent property '{DependentProperty}' does not exist");
        }

        if (dependentPropertyInfo.GetValue(validationContext.ObjectInstance, null) is not IEnumerable enumerable)
        {
            throw new InvalidOperationException($"The dependent property '{DependentProperty}' must be of type IEnumerable");
        }

        var dependentValues = enumerable.Cast<object>().ToArray();

        if (dependentValues.All(x => !PossibleDependantPropertyValues.Any(val => val.Equals(x))))
        {
            var memberNames = validationContext.MemberName != null
                ? new[] { validationContext.MemberName }
                : null;

            result = new ValidationResult(FormatErrorMessage(value, validationContext),
                memberNames);
        }


        return result;
    }

    private string FormatErrorMessage(object value, ValidationContext validationContext)
    {
        return string.Format(ErrorMessageString, DependentProperty, FormatDependantPropertyRequiredValues(),
            validationContext.DisplayName, value);
    }

    private string FormatDependantPropertyRequiredValues()
    {
        var commaValued = PossibleDependantPropertyValues.Take(PossibleDependantPropertyValues.Length - 1);

        var formatted = string.Join(", ", commaValued);

        if (string.IsNullOrEmpty(formatted))
        {
            return PossibleDependantPropertyValues.Single().ToString();
        }

        return formatted + " or " + PossibleDependantPropertyValues.Last();
    }
}