using System.Globalization;

namespace Exizent.ComponentModel.DataAnnotations;

public class CurrencyAttribute : ValidationAttribute
{
    private static readonly CultureInfo EnglishUnitedKingdomCulture = CultureInfo.GetCultureInfo("en-GB");

    public CurrencyAttribute()
        : base("The field {0} is not a valid currency value.")
    {
        
    }
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true;
        }

        return IsLessThan(EnglishUnitedKingdomCulture.NumberFormat.CurrencyDecimalDigits, (decimal)value);
    }
    
    private static bool IsLessThan(int decimalPlaces, decimal dec)
    {
        var value = dec * (int)Math.Pow(10, decimalPlaces);
        
        return value == Math.Floor(value);
    }
}