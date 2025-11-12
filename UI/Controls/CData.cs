using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

// NOTE: This setup assumes you are using the IUserRepository, IConfigurationManager, and IDataProcessorService
// from the previous code snippets, and that you have an AppDbContext for database access.

// --- 1. APPLICATION HOST BUILDER ---
var builder = WebApplication.CreateBuilder(args);

// --- 2. CONFIGURATION & LOGGING SETUP ---
// Configuration is automatically loaded from appsettings.json, environment variables, etc.
// The ConfigurationManager service below will wrap this base configuration.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// --- 3. SERVICES REGISTRATION (Dependency Injection) ---

// 3.1. Register Application Services (Singletons)
builder.Services.AddSingleton<IConfigurationManager, ConfigurationManager>();

// Fetch configuration immediately after registration (Note: Requires a working appsettings.json setup)
// In a real app, you might use IOptions pattern, but this demonstrates direct access.
var configManager = builder.Services.BuildServiceProvider().GetRequiredService<IConfigurationManager>();
var appConfig = configManager.GetSettings();

// 3.2. Register Database Context (Scoped)
// The AppDbContext is used to interact with the database (e.g., using Entity Framework Core)
builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
{
    // Use the ConnectionString retrieved from the loaded configuration
    options.UseSqlServer(appConfig.Database.ConnectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });

    if (appConfig.Database.EnableSensitiveDataLogging)
    {
        options.EnableSensitiveDataLogging();
    }
}, ServiceLifetime.Scoped);


// 3.3. Register Repository and Business Services (Scoped)
// Scoped services are created once per client request.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDataProcessorService, DataProcessorService>();

// 3.4. Register API Infrastructure
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { 
        Title = "Final Year Project API", 
        Version = "v1",
        Description = "API for data processing and user management."
    });

    // Optional: Include XML comments for Swagger documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});


// --- 4. APPLICATION BUILD ---
var app = builder.Build();

// --- 5. MIDDLEWARE PIPELINE ---

// 5.1. Development vs. Production Setup
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detailed error page for debugging
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FYP API v1"));

    // Ensure database migration runs automatically in development
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>() as DbContext;
        if (dbContext != null)
        {
            try
            {
                dbContext.Database.Migrate();
                app.Logger.LogInformation("Database migration successful.");
            }
            catch (Exception ex)
            {
                app.Logger.LogCritical(ex, "An error occurred during database migration.");
            }
        }
    }
}
else
{
    // Production/Staging setup
    app.UseExceptionHandler("/Error"); // Generic error page/handler
    app.UseHsts(); // Enforce HTTPS for production
}

app.UseHttpsRedirection();
app.UseRouting();

// 5.2. Custom Middleware (e.g., Global error handler, Security headers)
// Example: app.UseMiddleware<GlobalErrorHandlerMiddleware>();

app.UseAuthorization(); // Authentication/Authorization middleware

// --- 6. ENDPOINT MAPPING ---
// Map the Controllers defined earlier (e.g., DataController)
app.MapControllers();

// --- 7. START APPLICATION ---
app.Run();

// --- END OF FILE ---

// NOTE: Add necessary placeholder classes for compilation if needed
// These placeholders are required for the above code to compile against the previous files.

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
}

public interface IAppDbContext : IDisposable
{
    DbSet<User> Users { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

// Ensure the other interfaces/classes are accessible or defined similarly
// public interface IConfigurationManager { /* ... */ } 
// public class ConfigurationManager { /* ... */ }
// public interface IUserRepository { /* ... */ }
// public interface IDataProcessorService { /* ... */ }
