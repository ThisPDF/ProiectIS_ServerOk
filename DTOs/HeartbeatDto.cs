namespace OKServer.DTOs;

public class HeartbeatDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

