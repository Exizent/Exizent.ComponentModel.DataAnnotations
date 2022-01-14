namespace Exizent.ComponentModel.DataAnnotations;

public class StringKeyLengthAttribute : StringLengthAttribute
{
    public StringKeyLengthAttribute(int maximumLength) : base(maximumLength)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Validates and throws exception based on the range setup in the constructor
        IsValid(null);
        
        var result = ValidationResult.Success;

        if(value == null)
        {
            return result;
        }
        
        if(value is not IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            throw new InvalidOperationException("The value must be an IEnumerable<KeyValuePair<string, string>>");
        }

        var invalidKeys = keyValuePairs.Where(x => !IsValid(x.Key))
            .Select(x => $"{validationContext.MemberName}.{x.Key}")
            .ToArray();

        if (invalidKeys.Length > 0)
        {
            result = new ValidationResult(FormatErrorMessage(validationContext.DisplayName), invalidKeys);
        }

        return result;
    }

    public override string FormatErrorMessage(string name)
    {
        return base.FormatErrorMessage("keys of " + name);
    }
}