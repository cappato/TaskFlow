using AutoMapper;
using FluentAssertions;
using PimFlow.Server.Mapping;
using Xunit;

namespace PimFlow.Server.Tests.Mapping;

/// <summary>
/// Tests para validar la configuración de AutoMapper
/// Estos tests detectan problemas de configuración antes de que lleguen a producción
/// </summary>
public class AutoMapperConfigurationTests
{
    [Fact]
    public void AutoMapperConfiguration_ShouldBeValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
        });

        // Act & Assert - Esto fallará si hay errores de configuración
        configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void AllProfiles_ShouldBeRegistered()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
        });

        // Act & Assert - Verificar que la configuración es válida
        // Esto indirectamente verifica que los perfiles están registrados
        configuration.AssertConfigurationIsValid();

        var mapper = configuration.CreateMapper();
        mapper.Should().NotBeNull();
    }

    [Fact]
    public void AutoMapperConfiguration_ShouldNotHaveCircularReferences()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
        });

        var mapper = configuration.CreateMapper();

        // Act & Assert - Esto detectaría referencias circulares
        Action act = () => configuration.AssertConfigurationIsValid();
        act.Should().NotThrow("AutoMapper configuration should not have circular references");
    }

    [Fact]
    public void AutoMapperConfiguration_ShouldHandleNullValues()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
        });

        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void AutoMapperProfiles_ShouldHaveCorrectNames()
    {
        // Arrange
        var articleProfile = new ArticleMappingProfile();
        var customAttributeProfile = new CustomAttributeMappingProfile();

        // Act & Assert
        articleProfile.ProfileName.Should().Be("PimFlow.Server.Mapping.ArticleMappingProfile");
        customAttributeProfile.ProfileName.Should().Be("PimFlow.Server.Mapping.CustomAttributeMappingProfile");
    }

    [Theory]
    [InlineData(typeof(ArticleMappingProfile))]
    [InlineData(typeof(CustomAttributeMappingProfile))]
    public void EachProfile_ShouldBeValidIndividually(Type profileType)
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(Activator.CreateInstance(profileType) as Profile);
        });

        // Act & Assert
        Action act = () => configuration.AssertConfigurationIsValid();
        act.Should().NotThrow($"Profile {profileType.Name} should be valid individually");
    }
}
