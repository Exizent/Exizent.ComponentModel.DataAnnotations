using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class NationalInsuranceNumberAttributeTests
{
    class TestModel
    {
        [NationalInsuranceNumber]
        public string? NationalInsuranceNumber { get; set; }
    }

    [Theory]
    [InlineData("AA 12 34 56 A")]
    [InlineData("AA123456A")]
    public void ShouldBeValidForAValidNationalInsuranceNumber(string value)
    {
        var model = new TestModel { NationalInsuranceNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData("AA1 234 56A")]
    [InlineData("hjgdfksjkl")]
    [InlineData("AA123456")]
    [InlineData("A1 12 34 56 B")]
    [InlineData("AA1b3456B")]
    [InlineData("AA 12 3F 56 B")]
    [InlineData("$")]
    [InlineData("AAAAAAAAB")]
    [InlineData("123456789")]
    [InlineData("*& 12")]
    [InlineData("TsD")]
    [InlineData("Td s")]
    [InlineData("AA12345678B")]
    [InlineData("AA 12 34 5")]
    [InlineData("AA123 456B")]
    [InlineData("AA12 3456A")]
    [InlineData("AA12345")]
    [InlineData("AA 123456B")]
    [InlineData("AA12 3456B")]
    [InlineData("AA 12 34 56  B")]
    [InlineData("AA123456 B")]
    [InlineData("aa123456 b")]
    public void ShouldBeInvalidForAInvalidNationalInsuranceNumber(string value)
    {
        var model = new TestModel { NationalInsuranceNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.NationalInsuranceNumber)} must be a valid national insurance number.");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.NationalInsuranceNumber));
    }
}