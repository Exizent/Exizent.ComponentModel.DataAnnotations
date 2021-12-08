namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class RequiredIfAttributeTests
{
    class TestClass
    {
        public TestEnum Dependency { get; set; }

        [RequiredIf(nameof(Dependency), TestEnum.TestOne)]
        public string? Value { get; set; }
    }

    enum TestEnum
    {
        Default,
        TestOne,
        TestTwo
    }
    
    
    [Fact]
    public void ShouldBeValidWhenDependencyDoesNotMatchRequiredValue()
    {
        var model = new TestClass
        {
            Dependency = TestEnum.TestTwo
        };

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
          
    }


    [Fact]
    public void ValidationFailsIfDependentEnumValueMatchesRequiredIfValue()
    {
        var model = new TestClass
        {
            Dependency = TestEnum.TestOne
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestClass.Value)} is required if {nameof(TestClass.Dependency)} is {TestEnum.TestOne}.");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestClass.Value));
    }

}