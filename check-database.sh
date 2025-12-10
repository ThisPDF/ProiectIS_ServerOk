#!/bin/bash

echo "Checking PostgreSQL connection and database..."

# Check if PostgreSQL is running
if ! pg_isready -h localhost -p 5432 > /dev/null 2>&1; then
    echo "❌ PostgreSQL is not running or not accessible on localhost:5432"
    echo "Please start PostgreSQL and try again."
    exit 1
fi

echo "✅ PostgreSQL is running"

# Check if database exists
if psql -h localhost -p 5432 -U postgres -lqt | cut -d \| -f 1 | grep -qw OKServerDb; then
    echo "✅ Database 'OKServerDb' exists"
else
    echo "❌ Database 'OKServerDb' does not exist"
    echo "Creating database..."
    psql -h localhost -p 5432 -U postgres -c "CREATE DATABASE \"OKServerDb\";"
    if [ $? -eq 0 ]; then
        echo "✅ Database 'OKServerDb' created successfully"
    else
        echo "❌ Failed to create database. Please check your PostgreSQL credentials."
        exit 1
    fi
fi

# Check if migrations have been applied
echo ""
echo "Checking migrations..."
cd "$(dirname "$0")"
if [ -d "Migrations" ] && [ "$(ls -A Migrations 2>/dev/null)" ]; then
    echo "✅ Migrations folder exists"
    echo "Run: dotnet ef database update"
else
    echo "⚠️  No migrations found. Creating initial migration..."
    echo "Run: dotnet ef migrations add InitialCreate"
    echo "Then: dotnet ef database update"
fi

