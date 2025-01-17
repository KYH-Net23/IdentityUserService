﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models.DataModels;

public class CustomerEntity : IdentityUser
{
    [Required]
    [StringLength(
        200,
        MinimumLength = 5,
        ErrorMessage = "Username must be between 5 and 200 characters"
    )]
    public string StreetAddress { get; set; } = null!;

    [Required]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "First name must be between 2 and 100 characters"
    )]
    public string City { get; set; } = null!;

    [Required]
    [StringLength(5)]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "Please enter exactly 5 digits.")]
    public string PostalCode { get; set; } = null!;

    [Required]
    public DateTime DateOfBirth { get; set; }
    public DateTime AccountCreationDate { get; set; } = DateTime.Now;
    public DateTime LastActiveDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool? HasRequestedPasswordReset { get; set; }
    public string? PasswordResetGuid { get; set; }
}
