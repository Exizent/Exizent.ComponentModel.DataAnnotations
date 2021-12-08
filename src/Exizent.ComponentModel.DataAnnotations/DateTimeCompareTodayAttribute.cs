namespace Exizent.ComponentModel.DataAnnotations;

public class DateTimeCompareTodayAttribute : DateTimeCompareBaseAttribute
{
    public DateTimeCompareTodayAttribute(EqualityCondition equalityCondition)
        : base(equalityCondition)
    {
        
    }
    
    protected override DateTime? GetOtherDateTimeValue(ValidationContext validationContext)
        => DateTime.Today;
    
    protected override string FormatErrorMessage(ValidationContext validationContext) =>
        $"The field {validationContext.DisplayName} must be {FormatEqualityCondition()} today.";
}