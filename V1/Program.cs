using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using V1.Data;
using V1.Middlewares;
using V1.Repositories;
using V1.Security.JWT;
using V1.Services;
using V1.Utils.EmailSender;
using V1.Utils.PasswordHasher;

namespace V1
{
    /// <summary>
    /// The Program class contains the main entry point for the application 
    /// and configures services, middleware, and the HTTP request pipeline.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Protected constructor to prevent instantiation of the Program class.
        /// </summary>
        protected Program() { }

        /// <summary>
        /// Connects to the PostgreSQL database using the connection string from environment variables.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to register services.</param>
        private static void ConnectToPostgreSQL(WebApplicationBuilder builder)
        {
            // Load the PostgreSQL connection string from the environment variable
            string? connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN_STR");

            // Ensure that connection string is set when not in the Test environment
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Test" && string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("PostgreSQL connection string is not set");
            }

            // Register PostgreSQL DbContext for dependency injection
            builder.Services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        /// <summary>
        /// Configures email settings from environment variables and registers email settings in the configuration.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to register services.</param>
        private static void ConfigureEmailSettings(WebApplicationBuilder builder)
        {
            // Load and set SMTP email and password from environment variables
            builder.Configuration["EmailSettings:SenderEmail"] = Environment.GetEnvironmentVariable("SMTP_EMAIL");
            builder.Configuration["EmailSettings:Password"] = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            // Register email settings from the configuration to be used by the application
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        }

        /// <summary>
        /// Configures JWT settings from environment variables and registers them in the configuration.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to register services.</param>
        private static void ConfigureJwtSettings(WebApplicationBuilder builder)
        {
            // Load JWT secret key from the environment variable
            builder.Configuration["JwtSettings:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET");

            // Register JWT settings from the configuration
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        }

        /// <summary>
        /// Registers security services like JWT token generation for dependency injection.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to register services.</param>
        private static void RegisterSecurityServices(WebApplicationBuilder builder)
        {
            // Register the JWT generator service for token management
            builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
        }

        /// <summary>
        /// Registers utility services like password hashing and email sending for dependency injection.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to register services.</param>
        private static void RegisterUtilityServices(WebApplicationBuilder builder)
        {
            // Register password hasher and email sender utility services for dependency injection
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
        }

        /// <summary>
        /// Registers repositories like UserRepository for Dependency Injection.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to register services.</param>
        private static void RegisterRepositories(WebApplicationBuilder builder)
        {
            // Register the UserRepository to be injected into services or controllers
            builder.Services.AddScoped<IUserRepository, UserRepository>();
        }

        /// <summary>
        /// Registers application services like the UserService for Dependency Injection.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to register services.</param>
        private static void RegisterServices(WebApplicationBuilder builder)
        {
            // Register the UserService for managing user-related business logic
            builder.Services.AddScoped<IUserService, UserService>();
        }

        /// <summary>
        /// The main entry point of the application.
        /// Initializes the WebApplicationBuilder, loads environment variables, configures services, 
        /// and runs the application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task Main(string[] args)
        {
            // Load environment variables from the .env file (if present) for configuration
            Env.Load();

            // Initialize the WebApplicationBuilder
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add OpenAPI (Swagger) for API documentation
            builder.Services.AddOpenApi();

            // Configure CORS (Cross-Origin Resource Sharing) policy
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    // Get the frontend URL from environment variables for CORS configuration
                    string? frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");

                    // Ensure that the frontend URL is provided
                    if (string.IsNullOrEmpty(frontendUrl))
                    {
                        throw new InvalidOperationException("Frontend URL is not set");
                    }

                    // Allow CORS requests from the specified frontend URL
                    policy.WithOrigins(frontendUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Call methods to configure services (PostgreSQL, email, JWT, etc.)
            ConnectToPostgreSQL(builder);
            ConfigureEmailSettings(builder);
            ConfigureJwtSettings(builder);
            RegisterSecurityServices(builder);
            RegisterUtilityServices(builder);
            RegisterRepositories(builder);
            RegisterServices(builder);

            // Register controllers for Dependency Injection
            builder.Services.AddControllers();

            // Build the application
            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                // Enable OpenAPI/Swagger in development environment
                app.MapOpenApi();
                app.UseDeveloperExceptionPage();
            }

            // Use CORS policy
            app.UseCors();

            // Enforce HTTPS and apply security headers
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseMiddleware<SecurityHeaderMiddleware>();

            // Map controllers to endpoints
            app.MapControllers();

            // Run the application asynchronously
            await app.RunAsync();
        }
    }
}
