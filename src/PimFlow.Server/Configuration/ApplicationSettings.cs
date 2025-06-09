namespace PimFlow.Server.Configuration;

/// <summary>
/// Application-wide configuration settings
/// </summary>
public class ApplicationSettings
{
    public const string SectionName = "Application";
    
    public string Name { get; set; } = "Application";
    public string Version { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";
}

/// <summary>
/// Database configuration settings
/// </summary>
public class DatabaseSettings
{
    public const string SectionName = "Database";
    
    public string Provider { get; set; } = "SQLite";
    public string ConnectionString { get; set; } = "Data Source=App_Data/application.db";
    public string MigrationsAssembly { get; set; } = string.Empty;
}

/// <summary>
/// Feature flags configuration
/// </summary>
public class FeatureSettings
{
    public const string SectionName = "Features";
    
    public bool EnableSwagger { get; set; } = true;
    public bool EnableDetailedErrors { get; set; } = true;
    public bool EnableSeedData { get; set; } = true;
}

/// <summary>
/// Security configuration settings
/// </summary>
public class SecuritySettings
{
    public const string SectionName = "Security";
    
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
