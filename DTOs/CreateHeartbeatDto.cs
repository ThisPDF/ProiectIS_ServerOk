using System.ComponentModel.DataAnnotations;

namespace OKServer.DTOs;

public class CreateHeartbeatDto
{
    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(OK|WARN|ERROR)$", ErrorMessage = "Status must be OK, WARN, or ERROR")]
    public string Status { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
}

