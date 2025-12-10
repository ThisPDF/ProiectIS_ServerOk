#!/bin/bash

# Script to setup EF Core migrations for OKServer

echo "Installing EF Core tools..."
dotnet tool install --global dotnet-ef

echo "Creating initial migration..."
dotnet ef migrations add InitialCreate --project .

echo "Applying migration to database..."
dotnet ef database update --project .

echo "Migration setup complete!"

