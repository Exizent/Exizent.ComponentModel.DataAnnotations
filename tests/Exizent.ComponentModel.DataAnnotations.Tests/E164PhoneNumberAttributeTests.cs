namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class E164PhoneNumberAttributeTests
{
    class TestModel
    {
        [E164PhoneNumber]
        public string? PhoneNumber { get; set; }
    }

    [Theory]
    [InlineData("+10")]
    [InlineData("+200")]
    [InlineData("+3000")]
    [InlineData("+40000")]
    [InlineData("+500000")]
    [InlineData("+6000000")]
    [InlineData("+70000000")]
    [InlineData("+800000000")]
    [InlineData("+9000000000")]
    [InlineData("+10000000000")]
    [InlineData("+200000000000")]
    [InlineData("+31123456798")]
    [InlineData("+447777123456789")]
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
    [InlineData("1")]
    [InlineData("20")]
    [InlineData("300")]
    [InlineData("4000")]
    [InlineData("0")]
    [InlineData("000")]
    [InlineData("0000")]
    [InlineData("0000000000000")]
    [InlineData("(01112)222222")]
    [InlineData("00447777123456")]
    [InlineData("+1")]
    [InlineData("+0")]
    [InlineData("+00")]
    [InlineData("+000")]
    [InlineData("+0000")]
    [InlineData("+00000")]
    [InlineData("+000000")]
    [InlineData("+0000000")]
    [InlineData("+00000000")]
    [InlineData("+000000000")]
    [InlineData("+0000000000")]
    [InlineData("+00000000000")]
    [InlineData("+000000000000")]
    [InlineData("+01123456798")]
    [InlineData("+4477771234567890")]
    [InlineData("+44(0)7777123456")]
    [InlineData("+44 7777 123456")]
    public void ShouldBeInvalidForAInvalidPhoneNumber(string value)
    {
        var model = new TestModel { PhoneNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.PhoneNumber)} must be a valid E164 format phone number e.g. +447777******");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.PhoneNumber));
    }
}