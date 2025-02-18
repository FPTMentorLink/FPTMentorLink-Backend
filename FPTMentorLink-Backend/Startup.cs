using System.Text;
using FPTMentorLink_Backend.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Data;
using Repositories.UnitOfWork;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Mappings;
using Services.Services;
using Services.Utils;

namespace FPTMentorLink_Backend;

public static class Startup
{
    /// <summary>
    /// Configures JWT authentication with bearer token support
    /// - Sets up token validation parameters
    /// - Configures issuer, audience, and signing key validation
    /// </summary>
    public static void ConfigureJwt(this WebApplicationBuilder builder)
    {
        // Configure JWT
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JwtSettings is not configured properly.");
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(jwtSettings.AccessTokenSecret)
                    )
                };
            });
    }

    /// <summary>
    /// Configures the MySQL database connection
    /// - Sets up Entity Framework with connection string
    /// - Registers DbContext for dependency injection
    /// </summary>
    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        // Configure Database
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }

    /// <summary>
    /// Registers all application services for dependency injection
    /// - AutoMapper for object mapping
    /// - Unit of Work pattern implementation
    /// - Business logic services
    /// - Utility services
    /// </summary>
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Register AutoMapper
        builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(MappingProfile).Assembly);

        // Register Unit of Work
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Services
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IAppointmentService, AppointmentService>();
        builder.Services.AddScoped<ICheckpointService, CheckpointService>();
        builder.Services.AddScoped<ICheckpointTaskService, CheckpointTaskService>();
        builder.Services.AddScoped<IFeedbackService, FeedbackService>();
        builder.Services.AddScoped<IMentorAvailabilityService, MentorAvailabilityService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IProposalService, ProposalService>();
        builder.Services.AddScoped<IWeeklyReportService, WeeklyReportService>();


        // Register Utils
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    }

    /// <summary>
    /// Configures Swagger/OpenAPI documentation
    /// - Sets up API documentation
    /// - Adds JWT authentication support in Swagger UI
    /// - Configures security definitions
    /// </summary>
    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "FPTMentorLink API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Name = "Authorization"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                        { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                    Array.Empty<string>()
                }
            });
        });
    }

    /// <summary>
    /// Configures CORS policies
    /// - Development: Allows all origins
    /// - Production: Restricts to specific domain
    /// </summary>
    public static void ConfigureCors(this WebApplicationBuilder builder)
    {
        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", x =>
            {
                x.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
            // TODO: Add production policy for specific domain
            // options.AddPolicy("Production", x =>
            // {
            //     x.WithOrigins("https://example.com")
            //         .AllowAnyMethod()
            //         .AllowAnyHeader();
            // });
        });
    }

    /// <summary>
    /// Applies any pending database migrations
    /// - Ensures database schema is up to date
    /// - Runs on application startup
    /// </summary>
    public static void ApplyMigration(this WebApplicationBuilder builder)
    {
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }

    /// <summary>
    /// Configures the application request pipeline
    /// - Sets middleware order
    /// - Enables error handling, logging, rate limiting
    /// - Enable Cors, Swagger on Development environment
    /// - Configures routing and authorization
    /// </summary>
    public static void Configure(this WebApplication app)
    {
        app.UseErrorHandling();
        app.UseRequestLogging();
        // TODO: Implement rate limiting strategy
        // app.UseRateLimiter();

        if (app.Environment.IsDevelopment())
        {
            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "HocVienCaRong API V1"); });
        }

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
    }
}