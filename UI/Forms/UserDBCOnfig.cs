using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// --- CONFIGURATION MODELS ---

/// <summary>
/// Root model to hold all application settings.
/// </summary>
public class AppConfig
{
    public DatabaseSettings Database { get; set; } = new DatabaseSettings();
    public ApiServiceSettings ExternalApi { get; set; } = new ApiServiceSettings();
    public LoggingSettings Logging { get; set; } = new LoggingSettings();
}

/// <summary>
/// Settings related to the primary database connection.
/// </summary>
public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeoutSeconds { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; } = false;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new InvalidOperationException("Database connection string is required.");
        }
    }
}

/// <summary>
/// Settings for an external service dependency.
/// </summary>
public class ApiServiceSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutMilliseconds { get; set; } = 5000;
}

/// <summary>
/// Settings for application logging.
/// </summary>
public class LoggingSettings
{
    public string LogFilePath { get; set; } = "logs/app.log";
    public string LogLevel { get; set; } = "Information";
    public bool LogToConsole { get; set; } = true;
}

// --- CONFIGURATION SERVICE ---

public interface IConfigurationManager
{
    AppConfig GetSettings();
    void ReloadSettings();
}

/// <summary>
/// Manages loading and providing application configuration settings.
/// </summary>
public class ConfigurationManager : IConfigurationManager
{
    private readonly ILogger<ConfigurationManager> _logger;
    private AppConfig _config;

    // Default environment to load if not specified
    private const string DefaultEnvironment = "Development";

    public ConfigurationManager(ILogger<ConfigurationManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        // Load configuration during initialization
        LoadConfiguration();
    }

    /// <summary>
    /// Builds the configuration from appsettings files and environment variables.
    /// </summary>
    private void LoadConfiguration()
    {
        _logger.LogInformation("Starting configuration loading process.");

        try
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? DefaultEnvironment;
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // Load base settings
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                // Load environment-specific settings (e.g., appsettings.Development.json)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                // Override with environment variables
                .AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            // Bind the configuration sections to the strongly typed models
            _config = configurationRoot.Get<AppConfig>();

            // Perform post-load validation
            _config.Database.Validate();

            _logger.LogInformation("Configuration successfully loaded for environment: {Env}", environmentName);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "Configuration file not found. Ensure 'appsettings.json' is present.");
            throw new InvalidOperationException("Missing critical configuration file.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "FATAL: Failed to load or validate application configuration.");
            throw;
        }
    }
    
    /// <summary>
    /// Retrieves the fully loaded and validated configuration object.
    /// </summary>
    /// <returns>The AppConfig instance.</returns>
    public AppConfig GetSettings()
    {
        // Return the cached instance. Since reloadOnChange is true, 
        // a monitor/change token would typically handle real-time updates in a larger framework.
        return _config ?? throw new InvalidOperationException("Configuration has not been loaded.");
    }

    /// <summary>
    /// Forces a manual reload of the configuration.
    /// </summary>
    public void ReloadSettings()
    {
        _logger.LogWarning("Manually reloading application configuration.");
        LoadConfiguration();
    }
}
