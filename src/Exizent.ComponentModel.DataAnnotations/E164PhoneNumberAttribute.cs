namespace Exizent.ComponentModel.DataAnnotations;

public class E164PhoneNumberAttribute : RegularExpressionAttribute
{
    /// <summary>
    /// E164 format numbers start with a '+' followed by a non-zero digit and up to 14 digits in the range 0 to 9
    /// reference: https://www.twilio.com/docs/glossary/what-e164
    /// </summary>
    public E164PhoneNumberAttribute()
        : base(@"^\+[1-9]\d{1,14}$")
    {
        ErrorMessage = "The field {0} must be a valid E164 format phone number e.g. +447777******";
    }
}