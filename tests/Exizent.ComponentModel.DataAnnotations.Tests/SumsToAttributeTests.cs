namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class SumsToAttributeTests
{
    class TestModel
    {
        public class Value
        {
            public decimal Percentage { get; set; }
        }

        [SumsTo(nameof(Value.Percentage), 1)] public IEnumerable<Value>? Values { get; set; }
    }

    [Fact]
    public void ShouldBeValidWhenSumsToExpected()
    {
        var model = new TestModel
        {
            Values = new[]
            {
                new TestModel.Value
                {
                    Percentage = 0.5m
                },
                new TestModel.Value
                {
                    Percentage = 0.5m
                }
            }
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeValidWhenNull()
    {
        var model = new TestModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeValidWhenEmpty()
    {
        var model = new TestModel
        {
            Values = ArraySegment<TestModel.Value>.Empty
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeInvalidWhenDoesNotSumAsExpected()
    {
        var model = new TestModel
        {
            Values = new[]
            {
                new TestModel.Value
                {
                    Percentage = 0.01m
                },
                new TestModel.Value
                {
                    Percentage = 0.999m
                }
            }
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results.Should().NotBeEmpty().And.HaveCount(1);
        results[0].ErrorMessage.Should().Be("The 'Percentage' members of 'Values' must sum to 1");
        results[0].MemberNames.Single().Should().Be("Values[1].Percentage");
    }

    public class WhenChildValueIsNullable
    {
        class NullableTestModel
        {
            public class Value
            {
                public decimal? Percentage { get; set; }
            }

            [SumsTo(nameof(Value.Percentage), 1)] public IEnumerable<Value>? Values { get; set; }
        }

        [Fact]
        public void ShouldBeValidWhenSumsToExpected()
        {
            var model = new NullableTestModel
            {
                Values = new[]
                {
                    new NullableTestModel.Value
                    {
                        Percentage = 0.5m
                    },
                    new NullableTestModel.Value
                    {
                        Percentage = 0.5m
                    }
                }
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeInvalidWhenDoesNotSumAsExpected()
        {
            var model = new NullableTestModel
            {
                Values = new[]
                {
                    new NullableTestModel.Value
                    {
                        Percentage = null
                    },
                    new NullableTestModel.Value
                    {
                        Percentage = 0.999m
                    }
                }
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results.Should().NotBeEmpty().And.HaveCount(1);
            results[0].ErrorMessage.Should().Be("The 'Percentage' members of 'Values' must sum to 1");
            results[0].MemberNames.Single().Should().Be("Values[1].Percentage");
        }
    }
}