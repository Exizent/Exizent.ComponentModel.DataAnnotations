using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class PhoneNumberAttributeTests
{
    class TestModel
    {
        [PhoneNumber]
        public string? PhoneNumber { get; set; }
    }

    [Theory]
    [InlineData("00000")]
    [InlineData("000000")]
    [InlineData("0000000")]
    [InlineData("00000000")]
    [InlineData("000000000")]
    [InlineData("0000000000")]
    [InlineData("00000000000")]
    [InlineData("000000000000")]
    [InlineData("01123456798")]
    public void ShouldBeValidForAValidPhoneNumber(string value)
    {
        var model = new TestModel { PhoneNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("00")]
    [InlineData("000")]
    [InlineData("0000")]
    [InlineData("0000000000000")]
    [InlineData("(01112)222222")]
    [InlineData("+442222222222")]
    public void ShouldBeInvalidForAInvalidPhoneNumber(string value)
    {
        var model = new TestModel { PhoneNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.PhoneNumber)} must be a valid phone number.");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.PhoneNumber));
    }
}