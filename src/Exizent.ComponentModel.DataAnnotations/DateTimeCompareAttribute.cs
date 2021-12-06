using System;
using System.ComponentModel.DataAnnotations;

namespace Exizent.ComponentModel.DataAnnotations;

public class DateTimeCompareAttribute : ValidationAttribute
{
    private readonly EqualityCondition _equalityCondition;
    private readonly string _otherProperty;

    public DateTimeCompareAttribute(string otherProperty, EqualityCondition equalityCondition)
    {
        _equalityCondition = equalityCondition;
        _otherProperty = otherProperty;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var result = ValidationResult.Success!;

        var current = (DateTime?)value;

        if (validationContext.ObjectType.GetProperty(_otherProperty) is not { } otherPropertyInfo)
        {
            throw new InvalidOperationException($"The other property '{_otherProperty}' does not exist");
        }
        
        if(!Enum.IsDefined(typeof(EqualityCondition), _equalityCondition))
            throw new InvalidOperationException($"Invalid equality condition {_equalityCondition}");
        
        var other = (DateTime?)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

        if (!Compare(current, other))
        {
            var memberNames = validationContext.MemberName != null
                ? new[] { validationContext.MemberName }
                : null;

            result = new ValidationResult(
                $"{validationContext.MemberName} must be {FormatEqualityCondition()} {otherPropertyInfo?.Name}.",
                memberNames);
        }
    
        return result;
    }

    private string FormatEqualityCondition() =>
        _equalityCondition switch
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
        
        return _equalityCondition switch
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