using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using V1.Data;
using V1.Repositories;
using V1.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Load env
Env.Load();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add PostgreSQL connection string from env
string? connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN_STR");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("PostgreSQL connection string is not set");
}

// Add connection string to db context
builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(connectionString));

// Register repositories for Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register services for Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();

// Register the controllers
builder.Services.AddControllers();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();