namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class StringKeyLengthAttributeTests
{
    public class StringKeyMaximumLengthOnlyTests
    {
        class TestModel
        {
            [StringKeyLength(2)] public IDictionary<string, string>? KeyValuePairs { get; set; }
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
        public void ShouldBeValidForKeysInRange(int keyLength)
        {
            var model = new TestModel
            {
                KeyValuePairs = new Dictionary<string, string>
                {
                    [new string('a', keyLength)] = "value",
                    [new string('b', keyLength)] = "value",
                    [new string('c', keyLength)] = "value",
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
        public void ShouldBeInvalidForKeysOutOfRange(int keyLength)
        {
            var key1 = new string('a', keyLength);
            var key2 = new string('b', keyLength);
            var key3 = new string('c', keyLength);
            var model = new TestModel
            {
                KeyValuePairs = new Dictionary<string, string>
                {
                    [key1] = "value",
                    [key2] = "value",
                    [key3] = "value",
                    ["1"] = "Valid"
                }
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field keys of KeyValuePairs must be a string with a maximum length of 2.");
            results[0].MemberNames.Should()
                .BeEquivalentTo(
                    $"{nameof(TestModel.KeyValuePairs)}.{key1}",
                    $"{nameof(TestModel.KeyValuePairs)}.{key2}",
                    $"{nameof(TestModel.KeyValuePairs)}.{key3}"
                );
        }
    }

    public class StringKeyMinimumAndMaximumLengthTests
    {
        class TestModel
        {
            [StringKeyLength(5, MinimumLength = 2)]
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
        [InlineData(5)]
        public void ShouldBeValidForKeysInRange(int keyLength)
        {
            var model = new TestModel
            {
                KeyValuePairs = new Dictionary<string, string>
                {
                    [new string('a', keyLength)] = "value",
                    [new string('b', keyLength)] = "value",
                    [new string('c', keyLength)] = "value",
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
        public void ShouldBeInvalidForKeysOutOfRange(int keyLength)
        {
            var key1 = new string('a', keyLength);
            var key2 = new string('b', keyLength);
            var key3 = new string('c', keyLength);
            var model = new TestModel
            {
                KeyValuePairs = new Dictionary<string, string>
                {
                    [key1] = "value",
                    [key2] = "value",
                    [key3] = "value"
                }
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field keys of KeyValuePairs must be a string with a minimum length of 2 and a maximum length of 5.");
            results[0].MemberNames.Should()
                .BeEquivalentTo(
                    $"{nameof(TestModel.KeyValuePairs)}.{key1}",
                    $"{nameof(TestModel.KeyValuePairs)}.{key2}",
                    $"{nameof(TestModel.KeyValuePairs)}.{key3}"
                );
        }
    }

    public class InvalidTypeTests
    {
        class TestModel
        {
            [StringKeyLength(2)] public TestModel? KeyValuePairs { get; set; }
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
            [StringKeyLength(-1)]
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
            [StringKeyLength(2, MinimumLength = 5)]
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