using System.ComponentModel.DataAnnotations;

namespace Exizent.ComponentModel.DataAnnotations;

public class NationalInsuranceNumberAttribute : RegularExpressionAttribute
{
    public NationalInsuranceNumberAttribute()
        : base(@"^([abceghj-prstw-zABCEGHJ-PRSTW-Z]{2}[0-9]{2}[0-9]{2}[0-9]{2}[a-dfmA-DFM]{1,1})|([abceghj-prstw-zABCEGHJ-PRSTW-Z]{2} ?[0-9]{2} [0-9]{2} [0-9]{2} [a-dfmA-DFM]{1,1})$")
    {
        ErrorMessage = "The field {0} must be a valid national insurance number.";
    }
}