namespace Exizent.ComponentModel.DataAnnotations;

public class PaymentByAccountNumberAttribute : RegularExpressionAttribute
{
    public PaymentByAccountNumberAttribute()
        : base(@"^(PBA).{0,47}$")
    {
        ErrorMessage = "The field {0} must start with 'PBA' and not exceed 50 characters in length.";
    }
}