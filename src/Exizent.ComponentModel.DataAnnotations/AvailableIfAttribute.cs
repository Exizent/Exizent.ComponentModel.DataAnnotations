using System.Reflection;

namespace Exizent.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class AvailableIfAttribute : ValidationAttribute
{
    public string DependentProperty { get; }
    public object? DependantPropertyRequiredValue { get; }

    public AvailableIfAttribute(string dependentProperty, object? dependantPropertyRequiredValue)
    {
        DependentProperty = dependentProperty;
        DependantPropertyRequiredValue = dependantPropertyRequiredValue;
    }

    private bool IsValid(object? value, object? dependentPropertyValue)
    {
        return value is null
               || Equals(dependentPropertyValue, DependantPropertyRequiredValue);
    }
    
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var result = ValidationResult.Success!;
        var dependentProperty = validationContext.ObjectType.GetRuntimeProperty(DependentProperty);
        var dependentPropertyValue = dependentProperty.GetValue(validationContext.ObjectInstance, null);

        if (!IsValid(value, dependentPropertyValue))
        {
            var memberNames = validationContext.MemberName != null
                ? new[] { validationContext.MemberName }
                : null;
            
            result = new ValidationResult($"{dependentProperty.Name} must be set to {FormatDependantPropertyRequiredValue()} to assign {value} to {validationContext.MemberName}", memberNames);
        }

        return result;
    }

    private string FormatDependantPropertyRequiredValue()
        => DependantPropertyRequiredValue is null ? "null" : DependantPropertyRequiredValue.ToString();
}