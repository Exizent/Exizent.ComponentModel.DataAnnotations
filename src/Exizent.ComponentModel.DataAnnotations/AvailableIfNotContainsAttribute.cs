using System.Collections;

namespace Exizent.ComponentModel.DataAnnotations;

public class AvailableIfNotContainsAttribute : AvailableIfContainsAttribute
{
    public object[] NotDependantPropertyValues { get; }

    public AvailableIfNotContainsAttribute(string dependentProperty, params object[] notDependantPropertyValues) :
        base(dependentProperty, notDependantPropertyValues)
    {
        ErrorMessage = "The field {0} must not contain {1} to assign {2} to {3}.";
        NotDependantPropertyValues = notDependantPropertyValues;
    }

    protected override bool IsValid(object? value, object? dependentPropertyValue)
    {
        if(value == null)
            return true;
        
       return !base.IsValid(value, dependentPropertyValue);
    }
}
