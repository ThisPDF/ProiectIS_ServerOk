# PowerShell script to setup EF Core migrations for OKServer

Write-Host "Installing EF Core tools..."
dotnet tool install --global dotnet-ef

Write-Host "Creating initial migration..."
dotnet ef migrations add InitialCreate --project .

Write-Host "Applying migration to database..."
dotnet ef database update --project .

Write-Host "Migration setup complete!"

