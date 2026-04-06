namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class DateOnlyCompareAttributeTests
{
    class TestModel
    {
        public DateOnly? Value { get; set; }

        [DateOnlyCompareAttribute(nameof(Value), EqualityCondition.Equals)]
        public DateOnly? EqualsValue { get; set; }

        [DateOnlyCompareAttribute(nameof(Value), EqualityCondition.NotEquals)]
        public DateOnly? NotEqualsValue { get; set; }

        [DateOnlyCompareAttribute(nameof(Value), EqualityCondition.GreaterThan)]
        public DateOnly? GreaterThanValue { get; set; }

        [DateOnlyCompareAttribute(nameof(Value), EqualityCondition.GreaterThanOrEquals)]
        public DateOnly? GreaterThanOrEqualsValue { get; set; }

        [DateOnlyCompareAttribute(nameof(Value), EqualityCondition.LessThan)]
        public DateOnly? LessThanValue { get; set; }

        [DateOnlyCompareAttribute(nameof(Value), EqualityCondition.LessThanOrEquals)]
        public DateOnly? LessThanOrEqualsValue { get; set; }
    }

    class InvalidOtherPropertyTestClass
    {
        [DateOnlyCompareAttribute("PropertyThatDoesNotExist", EqualityCondition.Equals)]
        public DateOnly? Value { get; set; }
    }

    class InvalidEqualityConditionTestClass
    {
        public DateOnly? OtherValue { get; set; }
        [DateOnlyCompareAttribute(nameof(OtherValue), (EqualityCondition)int.MaxValue)]
        public DateOnly? Value { get; set; }
    }

    private static (List<ValidationResult> results, bool isValid) ValidateModel<TModel>(TModel model) where TModel : notnull
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);
        return (results, isValid);
    }

    [Fact]
    public void ShouldBeValidForNullOtherPropertyValue()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var model = new TestModel
        {
            Value = null,
            EqualsValue = today,
            NotEqualsValue = today,
            GreaterThanValue = today,
            GreaterThanOrEqualsValue = today,
            LessThanValue = today,
            LessThanOrEqualsValue = today,
        };

        var (results, isValid) = ValidateModel(model);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeValidForNullPropertyValue()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var model = new TestModel
        {
            Value = today
        };

        var (results, isValid) = ValidateModel(model);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void ShouldThrowInvalidOperationExceptionForInvalidOtherProperty()
    {
        var model = new InvalidOtherPropertyTestClass
        {
            Value = DateOnly.FromDateTime(DateTime.Today)
        };

        Action action = () => ValidateModel(model);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The other property '*' does not exist");
    }

    [Fact]
    public void ShouldThrowArgumentExceptionForInvalidEqualityCondition()
    {
        var model = new InvalidEqualityConditionTestClass
        {
            OtherValue = DateOnly.FromDateTime(DateTime.Today),
            Value = DateOnly.FromDateTime(DateTime.Today)
        };

        Action action = () => ValidateModel(model);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Invalid equality condition *");
    }

    [Theory]
    [InlineData(EqualityCondition.Equals)]
    [InlineData(EqualityCondition.NotEquals)]
    [InlineData(EqualityCondition.GreaterThan)]
    [InlineData(EqualityCondition.GreaterThanOrEquals)]
    [InlineData(EqualityCondition.LessThan)]
    [InlineData(EqualityCondition.LessThanOrEquals)]
    public void ShouldExposeEqualityCondition(EqualityCondition condition)
    {
        var attribute = new DateOnlyCompareAttribute("Test", condition);

        attribute.EqualityCondition.Should().Be(condition);
    }

    [Fact]
    public void ShouldExposeOtherPropertyName()
    {
        var attribute = new DateOnlyCompareAttribute("Test", EqualityCondition.Equals);

        attribute.OtherProperty.Should().Be("Test");
    }

    public class EqualsEqualityConditionTests
    {
        [Fact]
        public void ShouldBeValidForDatesThatAreEqual()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, EqualsValue = today };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidForDatesThatAreNotEqual()
        {
            var model = new TestModel { Value = DateOnly.MinValue, EqualsValue = DateOnly.MaxValue };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.EqualsValue)} must be equal to {nameof(TestModel.Value)}.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.EqualsValue));
        }
    }

    public class NotEqualsEqualityConditionTests
    {
        [Fact]
        public void ShouldBeValidForDatesThatAreNotEqual()
        {
            var model = new TestModel { Value = DateOnly.MinValue, NotEqualsValue = DateOnly.MaxValue };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidForDatesThatAreEqual()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, NotEqualsValue = today };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.NotEqualsValue)} must be not equal to {nameof(TestModel.Value)}.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.NotEqualsValue));
        }
    }

    public class GreaterThanEqualityConditionTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ShouldBeValidForDatesThatAreGreaterThan(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, GreaterThanValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ShouldBeInvalidForDatesThatAreNotGreaterThan(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, GreaterThanValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.GreaterThanValue)} must be greater than {nameof(TestModel.Value)}.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.GreaterThanValue));
        }
    }

    public class GreaterThanOrEqualsEqualityConditionTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ShouldBeValidForDatesThatAreGreaterThanOrEquals(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, GreaterThanOrEqualsValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void ShouldBeInvalidForDatesThatAreNotGreaterThanOrEquals(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, GreaterThanOrEqualsValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.GreaterThanOrEqualsValue)} must be greater than or equal to {nameof(TestModel.Value)}.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.GreaterThanOrEqualsValue));
        }
    }

    public class LessThanEqualityConditionTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void ShouldBeValidForDatesThatAreLessThan(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, LessThanValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ShouldBeInvalidForDatesThatAreNotLessThan(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, LessThanValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.LessThanValue)} must be less than {nameof(TestModel.Value)}.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.LessThanValue));
        }
    }

    public class LessThanOrEqualsEqualityConditionTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ShouldBeValidForDatesThatAreLessThanOrEquals(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, LessThanOrEqualsValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ShouldBeInvalidForDatesThatAreNotLessThanOrEquals(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { Value = today, LessThanOrEqualsValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.LessThanOrEqualsValue)} must be less than or equal to {nameof(TestModel.Value)}.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.LessThanOrEqualsValue));
        }
    }
}

