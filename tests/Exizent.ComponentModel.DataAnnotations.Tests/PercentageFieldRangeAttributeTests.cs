namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class PercentageFieldRangeAttributeTests
{
    class TestModel
    {
        // 0%-100%, displayed with a max of 4 decimal places (e.g. 98.1234),
        // but passed in as a normalised fraction (e.g. 0.981234), i.e. 6 decimal places.
        [PercentageFieldRange(0, 1, 4)]
        public decimal? Value { get; set; }
    }

    class CustomRangeMessageTestModel
    {
        [PercentageFieldRange(0, 1, 4, ErrorMessage = "{0} must sit within {1}% to {2}%.")]
        public decimal? Value { get; set; }
    }

    class CustomDecimalPlacesMessageTestModel
    {
        [PercentageFieldRange(0, 1, 4, DecimalPlacesErrorMessage = "{0} needs {1}%-{2}%, <= {4} raw dp ({3} display dp).")]
        public decimal? Value { get; set; }
    }

    [Theory]
    [InlineData(0)] // 0%
    [InlineData(0.000001)] // 0.0001%
    [InlineData(0.010000)] // 1%
    [InlineData(0.981234)] // 98.1234%
    [InlineData(1)] // 100%
    public void ShouldBeValidWhenWithinRangeAndMaxDecimalPlacesHonoured(decimal value)
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
    [InlineData(-0.1)] // -10%
    [InlineData(1.1)] // 110%
    public void ShouldBeInvalidWhenOutsideRange(decimal value)
    {
        var model = new TestModel { Value = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should()
            .Be($"The field {nameof(TestModel.Value)} must be a percentage between 0% and 100%.");
        results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.Value));
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
            .Be($"The field {nameof(TestModel.Value)} must be a percentage between 0% and 100% with up to 4 decimal places (passed in as a decimal fraction with a maximum of 6 decimal places, e.g. 98.1234% as 0.981234).");
        results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.Value));
    }

    [Fact]
    public void ShouldAllowOverridingTheRangeErrorMessageAndStillReferenceThePercentageBounds()
    {
        var model = new CustomRangeMessageTestModel { Value = -0.1m };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should()
            .Be($"{nameof(CustomRangeMessageTestModel.Value)} must sit within 0% to 100%.");
    }

    [Fact]
    public void ShouldAllowOverridingTheDecimalPlacesErrorMessageAndStillReferenceThePercentageBoundsAndDecimalPlaceValues()
    {
        var model = new CustomDecimalPlacesMessageTestModel { Value = 0.9812345m };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should()
            .Be($"{nameof(CustomDecimalPlacesMessageTestModel.Value)} needs 0%-100%, <= 6 raw dp (4 display dp).");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ShouldThrowWhenMaxDecimalPlacesIsNegative(int maxDecimalPlaces)
    {
        var act = () => new PercentageFieldRangeAttribute(0, 1, maxDecimalPlaces);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
