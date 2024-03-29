﻿namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class AvailableValuesIfContainsAttributeTests
{
    public enum TestEnum
    {
        Value1,
        Value2,
        Value3,
        Value4
    }

    class InvalidDependentPropertyTestClass
    {
        [AvailableValuesIfContains("PropertyThatDoesNotExist", 1)]
        public string? Value { get; set; }
    }

    class InvalidDependentPropertyTypeTestClass
    {
        public int DependentProperty { get; set; }

        [AvailableValuesIfContains(nameof(DependentProperty), 1)]
        public string? Value { get; set; }
    }

    [Fact]
    public void ShouldThrowInvalidOperationExceptionForInvalidDependentProperty()
    {
        var model = new InvalidDependentPropertyTestClass
        {
            Value = Guid.NewGuid().ToString()
        };

        Action action = () => ValidateModel(model);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The dependent property '*' does not exist");
    }

    public class DependentValueTests
    {
        const TestEnum PossibleDependentValue = TestEnum.Value2;

        class DependentValueTestModel
        {
            public TestEnum? DependentValue { get; set; }

            [AvailableValuesIfContains(nameof(DependentValue), PossibleDependentValue, "Hello", "World")]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeValidForNullPropertyValue()
        {
            var model = new DependentValueTestModel
            {
                Value = null
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Hello")]
        [InlineData("World")]
        public void ShouldBeValidForPossibleDependentAndValue(string value)
        {
            var model = new DependentValueTestModel
            {
                DependentValue = PossibleDependentValue,
                Value = value
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }
        
        [Theory]
        [InlineData("Hello")]
        [InlineData("World")]
        [InlineData("anything")]
        public void ShouldBeValidForAnyValueWhenNotPossibleDependentValue(string value)
        {
            var model = new DependentValueTestModel
            {
                DependentValue = TestEnum.Value1,
                Value = value
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }


        [Fact]
        public void ShouldBeInvalidWhenPossibleDependentValueAndFieldValuesDoNotMatch()
        {
            var model = new DependentValueTestModel
            {
                DependentValue = PossibleDependentValue,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(DependentValueTestModel.Value)} must contain Hello or World when {nameof(DependentValueTestModel.DependentValue)} is assigned to {PossibleDependentValue}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(DependentValueTestModel.Value));
        }
    }

    public class SinglePossibleDependentValueIEnumerableTests
    {
        const TestEnum PossibleDependentValue = TestEnum.Value2;

        class SinglePossibleDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [AvailableValuesIfContains(nameof(DependentValues), PossibleDependentValue, "Hello", "World")]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeValidForNullPropertyValue()
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                Value = null
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Hello")]
        [InlineData("World")]
        public void ShouldBeValidForPossibleDependentAndValue(string value)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = new[] { PossibleDependentValue },
                Value = value
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }
        
        [Theory]
        [InlineData("Hello")]
        [InlineData("World")]
        [InlineData("anything")]
        public void ShouldBeValidForAnyValueWhenNotPossibleDependentValue(string value)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = new[] { TestEnum.Value1, TestEnum.Value4 },
                Value = value
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }


        [Fact]
        public void ShouldBeInvalidWhenPossibleDependentValueAndFieldValuesDoNotMatch()
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = new[] { PossibleDependentValue },
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            
            results[0].ErrorMessage.Should()
                .Be($"The field {nameof(SinglePossibleDependentValueTestModel.Value)} must contain Hello or World when {nameof(SinglePossibleDependentValueTestModel.DependentValues)} is assigned to {PossibleDependentValue}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(SinglePossibleDependentValueTestModel.Value));
        }
    }

    private static (List<ValidationResult> results, bool isValid) ValidateModel<TModel>(TModel model)
        where TModel : notnull
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);
        return (results, isValid);
    }
}