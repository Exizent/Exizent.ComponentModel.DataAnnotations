namespace Exizent.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
public abstract class DependantPropertyBaseAttribute : ValidationAttribute
{
    public string DependentProperty { get; }

    protected DependantPropertyBaseAttribute(string dependentProperty, string errorMessage)
        :base(errorMessage)
    {
        DependentProperty = dependentProperty;
    }

    protected abstract bool IsValid(object? value, object? dependentPropertyValue);

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var result = ValidationResult.Success!;
        if (validationContext.ObjectType.GetProperty(DependentProperty) is not { } dependentPropertyInfo)
        {
            throw new InvalidOperationException($"The dependent property '{DependentProperty}' does not exist");
        }
        
        var dependentPropertyValue = dependentPropertyInfo.GetValue(validationContext.ObjectInstance, null);

        if (!IsValid(value, dependentPropertyValue))
        {
            var memberNames = validationContext.MemberName != null
                ? new[] { validationContext.MemberName }
                : null;

            result = new ValidationResult(
                FormatErrorMessage(value, dependentPropertyValue,
                    new DependentPropertyValidationContext(validationContext, dependentPropertyInfo)), memberNames);
        }

        return result;
    }

    protected abstract string FormatErrorMessage(object? value,
        object? dependentPropertyValue, DependentPropertyValidationContext validationContext);
}