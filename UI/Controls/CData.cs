using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

// -------------------------------------------------------------------------------------------
// PROGRAM ENTRY POINT — CLEANEST & MOST EXTENSIBLE VERSION
// -------------------------------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.Services.RegisterCoreServices(builder.Configuration);
builder.Services.RegisterDatabase(builder.Configuration);
builder.Services.RegisterDomainServices();
builder.Services.RegisterApiServices(builder.Configuration);

var app = builder.Build();

app.ConfigureMiddleware();
app.MapControllers();

app.Run();


// ==================================================================================================
//                              EXTENSION METHODS — SUPER CLEAN ARCHITECTURE
// ==================================================================================================

#region LOGGING

public static class LoggingExtensions
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
    }
}

#endregion


#region SERVICE REGISTRATION

public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Register core utilities, configuration managers, and global services.
    /// </summary>
    public static IServiceCollection RegisterCoreServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IConfigurationManager, ConfigurationManager>();
        return services;
    }

    /// <summary>
    /// Register database & EF Core configuration.
    /// </summary>
    public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration config)
    {
        var connection = config.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Missing connection string.");

        services.AddDbContext<IAppDbContext, AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connection, sql =>
            {
                sql.EnableRetryOnFailure(10, TimeSpan.FromSeconds(20), null);
            });

            if (bool.TryParse(config["Database:EnableSensitiveDataLogging"], out var sensitive)
                && sensitive)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }

    /// <summary>
    /// Register repositories & business logic.
    /// </summary>
    public static IServiceCollection RegisterDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDataProcessorService, DataProcessorService>();

        return services;
    }

    /// <summary>
    /// Register controllers, Swagger, API versioning, validation, etc.
    /// </summary>
    public static IServiceCollection RegisterApiServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Final Year Project API",
                Version = "v1",
                Description = "API backend for data processing & management",
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                swagger.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}

#endregion


#region MIDDLEWARE PIPELINE

public static class MiddlewareExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.ConfigureDevEnvironment();
        }
        else
        {
            app.ConfigureProdEnvironment();
        }

        // Common middleware
        app.UseHttpsRedirection();
        app.UseRouting();

        // Add authentication/authorization when implemented
        // app.UseAuthentication();
        app.UseAuthorization();
    }

    private static void ConfigureDevEnvironment(this WebApplication app)
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(ui =>
        {
            ui.SwaggerEndpoint("/swagger/v1/swagger.json", "FYP API v1");
            ui.DisplayRequestDuration();
        });

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>() as DbContext;

        if (db != null)
        {
            try
            {
                db.Database.Migrate();
                app.Logger.LogInformation("Database migrated successfully.");
            }
            catch (Exception ex)
            {
                app.Logger.LogCritical(ex, "Database migration failed.");
            }
        }
    }

    private static void ConfigureProdEnvironment(this WebApplication app)
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();

        // 👇 Future commits:
        // app.UseMiddleware<GlobalExceptionMiddleware>();
        // app.UseCors("DefaultPolicy");
    }
}

#endregion


#region DATABASE SUPPORT

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken token = default)
        => base.SaveChangesAsync(token);
}

public interface IAppDbContext : IDisposable
{
    DbSet<User> Users { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

#endregion

