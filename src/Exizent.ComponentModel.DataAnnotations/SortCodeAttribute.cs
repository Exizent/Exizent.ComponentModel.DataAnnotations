namespace Exizent.ComponentModel.DataAnnotations;

public class SortCodeAttribute : RegularExpressionAttribute
{
    const string SortCodeWithDashesRegExPattern = @"^[0-9]{2}-?[0-9]{2}-?[0-9]{2}$";
    const string SortCodeWithoutDashesRegExPattern = @"^[0-9]{6}$";

    public SortCodeAttribute(bool allowDashes = false)
        : base(allowDashes ? SortCodeWithDashesRegExPattern : SortCodeWithoutDashesRegExPattern)
    {
        ErrorMessage = "The field {0} must be a valid sort code.";
    }
}