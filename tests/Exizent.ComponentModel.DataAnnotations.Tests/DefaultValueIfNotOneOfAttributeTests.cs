// namespace Exizent.ComponentModel.DataAnnotations.Tests;
//
// public class DefaultValueIfNotOneOfAttributeTests
// {
//     public enum TestEnum
//     {
//         Value1,
//         Value2,
//         Value3,
//         Value4
//     }
//
//     class InvalidDependentPropertyTestClass
//     {
//         [DefaultValueIfNotOneOf("PropertyThatDoesNotExist", 1)]
//         public string? Value { get; set; }
//     }
//
//     class InvalidDependentPropertyTypeTestClass
//     {
//         public List<int>? DependentProperty { get; set; }
//
//         [DefaultValueIfNotOneOf(nameof(DependentProperty), 1)]
//         public string? Value { get; set; }
//     }
//
//     [Fact]
//     public void ShouldThrowInvalidOperationExceptionForInvalidDependentProperty()
//     {
//         var model = new InvalidDependentPropertyTestClass
//         {
//             Value = Guid.NewGuid().ToString()
//         };
//
//         Action action = () => ValidateModel(model);
//
//         action.Should()
//             .Throw<InvalidOperationException>()
//             .WithMessage("The dependent property '*' does not exist");
//     }
//
//     [Fact]
//     public void ShouldThrowInvalidOperationExceptionForInvalidDependentPropertyType()
//     {
//         var model = new InvalidDependentPropertyTypeTestClass
//         {
//             DependentProperty = new List<int>{ 4},
//             Value = Guid.NewGuid().ToString()
//         };
//
//         Action action = () => ValidateModel(model);
//
//         action.Should()
//             .Throw<InvalidOperationException>()
//             .WithMessage("The dependent property '*' must not be of type IEnumerable");
//     }
//
//
//     public class SingleRequiredDependentValueTests
//     {
//         const TestEnum RequiredDependentValue2 = TestEnum.Value2;
//
//         class SingleRequiredDependentValueTestModel
//         {
//             public TestEnum? DependentValue { get; set; }
//
//             [DefaultValueIfNotOneOf(nameof(DependentValue), RequiredDependentValue2)]
//             public string? Value { get; set; }
//             
//             [DefaultValueIfNotOneOf(nameof(DependentValue), RequiredDependentValue2, AllowEmptyStrings = true)]
//             public string? ValueThatAllowsEmptyString { get; set; }        
//         }
//
//         [Fact]
//         public void ShouldBeValidForNullDependentPropertyValueAndNullValue()
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = null,
//                 Value = null
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//
//         [Fact]
//         public void ShouldBeValidForRequiredDependentValueAndSetValue()
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = RequiredDependentValue2,
//                 Value = Guid.NewGuid().ToString()
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//
//         [Fact]
//         public void ShouldBeValidForNotRequiredDependentValueAndNullValue()
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = TestEnum.Value1,
//                 Value = null
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//
//         [Fact]
//         public void ShouldBeValidWhenRequiredDependentValueAndValueIsNull()
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = RequiredDependentValue2,
//                 Value = null
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//
//         [Fact]
//         public void ShouldBeInvalidForNullDependentPropertyValueAndValueIsSet()
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = null,
//                 Value = Guid.NewGuid().ToString()
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeFalse();
//             results[0].ErrorMessage.Should()
//                 .Be(
//                     $"The field {nameof(SingleRequiredDependentValueTestModel.Value)} must not be set when {nameof(SingleRequiredDependentValueTestModel.DependentValue)} is one of {RequiredDependentValue2}.");
//             results[0].MemberNames.Should().OnlyContain(x => x == nameof(SingleRequiredDependentValueTestModel.Value));
//         }
//         
//         [Fact]
//         public void ShouldBeInvalidForNotRequiredDependentPropertyValueAndValueIsSet()
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = TestEnum.Value1,
//                 Value = Guid.NewGuid().ToString()
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeFalse();
//             results[0].ErrorMessage.Should()
//                 .Be(
//                     $"The field {nameof(SingleRequiredDependentValueTestModel.Value)} must not be set when {nameof(SingleRequiredDependentValueTestModel.DependentValue)} is one of {RequiredDependentValue2}.");
//             results[0].MemberNames.Should().OnlyContain(x => x == nameof(SingleRequiredDependentValueTestModel.Value));
//         }
//         
//         [Fact]
//         public void ShouldBeInvalidForNullDependentPropertyValueAndEmptyString()
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = null,
//                 Value = ""
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeFalse();
//             results[0].ErrorMessage.Should()
//                 .Be(
//                     $"The field {nameof(SingleRequiredDependentValueTestModel.Value)} must not be set when {nameof(SingleRequiredDependentValueTestModel.DependentValue)} is one of {RequiredDependentValue2}.");
//             results[0].MemberNames.Should().OnlyContain(x => x == nameof(SingleRequiredDependentValueTestModel.Value));
//         }
//         
//         [Theory]
//         [InlineData("")]
//         [InlineData("  ")]
//         public void ShouldBeValidForNullDependentPropertyValueAndEmptyStringWhenEmptyStringAllowed(string stringValue)
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = null,
//                 ValueThatAllowsEmptyString = stringValue
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//         
//         [Theory]
//         [InlineData("")]
//         [InlineData("  ")]
//         public void ShouldBeValidForNotRequiredDependentPropertyValueAndEmptyStringWhenEmptyStringAllowed(string stringValue)
//         {
//             var model = new SingleRequiredDependentValueTestModel
//             {
//                 DependentValue = TestEnum.Value1,
//                 ValueThatAllowsEmptyString = stringValue
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//     }
//
//     public class ThreeRequiredDependentValueTests
//     {
//         private const TestEnum RequiredDependentValue2 = TestEnum.Value2;
//         private const TestEnum RequiredDependentValue3 = TestEnum.Value3;
//         private const TestEnum RequiredDependentValue4 = TestEnum.Value4;
//
//         class ThreeRequiredDependentValueTestModel
//         {
//             public TestEnum? DependentValue { get; set; }
//
//             [DefaultValueIfNotOneOf(nameof(DependentValue),
//                 RequiredDependentValue2, RequiredDependentValue3, RequiredDependentValue4)]
//             public string? Value { get; set; }
//             
//             [DefaultValueIfNotOneOf(nameof(DependentValue),
//                 RequiredDependentValue2, RequiredDependentValue3, RequiredDependentValue4, AllowEmptyStrings = true)]
//             public string? ValueThatAllowsEmptyString { get; set; }               
//         }
//
//         [Theory]
//         [InlineData(RequiredDependentValue2)]
//         [InlineData(RequiredDependentValue3)]
//         [InlineData(RequiredDependentValue4)]
//         public void ShouldBeValidWhenHasRequiredDependentValuesAndValueIsNull(TestEnum requiredDependentValue)
//         {
//             var model = new ThreeRequiredDependentValueTestModel
//             {
//                 DependentValue = requiredDependentValue,
//                 Value = null
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//
//         [Fact]
//         public void ShouldBeValidWhenHasRequiredDependentValuesAndValueIsSet()
//         {
//             var model = new ThreeRequiredDependentValueTestModel
//             {
//                 DependentValue = RequiredDependentValue2,
//                 Value = "Some value"
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//         
//         [Fact]
//         public void ShouldBeValidWhenHasNotRequiredDependentValuesAndValueIsSetToEmptyStringWhenEmptyStringIsAllowed()
//         {
//             var model = new ThreeRequiredDependentValueTestModel
//             {
//                 DependentValue = TestEnum.Value1,
//                 ValueThatAllowsEmptyString = ""
//             };
//
//             var (results, isValid) = ValidateModel(model);
//
//             using var _ = new AssertionScope();
//             isValid.Should().BeTrue();
//             results.Should().BeEmpty();
//         }
//     }
//
//     private static (List<ValidationResult> results, bool isValid) ValidateModel<TModel>(TModel model)
//         where TModel : notnull
//     {
//         var context = new ValidationContext(model);
//         var results = new List<ValidationResult>();
//
//         var isValid = Validator.TryValidateObject(model, context, results, true);
//         return (results, isValid);
//     }
// }