namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class SortCodeAttributeTests
{
    public class SortCodeAttributeTestsWithoutDashes
    {
        class TestModel
        {
            [SortCode(allowDashes: false)] public string? SortCode { get; set; }
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
        [InlineData("123456")]
        [InlineData("000000")]
        [InlineData("999999")]
        public void ShouldBeValidForValidPostCode(string sortCode)
        {
            var model = new TestModel() { SortCode = sortCode };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("1a2a3a")]
        [InlineData("aaaaaa")]
        [InlineData("12345")]
        [InlineData("1234567")]
        [InlineData("12-34-56")]
        public void ShouldBeInValidForInValidPostCode(string sortCode)
        {
            var model = new TestModel() { SortCode = sortCode };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field SortCode must be a valid sort code.");
            results[0].MemberNames.Should()
                .BeEquivalentTo(
                    $"{nameof(TestModel.SortCode)}"
                );
        }
    }

    public class SortCodeAttributeTestsWithDashes
    {
        class TestModel
        {
            [SortCode(allowDashes: true)] public string? SortCode { get; set; }
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
        [InlineData("123456")]
        [InlineData("12-34-56")]
        [InlineData("000000")]
        [InlineData("00-00-00")]
        [InlineData("999999")]
        [InlineData("99-99-99")]
        public void ShouldBeValidForValidPostCode(string sortCode)
        {
            var model = new TestModel() { SortCode = sortCode };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("1a2a3a")]
        [InlineData("aaaaaa")]
        [InlineData("12345")]
        [InlineData("1234567")]
        [InlineData("99--99--99")]
        public void ShouldBeInValidForInValidPostCode(string sortCode)
        {
            var model = new TestModel() { SortCode = sortCode };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should()
                .Be($"The field SortCode must be a valid sort code.");
            results[0].MemberNames.Should()
                .BeEquivalentTo(
                    $"{nameof(TestModel.SortCode)}"
                );
        }
    }
}