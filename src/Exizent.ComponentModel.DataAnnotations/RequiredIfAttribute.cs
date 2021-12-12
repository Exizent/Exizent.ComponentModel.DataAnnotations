namespace Exizent.ComponentModel.DataAnnotations;

public class RequiredIfAttribute : DependantPropertyBaseAttribute
{
    private readonly object? _requiredValue;
    private readonly RequiredAttribute _innerAttribute = new();

    public RequiredIfAttribute(string dependentProperty, object? requiredValue)
        : base(dependentProperty, "The field {0} is required if {1} is {2}.")
    {
        _requiredValue = requiredValue;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue)
    {
        return Equals(_requiredValue, dependentPropertyValue)
            ? ValidateInnerAttribute(value)
            : true;
        
    }
    
    private bool ValidateInnerAttribute(object? value)
    {
        return _innerAttribute.IsValid(value);
    }

    protected override string FormatErrorMessage(object? value, object? dependentPropertyValue,
        DependentPropertyValidationContext validationContext)
    {
        return string.Format(ErrorMessageString, 
            validationContext.ValidationContext.DisplayName,
            validationContext.GetDependentPropertyDisplayName(),
            FormatRequiredValue());
    }

    private string FormatRequiredValue()
        => ValueFormatter.FormatValue(_requiredValue);
}
