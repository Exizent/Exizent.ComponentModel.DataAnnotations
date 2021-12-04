using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Exizent.ComponentModel.DataAnnotations.Tests
{
    public class AddressAttributeTests
    {
        class TestModel
        {
            [AddressField]
            public string? AddressLine1 { get; set; }
        }

        [Theory]
        [InlineData("1 Test Lane")]
        [InlineData("Glasgow")]
        public void ShouldBeValidForAddressWithNoSpacesAtStartOrEnd(string value)
        {
            var model = new TestModel { AddressLine1 = value };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            
            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("1 Test Lane ")]
        [InlineData(" 1 Test Lane")]
        [InlineData(" Glasgow")]
        [InlineData("Glasgow ")]
        public void ShouldBeInvalidForAddressWithSpacesAtStartOrEnd(string value)
        {
            var model = new TestModel { AddressLine1 = value };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            
            var isValid = Validator.TryValidateObject(model, context, results, true);

            using var _ = new AssertionScope();
            isValid.Should().BeFalse();
            results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.AddressLine1)} must not start or end with space");
            results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.AddressLine1));
        }
    }
}