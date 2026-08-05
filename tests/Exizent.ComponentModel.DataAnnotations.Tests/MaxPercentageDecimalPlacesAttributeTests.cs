namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class MaxPercentageDecimalPlacesAttributeTests
{
    class TestModel
    {
        // Displayed as a percentage with a max of 4 decimal places (e.g. 98.1234),
        // but passed in as a normalised fraction (e.g. 0.981234), i.e. 6 decimal places.
        [MaxPercentageDecimalPlaces(4)]
        public decimal? Value { get; set; }
    }

    [Theory]
    [InlineData(0.000001)] // 0.0001%
    [InlineData(0.010000)] // 1%
    [InlineData(0.981234)] // 98.1234%
    [InlineData(1)] // 100%
    public void ShouldBeValidWhenNormalisedFractionIsWithinMaxDecimalPlaces(decimal value)
    {
        var model = new TestModel { Value = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeValidWhenValueIsNull()
    {
        var model = new TestModel { Value = null };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0.9812345)] // 98.12345%, one decimal place too many
    [InlineData(0.0000001)] // 0.00001%, one decimal place too many
    public void ShouldBeInvalidWhenNormalisedFractionExceedsMaxDecimalPlaces(decimal value)
    {
        var model = new TestModel { Value = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should()
            .Be($"The field {nameof(TestModel.Value)} must be passed as a decimal fraction with a maximum of 6 decimal places (a percentage with up to 4 decimal places, e.g. 98.1234% as 0.981234).");
        results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.Value));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ShouldThrowWhenMaxDecimalPlacesIsNegative(int maxDecimalPlaces)
    {
        var act = () => new MaxPercentageDecimalPlacesAttribute(maxDecimalPlaces);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
