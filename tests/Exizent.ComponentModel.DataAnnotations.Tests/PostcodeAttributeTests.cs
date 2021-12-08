using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class PostcodeAttributeTests
{
    class TestModel
    {
        [Postcode]
        public string? Postcode { get; set; }
    }

    [Theory]
    [InlineData("PA1 0AF")]
    [InlineData("AB10 0BA")]
    public void ShouldBeValidForAValidPostcode(string value)
    {
        var model = new TestModel { Postcode = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData("PA1  0AF")]
    [InlineData("000000")]
    [InlineData("PA PAa")]
    [InlineData("PPPPPPP")]
    public void ShouldBeInvalidForAInvalidPostcode(string value)
    {
        var model = new TestModel { Postcode = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.Postcode)} must be a valid postcode.");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.Postcode));
    }
}