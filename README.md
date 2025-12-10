# OKServer - .NET 8 Web API

A complete .NET 8 Web API project with a modern web UI for managing devices and their heartbeats.

## Features

- 🎨 **Modern Web UI** - Beautiful, responsive interface for managing devices and heartbeats
- 🔌 **RESTful API** - Complete REST API with Swagger documentation
- 🗄️ **PostgreSQL Database** - Robust data storage with EF Core
- ✅ **Data Validation** - Built-in validation for all inputs
- 🗺️ **AutoMapper** - Automatic DTO mapping

## Architecture

- **Controller → Service → Repository** pattern
- **PostgreSQL** database with EF Core
- **AutoMapper** for DTO mapping
- **Data Annotations** for data validation

## Entities

### Device
- `Id` (Guid)
- `Name` (string, required)
- `OwnerEmail` (string, required, valid email)
- `CreatedAt` (DateTime)

### Heartbeat
- `Id` (Guid)
- `DeviceId` (Guid, foreign key)
- `Status` (string: OK/WARN/ERROR)
- `Message` (string)
- `Timestamp` (DateTime)

## API Endpoints

### DeviceController
- `GET /api/devices` - Get all devices
- `GET /api/devices/{id}` - Get device by ID
- `POST /api/devices` - Create a new device
- `PUT /api/devices/{id}` - Update a device
- `DELETE /api/devices/{id}` - Delete a device

### HeartbeatController
- `GET /api/devices/{id}/heartbeats` - Get all heartbeats for a device
- `POST /api/devices/{id}/heartbeats` - Create a new heartbeat for a device
- `GET /api/heartbeats/{id}` - Get heartbeat by ID
- `DELETE /api/heartbeats/{id}` - Delete a heartbeat

## Setup

1. **Install PostgreSQL** and create a database named `OKServerDb`

2. **Update connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=OKServerDb;Username=your_username;Password=your_password"
   }
   ```

3. **Run migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

5. **Access the Web UI**:
   - Open your browser and navigate to `http://localhost:5000` or `https://localhost:5001`
   - The modern web interface allows you to:
     - View and manage devices
     - Add, edit, and delete devices
     - View heartbeats for each device
     - Add new heartbeats
     - View all heartbeats across all devices

6. **Access Swagger UI** (optional):
   - Navigate to `https://localhost:5001/swagger` for API documentation

## Project Structure

```
OKServer/
├── Controllers/
│   ├── DeviceController.cs
│   └── HeartbeatController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── DTOs/
│   ├── DeviceDto.cs
│   ├── CreateDeviceDto.cs
│   ├── UpdateDeviceDto.cs
│   ├── HeartbeatDto.cs
│   └── CreateHeartbeatDto.cs
├── Mappings/
│   └── MappingProfile.cs
├── Models/
│   ├── Device.cs
│   └── Heartbeat.cs
├── Repositories/
│   ├── IDeviceRepository.cs
│   ├── DeviceRepository.cs
│   ├── IHeartbeatRepository.cs
│   └── HeartbeatRepository.cs
├── Services/
│   ├── IDeviceService.cs
│   ├── DeviceService.cs
│   ├── IHeartbeatService.cs
│   └── HeartbeatService.cs
├── wwwroot/
│   ├── index.html
│   ├── css/
│   │   └── styles.css
│   └── js/
│       └── app.js
├── appsettings.json
├── Program.cs
└── OKServer.csproj
```

## Web UI Features

The web interface provides:

- **Devices Tab**:
  - View all devices in a card-based layout
  - Add new devices with validation
  - Edit existing devices
  - Delete devices (with confirmation)
  - View heartbeats for each device

- **All Heartbeats Tab**:
  - View all heartbeats from all devices
  - Color-coded status indicators (OK/WARN/ERROR)
  - Delete individual heartbeats
  - Filtered by device

- **Device Heartbeats Modal**:
  - View all heartbeats for a specific device
  - Add new heartbeats directly from the modal
  - Real-time updates

The UI is fully responsive and works on desktop, tablet, and mobile devices.

