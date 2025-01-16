using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using V1.Data;
using V1.Middlewares;
using V1.Repositories;
using V1.Services;
using V1.Utils.EmailSender;
using V1.Utils.PasswordHasher;

namespace V1
{
    public class Program
    {
        protected Program() { }

        private static async Task Main(string[] args)
        {
            // Load environment variables from .env file (if present)
            Env.Load();

            // Initialize the WebApplicationBuilder
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddOpenApi();

            // Add CORS policy
            builder.Services.AddCors(options =>
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

            // Load PostgreSQL connection string from environment variable
            string? connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN_STR");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Test" && string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("PostgreSQL connection string is not set");
            }

            // Register PostgreSQL DbContext
            builder.Services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Load EmailSettings and replace placeholders with environment variables
            builder.Configuration["EmailSettings:SenderEmail"] = Environment.GetEnvironmentVariable("SMTP_EMAIL");
            builder.Configuration["EmailSettings:Username"] = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            builder.Configuration["EmailSettings:Password"] = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            // Register email settings from configuration
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Register utilities services
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            // Register repositories for Dependency Injection
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Register services for Dependency Injection
            builder.Services.AddScoped<IUserService, UserService>();

            // Register controllers for Dependency Injection
            builder.Services.AddControllers();

            // Build the application
            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseDeveloperExceptionPage();
            }

            // Use CORS
            app.UseCors();

            // Use HTTPS and Security Middleware
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseMiddleware<SecurityHeaderMiddleware>();

            // Map controllers to endpoints
            app.MapControllers();

            // Run the application
            await app.RunAsync();
        }
    }
}
