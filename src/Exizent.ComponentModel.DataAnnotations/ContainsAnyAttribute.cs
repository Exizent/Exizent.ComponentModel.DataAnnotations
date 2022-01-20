using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Exizent.ComponentModel.DataAnnotations;

public class ContainsAnyAttribute : ValidationAttribute
{
    private readonly Type _valueProviderType;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueProviderType">A type that implements IEnumerable which returns the valid values or a type that contains a `Contains` method.</param>
    public ContainsAnyAttribute(Type valueProviderType)
    {
        _valueProviderType = valueProviderType;
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true;

        var instance = Activator.CreateInstance(_valueProviderType);

        if(ContainsMethodHelper.TryGetContainsMethod(instance, out var containsMethod))
        {
            return containsMethod(value);
        }

        if (instance is not IEnumerable values)
        {
            throw new InvalidOperationException($"{nameof(ContainsAnyAttribute)} requires a type that implements IEnumerable or a type that contains a `Contains` method.");
        }

        return values.OfType<object>()
            .Any(x => Equals(x, value));
    }
    
    private static class ContainsMethodHelper
    {
        public static bool TryGetContainsMethod(object value, [MaybeNullWhen(false)] out Func<object, bool> contains)
        {
            var methodInfo = value.GetType().GetMethod("Contains");
            if (methodInfo != null && methodInfo.GetParameters().Length == 1 && methodInfo.ReturnType == typeof(bool))
            {
                contains = input => (bool)methodInfo.Invoke(value, new[] { input });
                return true;
            }
            
            contains = null;
            return false;
        }
    }
}