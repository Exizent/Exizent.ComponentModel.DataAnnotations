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

        var propertyInfo = evaluated.ElementAt(0).GetType().GetProperty(ChildPropertyName);
        if (propertyInfo is null)
            throw new InvalidOperationException($"The property '{ChildPropertyName}' is missing");

        decimal sum = 0m;
        int index = 0;
        foreach (var item in evaluated)
        {
            sum += Convert.ToDecimal(propertyInfo.GetValue(item));
            if (sum > _expected || index == evaluated.Length - 1)
                break;
            index++;
        }

        return sum == (decimal)Expected ? ValidationResult.Success : new ValidationResult(
            $"The '{ChildPropertyName}' members of '{validationContext.MemberName}' must sum to {Expected}", 
            new []{$"{validationContext.MemberName}[{index}].{ChildPropertyName}"}
        );
    }
}