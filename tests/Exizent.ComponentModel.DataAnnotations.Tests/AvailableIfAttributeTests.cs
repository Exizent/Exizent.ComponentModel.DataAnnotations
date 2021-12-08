namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class AvailableIfAttributeTests
{
    class BooleanTestModel
    {
        public bool BoolProp { get; set; }
        [AvailableIf(nameof(BoolProp), true)]
        public string? AvailableIfBoolProp { get; set; }
    }
    
    class NullBooleanTestModel
    {
        public bool? BoolProp { get; set; }
        [AvailableIf(nameof(BoolProp), null)]
        public string? AvailableIfBoolProp { get; set; }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShouldBeValidForAnyDependantPropertyRequiredValueWhenAvailableIfPropertyValueIsNull(bool dependantPropertyRequiredValue)
    {
        var model = new BooleanTestModel { BoolProp = dependantPropertyRequiredValue, AvailableIfBoolProp = null };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }
        
    [Fact]
    public void ShouldBeValidWhenDependantPropertyRequiredValueMatches()
    {
        var model = new BooleanTestModel { BoolProp = true, AvailableIfBoolProp = "test" };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }
    
    [Fact]
    public void ShouldBeInvalidWhenDependantPropertyRequiredValueDoesNotMatch()
    {
        var model = new BooleanTestModel { BoolProp = false, AvailableIfBoolProp = "test" };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"{nameof(BooleanTestModel.BoolProp)} must be set to {true} to assign {model.AvailableIfBoolProp} to {nameof(BooleanTestModel.AvailableIfBoolProp)}");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(BooleanTestModel.AvailableIfBoolProp));
    }

    [Fact]
    public void ShouldBeValidWhenDependantPropertyRequiredValueIsNull()
    {
        var model = new NullBooleanTestModel { BoolProp = null, AvailableIfBoolProp = "test" };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShouldBeInvalidWhenDependantPropertyRequiredValueIsNotNull(bool boolProp)
    {
        var model = new NullBooleanTestModel { BoolProp = boolProp, AvailableIfBoolProp = "test" };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"{nameof(BooleanTestModel.BoolProp)} must be set to null to assign {model.AvailableIfBoolProp} to {nameof(BooleanTestModel.AvailableIfBoolProp)}");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(BooleanTestModel.AvailableIfBoolProp));
    }
}