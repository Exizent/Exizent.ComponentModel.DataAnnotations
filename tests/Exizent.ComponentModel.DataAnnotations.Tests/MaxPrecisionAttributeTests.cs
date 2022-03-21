namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class MaxPrecisionAttributeTests
{
    class TestModel
    {
        [MaxPrecision(4)]
        public decimal? Value { get; set; }
    }

    [Theory]
    [InlineData(0.0001)]
    [InlineData(0.001)]
    [InlineData(0.01)]
    [InlineData(0.1)]
    [InlineData(1)]
    [InlineData(1.00000)]
    [InlineData(1.0001)]
    public void ShouldBeValidWhenMaxPrecisionIsHonoured(decimal value)
    {
        var model = new TestModel { Value = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }
    
    [Theory]
    [InlineData(0.00001)]
    [InlineData(99.99991)]
    public void ShouldBeInvalidWhenMaxPrecisionExceeded(decimal value)
    {
        var model = new TestModel { Value = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should()
            .Be($"The field {nameof(TestModel.Value)} must have a max precision of 4 decimal places.");
        results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.Value));
    }
    
}