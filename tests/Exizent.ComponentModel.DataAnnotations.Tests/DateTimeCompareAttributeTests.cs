using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class DateTimeCompareAttributeTests
{
    class TestModel
    {
        public DateTime? Value { get; set; }
        
        [DateTimeCompareAttribute(nameof(Value), EqualityCondition.Equals)]
        public DateTime? EqualsValue { get; set; }
        
        [DateTimeCompareAttribute(nameof(Value), EqualityCondition.NotEquals)]
        public DateTime? NotEqualsValue { get; set; }
        
        [DateTimeCompareAttribute(nameof(Value), EqualityCondition.GreaterThan)]
        public DateTime? GreaterThanValue { get; set; }
        
        [DateTimeCompareAttribute(nameof(Value), EqualityCondition.GreaterThanOrEquals)]
        public DateTime? GreaterThanOrEqualsValue { get; set; }
        
        [DateTimeCompareAttribute(nameof(Value), EqualityCondition.LessThan)]
        public DateTime? LessThanValue { get; set; }
        
        [DateTimeCompareAttribute(nameof(Value), EqualityCondition.LessThanOrEquals)]
        public DateTime? LessThanOrEqualsValue { get; set; }
    }

    class InvalidOtherPropertyTestClass
    {
        [DateTimeCompareAttribute("PropertyThatDoesNotExist", EqualityCondition.Equals)]
        public DateTime? Value { get; set; }
    }
    
    class InvalidEqualityConditionTestClass
    {
        public DateTime? OtherValue { get; set; }
        [DateTimeCompareAttribute(nameof(OtherValue), (EqualityCondition)int.MaxValue)]
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
    public void ShouldBeValidForNullOtherPropertyValue()
    {
        var today = DateTime.Today;
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
        var today = DateTime.Today;
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
            Value = DateTime.Today
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
            OtherValue = DateTime.Today, 
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
            var model = new TestModel { Value = today, EqualsValue = today };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidForDatesThatAreNotEqual()
        {
            var model = new TestModel { Value = DateTime.MinValue, EqualsValue = DateTime.MaxValue };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"{nameof(TestModel.EqualsValue)} must be equal to {nameof(TestModel.Value)}.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.EqualsValue));
        }
    }
    
    public class NotEqualsEqualityConditionTests
    {
        [Fact]
        public void ShouldBeValidForDatesThatAreNotEqual()
        {
            var model = new TestModel { Value = DateTime.MinValue, NotEqualsValue = DateTime.MaxValue };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidForDatesThatAreEqual()
        {
            var today = DateTime.Today;
            var model = new TestModel { Value = today, NotEqualsValue = today };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"{nameof(TestModel.NotEqualsValue)} must be not equal to {nameof(TestModel.Value)}.");
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
            var model = new TestModel { Value = today, GreaterThanValue = today.AddSeconds(secondsToAdd) };

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
            var model = new TestModel { Value = today, GreaterThanValue = today.AddSeconds(secondsToAdd) };
           
            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"{nameof(TestModel.GreaterThanValue)} must be greater than {nameof(TestModel.Value)}.");
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
            var model = new TestModel { Value = today, GreaterThanOrEqualsValue = today.AddSeconds(secondsToAdd) };

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
            var model = new TestModel { Value = today, GreaterThanOrEqualsValue = today.AddSeconds(secondsToAdd) };
            
            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"{nameof(TestModel.GreaterThanOrEqualsValue)} must be greater than or equal to {nameof(TestModel.Value)}.");
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
            var model = new TestModel { Value = today, LessThanValue = today.AddSeconds(secondsToAdd) };

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
            var model = new TestModel { Value = today, LessThanValue = today.AddSeconds(secondsToAdd) };
           
            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"{nameof(TestModel.LessThanValue)} must be less than {nameof(TestModel.Value)}.");
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
            var model = new TestModel { Value = today, LessThanOrEqualsValue = today.AddSeconds(secondsToAdd) };

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
            var model = new TestModel { Value = today, LessThanOrEqualsValue = today.AddSeconds(secondsToAdd) };
            
            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"{nameof(TestModel.LessThanOrEqualsValue)} must be less than or equal to {nameof(TestModel.Value)}.");
            results[0].MemberNames.Should().BeEquivalentTo(nameof(TestModel.LessThanOrEqualsValue));
        }
    }
}