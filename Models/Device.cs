namespace OKServer.Models;

public class Device
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public ICollection<Heartbeat> Heartbeats { get; set; } = new List<Heartbeat>();
}

