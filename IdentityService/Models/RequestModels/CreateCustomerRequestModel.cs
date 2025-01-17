﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using IdentityService.Infrastructure;

namespace IdentityService.Models.RequestModels;

public class CreateCustomerRequestModel
{
    [Required]
    [StringLength(
        50,
        MinimumLength = 5,
        ErrorMessage = "User name must be between 5 and 50 characters"
    )]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(
        50,
        MinimumLength = 5,
        ErrorMessage = "Email must be between 5 and 50 characters"
    )]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [JsonIgnore]
    public bool EmailConfirmed { get; set; } = true;

    [Required]
    [StringLength(
        50,
        MinimumLength = 5,
        ErrorMessage = "Password must be between 5 and 50 characters"
    )]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(
        50,
        MinimumLength = 5,
        ErrorMessage = "Phone number must be between 5 and 50 characters"
    )]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [StringLength(5)]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "Please enter exactly 5 digits.")]
    public string PostalCode { get; set; } = null!;

    [Required]
    [StringLength(
        50,
        MinimumLength = 5,
        ErrorMessage = "Address must be between 5 and 50 characters"
    )]
    public string StreetAddress { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "City must be between 5 and 50 characters")]
    public string City { get; set; } = null!;

    [Required]
    [MinimumAge]
    public DateTime DateOfBirth { get; set; }

    [JsonIgnore]
    public DateTime AccountCreationDate { get; } = DateTime.Now;

    [JsonIgnore]
    public DateTime LastActiveDate { get; set; }
}
