namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class AvailableValuesIfNotContainsAttributeTests
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
        [AvailableValuesIfNotContains("PropertyThatDoesNotExist", 1)]
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
        const TestEnum NotDependentValue = TestEnum.Value2;

        class DependentValueTestModel
        {
            public TestEnum? DependentValue { get; set; }

            [AvailableValuesIfNotContains(nameof(DependentValue),
                NotDependentValue, "Hello", "World")]
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
        public void ShouldBeValidForNotDependentAndValue(string value)
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

        [Theory]
        [InlineData("Hello")]
        [InlineData("World")]
        [InlineData("anything")]
        public void ShouldBeValidForContainingDependentValue(string value)
        {
            var model = new DependentValueTestModel
            {
                DependentValue = NotDependentValue,
                Value = value
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
          }

        [Theory]
        [InlineData("anything")]
        [InlineData("else")]
        public void ShouldBeInvalidForNotContainingDependentValue(string value)
        {
            var model = new DependentValueTestModel
            {
                DependentValue = TestEnum.Value1,
                Value = value
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(DependentValueTestModel.Value)} must contain Hello or World when {nameof(DependentValueTestModel.DependentValue)} is not assign to {NotDependentValue}.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(DependentValueTestModel.Value));

        }
    }
    
    public class SinglePossibleDependentValueIEnumerableTests
    {
        const TestEnum NotDependentValue = TestEnum.Value2;

        class SinglePossibleDependentValueTestModel
        {
            public IEnumerable<TestEnum>? DependentValues { get; set; }

            [AvailableValuesIfNotContains(nameof(DependentValues),
                NotDependentValue, "Hello", "World")]
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
        public void ShouldBeValidForNotDependentAndValue(string value)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = new[] { TestEnum.Value1 },
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
        public void ShouldBeValidForContainingDependentValue(string value)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = new[] { NotDependentValue },
                Value = value
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
          }

        [Theory]
        [InlineData("anything")]
        [InlineData("else")]
        public void ShouldBeInvalidForNotContainingDependentValue(string value)
        {
            var model = new SinglePossibleDependentValueTestModel
            {
                DependentValues = new[] { TestEnum.Value1 },
                Value = value
            };

            var (results, isValid) = ValidateModel(model);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be(
                    $"The field {nameof(SinglePossibleDependentValueTestModel.Value)} must contain Hello or World when {nameof(SinglePossibleDependentValueTestModel.DependentValues)} is not assign to {NotDependentValue}.");
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