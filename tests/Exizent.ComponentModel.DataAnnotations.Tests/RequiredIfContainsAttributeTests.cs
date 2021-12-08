﻿namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class RequiredIfContainsAttributeTests
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
        [RequiredIfContains("PropertyThatDoesNotExist", 1)]
        public string? Value { get; set; }
    }

    class InvalidDependentPropertyTypeTestClass
    {
        public int DependentProperty { get; set; }

        [RequiredIfContains(nameof(DependentProperty), 1)]
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

    [Fact]
    public void ShouldThrowInvalidOperationExceptionForInvalidDependentPropertyType()
    {
        var model = new InvalidDependentPropertyTypeTestClass
        {
            DependentProperty = 4,
            Value = Guid.NewGuid().ToString()
        };

        Action action = () => ValidateModel(model);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The dependent property '*' must be of type IEnumerable");
    }


    public class SingleRequiredDependentValueTests
    {
        const TestEnum RequiredDependentValue = TestEnum.Value2;

        class SingleRequiredDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [RequiredIfContains(nameof(DependentValues), RequiredDependentValue)]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeValidForNullDependentPropertyValue()
        {
            var model = new SingleRequiredDependentValueTestModel
            {
                DependentValues = null,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(RequiredDependentValue)]
        [InlineData(TestEnum.Value1, RequiredDependentValue)]
        [InlineData(RequiredDependentValue, TestEnum.Value2)]
        [InlineData(TestEnum.Value1, RequiredDependentValue, TestEnum.Value3)]
        public void ShouldBeValidForRequiredDependentValueAndSetValue(params TestEnum[] extraValues)
        {
            var model = new SingleRequiredDependentValueTestModel
            {
                DependentValues = extraValues,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(RequiredDependentValue)]
        [InlineData(TestEnum.Value1, RequiredDependentValue)]
        [InlineData(RequiredDependentValue, TestEnum.Value2)]
        [InlineData(TestEnum.Value1, RequiredDependentValue, TestEnum.Value3)]
        public void ShouldBeInvalidWhenRequiredDependentValueAndValueIsNull(params TestEnum[] dependentValues)
        {
            var model = new SingleRequiredDependentValueTestModel
            {
                DependentValues = dependentValues,
                Value = null
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(SingleRequiredDependentValueTestModel.DependentValues)} must contain Value2 to assign {nameof(SingleRequiredDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(SingleRequiredDependentValueTestModel.Value));
        }
    }

    public class TwoRequiredDependentValueTests
    {
        private const TestEnum RequiredDependentValue2 = TestEnum.Value2;
        private const TestEnum RequiredDependentValue3 = TestEnum.Value3;

        class RequiredDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [RequiredIfContains(nameof(DependentValues), RequiredDependentValue2, RequiredDependentValue3)]
            public string? Value { get; set; }
        }

        [Theory]
        [InlineData(RequiredDependentValue2, RequiredDependentValue3)]
        [InlineData(TestEnum.Value1, RequiredDependentValue2, RequiredDependentValue3)]
        [InlineData(TestEnum.Value1, RequiredDependentValue2, RequiredDependentValue3, TestEnum.Value4)]
        public void ShouldBeValidForRequiredDependentValuesAndSetValue(params TestEnum[] extraValues)
        {
            var model = new RequiredDependentValueTestModel
            {
                DependentValues = extraValues,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(TestEnum.Value1)]
        [InlineData(RequiredDependentValue2)]
        [InlineData(RequiredDependentValue3)]
        [InlineData(TestEnum.Value4)]
        [InlineData(TestEnum.Value1, RequiredDependentValue2, TestEnum.Value4)]
        [InlineData(TestEnum.Value1, RequiredDependentValue3, TestEnum.Value4)]
        public void ShouldBeInvalidWhenRequiredDependentValuesIsNotContained(params TestEnum[] dependentValues)
        {
            var model = new RequiredDependentValueTestModel
            {
                DependentValues = dependentValues,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(RequiredDependentValueTestModel.DependentValues)} must contain {RequiredDependentValue2} and {RequiredDependentValue3} to assign {nameof(RequiredDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(RequiredDependentValueTestModel.Value));
        }
    }

    public class ThreeRequiredDependentValueTests
    {
        private const TestEnum RequiredDependentValue2 = TestEnum.Value2;
        private const TestEnum RequiredDependentValue3 = TestEnum.Value3;
        private const TestEnum RequiredDependentValue4 = TestEnum.Value4;

        class ThreeRequiredDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [RequiredIfContains(nameof(DependentValues),
                RequiredDependentValue2, RequiredDependentValue3, RequiredDependentValue4)]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeInvalidWhenRequiredDependentValuesIsNotContained()
        {
            var model = new ThreeRequiredDependentValueTestModel
            {
                DependentValues = Array.Empty<TestEnum>(),
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(ThreeRequiredDependentValueTestModel.DependentValues)} must contain {RequiredDependentValue2}, {RequiredDependentValue3} and {RequiredDependentValue4} to assign {nameof(ThreeRequiredDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(ThreeRequiredDependentValueTestModel.Value));
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