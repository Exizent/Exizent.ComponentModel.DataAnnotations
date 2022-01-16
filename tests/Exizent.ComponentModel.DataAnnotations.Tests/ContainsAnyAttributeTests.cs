using System.Collections;

namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class ContainsAnyAttributeTests
{
    public class StringContainsAnyAttributeTests
    {
        class TestModel : IEnumerable
        {
            [ContainsAny(typeof(TestModel))] public string? CountryCode { get; set; }

            public IEnumerator GetEnumerator()
            {
                IEnumerable<string> GetCountries()
                {
                    yield return "ES";
                    yield return "GB";
                    yield return "MX";
                }

                return GetCountries().GetEnumerator();
            }
        }

        [Theory]
        [InlineData("ES")]
        [InlineData("GB")]
        [InlineData("MX")]
        public void ShouldBeValidWhenValueIsContainedInIEnumerableType(string countryCode)
        {
            var model = new TestModel { CountryCode = countryCode };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeValidForNullValue()
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
        [InlineData("Invalid")]
        [InlineData("Value")]
        [InlineData("")]
        public void ShouldBeInvalidWhenValueIsNotContainedInIEnumerableType(string countryCode)
        {
            var model = new TestModel { CountryCode = countryCode };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.CountryCode)} is invalid.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.CountryCode));
        }
    }

    public class IntContainsAnyAttributeTests
    {
        class TestModel : IEnumerable
        {
            [ContainsAny(typeof(TestModel))] public int? PrimeNumber { get; set; }

            public IEnumerator GetEnumerator()
            {
                IEnumerable<int> GetPrimeNumber()
                {
                    yield return 2;
                    yield return 3;
                    yield return 5;
                    yield return 7;
                    // etc...
                }

                return GetPrimeNumber().GetEnumerator();
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        public void ShouldBeValidWhenValueIsContainedInIEnumerableType(int primeNumber)
        {
            var model = new TestModel { PrimeNumber = primeNumber };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void ShouldBeValidForNullValue()
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
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(6)]
        public void ShouldBeInvalidWhenValueIsNotContainedInIEnumerableType(int primeNumber)
        {
            var model = new TestModel { PrimeNumber = primeNumber };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.PrimeNumber)} is invalid.");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.PrimeNumber));
        }
    }

    public class InvalidContainsAnyAttributeTests
    {
        class TestModel
        {
            [ContainsAny(typeof(TestModel))]
            public object? PrimeNumber { get; set; }
        }

        [Fact]
        public void ShouldThrowExceptionWhenTypeDoesNotImplementIEnumerable()
        {
            var model = new TestModel();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            Action action = () => Validator.TryValidateObject(model, context, results, true);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("ContainsAnyAttribute requires a type that implements IEnumerable");
        }
    }
}