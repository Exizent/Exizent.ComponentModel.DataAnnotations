namespace Exizent.ComponentModel.DataAnnotations;

public class EmailAddressAttribute : DataTypeRegularExpressionAttribute
{
    public EmailAddressAttribute()
        : base(DataType.EmailAddress, @"[^\s@]+@[^\s\.]+\..+")
    {
        ErrorMessage = "The field {0} must be a valid email address.";
    }
}
