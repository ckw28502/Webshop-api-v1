@REM Run docker compose up
docker compose up -d

@REM Migrate database
dotnet ef database update

@REM Run the application
dotnet watch run