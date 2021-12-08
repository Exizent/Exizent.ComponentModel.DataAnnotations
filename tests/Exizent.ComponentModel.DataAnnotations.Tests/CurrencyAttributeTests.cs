using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class CurrencyAttributeTests
{
    class TestModel
    {
        [CurrencyAttribute]
        public decimal? Value { get; set; }
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(0.02)]
    [InlineData(9.99)]
    [InlineData(9999.00)]
    [InlineData(9999)]
    public void ShouldBeValidForValidEnglishUnitedKingdomCurrencyValue(decimal value)
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
    [InlineData(0.001)]
    [InlineData(0.002)]
    public void ShouldBeInvalidForInvalidEnglishUnitedKingdomCurrencyValue(decimal value)
    {
        var model = new TestModel { Value = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should()
            .Be($"The field {nameof(TestModel.Value)} is not a valid currency value.");
        results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.Value));
    }
    
}