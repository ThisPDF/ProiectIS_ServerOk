using Microsoft.EntityFrameworkCore;
using OKServer.Data;
using OKServer.Models;

namespace OKServer.Repositories;

public class HeartbeatRepository : IHeartbeatRepository
{
    private readonly ApplicationDbContext _context;

    public HeartbeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Heartbeat>> GetByDeviceIdAsync(Guid deviceId)
    {
        return await _context.Heartbeats
            .Where(h => h.DeviceId == deviceId)
            .OrderByDescending(h => h.Timestamp)
            .ToListAsync();
    }

    public async Task<Heartbeat?> GetByIdAsync(Guid id)
    {
        return await _context.Heartbeats.FindAsync(id);
    }

    public async Task<Heartbeat> CreateAsync(Heartbeat heartbeat)
    {
        _context.Heartbeats.Add(heartbeat);
        await _context.SaveChangesAsync();
        return heartbeat;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var heartbeat = await _context.Heartbeats.FindAsync(id);
        if (heartbeat == null)
            return false;

        _context.Heartbeats.Remove(heartbeat);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Heartbeats.AnyAsync(h => h.Id == id);
    }
}

