namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class DateTimeCompareTodayAttributeTests
{
    class TestModel
    {
        [DateTimeCompareToday(EqualityCondition.Equals)]
        public DateTime? EqualsValue { get; set; }
        
        [DateTimeCompareToday( EqualityCondition.NotEquals)]
        public DateTime? NotEqualsValue { get; set; }
        
        [DateTimeCompareToday(EqualityCondition.GreaterThan)]
        public DateTime? GreaterThanValue { get; set; }
        
        [DateTimeCompareToday(EqualityCondition.GreaterThanOrEquals)]
        public DateTime? GreaterThanOrEqualsValue { get; set; }
        
        [DateTimeCompareToday( EqualityCondition.LessThan)]
        public DateTime? LessThanValue { get; set; }
        
        [DateTimeCompareToday( EqualityCondition.LessThanOrEquals)]
        public DateTime? LessThanOrEqualsValue { get; set; }
    }
    
    class InvalidEqualityConditionTestClass
    {
        [DateTimeCompareToday( (EqualityCondition)int.MaxValue)]
        public DateTime? Value { get; set; }
    }
    
    private static (List<ValidationResult> results, bool isValid) ValidateModel<TModel>(TModel model) where TModel : notnull
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);
        return (results, isValid);
    }
    
    [Fact]
    public void ShouldBeValidForNullPropertyValue()
    {
        var model = new TestModel();

        var (results, isValid) = ValidateModel(model);
        
        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }
    
   [Fact]
    public void ShouldThrowArgumentExceptionForInvalidEqualityCondition()
    {
        var model = new InvalidEqualityConditionTestClass
        {
            Value = DateTime.Today
        };

        Action action = () => ValidateModel(model);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Invalid equality condition *");
    }
    
    public class EqualsEqualityConditionTests
    {
        [Fact]
        public void ShouldBeValidForDatesThatAreEqual()
        {
            var today = DateTime.Today;
            var model = new TestModel { EqualsValue = today };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidForDatesThatAreNotEqual()
        {
            var model = new TestModel { EqualsValue = DateTime.MaxValue };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.EqualsValue)} must be equal to today.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.EqualsValue));
        }
    }
    
    public class NotEqualsEqualityConditionTests
    {
        [Fact]
        public void ShouldBeValidForDatesThatAreNotEqual()
        {
            var model = new TestModel { NotEqualsValue = DateTime.MaxValue };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidForDatesThatAreEqual()
        {
            var today = DateTime.Today;
            var model = new TestModel { NotEqualsValue = today };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.NotEqualsValue)} must be not equal to today.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.NotEqualsValue));
        }
    }
    
    public class GreaterThanEqualityConditionTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ShouldBeValidForDatesThatAreGreaterThan(int secondsToAdd)
        {
            var today = DateTime.Today;
            var model = new TestModel { GreaterThanValue = today.AddSeconds(secondsToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ShouldBeInvalidForDatesThatAreNotGreaterThan(int secondsToAdd)
        {
            var today = DateTime.Today;
            var model = new TestModel { GreaterThanValue = today.AddSeconds(secondsToAdd) };
           
            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.GreaterThanValue)} must be greater than today.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.GreaterThanValue));
        }
    }
    
    public class GreaterThanOrEqualsEqualityConditionTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ShouldBeValidForDatesThatAreGreaterThanOrEquals(int secondsToAdd)
        {
            var today = DateTime.Today;
            var model = new TestModel { GreaterThanOrEqualsValue = today.AddSeconds(secondsToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void ShouldBeInvalidForDatesThatAreNotGreaterThanOrEquals(int secondsToAdd)
        {
            var today = DateTime.Today;
            var model = new TestModel { GreaterThanOrEqualsValue = today.AddSeconds(secondsToAdd) };
            
            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.GreaterThanOrEqualsValue)} must be greater than or equal to today.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.GreaterThanOrEqualsValue));
        }
    }
    
    public class LessThanEqualityConditionTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void ShouldBeValidForDatesThatAreLessThan(int secondsToAdd)
        {
            var today = DateTime.Today;
            var model = new TestModel { LessThanValue = today.AddSeconds(secondsToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ShouldBeInvalidForDatesThatAreNotLessThan(int secondsToAdd)
        {
            var today = DateTime.Today;
            var model = new TestModel { LessThanValue = today.AddSeconds(secondsToAdd) };
           
            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.LessThanValue)} must be less than today.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.LessThanValue));
        }
    }
    
    public class LessThanOrEqualsEqualityConditionTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ShouldBeValidForDatesThatAreLessThanOrEquals(int secondsToAdd)
        {
            var today = DateTime.Today;
            var model = new TestModel { LessThanOrEqualsValue = today.AddSeconds(secondsToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ShouldBeInvalidForDatesThatAreNotLessThanOrEquals(int secondsToAdd)
        {
            var today = DateTime.Today;
            var model = new TestModel { LessThanOrEqualsValue = today.AddSeconds(secondsToAdd) };
            
            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.LessThanOrEqualsValue)} must be less than or equal to today.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.LessThanOrEqualsValue));
        }
    }
}