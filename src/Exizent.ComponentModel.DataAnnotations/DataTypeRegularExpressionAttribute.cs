using System.Globalization;
using System.Text.RegularExpressions;

namespace Exizent.ComponentModel.DataAnnotations;

public abstract class DataTypeRegularExpressionAttribute : DataTypeAttribute
{
    private Regex? _regex;

    protected DataTypeRegularExpressionAttribute(DataType dataType, string pattern)
        : base(dataType)
    {
        Pattern = pattern;
    }

    /// <summary>
    ///     Gets the regular expression pattern to use
    /// </summary>
    private string Pattern { get; }


    /// <summary>
    ///     Override of <see cref="ValidationAttribute.IsValid(object)" />
    /// </summary>
    /// <remarks>
    ///     This override performs the specific regular expression matching of the given <paramref name="value" />
    /// </remarks>
    /// <param name="value">The value to test for validity.</param>
    /// <returns><c>true</c> if the given value matches the current regular expression pattern</returns>
    /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
    /// <exception cref="ArgumentException"> is thrown if the <see cref="Pattern" /> is not a valid regular expression.</exception>
    public override bool IsValid(object? value)
    {
        SetupRegex();

        var stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);

        if (string.IsNullOrEmpty(stringValue))
        {
            return true;
        }

        var m = _regex!.Match(stringValue);

        // We are looking for an exact match, not just a search hit. This matches what
        // the RegularExpressionValidator control does
        return (m.Success && m.Index == 0 && m.Length == stringValue.Length);
    }

    /// <summary>
    ///     Override of <see cref="ValidationAttribute.FormatErrorMessage" />
    /// </summary>
    /// <remarks>This override provide a formatted error message describing the pattern</remarks>
    /// <param name="name">The user-visible name to include in the formatted message.</param>
    /// <returns>The localized message to present to the user</returns>
    /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
    /// <exception cref="ArgumentException"> is thrown if the <see cref="Pattern" /> is not a valid regular expression.</exception>
    public override string FormatErrorMessage(string name)
    {
        SetupRegex();

        return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Pattern);
    }


    /// <summary>
    ///     Sets up the <see cref="_regex" /> property from the <see cref="Pattern" /> property.
    /// </summary>
    /// <exception cref="ArgumentException"> is thrown if the current <see cref="Pattern" /> cannot be parsed</exception>
    /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
    private void SetupRegex()
    {
        if (_regex == null)
        {
            if (string.IsNullOrEmpty(Pattern))
            {
                throw new InvalidOperationException("The Pattern property must be set.");
            }

            _regex = new Regex(Pattern, default, TimeSpan.FromMilliseconds(2000));
        }
    }
}