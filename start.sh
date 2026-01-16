#!/bin/bash

# Initialize PostgreSQL if not already done
if [ ! -f "$PGDATA/PG_VERSION" ]; then
  echo "Initializing PostgreSQL database..."
  su - postgres -c "PATH=/usr/lib/postgresql/15/bin:$PATH initdb -D $PGDATA --username=postgres"
fi

# Start PostgreSQL as postgres user
su - postgres -c "PATH=/usr/lib/postgresql/15/bin:$PATH pg_ctl -D $PGDATA -l /var/log/postgresql/postgres.log start"

# Wait a bit for PostgreSQL to start
sleep 5

# Set the password
su - postgres -c "PATH=/usr/lib/postgresql/15/bin:$PATH psql -c \"ALTER USER postgres PASSWORD '$POSTGRES_PASSWORD';\""

# Wait for PostgreSQL to be ready
until /usr/lib/postgresql/15/bin/pg_isready -h localhost -p 5432; do
  echo "Waiting for PostgreSQL..."
  sleep 2
done

echo "PostgreSQL is ready!"

# Start the .NET app
dotnet OKServer.dll