using System.Collections;

[AttributeUsage(AttributeTargets.Property)]
public class SumsToAttribute : ValidationAttribute
{
    private readonly decimal _expected;
    public string ChildPropertyName { get; }
    public double Expected { get; }

    public SumsToAttribute(string childPropertyName, double expected)
    {
        ChildPropertyName = childPropertyName;
        Expected = expected;
        _expected = (decimal) expected;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is not IEnumerable enumerable)
            throw new InvalidOperationException(
                $"The property '{validationContext.MemberName}' must be of type IEnumerable");

        var evaluated = enumerable.Cast<object>().ToArray();
        if (evaluated.Length == 0)
            return ValidationResult.Success;

        decimal sum = 0m;
        foreach (var item in evaluated)
        {
            var propertyInfo = item.GetType().GetProperty(ChildPropertyName);
            if (propertyInfo is null)
                throw new InvalidOperationException($"The property '{ChildPropertyName}' is missing");

            sum += Convert.ToDecimal(propertyInfo.GetValue(item));
            if (sum > _expected)
                break;
        }

        return sum == (decimal)Expected ? ValidationResult.Success : new ValidationResult(
            $"The {ChildPropertyName} members of {validationContext.MemberName} must sum to {Expected}.", 
            new []{ChildPropertyName}
        );
    }
}