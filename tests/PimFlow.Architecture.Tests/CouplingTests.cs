using System.Reflection;
using FluentAssertions;
using Xunit;

namespace PimFlow.Architecture.Tests;

/// <summary>
/// Tests para detectar acoplamientos arquitectónicos
/// Inspirados en el problema del rename que afectó 90+ archivos
/// </summary>
public class CouplingTests
{
    [Fact]
    [Trait("Category", "Critical")]
    [Trait("Type", "Architecture")]
    public void Architecture_ShouldNotHaveCircularDependencies()
    {
        // Arrange
        var domainAssembly = typeof(PimFlow.Domain.Entities.Article).Assembly;
        var sharedAssembly = typeof(PimFlow.Shared.DTOs.ArticleDto).Assembly;
        var serverAssembly = typeof(PimFlow.Server.Services.ArticleService).Assembly;

        // Act & Assert
        // Shared NO debe referenciar Domain (viola Clean Architecture)
        var sharedReferences = sharedAssembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToList();

        sharedReferences.Should().NotContain("PimFlow.Domain", 
            "Shared layer should not depend on Domain layer in Clean Architecture");
    }

    [Fact]
    [Trait("Category", "Aspirational")]
    [Trait("Type", "Architecture")]
    public void Services_ShouldNotContainHardcodedMapping()
    {
        // Arrange
        var serverAssembly = typeof(PimFlow.Server.Services.ArticleService).Assembly;
        var serviceTypes = serverAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Service") && !t.IsInterface)
            .ToList();

        // Act & Assert
        foreach (var serviceType in serviceTypes)
        {
            var methods = serviceType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name.Contains("MapToDto") || m.Name.Contains("MapTo"))
                .ToList();

            methods.Should().BeEmpty(
                $"Service {serviceType.Name} should not contain hardcoded mapping methods. " +
                "Use centralized mapper instead to reduce coupling.");
        }
    }

    [Fact]
    public void Mappers_ShouldBeCentralized()
    {
        // Arrange
        var sharedAssembly = typeof(PimFlow.Shared.DTOs.ArticleDto).Assembly;
        var mapperTypes = sharedAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Mapper"))
            .ToList();

        // Act & Assert
        mapperTypes.Should().NotBeEmpty("Should have centralized mappers");
        
        // Todos los mappers deben estar en el namespace correcto
        foreach (var mapperType in mapperTypes)
        {
            mapperType.Namespace.Should().StartWith("PimFlow.Shared.Mappers",
                $"Mapper {mapperType.Name} should be in centralized Mappers namespace");
        }
    }

    [Fact]
    public void Entities_ShouldNotDependOnInfrastructure()
    {
        // Arrange
        var domainAssembly = typeof(PimFlow.Domain.Entities.Article).Assembly;
        var entityTypes = domainAssembly.GetTypes()
            .Where(t => t.Namespace?.Contains("Entities") == true)
            .ToList();

        // Act & Assert
        foreach (var entityType in entityTypes)
        {
            var dependencies = entityType.GetProperties()
                .Select(p => p.PropertyType)
                .Concat(entityType.GetFields().Select(f => f.FieldType))
                .Where(t => t.Namespace != null)
                .ToList();

            var infrastructureDependencies = dependencies
                .Where(t => t.Namespace.Contains("EntityFramework") || 
                           t.Namespace.Contains("Microsoft.Data") ||
                           t.Namespace.Contains("System.Data"))
                .ToList();

            infrastructureDependencies.Should().BeEmpty(
                $"Entity {entityType.Name} should not depend on infrastructure concerns");
        }
    }

    [Fact]
    public void ValidationLogic_ShouldNotBeDuplicated()
    {
        // Arrange
        var domainAssembly = typeof(PimFlow.Domain.Entities.Article).Assembly;
        var sharedAssembly = typeof(PimFlow.Shared.DTOs.ArticleDto).Assembly;

        // Act
        var domainValidationMethods = GetValidationMethods(domainAssembly);
        var sharedValidationMethods = GetValidationMethods(sharedAssembly);

        // Assert
        // Buscar patrones de validación duplicados
        var duplicatedValidations = FindDuplicatedValidationLogic(
            domainValidationMethods, 
            sharedValidationMethods);

        duplicatedValidations.Should().BeEmpty(
            "Validation logic should not be duplicated between Domain and Shared layers. " +
            "Consider using Domain validation in Shared validators.");
    }

    [Fact]
    [Trait("Category", "Monitoring")]
    [Trait("Type", "Architecture")]
    public void ProjectRename_ShouldAffectMinimalFiles()
    {
        // Arrange
        var projectRoot = GetProjectRoot();
        var allFiles = Directory.GetFiles(projectRoot, "*.*", SearchOption.AllDirectories)
            .Where(f => !f.Contains("bin") && !f.Contains("obj") && !f.Contains(".git"))
            .ToList();

        // Act
        var filesWithProjectName = allFiles
            .Where(f => FileContainsProjectName(f, "PimFlow"))
            .ToList();

        // Assert
        // Archivos que deberían cambiar en un rename:
        // - .csproj files (namespaces)
        // - Program.cs (configuration)
        // - appsettings.json (configuration)
        // - README.md (documentation)
        // - Solution file
        // - Assembly info
        var expectedMaxFiles = 30;

        filesWithProjectName.Should().HaveCountLessThanOrEqualTo(expectedMaxFiles,
            $"Project rename should affect maximum {expectedMaxFiles} files. " +
            $"Current: {filesWithProjectName.Count} files would be affected. " +
            "Consider reducing coupling by using configuration-based naming.");

        // Log affected files for analysis
        foreach (var file in filesWithProjectName.Take(10))
        {
            Console.WriteLine($"Affected file: {Path.GetRelativePath(projectRoot, file)}");
        }
    }

    [Fact]
    public void Services_ShouldFollowSingleResponsibilityPrinciple()
    {
        // Arrange
        var serverAssembly = typeof(PimFlow.Server.Services.ArticleService).Assembly;
        var serviceTypes = serverAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Service") && !t.IsInterface)
            .ToList();

        // Act & Assert
        foreach (var serviceType in serviceTypes)
        {
            var publicMethods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName && m.DeclaringType == serviceType)
                .ToList();

            // Un servicio no debería tener más de 10 métodos públicos
            publicMethods.Should().HaveCountLessThanOrEqualTo(10,
                $"Service {serviceType.Name} has too many responsibilities. " +
                "Consider splitting into smaller services or using CQRS pattern.");

            // Verificar que no mezcle concerns diferentes
            var methodNames = publicMethods.Select(m => m.Name).ToList();
            var hasCrudMethods = methodNames.Any(n => n.StartsWith("Create") || n.StartsWith("Update") || n.StartsWith("Delete"));
            var hasQueryMethods = methodNames.Any(n => n.StartsWith("Get") || n.StartsWith("Search"));
            var hasMappingMethods = methodNames.Any(n => n.Contains("Map"));
            var hasValidationMethods = methodNames.Any(n => n.Contains("Validate"));

            if (hasCrudMethods && hasQueryMethods && (hasMappingMethods || hasValidationMethods))
            {
                Assert.True(false, 
                    $"Service {serviceType.Name} mixes too many concerns (CRUD + Query + Mapping/Validation). " +
                    "Consider using CQRS pattern to separate commands and queries.");
            }
        }
    }

    private static List<MethodInfo> GetValidationMethods(Assembly assembly)
    {
        return assembly.GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.Name.Contains("Validate") || m.Name.Contains("IsValid"))
            .ToList();
    }

    private static List<string> FindDuplicatedValidationLogic(
        List<MethodInfo> domainMethods, 
        List<MethodInfo> sharedMethods)
    {
        // Simplificado: buscar métodos con nombres similares
        var duplicated = new List<string>();
        
        foreach (var domainMethod in domainMethods)
        {
            var similarSharedMethods = sharedMethods
                .Where(sm => sm.Name.Contains(domainMethod.Name.Replace("IsValid", "")) ||
                            domainMethod.Name.Contains(sm.Name.Replace("Validate", "")))
                .ToList();

            if (similarSharedMethods.Any())
            {
                duplicated.Add($"Potential duplication: {domainMethod.DeclaringType?.Name}.{domainMethod.Name}");
            }
        }

        return duplicated;
    }

    private static string GetProjectRoot()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        while (currentDirectory != null && !File.Exists(Path.Combine(currentDirectory, "PimFlow.sln")))
        {
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }
        return currentDirectory ?? Directory.GetCurrentDirectory();
    }

    private static bool FileContainsProjectName(string filePath, string projectName)
    {
        try
        {
            var content = File.ReadAllText(filePath);
            return content.Contains(projectName, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
