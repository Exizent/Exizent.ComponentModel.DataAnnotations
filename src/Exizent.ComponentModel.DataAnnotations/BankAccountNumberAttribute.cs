using System.ComponentModel.DataAnnotations;

namespace Exizent.ComponentModel.DataAnnotations;

public class BankAccountNumberAttribute : RegularExpressionAttribute
{
    public BankAccountNumberAttribute()
        : base(@"^[0-9]{6,10}$")
    {
        ErrorMessage = "The field {0} must be a valid bank account number.";
    }
}