using System.Reflection;

namespace PimFlow.Contracts.Configuration;

/// <summary>
/// Información centralizada de la aplicación
/// Permite cambiar el nombre del proyecto sin modificar múltiples archivos
/// </summary>
public static class ApplicationInfo
{
    /// <summary>
    /// Nombre de la aplicación (configurable)
    /// </summary>
    public static string Name => GetConfiguredName();
    
    /// <summary>
    /// Versión de la aplicación
    /// </summary>
    public static string Version => GetAssemblyVersion();
    
    /// <summary>
    /// Namespace base de la aplicación
    /// </summary>
    public static string BaseNamespace => GetBaseNamespace();
    
    /// <summary>
    /// Prefijo para nombres de clases y servicios
    /// </summary>
    public static string ClassPrefix => Name;
    
    /// <summary>
    /// Obtiene el nombre configurado de la aplicación
    /// Prioridad: Variable de entorno > Assembly Attribute > Default
    /// </summary>
    private static string GetConfiguredName()
    {
        // 1. Intentar variable de entorno
        var envName = Environment.GetEnvironmentVariable("APPLICATION_NAME");
        if (!string.IsNullOrWhiteSpace(envName))
            return envName;
        
        // 2. Intentar Assembly Attribute
        var assembly = Assembly.GetExecutingAssembly();
        var titleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
        if (titleAttribute != null && !string.IsNullOrWhiteSpace(titleAttribute.Title))
            return titleAttribute.Title;
        
        // 3. Default basado en namespace
        var namespaceParts = typeof(ApplicationInfo).Namespace?.Split('.');
        if (namespaceParts?.Length > 0)
            return namespaceParts[0];
        
        // 4. Fallback
        return "PimFlow";
    }
    
    /// <summary>
    /// Obtiene la versión del assembly
    /// </summary>
    private static string GetAssemblyVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version?.ToString() ?? "1.0.0";
    }
    
    /// <summary>
    /// Obtiene el namespace base
    /// </summary>
    private static string GetBaseNamespace()
    {
        var namespaceParts = typeof(ApplicationInfo).Namespace?.Split('.');
        if (namespaceParts?.Length > 0)
            return namespaceParts[0];
        
        return Name;
    }
    
    /// <summary>
    /// Genera un nombre de clase con el prefijo de la aplicación
    /// </summary>
    public static string GetClassName(string baseName)
    {
        return $"{ClassPrefix}.{baseName}";
    }
    
    /// <summary>
    /// Genera un namespace con el prefijo de la aplicación
    /// </summary>
    public static string GetNamespace(string subNamespace)
    {
        return $"{BaseNamespace}.{subNamespace}";
    }
}
