using System.ComponentModel.DataAnnotations;

namespace Exizent.ComponentModel.DataAnnotations;

public class EmailAddressAttribute : RegularExpressionAttribute
{
    public EmailAddressAttribute()
        : base(@"[^\s@]+@[^\s\.]+\..+")
    {
        ErrorMessage = "The field {0} must be a valid email address.";
    }
}
