namespace Exizent.ComponentModel.DataAnnotations;

public class DateTimeCompareTodayAttribute : DateTimeCompareBaseAttribute
{
    public DateTimeCompareTodayAttribute(EqualityCondition equalityCondition)
        : base(equalityCondition, "The field {0} must be {1} today.")
    {
    }

    protected override DateTime? GetOtherDateTimeValue(ValidationContext validationContext)
        => DateTime.Today;
}