namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class PaymentByAccountNumberAttributeTests
{
    class TestModel
    {
        [PaymentByAccountNumber]
        public string? PaymentByAccountNumber { get; set; }
    }

    [Theory]
    [InlineData("PBA")]
    [InlineData("PBA1234567890")]
    [InlineData("PBAabcdefghijk")]
    [InlineData("PBAABCDEFGHIJ")]
    [InlineData("PBA 1234567890")]
    [InlineData("PBA abcdefghijk")]
    [InlineData("PBA ABCDEFGHIJ")]
    [InlineData("PBA-1234567890")]
    [InlineData("PBA - abcdefghijk")]
    [InlineData("PBA: ABCDEFGHIJ")]
    [InlineData("PBA01234567890123456789012345678901234567890123456")]
    public void ShouldBeValidForAValidPhoneNumber(string value)
    {
        var model = new TestModel { PaymentByAccountNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData("pba")]
    [InlineData("123456789")]
    [InlineData("abcdefghijk")]
    [InlineData("ABCDEFGHIJK")]
    [InlineData("PBA012345678901234567890123456789012345678901234567")]
    public void ShouldBeInvalidForAInvalidPhoneNumber(string value)
    {
        var model = new TestModel { PaymentByAccountNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.PaymentByAccountNumber)} must start with 'PBA' and not exceed 50 characters in length.");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.PaymentByAccountNumber));
    }
}