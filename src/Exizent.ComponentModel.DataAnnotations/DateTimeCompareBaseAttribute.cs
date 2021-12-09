namespace Exizent.ComponentModel.DataAnnotations;

public abstract class DateTimeCompareBaseAttribute : ValidationAttribute
{
    protected DateTimeCompareBaseAttribute(EqualityCondition equalityCondition,
        string errorMessage = "The field {0} must be {1} {2}.")
        : base(errorMessage)
    {
        EqualityCondition = equalityCondition;
    }

    public EqualityCondition EqualityCondition { get; }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var result = ValidationResult.Success!;

        var current = (DateTime?)value;

        var other = GetOtherDateTimeValue(validationContext);

        if (!Enum.IsDefined(typeof(EqualityCondition), EqualityCondition))
            throw new InvalidOperationException($"Invalid equality condition {EqualityCondition}");


        if (!Compare(current, other))
        {
            var memberNames = validationContext.MemberName != null
                ? new[] { validationContext.MemberName }
                : null;

            result = new ValidationResult(
                FormatErrorMessage(validationContext, other),
                memberNames);
        }

        return result;
    }

    protected virtual string FormatErrorMessage(ValidationContext validationContext, DateTime? dateTime)
        => string.Format(ErrorMessageString, validationContext.DisplayName, FormatEqualityCondition(EqualityCondition), dateTime);

    protected abstract DateTime? GetOtherDateTimeValue(ValidationContext validationContext);

    public static string FormatEqualityCondition(EqualityCondition condition) =>
        condition switch
        {
            EqualityCondition.Equals => "equal to",
            EqualityCondition.NotEquals => "not equal to",
            EqualityCondition.GreaterThan => "greater than",
            EqualityCondition.GreaterThanOrEquals => "greater than or equal to",
            EqualityCondition.LessThan => "less than",
            EqualityCondition.LessThanOrEquals => "less than or equal to",
            _ => throw new ArgumentOutOfRangeException()
        };

    private bool Compare(DateTime? current, DateTime? other)
    {
        if (current is null || other is null)
            return true;

        return EqualityCondition switch
        {
            EqualityCondition.Equals => current == other,
            EqualityCondition.NotEquals => current != other,
            EqualityCondition.GreaterThan => current > other,
            EqualityCondition.GreaterThanOrEquals => current >= other,
            EqualityCondition.LessThan => current < other,
            EqualityCondition.LessThanOrEquals => current <= other,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}