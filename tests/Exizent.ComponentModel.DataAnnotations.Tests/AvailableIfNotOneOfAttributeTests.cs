namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class AvailableIfNotOneOfAttributeTests
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
        [AvailableIfOneOf("PropertyThatDoesNotExist", 1)]
        public string? Value { get; set; }
    }
    
    class InvalidDependentPropertyTypeTestClass
    {
        public List<int>? DependentProperty { get; set; }
        [AvailableIfOneOf(nameof(DependentProperty), 1)]
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
            DependentProperty = new List<int>{4},
            Value = Guid.NewGuid().ToString()
        };

        Action action = () => ValidateModel(model);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The dependent property '*' must not be of type IEnumerable");
    }
    

    public class SinglePossibleDependentValueTests
    {
        const TestEnum PossibleDependentValue = TestEnum.Value2;

        class SinglePossibleDependentValueTestModel
        {
            public TestEnum? DependentValue { get; set; }

            [AvailableIfOneOf(nameof(DependentValue), PossibleDependentValue)]
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

        [Fact]
        public void ShouldBeValidForPossibleDependentValue()
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValue = PossibleDependentValue,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(TestEnum.Value1)]
        [InlineData(TestEnum.Value3)]
        public void ShouldBeInvalidWhenDependentPropertyIsNotOneOfPossibleDependentValues(TestEnum dependentValue)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValue = dependentValue,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(SinglePossibleDependentValueTestModel.DependentValue)} must be one of Value2 to assign {nameof(SinglePossibleDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(SinglePossibleDependentValueTestModel.Value));
        }
    }

    public class TwoPossibleDependentValueTests
    {
        private const TestEnum PossibleDependentValue1 = TestEnum.Value2;
        private const TestEnum PossibleDependentValue2 = TestEnum.Value3;

        class TwoPossibleDependentValueTestModel
        {
            public TestEnum? DependentValue { get; set; }

            [AvailableIfOneOf(nameof(DependentValue), PossibleDependentValue1, PossibleDependentValue2)]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeValidForNullPropertyValue()
        {
            var model = new TwoPossibleDependentValueTestModel
            {
                Value = null
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(PossibleDependentValue1)]
        [InlineData(PossibleDependentValue2)]
        public void ShouldBeValidForPossibleDependentValue( TestEnum dependentValue)
        {
            var model = new TwoPossibleDependentValueTestModel
            {
                DependentValue = dependentValue,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(TestEnum.Value1)]
        [InlineData(TestEnum.Value4)]
        public void ShouldBeInvalidWhenDependentPropertyIsNotSetToOneOfPossibleDependentValues(TestEnum dependentValue)
        {
            var model = new TwoPossibleDependentValueTestModel
            {
                DependentValue = dependentValue,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(TwoPossibleDependentValueTestModel.DependentValue)} must be one of {PossibleDependentValue1} or {PossibleDependentValue2} to assign {nameof(TwoPossibleDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(TwoPossibleDependentValueTestModel.Value));
        }
    }

    public class ThreePossibleDependentValueTests
    {
        private const TestEnum PossibleDependentValue1 = TestEnum.Value2;
        private const TestEnum PossibleDependentValue2 = TestEnum.Value3;
        private const TestEnum PossibleDependentValue3 = TestEnum.Value3;

        class ThreePossibleDependentValueTestModel
        {
            public TestEnum? DependentValues { get; set; }

            [AvailableIfOneOf(nameof(DependentValues),
                PossibleDependentValue1, PossibleDependentValue2, PossibleDependentValue3)]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeInvalidWhenDependentPropertyIsNotSetToOneOfPossibleDependentValues()
        {
            var model = new ThreePossibleDependentValueTestModel
            {
                DependentValues = TestEnum.Value1,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(ThreePossibleDependentValueTestModel.DependentValues)} must be one of {PossibleDependentValue1}, {PossibleDependentValue2} or {PossibleDependentValue2} to assign {nameof(ThreePossibleDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(ThreePossibleDependentValueTestModel.Value));
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