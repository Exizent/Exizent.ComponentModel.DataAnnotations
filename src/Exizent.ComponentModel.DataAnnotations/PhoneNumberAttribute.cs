namespace Exizent.ComponentModel.DataAnnotations;

public class PhoneNumberAttribute : RegularExpressionAttribute
{
    public PhoneNumberAttribute()
        : base(@"^[0-9]{5,12}$")
    {
        ErrorMessage = "The field {0} must be a valid phone number.";
    }
}