using System.Collections;

namespace Exizent.ComponentModel.DataAnnotations;

public class ContainsAnyAttribute : ValidationAttribute
{
    private readonly Type _valueProviderType;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueProviderType">A type that implements IEnumerable which returns the valid values.</param>
    public ContainsAnyAttribute(Type valueProviderType)
    {
        _valueProviderType = valueProviderType;
    }

    public override bool IsValid(object? value)
    {
        EnsureCorrectValueProviderType();
        
        if (value == null)
            return true;

        var values = (IEnumerable)Activator.CreateInstance(_valueProviderType);

        return values.OfType<object>()
            .Any(x => Equals(x, value));
    }
    
    private void EnsureCorrectValueProviderType()
    {
        if (!typeof(IEnumerable).IsAssignableFrom(_valueProviderType))
        {
            throw new InvalidOperationException($"{nameof(ContainsAnyAttribute)} requires a type that implements IEnumerable");
        }
    }
}