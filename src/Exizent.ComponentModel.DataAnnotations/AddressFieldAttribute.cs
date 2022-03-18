namespace Exizent.ComponentModel.DataAnnotations
{
    public class AddressFieldAttribute : RegularExpressionAttribute
    {
        public AddressFieldAttribute() : base(@"^[^\s]+(\s+[^\s]+)*$")
        {
            ErrorMessage = "The field {0} must not start or end with space";
        }
    }
}