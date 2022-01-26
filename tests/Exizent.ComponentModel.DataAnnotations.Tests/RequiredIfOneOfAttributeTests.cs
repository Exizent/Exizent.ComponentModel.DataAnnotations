namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class RequiredIfOneOfAttributeTests
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
        [RequiredIfOneOf("PropertyThatDoesNotExist", 1)]
        public string? Value { get; set; }
    }

    class InvalidDependentPropertyTypeTestClass
    {
        public List<int>? DependentProperty { get; set; }

        [RequiredIfOneOf(nameof(DependentProperty), 1)]
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
            DependentProperty = new List<int>{ 4},
            Value = Guid.NewGuid().ToString()
        };

        Action action = () => ValidateModel(model);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The dependent property '*' must not be of type IEnumerable");
    }


    public class SingleRequiredDependentValueTests
    {
        const TestEnum RequiredDependentValue2 = TestEnum.Value2;

        class SingleRequiredDependentValueTestModel
        {
            public TestEnum? DependentValue { get; set; }

            [RequiredIfOneOf(nameof(DependentValue), RequiredDependentValue2)]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeValidForNullDependentPropertyValueAndNullValue()
        {
            var model = new SingleRequiredDependentValueTestModel
            {
                DependentValue = null,
                Value = null
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeValidForNullDependentPropertyValue()
        {
            var model = new SingleRequiredDependentValueTestModel
            {
                DependentValue = null,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeValidForRequiredDependentValueAndSetValue()
        {
            var model = new SingleRequiredDependentValueTestModel
            {
                DependentValue = RequiredDependentValue2,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeValidForNotRequiredDependentValueAndNullValue()
        {
            var model = new SingleRequiredDependentValueTestModel
            {
                DependentValue = TestEnum.Value1,
                Value = null
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidWhenRequiredDependentValueAndValueIsNull()
        {
            var model = new SingleRequiredDependentValueTestModel
            {
                DependentValue = RequiredDependentValue2,
                Value = null
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(SingleRequiredDependentValueTestModel.Value)} is required when {nameof(SingleRequiredDependentValueTestModel.DependentValue)} is one of {RequiredDependentValue2}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(SingleRequiredDependentValueTestModel.Value));
        }
    }

    public class ThreeRequiredDependentValueTests
    {
        private const TestEnum RequiredDependentValue2 = TestEnum.Value2;
        private const TestEnum RequiredDependentValue3 = TestEnum.Value3;
        private const TestEnum RequiredDependentValue4 = TestEnum.Value4;

        class ThreeRequiredDependentValueTestModel
        {
            public TestEnum? DependentValue { get; set; }

            [RequiredIfOneOf(nameof(DependentValue),
                RequiredDependentValue2, RequiredDependentValue3, RequiredDependentValue4)]
            public string? Value { get; set; }
        }

        [Theory]
        [InlineData(RequiredDependentValue2)]
        [InlineData(RequiredDependentValue3)]
        [InlineData(RequiredDependentValue4)]
        public void ShouldBeInvalidWhenHasRequiredDependentValuesAndValueIsNull(TestEnum requiredDependentValue)
        {
            var model = new ThreeRequiredDependentValueTestModel
            {
                DependentValue = requiredDependentValue,
                Value = null
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(ThreeRequiredDependentValueTestModel.Value)} is required when {nameof(ThreeRequiredDependentValueTestModel.DependentValue)} is one of {RequiredDependentValue2}, {RequiredDependentValue3} and {RequiredDependentValue4}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(ThreeRequiredDependentValueTestModel.Value));
        }

        [Fact]
        public void ShouldBeValidWhenHasNotRequiredDependentValuesAndValueIsNull()
        {
            var model = new ThreeRequiredDependentValueTestModel
            {
                DependentValue = TestEnum.Value1,
                Value = null
            };

            var (_, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
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