namespace OKServer.Models;

public class Heartbeat
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string Status { get; set; } = string.Empty; // OK/WARN/ERROR
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    
    // Navigation property
    public Device Device { get; set; } = null!;
}

