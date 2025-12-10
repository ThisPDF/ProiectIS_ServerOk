using System.ComponentModel.DataAnnotations;

namespace OKServer.DTOs;

public class UpdateDeviceDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "OwnerEmail is required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address format")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string OwnerEmail { get; set; } = string.Empty;
}

