namespace Exizent.ComponentModel.DataAnnotations;

public class DateTimeCompareAttribute : DateTimeCompareBaseAttribute
{
    private readonly string _otherProperty;

    public DateTimeCompareAttribute(string otherProperty, EqualityCondition equalityCondition)
        : base(equalityCondition)
    {
        _otherProperty = otherProperty;
    }

    protected override DateTime? GetOtherDateTimeValue(ValidationContext validationContext)
    {
        if (validationContext.ObjectType.GetProperty(_otherProperty) is not { } otherPropertyInfo)
        {
            throw new InvalidOperationException($"The other property '{_otherProperty}' does not exist");
        }

        return (DateTime?)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
    }
    
    protected override string FormatErrorMessage(ValidationContext validationContext, DateTime? dateTime)
        => string.Format(ErrorMessageString, validationContext.DisplayName, FormatEqualityCondition(), _otherProperty);
}