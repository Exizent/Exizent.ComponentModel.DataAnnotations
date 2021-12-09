namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class DateTimeCompareBaseAttributeTests
{
    [Theory]
    [InlineData(EqualityCondition.Equals, "equal to")]
    [InlineData(EqualityCondition.NotEquals, "not equal to")]
    [InlineData(EqualityCondition.GreaterThan, "greater than")]
    [InlineData(EqualityCondition.GreaterThanOrEquals, "greater than or equal to")]
    [InlineData(EqualityCondition.LessThan, "less than")]
    [InlineData(EqualityCondition.LessThanOrEquals, "less than or equal to")]
    public void ShouldFormatEqualityCondition(EqualityCondition condition, string text)
    {
        DateTimeCompareBaseAttribute.FormatEqualityCondition(condition).Should().Be(text);
    }
}