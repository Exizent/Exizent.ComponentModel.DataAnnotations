namespace Exizent.ComponentModel.DataAnnotations;

public class StringValueLengthAttribute : StringLengthAttribute
{
    public StringValueLengthAttribute(int maximumLength) : base(maximumLength)
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

        var invalidValues = keyValuePairs
            .Where(x => !IsValid(x.Value))
            .Select(x => $"{validationContext.MemberName}.{x.Key}")
            .ToArray();

        if (invalidValues.Length > 0)
        {
            result = new ValidationResult(FormatErrorMessage(validationContext.DisplayName), invalidValues);
        }

        return result;
    }
    
    public override string FormatErrorMessage(string value)
    {
        return base.FormatErrorMessage("values of " + value);
    }
}