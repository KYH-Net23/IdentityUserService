﻿namespace IdentityService.Models;

public class CustomerRequestResponse
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Username { get; set; }
}