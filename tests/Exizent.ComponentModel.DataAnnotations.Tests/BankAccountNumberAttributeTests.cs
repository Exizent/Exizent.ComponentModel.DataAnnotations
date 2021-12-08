namespace Exizent.ComponentModel.DataAnnotations.Tests;

public class BankAccountNumberAttributeTests
{
    class TestModel
    {
        [BankAccountNumber]
        public string? BankAccountNumber { get; set; }
    }

    [Theory]
    [InlineData("12346578")]
    [InlineData("123456")]
    [InlineData("1234567890")]
    public void ShouldBeValidForAValidBankAccountNumber(string value)
    {
        var model = new TestModel { BankAccountNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("12345678901")]
    [InlineData("£££££")]
    [InlineData("helloWorld")]
    public void ShouldBeInvalidForAInvalidBankAccountNumber(string value)
    {
        var model = new TestModel { BankAccountNumber = value };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
            
        var isValid = Validator.TryValidateObject(model, context, results, true);

        using var _ = new AssertionScope();
        isValid.Should().BeFalse();
        results[0].ErrorMessage.Should().Be($"The field {nameof(TestModel.BankAccountNumber)} must be a valid bank account number.");
        results[0].MemberNames.Should().OnlyContain(x => x == nameof(TestModel.BankAccountNumber));
    }
}