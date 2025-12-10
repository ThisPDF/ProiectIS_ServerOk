# Setup Database - OKServer

## Problem: "Failed to create device: An exception has been raised that is likely due to a transient failure"

Această eroare apare de obicei când:
1. Baza de date nu există
2. Migrațiile nu au fost aplicate
3. PostgreSQL nu rulează
4. Connection string-ul este incorect

## Soluție pas cu pas:

### 1. Verifică că PostgreSQL rulează

```bash
# macOS (Homebrew)
brew services list | grep postgresql

# Linux
sudo systemctl status postgresql

# Sau testează conexiunea
psql -h localhost -p 5432 -U postgres -c "SELECT version();"
```

### 2. Creează baza de date (dacă nu există)

```bash
psql -h localhost -p 5432 -U postgres -c "CREATE DATABASE \"OKServerDb\";"
```

### 3. Verifică connection string-ul

Editează `appsettings.json` și asigură-te că connection string-ul este corect:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=OKServerDb;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

**Important:** Înlocuiește `YOUR_PASSWORD` cu parola ta PostgreSQL!

### 4. Instalează EF Core Tools (dacă nu sunt instalate)

```bash
dotnet tool install --global dotnet-ef
```

### 5. Creează migrațiile

```bash
cd OKServer
dotnet ef migrations add InitialCreate
```

### 6. Aplică migrațiile la baza de date

```bash
dotnet ef database update
```

### 7. Verifică că totul funcționează

```bash
# Rulează aplicația
dotnet run

# Testează API-ul
curl http://localhost:5000/api/devices
```

Ar trebui să returneze `[]` (array gol) dacă nu există device-uri.

## Script automat

Poți folosi scriptul `check-database.sh` pentru a verifica configurația:

```bash
./check-database.sh
```

## Troubleshooting

### Eroare: "role postgres does not exist"
Creează utilizatorul:
```bash
createuser -s postgres
```

### Eroare: "password authentication failed"
Verifică parola în `appsettings.json` sau creează un utilizator nou:
```bash
psql -h localhost -p 5432 -U postgres
CREATE USER okuser WITH PASSWORD 'yourpassword';
GRANT ALL PRIVILEGES ON DATABASE "OKServerDb" TO okuser;
```

Apoi actualizează connection string-ul:
```
Host=localhost;Port=5432;Database=OKServerDb;Username=okuser;Password=yourpassword
```

### Eroare: "database does not exist"
Creează baza de date:
```bash
psql -h localhost -p 5432 -U postgres -c "CREATE DATABASE \"OKServerDb\";"
```

