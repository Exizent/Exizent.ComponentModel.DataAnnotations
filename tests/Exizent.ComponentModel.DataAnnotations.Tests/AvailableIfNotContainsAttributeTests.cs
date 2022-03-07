namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class AvailableIfNotContainsAttributeTests
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
        [AvailableIfNotContains("PropertyThatDoesNotExist", 1)]
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

    public class SingleDependentValueTests
    {
        const TestEnum NotDependentValue = TestEnum.Value2;

        class DependentValueTestModel
        {
            public TestEnum? DependentValues { get; set; }

            [AvailableIfNotContains(nameof(DependentValues), NotDependentValue)]
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
        [InlineData(TestEnum.Value1)]
        [InlineData(TestEnum.Value3)]
        public void ShouldBeValidForNotDependentValue(TestEnum extraValues)
        {
            var model = new DependentValueTestModel
            {
                DependentValues = extraValues,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidWhenNotDependentValueIsContained()
        {
            var model = new DependentValueTestModel
            {
                DependentValues = NotDependentValue,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(DependentValueTestModel.DependentValues)} must not contain Value2 to assign {nameof(DependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(DependentValueTestModel.Value));
        }
    }

    public class SinglePossibleDependentValueIEnumerableTests
    {
        const TestEnum NotDependentValue = TestEnum.Value2;

        class SinglePossibleDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [AvailableIfNotContains(nameof(DependentValues), NotDependentValue)]
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
        [InlineData(TestEnum.Value3)]
        [InlineData(TestEnum.Value1, TestEnum.Value3)]
        public void ShouldBeValidForNotDependentValue(params TestEnum[] extraValues)
        {
            var model = new SinglePossibleDependentValueTestModel
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
        [InlineData(TestEnum.Value3)]
        [InlineData(TestEnum.Value1, TestEnum.Value3)]
        public void ShouldBeInvalidWhenNotDependentValueIsContained(params TestEnum[] dependentValues)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = dependentValues.Concat(new[] { NotDependentValue }),
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(SinglePossibleDependentValueTestModel.DependentValues)} must not contain Value2 to assign {nameof(SinglePossibleDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(SinglePossibleDependentValueTestModel.Value));
        }
    }

    public class TwoPossibleDependentValueIEnumerableTests
    {
        private const TestEnum NotDependentValue1 = TestEnum.Value2;
        private const TestEnum NotDependentValue2 = TestEnum.Value3;

        class TwoPossibleDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [AvailableIfNotContains(nameof(DependentValues),
                NotDependentValue1, NotDependentValue2)]
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
        [InlineData(TestEnum.Value1)]
        [InlineData(TestEnum.Value1, TestEnum.Value4)]
        [InlineData(TestEnum.Value4)]
        public void ShouldBeValidForNotAllDependentValue(params TestEnum[] dependentValues)
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
        [InlineData(TestEnum.Value1, NotDependentValue1)]
        [InlineData(TestEnum.Value1, NotDependentValue2)]
        [InlineData(NotDependentValue1)]
        [InlineData(NotDependentValue2)]
        public void ShouldBeInvalidForNotPossibleDependentValueContained(params TestEnum[] dependentValues)
        {
            var model = new TwoPossibleDependentValueTestModel
            {
                DependentValues = dependentValues,
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(TwoPossibleDependentValueTestModel.DependentValues)} must not contain {NotDependentValue1} or {NotDependentValue2} to assign {nameof(TwoPossibleDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(TwoPossibleDependentValueTestModel.Value));
        }

        [Theory]
        [InlineData(TestEnum.Value1, NotDependentValue1, NotDependentValue2)]
        [InlineData(NotDependentValue1, NotDependentValue2)]
        [InlineData(NotDependentValue1, NotDependentValue2, TestEnum.Value4)]
        public void ShouldBeInvalidWhenAllNotDependentValueIsContained(params TestEnum[] dependentValues)
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
                    $"The field {nameof(TwoPossibleDependentValueTestModel.DependentValues)} must not contain {NotDependentValue1} or {NotDependentValue2} to assign {nameof(TwoPossibleDependentValueTestModel.Value)} to {model.Value}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(TwoPossibleDependentValueTestModel.Value));
        }
    }

    public class ThreePossibleDependentValueIEnumerableTests
    {
        private const TestEnum NotDependentValue1 = TestEnum.Value2;
        private const TestEnum NotDependentValue2 = TestEnum.Value3;
        private const TestEnum NotDependentValue3 = TestEnum.Value3;

        class ThreePossibleDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [AvailableIfNotContains(nameof(DependentValues),
                NotDependentValue1,
                NotDependentValue2,
                NotDependentValue3)]
            public string? Value { get; set; }
        }

        [Fact]
        public void ShouldBeInvalidWhenNotDependentValueIsContained()
        {
            var model = new ThreePossibleDependentValueTestModel
            {
                DependentValues = new[]
                {
                    NotDependentValue1,
                    NotDependentValue2,
                    NotDependentValue3
                },
                Value = Guid.NewGuid().ToString()
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(ThreePossibleDependentValueTestModel.DependentValues)} must not contain {NotDependentValue1}, {NotDependentValue2} or {NotDependentValue2} to assign {nameof(ThreePossibleDependentValueTestModel.Value)} to {model.Value}.");
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