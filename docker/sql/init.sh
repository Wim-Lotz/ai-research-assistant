#!/bin/bash
/opt/mssql/bin/sqlservr &
SQL_PID=$!

echo "Waiting for SQL Server..."
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" -C &>/dev/null; do
    sleep 2
done

DB_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name='ResearchAssistant'" -C -h -1 | tr -d '[:space:]')

if [ "$DB_EXISTS" = "0" ]; then
    echo "Running init scripts..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /init/01_schema.sql -C
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /init/02_seed.sql -C
    echo "Init complete."
else
    echo "Database already exists, skipping init."
fi

wait $SQL_PID
