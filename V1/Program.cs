using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using V1.Data;
using V1.Middlewares;
using V1.Repositories;
using V1.Services;
using V1.Utils;

namespace V1
{
    public class Program
    {
        protected Program() { }

        private static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Load env
            Env.Load();

            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Add CORS policy
            builder.Services.AddCors( options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    string? frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");

                    // Ensure the frontend URL is provided
                    if (string.IsNullOrEmpty(frontendUrl))
                    {
                        throw new InvalidOperationException("Frontend URL is not set");
                    }

                    // Allow CORS from the specified frontend URL
                    policy.WithOrigins(frontendUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Add PostgreSQL connection string from env
            string? connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN_STR");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Test" && string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("PostgreSQL connection string is not set");
            }

            // Add connection string to db context
            builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(connectionString));

            // Register utils services
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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

            app.UseCors();

            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseMiddleware<SecurityHeaderMiddleware>();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}