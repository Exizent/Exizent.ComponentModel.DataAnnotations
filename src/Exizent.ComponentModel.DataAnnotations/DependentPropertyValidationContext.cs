using System.Reflection;

namespace Exizent.ComponentModel.DataAnnotations;

public class DependentPropertyValidationContext
{
    public ValidationContext ValidationContext { get; }
    public PropertyInfo DependentProperty { get; }

    internal DependentPropertyValidationContext(ValidationContext validationContext, PropertyInfo dependentProperty)
    {
        ValidationContext = validationContext;
        DependentProperty = dependentProperty;
    }

    public string GetDependentPropertyDisplayName()
    {
        var attributes = CustomAttributeExtensions.GetCustomAttributes(DependentProperty, true);
        var display = attributes.OfType<DisplayAttribute>().FirstOrDefault();

        return display?.GetName() ?? DependentProperty.Name;
    }
}