using System.ComponentModel.DataAnnotations;

namespace Exizent.ComponentModel.DataAnnotations;

public class PostcodeAttribute : DataTypeRegularExpressionAttribute
{
    const string regex =
        @"^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) ?[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$";

    public PostcodeAttribute()
        : base(DataType.PostalCode, regex)
    {
        this.ErrorMessage = "The field {0} must be a valid postcode.";
    }
}