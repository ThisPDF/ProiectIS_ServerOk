using Microsoft.EntityFrameworkCore;
using OKServer.Data;
using OKServer.Models;

namespace OKServer.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly ApplicationDbContext _context;

    public DeviceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Device>> GetAllAsync()
    {
        return await _context.Devices.ToListAsync();
    }

    public async Task<Device?> GetByIdAsync(Guid id)
    {
        return await _context.Devices.FindAsync(id);
    }

    public async Task<Device> CreateAsync(Device device)
    {
        try
        {
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return device;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            throw new InvalidOperationException($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}. Please ensure the database exists and migrations are applied.", dbEx);
        }
        catch (Npgsql.NpgsqlException npgsqlEx)
        {
            throw new InvalidOperationException($"PostgreSQL connection error: {npgsqlEx.Message}. Please check your connection string and ensure PostgreSQL is running.", npgsqlEx);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create device: {ex.Message}", ex);
        }
    }

    public async Task<Device> UpdateAsync(Device device)
    {
        _context.Devices.Update(device);
        await _context.SaveChangesAsync();
        return device;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null)
            return false;

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Devices.AnyAsync(d => d.Id == id);
    }
}

