using System;
using System.ComponentModel.DataAnnotations;

namespace Auto.Website.Models;

public class OwnerDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    [Range(18,100)]
    public int Age { get; set; }
    [Required]
    public string VehicleRegistration { get; set; }
}