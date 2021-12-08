using System.Reflection;

namespace Exizent.ComponentModel.DataAnnotations;

public class RequiredIfAttribute : ValidationAttribute
{
    private readonly RequiredAttribute _innerAttribute;
    private readonly object _requiredValue;
    private readonly string _dependentProperty;

    public RequiredIfAttribute(string dependentProperty, object requiredValue)
    {
        _innerAttribute = new RequiredAttribute();
        _dependentProperty = dependentProperty;
        _requiredValue = requiredValue;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var dependentProperty = validationContext.ObjectType.GetRuntimeProperty(_dependentProperty);
        var dependentPropertyValue = dependentProperty.GetValue(validationContext.ObjectInstance, null);
        
        return _requiredValue.Equals(dependentPropertyValue)
            ? ValidateInnerAttribute(value, validationContext)
            : ValidationResult.Success!;
    }

    private ValidationResult ValidateInnerAttribute(object? value, ValidationContext validationContext)
    {
        if (!_innerAttribute.IsValid(value))
        {
            return new ValidationResult($"The field {validationContext.DisplayName} is required if {_dependentProperty} is {_requiredValue}.",
                new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success!;
    }
}
