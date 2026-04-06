namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class DateOnlyCompareTodayAttributeTests
{
    class TestModel
    {
        [DateOnlyCompareToday(EqualityCondition.Equals)]
        public DateOnly? EqualsValue { get; set; }

        [DateOnlyCompareToday(EqualityCondition.NotEquals)]
        public DateOnly? NotEqualsValue { get; set; }

        [DateOnlyCompareToday(EqualityCondition.GreaterThan)]
        public DateOnly? GreaterThanValue { get; set; }

        [DateOnlyCompareToday(EqualityCondition.GreaterThanOrEquals)]
        public DateOnly? GreaterThanOrEqualsValue { get; set; }

        [DateOnlyCompareToday(EqualityCondition.LessThan)]
        public DateOnly? LessThanValue { get; set; }

        [DateOnlyCompareToday(EqualityCondition.LessThanOrEquals)]
        public DateOnly? LessThanOrEqualsValue { get; set; }
    }

    class InvalidEqualityConditionTestClass
    {
        [DateOnlyCompareToday((EqualityCondition)int.MaxValue)]
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
        var attribute = new DateOnlyCompareTodayAttribute(condition);

        attribute.EqualityCondition.Should().Be(condition);
    }

    public class EqualsEqualityConditionTests
    {
        [Fact]
        public void ShouldBeValidForDatesThatAreEqual()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { EqualsValue = today };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidForDatesThatAreNotEqual()
        {
            var model = new TestModel { EqualsValue = DateOnly.MaxValue };

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
            var model = new TestModel { NotEqualsValue = DateOnly.MaxValue };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidForDatesThatAreEqual()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
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
        public void ShouldBeValidForDatesThatAreGreaterThan(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { GreaterThanValue = today.AddDays(daysToAdd) };

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
            var model = new TestModel { GreaterThanValue = today.AddDays(daysToAdd) };

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
        public void ShouldBeValidForDatesThatAreGreaterThanOrEquals(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { GreaterThanOrEqualsValue = today.AddDays(daysToAdd) };

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
            var model = new TestModel { GreaterThanOrEqualsValue = today.AddDays(daysToAdd) };

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
        public void ShouldBeValidForDatesThatAreLessThan(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { LessThanValue = today.AddDays(daysToAdd) };

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
            var model = new TestModel { LessThanValue = today.AddDays(daysToAdd) };

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
        public void ShouldBeValidForDatesThatAreLessThanOrEquals(int daysToAdd)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var model = new TestModel { LessThanOrEqualsValue = today.AddDays(daysToAdd) };

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
            var model = new TestModel { LessThanOrEqualsValue = today.AddDays(daysToAdd) };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(TestModel.LessThanOrEqualsValue)} must be less than or equal to today.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.LessThanOrEqualsValue));
        }
    }
}

