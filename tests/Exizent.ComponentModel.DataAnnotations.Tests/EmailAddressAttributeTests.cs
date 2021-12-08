using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class EmailAddressAttributeTests
{
    class TestModel
    {
        [EmailAddress]
        public string? EmailAddress { get; set; }
    }

    [Theory]
    [InlineData("ben.davidson@exizent.com")]
    [InlineData("ghghjdgfhjds133243243212.32323232@subdomain.website.co.uk")]
    public void ShouldBeValidForAValidEmailAddress(string value)
    {
        var model = new TestModel { EmailAddress = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData("ben@")]
    [InlineData("exizent.com")]
    [InlineData("ben")]
    [InlineData(" @ . com")]
    public void ShouldBeInvalidForAInvalidEmailAddress(string value)
    {
        var model = new TestModel { EmailAddress = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.EmailAddress)} must be a valid email address.");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.EmailAddress));
    }
}