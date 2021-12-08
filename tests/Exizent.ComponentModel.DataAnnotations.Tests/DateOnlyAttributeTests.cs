namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class DateOnlyAttributeTests
{
    class TestModel
    {
        [DateOnly]
        public DateTime? DateTime { get; set; }
    }

    [Fact]
    public void ShouldBeValidForNull()
    {
        var model = new TestModel { DateTime = null };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeValidForDateOnly()
    {
        var model = new TestModel { DateTime = DateTime.Today };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }
    
    [Fact]
    public void ShouldBeInvalidForDateWithTimePart()
    {
        var dateTime = new DateTime(2021, 01, 02, 08, 30, 00);
        var model = new TestModel { DateTime = dateTime };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.DateTime)} must be a date only.");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.DateTime));
    }
}