namespace Exizent.ComponentModel.DataAnnotations;

public class DateOnlyCompareTodayAttribute : DateOnlyCompareBaseAttribute
{
    public DateOnlyCompareTodayAttribute(EqualityCondition equalityCondition)
        : base(equalityCondition, "The field {0} must be {1} today.")
    {
    }

    protected override DateOnly? GetOtherDateOnlyValue(ValidationContext validationContext)
        => DateOnly.FromDateTime(DateTime.Today);
}

