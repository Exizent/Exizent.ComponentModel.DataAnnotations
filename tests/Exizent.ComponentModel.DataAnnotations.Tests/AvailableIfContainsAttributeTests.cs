namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class AvailableIfContainsAttributeTests
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
        [AvailableIfContains("PropertyThatDoesNotExist", 1)]
        public string? Value { get; set; }
    }
    
    class InvalidDependentPropertyTypeTestClass
    {
        public int DependentProperty { get; set; }
        [AvailableIfContains(nameof(DependentProperty), 1)]
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
    

    public class SinglePossibleDependentValueTests
    {
        const TestEnum PossibleDependentValue = TestEnum.Value2;

        class SinglePossibleDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [AvailableIfContains(nameof(DependentValues), PossibleDependentValue)]
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
        [InlineData(TestEnum.Value1)]
        [InlineData(TestEnum.Value2)]
        [InlineData(TestEnum.Value1, TestEnum.Value3)]
        public void ShouldBeValidForPossibleDependentValue(params TestEnum[] extraValues)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = new[] { PossibleDependentValue }.Concat(extraValues).ToArray(),
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
        [InlineData(TestEnum.Value1, TestEnum.Value3)]
        public void ShouldBeInvalidWhenPossibleDependentValueIsNotContained(params TestEnum[] dependentValues)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = dependentValues,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(SinglePossibleDependentValueTestModel.DependentValues)} must contain Value2 to assign {nameof(SinglePossibleDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(SinglePossibleDependentValueTestModel.Value));
        }
    }

    public class TwoPossibleDependentValueTests
    {
        private const TestEnum PossibleDependentValue1 = TestEnum.Value2;
        private const TestEnum PossibleDependentValue2 = TestEnum.Value3;

        class TwoPossibleDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [AvailableIfContains(nameof(DependentValues), PossibleDependentValue1, PossibleDependentValue2)]
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
        [InlineData(PossibleDependentValue1, PossibleDependentValue2)]
        [InlineData(TestEnum.Value1, PossibleDependentValue1, PossibleDependentValue2, TestEnum.Value4)]
        public void ShouldBeValidForPossibleDependentValue(params TestEnum[] dependentValues)
        {
            var model = new TwoPossibleDependentValueTestModel
            {
                DependentValues = dependentValues,
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
        [InlineData(TestEnum.Value1, TestEnum.Value4)]
        public void ShouldBeInvalidWhenPossibleDependentValueIsNotContained(params TestEnum[] dependentValues)
        {
            var model = new TwoPossibleDependentValueTestModel
            {
                DependentValues = dependentValues,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(TwoPossibleDependentValueTestModel.DependentValues)} must contain {PossibleDependentValue1} or {PossibleDependentValue2} to assign {nameof(TwoPossibleDependentValueTestModel.Value)} to {model.Value}.");
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
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [AvailableIfContains(nameof(DependentValues),
                PossibleDependentValue1, PossibleDependentValue2, PossibleDependentValue3)]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeInvalidWhenPossibleDependentValueIsNotContained()
        {
            var model = new ThreePossibleDependentValueTestModel
            {
                DependentValues = Array.Empty<TestEnum>(),
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(ThreePossibleDependentValueTestModel.DependentValues)} must contain {PossibleDependentValue1}, {PossibleDependentValue2} or {PossibleDependentValue2} to assign {nameof(ThreePossibleDependentValueTestModel.Value)} to {model.Value}.");
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