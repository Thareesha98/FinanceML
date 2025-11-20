using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

// -----------------------------------------------------------------------------
// PROGRAM ENTRY POINT — CLEAN, MAINTAINABLE, CONTRIBUTION-FRIENDLY SETUP
// -----------------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// 1. Logging Configuration -----------------------------------------------------
ConfigureLogging(builder.Logging);

// 2. Register Application Services --------------------------------------------
RegisterCoreServices(builder.Services, builder.Configuration);

// 3. Register Database Context -------------------------------------------------
RegisterDatabase(builder.Services, builder.Configuration);

// 4. Register Business Services & Repositories --------------------------------
RegisterDomainServices(builder.Services);

// 5. API & Swagger -------------------------------------------------------------
RegisterApi(builder.Services);

var app = builder.Build();

// 6. Middleware Pipeline -------------------------------------------------------
ConfigureMiddleware(app);

// 7. Map Endpoints -------------------------------------------------------------
app.MapControllers();

// 8. Run Application -----------------------------------------------------------
app.Run();


// ============================================================================
//                         CONFIGURATION MODULES
// ============================================================================

static void ConfigureLogging(ILoggingBuilder logging)
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
}


// ============================================================================
//                        SERVICE REGISTRATION MODULES
// ============================================================================

static void RegisterCoreServices(IServiceCollection services, IConfiguration config)
{
    // Wrap config in strongly-typed manager (future commit opportunity)
    services.AddSingleton<IConfigurationManager, ConfigurationManager>();
}

static void RegisterDatabase(IServiceCollection services, IConfiguration config)
{
    // Load settings (optional) - using IOptions is recommended in future commits
    var connectionString = config.GetConnectionString("DefaultConnection");

    services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    {
        options.UseSqlServer(connectionString, sql =>
        {
            sql.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        });
        
        // Toggle sensitive data logging based on config
        if (bool.TryParse(config["Database:EnableSensitiveDataLogging"], out var sensitiveLogging)
            && sensitiveLogging)
        {
            options.EnableSensitiveDataLogging();
        }

    }, ServiceLifetime.Scoped);
}

static void RegisterDomainServices(IServiceCollection services)
{
    // Repositories
    services.AddScoped<IUserRepository, UserRepository>();

    // Business logic/services
    services.AddScoped<IDataProcessorService, DataProcessorService>();
}

static void RegisterApi(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();

    // Swagger setup
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Final Year Project API",
            Version = "v1",
            Description = "API backend for data processing & management."
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });
}


// ============================================================================
//                        MIDDLEWARE CONFIGURATION
// ============================================================================

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        ConfigureDevEnvironment(app);
    }
    else
    {
        ConfigureProdEnvironment(app);
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();

    // Future Commit Idea:
    // app.UseMiddleware<GlobalErrorHandlerMiddleware>();
}

static void ConfigureDevEnvironment(WebApplication app)
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FYP API v1");
    });

    // Auto migration during development
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>() as DbContext;

    if (db != null)
    {
        try
        {
            db.Database.Migrate();
            app.Logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogCritical(ex, "Database migration error occurred.");
        }
    }
}

static void ConfigureProdEnvironment(WebApplication app)
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


// ============================================================================
//                          SUPPORTING DB CLASSES
// ============================================================================

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
}

public interface IAppDbContext : IDisposable
{
    DbSet<User> Users { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

