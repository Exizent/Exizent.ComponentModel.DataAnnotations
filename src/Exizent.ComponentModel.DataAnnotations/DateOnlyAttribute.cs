namespace Exizent.ComponentModel.DataAnnotations;

public class DateOnlyAttribute : ValidationAttribute
{
    public DateOnlyAttribute()
        : base("The field {0} must be a date only.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true;
        
        var dateTime = (DateTime?)value;
        
        return dateTime.Value.TimeOfDay == TimeSpan.Zero;
    }
}