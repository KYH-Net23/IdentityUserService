using System.ComponentModel.DataAnnotations;

namespace IdentityService.Infrastructure;

public class MinimumAge : ValidationAttribute
{
    private const int MinAge = 18;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime birthDay)
            return new ValidationResult("Please enter a valid date format.");

        if (birthDay.AddYears(MinAge) <= DateTime.Now)
            return ValidationResult.Success;

        return new ValidationResult("You have to be at least 18 years old to register.");
    }
}
