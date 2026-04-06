namespace Exizent.ComponentModel.DataAnnotations;

public class DateOnlyCompareAttribute : DateOnlyCompareBaseAttribute
{
    public DateOnlyCompareAttribute(string otherProperty, EqualityCondition equalityCondition)
        : base(equalityCondition)
    {
        OtherProperty = otherProperty;
    }

    public string OtherProperty { get; }

    protected override DateOnly? GetOtherDateOnlyValue(ValidationContext validationContext)
    {
        if (validationContext.ObjectType.GetProperty(OtherProperty) is not { } otherPropertyInfo)
        {
            throw new InvalidOperationException($"The other property '{OtherProperty}' does not exist");
        }

        return (DateOnly?)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
    }

    protected override string FormatErrorMessage(ValidationContext validationContext, DateOnly? date)
        => string.Format(ErrorMessageString,
            validationContext.DisplayName,
            DateTimeCompareBaseAttribute.FormatEqualityCondition(EqualityCondition),
            OtherProperty);
}

