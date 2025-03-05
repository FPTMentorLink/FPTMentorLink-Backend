using System.Text;
using FPTMentorLink_Backend.Middlewares;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Data;
using Repositories.UnitOfWork;
using Repositories.UnitOfWork.Interfaces;
using Services.InfrastructureService.Redis;
using Services.Interfaces;
using Services.Mappings;
using Services.Services;
using Services.Settings;
using Services.Utils;
using StackExchange.Redis;

namespace FPTMentorLink_Backend;

public static class Startup
{
    /// <summary>
    /// Configures the authentication system for the application
    /// - Sets up JWT authentication
    /// - Adds cookie authentication (temporarily store user's identity after sign in)
    /// - Adds Google authentication
    /// </summary>
    /// <param name="builder"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        // Get settings
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JwtSettings is not configured properly.");
        var googleAuthSettings = builder.Configuration.GetSection("GoogleAuthSettings").Get<GoogleSettings>()
                                 ?? throw new InvalidOperationException(
                                     "GoogleAuthSettings is not configured properly.");
        var redirectUrlSettings = builder.Configuration.GetSection("RedirectUrlSettings").Get<RedirectUrlSettings>()
                                  ?? throw new InvalidOperationException(
                                      "RedirectUrlSettings is not configured properly.");

        // Configure JWT settings for DI
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        // Configure authentication with a single call
        builder.Services.AddAuthentication(options =>
            {
                // Use JWT as the default for API authentication
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                // Use cookies for external authentication sign-in
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            // Add cookie authentication
            .AddCookie(options =>
            {
                // Prevents client-side scripts from accessing the cookie, 
                // enhancing security against XSS attacks.
                options.Cookie.HttpOnly = true;
                // Allows cookies to be sent with requests to the same site, 
                // but not with requests to other sites.
                options.Cookie.SameSite = SameSiteMode.Lax;
                // Ensures cookies are only sent over HTTPS connections, 
                // preventing interception over insecure channels.
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                // Sets the expiration time for the cookie to 5 minutes.
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            // Add JWT bearer authentication
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.AccessTokenSecret)
                    )
                };
            })
            // Add Google authentication
            .AddGoogle(options =>
            {
                options.ClientId = googleAuthSettings.ClientId;
                options.ClientSecret = googleAuthSettings.ClientSecret;

                options.Events = new OAuthEvents
                {
                    OnRedirectToAuthorizationEndpoint = context =>
                    {
                        context.Response.Redirect(context.RedirectUri);
                        // redirectUri param will be https://domain.com/signin-google (default google handler path)
                        return Task.CompletedTask;
                    },
                    OnRemoteFailure = context =>
                    {
                        var error = context.Failure?.Message ?? "Unknown error";
                        Console.WriteLine($"Remote authentication error: {error}");

                        // Redirect back to google sign in api 
                        context.Response.Redirect(redirectUrlSettings.LoginFailedUrl);
                        context.HandleResponse(); // Prevent further processing

                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static void ConfigureRedis(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisSettings = builder.Configuration.GetSection("ConnectionStrings:Redis").Get<RedisSettings>()
                                ?? throw new InvalidOperationException("RedisConnection is not configured properly.");
            return ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { { redisSettings.EndPoints, redisSettings.Port } },
                    User = redisSettings.User,
                    Password = redisSettings.Password
                }
            );
        });
        builder.Services.AddSingleton<IRedisService, RedisService>();
    }

    public static void ConfigureRedirectUrl(this WebApplicationBuilder builder)
    {
        _ = builder.Configuration.GetSection("RedirectUrlSettings").Get<RedirectUrlSettings>()
            ?? throw new InvalidOperationException(
                "RedirectUrlSettings is not configured properly.");
        builder.Services.Configure<RedirectUrlSettings>(builder.Configuration.GetSection("RedirectUrlSettings"));
    }

    public static void ConfigureEmailService(this WebApplicationBuilder builder)
    {
        _ = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>()
            ?? throw new InvalidOperationException("EmailSettings is not configured properly.");

        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.AddSingleton<IEmailService, EmailService>();
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
        // Register Mappings
        MappingConfig.RegisterMappings();
        builder.Services.AddScoped<IMapper, Mapper>();

        // Register Unit of Work
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Services
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IAppointmentService, AppointmentService>();
        builder.Services.AddScoped<ICheckpointService, CheckpointService>();
        builder.Services.AddScoped<ICheckpointTaskService, CheckpointTaskService>();
        builder.Services.AddScoped<IFacultyService, FacultyService>();
        builder.Services.AddScoped<ILecturingProposalService, LecturingProposalService>();
        builder.Services.AddScoped<IFeedbackService, FeedbackService>();
        builder.Services.AddScoped<IMentorAvailabilityService, MentorAvailabilityService>();
        builder.Services.AddScoped<IMentoringProposalService, MentoringProposalService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IProjectStudentService, ProjectStudentService>();
        builder.Services.AddScoped<IProposalService, ProposalService>();
        builder.Services.AddScoped<IWeeklyReportService, WeeklyReportService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ITermService, TermService>();

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
        }
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "FPTMentorLink API V1"); });
        app.UseRouting();
        app.UseHttpsRedirection();

        // Authentication must come before Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");
    }
}