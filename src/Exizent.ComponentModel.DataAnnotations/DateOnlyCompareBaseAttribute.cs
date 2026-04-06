namespace Exizent.ComponentModel.DataAnnotations;

public abstract class DateOnlyCompareBaseAttribute : ValidationAttribute
{
    protected DateOnlyCompareBaseAttribute(EqualityCondition equalityCondition,
        string errorMessage = "The field {0} must be {1} {2}.")
        : base(errorMessage)
    {
        EqualityCondition = equalityCondition;
    }

    public EqualityCondition EqualityCondition { get; }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var result = ValidationResult.Success!;

        var current = (DateOnly?)value;

        var other = GetOtherDateOnlyValue(validationContext);

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

    protected virtual string FormatErrorMessage(ValidationContext validationContext, DateOnly? date)
        => string.Format(ErrorMessageString, validationContext.DisplayName,
            DateTimeCompareBaseAttribute.FormatEqualityCondition(EqualityCondition), date);

    protected abstract DateOnly? GetOtherDateOnlyValue(ValidationContext validationContext);

    private bool Compare(DateOnly? current, DateOnly? other)
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

