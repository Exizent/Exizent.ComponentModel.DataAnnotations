# Exizent.ComponentModel.DataAnnotations

A collection of custom `System.ComponentModel.DataAnnotations` validation attributes for .NET.

[![NuGet](https://img.shields.io/nuget/v/Exizent.ComponentModel.DataAnnotations.svg)](https://www.nuget.org/packages/Exizent.ComponentModel.DataAnnotations)

## Installation

```bash
dotnet add package Exizent.ComponentModel.DataAnnotations
```

## Usage

Add the namespace to your file:

```csharp
using Exizent.ComponentModel.DataAnnotations;
```

Validate a model using the standard `Validator`:

```csharp
var model = new MyModel { /* ... */ };
var context = new ValidationContext(model);
var results = new List<ValidationResult>();

bool isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);
```

---

## Table of Contents

### Format & Regex Validators

- [AddressField](#addressfield)
- [BankAccountNumber](#bankaccountnumber)
- [Currency](#currency)
- [DateOnly](#dateonly)
- [E164PhoneNumber](#e164phonenumber)
- [EmailAddress](#emailaddress)
- [MaxPrecision](#maxprecision)
- [NationalInsuranceNumber](#nationalinsurancenumber)
- [PhoneNumber](#phonenumber)
- [Postcode](#postcode)
- [SortCode](#sortcode)

### Conditional Availability Attributes

- [AvailableIf](#availableif)
- [AvailableIfContains](#availableifcontains)
- [AvailableIfNotContains](#availableifnotcontains)
- [AvailableIfOneOf](#availableifoneof)
- [AvailableValuesIfContains](#availablevaluesifcontains)
- [AvailableValuesIfNotContains](#availablevaluesifnotcontains)

### Conditional Required Attributes

- [RequiredIf](#requiredif)
- [RequiredIfContains](#requiredifcontains)
- [RequiredIfOneOf](#requiredifoneof)

### Collection, Comparison & Membership Attributes

- [ContainsAny](#containsany)
- [DateTimeCompare](#datetimecompare)
- [DateTimeCompareToday](#datetimecomparetoday)
- [StringKeyLength](#stringkeylength)
- [StringValueLength](#stringvaluelength)
- [SumsTo](#sumsto)

### Reference

- [EqualityCondition Enum](#equalitycondition-enum)

---

## Format & Regex Validators

### AddressField

Validates that a string does not start or end with whitespace.

```csharp
public class Address
{
    [AddressField]
    public string? AddressLine1 { get; set; }
}
```

**Error message:** `"The field {0} must not start or end with space"`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### BankAccountNumber

Validates that a string is a numeric bank account number between 6 and 10 digits.

```csharp
public class BankAccount
{
    [BankAccountNumber]
    public string? AccountNumber { get; set; }
}
```

**Error message:** `"The field {0} must be a valid bank account number."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### Currency

Validates that a `decimal` value does not exceed the number of decimal places allowed by the en-GB culture (2 decimal places).

```csharp
public class Payment
{
    [Currency]
    public decimal? Amount { get; set; }
}
```

**Error message:** `"The field {0} is not a valid currency value."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### DateOnly

Validates that a `DateTime` value has no time component (time must be `00:00:00`).

```csharp
public class Event
{
    [DateOnly]
    public DateTime? EventDate { get; set; }
}
```

**Error message:** `"The field {0} must be a date only."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### E164PhoneNumber

Validates that a string is a valid E.164 format phone number (starts with `+`, followed by a non-zero digit and up to 14 digits).

```csharp
public class Contact
{
    [E164PhoneNumber]
    public string? PhoneNumber { get; set; }
}
```

**Error message:** `"The field {0} must be a valid E164 format phone number e.g. +447777******"`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### EmailAddress

Validates that a string is a valid email address format.

```csharp
public class Contact
{
    [EmailAddress]
    public string? Email { get; set; }
}
```

**Error message:** `"The field {0} must be a valid email address."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### MaxPrecision

Validates that a decimal value does not exceed a specified number of decimal places.

```csharp
public class Measurement
{
    [MaxPrecision(4)]
    public decimal? Value { get; set; }
}
```

**Error message:** `"The field {0} must have a max precision of N decimal places."`

> **Note:** `N` is baked into the message at construction time based on the `maxPrecision` argument.

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### NationalInsuranceNumber

Validates that a string is a valid UK National Insurance number (e.g. `AA123456A` or `AA 12 34 56 A`).

```csharp
public class Person
{
    [NationalInsuranceNumber]
    public string? NINumber { get; set; }
}
```

**Error message:** `"The field {0} must be a valid national insurance number."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### PhoneNumber

Validates that a string is a numeric phone number between 5 and 12 digits.

```csharp
public class Contact
{
    [PhoneNumber]
    public string? PhoneNumber { get; set; }
}
```

**Error message:** `"The field {0} must be a valid phone number."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### Postcode

Validates that a string is a valid UK postcode.

```csharp
public class Address
{
    [Postcode]
    public string? Postcode { get; set; }
}
```

**Error message:** `"The field {0} must be a valid postcode."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### SortCode

Validates that a string is a valid 6-digit sort code. Optionally allows dashes (e.g. `12-34-56`).

```csharp
public class BankAccount
{
    [SortCode]
    public string? SortCode { get; set; }

    [SortCode(allowDashes: true)]
    public string? SortCodeWithDashes { get; set; }
}
```

**Error message:** `"The field {0} must be a valid sort code."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

## Conditional Availability Attributes

These attributes validate that a property is only assigned a value when a dependent property meets a specific condition. A `null` value on the decorated property always passes validation.

### AvailableIf

The property can only have a value when the dependent property equals the specified value.

```csharp
public class Claim
{
    public bool HasInsurance { get; set; }

    [AvailableIf(nameof(HasInsurance), true)]
    public string? PolicyNumber { get; set; }
}
```

**Error message:** `"The field {0} must be set to {1} to assign {2} to {3}"`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Dependent property name |
| `{1}` | Required dependent property value |
| `{2}` | Current property value |
| `{3}` | Current property name |

---

### AvailableIfContains

The property can only have a value when the dependent property (single value or `IEnumerable`) contains any of the specified values.

```csharp
public enum Feature { Basic, Premium, Enterprise }

public class Subscription
{
    public IEnumerable<Feature>? Features { get; set; }

    [AvailableIfContains(nameof(Features), Feature.Premium)]
    public string? PremiumConfig { get; set; }
}
```

**Error message:** `"The field {0} must contain {1} to assign {2} to {3}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Dependent property name |
| `{1}` | Possible dependant property values (or-joined) |
| `{2}` | Current property name |
| `{3}` | Current property value |

---

### AvailableIfNotContains

The property can only have a value when the dependent property (single value or `IEnumerable`) does **not** contain any of the specified values. Inverse of `AvailableIfContains`.

```csharp
public enum Status { Active, Suspended, Closed }

public class Account
{
    public IEnumerable<Status>? Statuses { get; set; }

    [AvailableIfNotContains(nameof(Statuses), Status.Closed)]
    public string? ReopenReason { get; set; }
}
```

**Error message:** `"The field {0} must not contain {1} to assign {2} to {3}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Dependent property name |
| `{1}` | Not-allowed dependant property values (or-joined) |
| `{2}` | Current property name |
| `{3}` | Current property value |

---

### AvailableIfOneOf

The property can only have a value when the dependent property equals one of the specified values. The dependent property must **not** be an `IEnumerable`.

```csharp
public enum AccountType { Current, Savings, Business }

public class Account
{
    public AccountType? Type { get; set; }

    [AvailableIfOneOf(nameof(Type), AccountType.Current, AccountType.Business)]
    public string? OverdraftLimit { get; set; }
}
```

**Error message:** `"The field {0} must be one of {1} to assign {2} to {3}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Dependent property name |
| `{1}` | Possible dependant property values (or-joined) |
| `{2}` | Current property name |
| `{3}` | Current property value |

---

### AvailableValuesIfContains

Restricts the allowed values of the property to a specific set when the dependent property contains the specified value.

```csharp
public enum Category { Standard, Special }

public class Product
{
    public IEnumerable<Category>? Categories { get; set; }

    [AvailableValuesIfContains(nameof(Categories), Category.Special, "Gold", "Platinum")]
    public string? Tier { get; set; }
}
```

**Error message:** `"The field {0} must contain {1} when {2} is assigned to {3}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Current property name |
| `{1}` | Available field values (or-joined) |
| `{2}` | Dependent property name |
| `{3}` | Dependant property value |

---

### AvailableValuesIfNotContains

Restricts the allowed values of the property to a specific set when the dependent property does **not** contain the specified value. Inverse of `AvailableValuesIfContains`.

```csharp
public enum Membership { Free, Paid }

public class User
{
    public IEnumerable<Membership>? Memberships { get; set; }

    [AvailableValuesIfNotContains(nameof(Memberships), Membership.Paid, "Basic", "Trial")]
    public string? Plan { get; set; }
}
```

**Error message:** `"The field {0} must contain {1} when {2} is not assigned to {3}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Current property name |
| `{1}` | Available field values (or-joined) |
| `{2}` | Dependent property name |
| `{3}` | Dependant property value |

---

## Conditional Required Attributes

These attributes make a property required based on the value of a dependent property.

### RequiredIf

The property is required when the dependent property equals the specified value.

```csharp
public enum ContactMethod { Email, Phone }

public class Contact
{
    public ContactMethod PreferredMethod { get; set; }

    [RequiredIf(nameof(PreferredMethod), ContactMethod.Email)]
    public string? EmailAddress { get; set; }
}
```

**Error message:** `"The field {0} is required if {1} is {2}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Current property name |
| `{1}` | Dependent property name |
| `{2}` | Required dependent property value |

---

### RequiredIfContains

The property is required when the dependent `IEnumerable` property contains **all** of the specified values.

```csharp
public enum Document { Passport, DrivingLicence, BirthCertificate }

public class Application
{
    public IEnumerable<Document>? SubmittedDocuments { get; set; }

    [RequiredIfContains(nameof(SubmittedDocuments), Document.Passport)]
    public string? PassportNumber { get; set; }
}
```

**Error message:** `"The field {0} is required when {1} contains {2}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Current property name |
| `{1}` | Dependent property name |
| `{2}` | Required dependent property values (and-joined) |

---

### RequiredIfOneOf

The property is required when the dependent property equals any one of the specified values. The dependent property must **not** be an `IEnumerable`.

```csharp
public enum Priority { Low, Medium, High, Critical }

public class Ticket
{
    public Priority? Level { get; set; }

    [RequiredIfOneOf(nameof(Level), Priority.High, Priority.Critical)]
    public string? Justification { get; set; }
}
```

**Error message:** `"The field {0} is required when {1} is one of {2}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Current property name |
| `{1}` | Dependent property name |
| `{2}` | Required dependent property values (and-joined) |

---

## Collection, Comparison & Membership Attributes

### ContainsAny

Validates that the property value exists within a set of allowed values provided by a type. The type must either implement `IEnumerable` or expose a `Contains` method.

```csharp
public class CountryProvider : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return "ES";
        yield return "GB";
        yield return "MX";
    }
}

public class Address
{
    [ContainsAny(typeof(CountryProvider))]
    public string? CountryCode { get; set; }
}
```

You can also use a type with a `Contains` method instead of `IEnumerable`:

```csharp
public class PrimeNumberProvider
{
    private static readonly IReadOnlySet<int> _values = new HashSet<int> { 2, 3, 5, 7 };

    public bool Contains(int value) => _values.Contains(value);
}

public class Maths
{
    [ContainsAny(typeof(PrimeNumberProvider))]
    public int? PrimeNumber { get; set; }
}
```

**Error message:** `"The field {0} must be one of the allowed values."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name |

---

### DateTimeCompare

Compares a `DateTime` property against another `DateTime` property using an `EqualityCondition`.

```csharp
public class DateRange
{
    public DateTime? StartDate { get; set; }

    [DateTimeCompare(nameof(StartDate), EqualityCondition.GreaterThan)]
    public DateTime? EndDate { get; set; }
}
```

**Error message:** `"The field {0} must be {1} {2}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Current property name |
| `{1}` | Equality condition text (e.g. "greater than") |
| `{2}` | Other property name |

---

### DateTimeCompareToday

Compares a `DateTime` property against today's date using an `EqualityCondition`.

```csharp
public class Booking
{
    [DateTimeCompareToday(EqualityCondition.GreaterThanOrEquals)]
    public DateTime? CheckInDate { get; set; }
}
```

**Error message:** `"The field {0} must be {1} today."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Current property name |
| `{1}` | Equality condition text (e.g. "greater than or equal to") |

---

### StringKeyLength

Validates the length of **keys** in an `IDictionary<string, string>` or `IEnumerable<KeyValuePair<string, string>>`. Supports both maximum and minimum length constraints.

```csharp
public class Metadata
{
    [StringKeyLength(10)]
    public IDictionary<string, string>? Tags { get; set; }

    [StringKeyLength(10, MinimumLength = 2)]
    public IDictionary<string, string>? Properties { get; set; }
}
```

**Error message (max only):** `"The field keys of {0} must be a string with a maximum length of N."`

**Error message (min and max):** `"The field keys of {0} must be a string with a minimum length of N and a maximum length of M."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name (prefixed with "keys of") |

---

### StringValueLength

Validates the length of **values** in an `IDictionary<string, string>` or `IEnumerable<KeyValuePair<string, string>>`. Supports both maximum and minimum length constraints.

```csharp
public class Metadata
{
    [StringValueLength(100)]
    public IDictionary<string, string>? Tags { get; set; }

    [StringValueLength(100, MinimumLength = 1)]
    public IDictionary<string, string>? Properties { get; set; }
}
```

**Error message (max only):** `"The field values of {0} must be a string with a maximum length of N."`

**Error message (min and max):** `"The field values of {0} must be a string with a minimum length of N and a maximum length of M."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Property name (prefixed with "values of") |

---

### SumsTo

Validates that a specified numeric child property across all items in a collection sums to an expected value.

```csharp
public class Portfolio
{
    public class Allocation
    {
        public decimal Percentage { get; set; }
    }

    [SumsTo(nameof(Allocation.Percentage), 1)]
    public IEnumerable<Allocation>? Allocations { get; set; }
}
```

**Error message:** `"The {0} members of {1} must sum to {2}."`

| Parameter | Description |
|-----------|-------------|
| `{0}` | Child property name |
| `{1}` | Collection property name |
| `{2}` | Expected sum |

---

## EqualityCondition Enum

Used by [`DateTimeCompare`](#datetimecompare) and [`DateTimeCompareToday`](#datetimecomparetoday) to specify the comparison operation.

| Value | Formatted Text |
|-------|---------------|
| `Equals` | "equal to" |
| `NotEquals` | "not equal to" |
| `GreaterThan` | "greater than" |
| `GreaterThanOrEquals` | "greater than or equal to" |
| `LessThan` | "less than" |
| `LessThanOrEquals` | "less than or equal to" |

---

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

Copyright © Exizent 2021

