using System.Reflection;
using FluentAssertions;
using Xunit;

namespace PimFlow.Architecture.Tests;

/// <summary>
/// Tests para validar que la documentación de onboarding esté consistente con el proyecto actual
/// </summary>
public class OnboardingDocumentationTests
{
    [Fact]
    [Trait("Category", "Documentation")]
    [Trait("Type", "Consistency")]
    public void OnboardingRules_ShouldNotContainIncorrectProjectReferences()
    {
        // Arrange
        var projectRoot = GetProjectRoot();
        var onboardingFile = Path.Combine(projectRoot, "docs", "onboarding-rules.md");
        
        // Act
        File.Exists(onboardingFile).Should().BeTrue("Onboarding rules file should exist");
        var content = File.ReadAllText(onboardingFile);
        
        // Assert
        content.Should().NotContain("TaskFlow PIM", "Should not reference old project name");
        content.Should().NotContain("./gitflow.sh", "Should not reference non-existent gitflow script");
        content.Should().Contain("PimFlow", "Should reference correct project name");
        content.Should().Contain("./scripts/create-feature.sh", "Should reference correct feature creation script");
    }

    [Fact]
    [Trait("Category", "Documentation")]
    [Trait("Type", "Scripts")]
    public void OnboardingRules_ShouldReferenceExistingScripts()
    {
        // Arrange
        var projectRoot = GetProjectRoot();
        var onboardingFile = Path.Combine(projectRoot, "docs", "onboarding-rules.md");
        var content = File.ReadAllText(onboardingFile);
        
        // Scripts que deberían existir según la documentación
        var expectedScripts = new[]
        {
            "scripts/create-feature.sh",
            "scripts/git-flow-status.sh", 
            "scripts/validate-feature.sh",
            "scripts/pre-merge-check.sh",
            "scripts/validate-onboarding-docs.sh"
        };
        
        // Act & Assert
        foreach (var script in expectedScripts)
        {
            var scriptPath = Path.Combine(projectRoot, script);
            File.Exists(scriptPath).Should().BeTrue($"Script {script} should exist as referenced in onboarding docs");
            
            if (content.Contains(script))
            {
                // Si está mencionado en la documentación, debe existir
                File.Exists(scriptPath).Should().BeTrue($"Referenced script {script} should exist");
            }
        }
    }

    [Fact]
    [Trait("Category", "Documentation")]
    [Trait("Type", "ProjectStructure")]
    public void OnboardingRules_ShouldReferenceCorrectProjectStructure()
    {
        // Arrange
        var projectRoot = GetProjectRoot();
        var onboardingFile = Path.Combine(projectRoot, "docs", "onboarding-rules.md");
        var content = File.ReadAllText(onboardingFile);
        
        // Proyectos que deberían existir según la documentación
        var expectedProjects = new[]
        {
            "src/PimFlow.Domain",
            "src/PimFlow.Server", 
            "src/PimFlow.Client",
            "src/PimFlow.Shared",
            "src/PimFlow.Contracts",
            "tests/PimFlow.Domain.Tests",
            "tests/PimFlow.Server.Tests",
            "tests/PimFlow.Shared.Tests",
            "tests/PimFlow.Architecture.Tests"
        };
        
        // Act & Assert
        foreach (var project in expectedProjects)
        {
            var projectPath = Path.Combine(projectRoot, project);
            Directory.Exists(projectPath).Should().BeTrue($"Project directory {project} should exist as referenced in onboarding docs");
        }
    }

    [Fact]
    [Trait("Category", "Documentation")]
    [Trait("Type", "Commands")]
    public void OnboardingRules_ShouldContainValidTestCommands()
    {
        // Arrange
        var projectRoot = GetProjectRoot();
        var onboardingFile = Path.Combine(projectRoot, "docs", "onboarding-rules.md");
        var content = File.ReadAllText(onboardingFile);
        
        // Act & Assert
        content.Should().Contain("dotnet test tests/PimFlow.Server.Tests/", "Should contain correct test command for Server tests");
        content.Should().Contain("dotnet test tests/PimFlow.Domain.Tests/", "Should contain correct test command for Domain tests");
        content.Should().Contain("dotnet test tests/PimFlow.Architecture.Tests/", "Should contain correct test command for Architecture tests");
        content.Should().Contain("dotnet run --project src/PimFlow.Server", "Should contain correct run command");
    }

    [Fact]
    [Trait("Category", "Documentation")]
    [Trait("Type", "URLs")]
    public void OnboardingRules_ShouldContainCorrectURLs()
    {
        // Arrange
        var projectRoot = GetProjectRoot();
        var onboardingFile = Path.Combine(projectRoot, "docs", "onboarding-rules.md");
        var content = File.ReadAllText(onboardingFile);
        
        // Act & Assert
        content.Should().Contain("http://localhost:5001", "Should reference correct port");
        content.Should().Contain("api/articles", "Should reference correct API endpoints");
        content.Should().Contain("api/categories", "Should reference correct API endpoints");
        content.Should().Contain("api/customattributes", "Should reference correct API endpoints");
    }

    [Fact]
    [Trait("Category", "Documentation")]
    [Trait("Type", "Validation")]
    public void ValidationScript_ShouldExistAndBeExecutable()
    {
        // Arrange
        var projectRoot = GetProjectRoot();
        var validationScript = Path.Combine(projectRoot, "scripts", "validate-onboarding-docs.sh");
        
        // Act & Assert
        File.Exists(validationScript).Should().BeTrue("Validation script should exist");
        
        // Verificar que el script tiene contenido
        var content = File.ReadAllText(validationScript);
        content.Should().NotBeEmpty("Validation script should have content");
        content.Should().Contain("#!/bin/bash", "Should be a bash script");
        content.Should().Contain("Validando Documentación", "Should contain validation logic");
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
}
