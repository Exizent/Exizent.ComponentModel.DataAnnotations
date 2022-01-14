namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class StringValueLengthAttributeTests
{
    class TestModel
    {
        [StringValueLength(2)] 
        public IDictionary<string, string>? KeyValuePairs { get; set; }
    }

    [Fact]
    public void ShouldBeValidForNullPropertyValue()
    {
        var model = new TestModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void ShouldBeValidForValuesInRange(int valueLength)
    {
        var model = new TestModel
        {
            KeyValuePairs = new Dictionary<string, string>
            {
                ["key1"] = new('a', valueLength),
                ["key2"] = new('b', valueLength),
                ["key3"] = new('c', valueLength),
            }
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public void ShouldBeInvalidForValuesOutOfRange(int valueLength)
    {
        var value1 = new string('a', valueLength);
        var value2 = new string('b', valueLength);
        var value3 = new string('c', valueLength);

        var model = new TestModel
        {
            KeyValuePairs = new Dictionary<string, string>
            {
                ["key1"] = value1,
                ["key2"] = value2,
                ["key3"] = value3,
                ["validKey"] = new('d', 2),
            }
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should()
            .Be("The field values of KeyValuePairs must be a string with a maximum length of 2.");
        results[0].MemberNames.Should()
            .BeEquivalentTo(
                $"{nameof(TestModel.KeyValuePairs)}.{value1}",
                $"{nameof(TestModel.KeyValuePairs)}.{value2}",
                $"{nameof(TestModel.KeyValuePairs)}.{value3}"
            );
    }

    public class StringKeyMinimumAndMaximumLengthTests
    {
        class TestModel
        {
            [StringValueLength(5, MinimumLength = 2)]
            public IDictionary<string, string>? KeyValuePairs { get; set; }
        }


        [Fact]
        public void ShouldBeValidForNullPropertyValue()
        {
            var model = new TestModel();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ShouldBeValidForValuesInRange(int valueLength)
        {
            var model = new TestModel
            {
                KeyValuePairs = new Dictionary<string, string>
                {
                    ["key1"] = new('a', valueLength),
                    ["key2"] = new('b', valueLength),
                    ["key3"] = new('c', valueLength),
                }
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(6)]
        [InlineData(7)]
        public void ShouldBeInvalidForValuesOutOfRange(int valueLength)
        {
            var value1 = new string('a', valueLength);
            var value2 = new string('b', valueLength);
            var value3 = new string('c', valueLength);

            var model = new TestModel
            {
                KeyValuePairs = new Dictionary<string, string>
                {
                    ["key1"] = value1,
                    ["key2"] = value2,
                    ["key3"] = value3,
                    ["validKey"] = new('d', 2),
                }
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be("The field values of KeyValuePairs must be a string with a minimum length of 2 and a maximum length of 5.");
            results[0].MemberNames.Should()
                .BeEquivalentTo(
                    $"{nameof(TestModel.KeyValuePairs)}.{value1}",
                    $"{nameof(TestModel.KeyValuePairs)}.{value2}",
                    $"{nameof(TestModel.KeyValuePairs)}.{value3}"
                );
        }
    }

    public class InvalidTypeTests
    {
        class TestModel
        {
            [StringValueLength(2)] 
            public TestModel? KeyValuePairs { get; set; }
        }

        [Fact]
        public void ShouldThrowInvalidOperationExceptionForInvalidPropertyType()
        {
            var model = new TestModel
            {
                KeyValuePairs = new TestModel()
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            Action action = () => Validator.TryValidateObject(model, context, results, true);

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The value must be an IEnumerable<KeyValuePair<string, string>>");
        }
    }

    public class InvalidMaximumTests
    {
        class TestModel
        {
            [StringValueLength(-1)] 
            public IDictionary<string, string>? KeyValuePairs { get; set; }
        }

        [Fact]
        public void ShouldThrowInvalidOperationExceptionForInvalidRangeSetup()
        {
            var model = new TestModel
            {
                KeyValuePairs = new Dictionary<string, string>()
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            Action action = () => Validator.TryValidateObject(model, context, results, true);

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The maximum length must be a nonnegative integer.");
        }
    }

    public class InvalidRangeTests
    {
        class TestModel
        {
            [StringValueLength(2, MinimumLength = 5)]
            public IDictionary<string, string>? KeyValuePairs { get; set; }
        }

        [Fact]
        public void ShouldThrowInvalidOperationExceptionForInvalidRangeSetup()
        {
            var model = new TestModel
            {
                KeyValuePairs = new Dictionary<string, string>()
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            Action action = () => Validator.TryValidateObject(model, context, results, true);

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The maximum value '2' must be greater than or equal to the minimum value '5'.");
        }
    }
}