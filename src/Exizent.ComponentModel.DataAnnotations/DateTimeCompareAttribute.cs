namespace Exizent.ComponentModel.DataAnnotations;

public class DateTimeCompareAttribute : DateTimeCompareBaseAttribute
{
    public DateTimeCompareAttribute(string otherProperty, EqualityCondition equalityCondition)
        : base(equalityCondition)
    {
        OtherProperty = otherProperty;
    }

    public string OtherProperty { get; }

    protected override DateTime? GetOtherDateTimeValue(ValidationContext validationContext)
    {
        if (validationContext.ObjectType.GetProperty(OtherProperty) is not { } otherPropertyInfo)
        {
            throw new InvalidOperationException($"The other property '{OtherProperty}' does not exist");
        }

        return (DateTime?)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
    }
    
    protected override string FormatErrorMessage(ValidationContext validationContext, DateTime? dateTime)
        => string.Format(ErrorMessageString,
            validationContext.DisplayName,
            FormatEqualityCondition(EqualityCondition),
            OtherProperty);
}