using AutoMapper;
using FluentAssertions;
using Moq;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Interfaces;
using PimFlow.Server.Mapping;
using PimFlow.Server.Services;
using PimFlow.Server.Validation;
using PimFlow.Shared.DTOs;
using Xunit;

namespace PimFlow.Server.Tests.Services;

/// <summary>
/// Tests for ArticleCommandService (CQRS Command side)
/// Tests the actual business logic for write operations
/// </summary>
public class ArticleCommandServiceTests
{
    private readonly Mock<IArticleRepository> _mockRepository;
    private readonly Mock<IArticleAttributeValueRepository> _mockAttributeValueRepository;
    private readonly Mock<IArticleValidationService> _mockValidationService;
    private readonly IMapper _mapper;
    private readonly ArticleCommandService _service;

    public ArticleCommandServiceTests()
    {
        _mockRepository = new Mock<IArticleRepository>();
        _mockAttributeValueRepository = new Mock<IArticleAttributeValueRepository>();
        _mockValidationService = new Mock<IArticleValidationService>();
        
        // Configure real AutoMapper
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArticleMappingProfile>();
            cfg.AddProfile<CustomAttributeMappingProfile>();
        });
        _mapper = configuration.CreateMapper();
        
        _service = new ArticleCommandService(
            _mockRepository.Object,
            _mockAttributeValueRepository.Object,
            _mockValidationService.Object,
            _mapper);
    }

    [Fact]
    public async Task CreateArticleAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateArticleDto
        {
            SKU = "NEW-001",
            Name = "New Article",
            CategoryId = 1
        };

        var createdArticle = new Article
        {
            Id = 1,
            SKU = "NEW-001",
            Name = "New Article",
            CategoryId = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _mockValidationService.Setup(x => x.ValidateCreateAsync(createDto))
            .ReturnsAsync(ValidationResult.Success());

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Article>()))
            .ReturnsAsync(createdArticle);

        // Act
        var result = await _service.CreateArticleAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.SKU.Should().Be("NEW-001");
        result.Name.Should().Be("New Article");
        
        _mockValidationService.Verify(x => x.ValidateCreateAsync(createDto), Times.Once);
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Article>()), Times.Once);
    }

    [Fact]
    public async Task CreateArticleAsync_WithValidationErrors_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateArticleDto
        {
            SKU = "",
            Name = ""
        };

        var validationResult = ValidationResult.Failure("SKU es requerido", "Nombre es requerido");
        
        _mockValidationService.Setup(x => x.ValidateCreateAsync(createDto))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateArticleAsync(createDto));

        exception.Message.Should().Contain("SKU es requerido");
        exception.Message.Should().Contain("Nombre es requerido");
        
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Article>()), Times.Never);
    }

    [Fact]
    public async Task UpdateArticleAsync_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingArticle = new Article
        {
            Id = 1,
            SKU = "EXISTING-001",
            Name = "Original Name",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var updateDto = new UpdateArticleDto
        {
            Name = "Updated Name",
            IsActive = false
        };

        var updatedArticle = new Article
        {
            Id = 1,
            SKU = "EXISTING-001",
            Name = "Updated Name",
            CreatedAt = existingArticle.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsActive = false
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingArticle);
        _mockValidationService.Setup(x => x.ValidateUpdateAsync(1, updateDto))
            .ReturnsAsync(ValidationResult.Success());
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Article>())).ReturnsAsync(updatedArticle);

        // Act
        var result = await _service.UpdateArticleAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.IsActive.Should().BeFalse();
        
        _mockValidationService.Verify(x => x.ValidateUpdateAsync(1, updateDto), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Once);
    }

    [Fact]
    public async Task UpdateArticleAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var updateDto = new UpdateArticleDto { Name = "Updated Name" };
        
        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Article?)null);

        // Act
        var result = await _service.UpdateArticleAsync(999, updateDto);

        // Assert
        result.Should().BeNull();
        
        _mockValidationService.Verify(x => x.ValidateUpdateAsync(It.IsAny<int>(), It.IsAny<UpdateArticleDto>()), Times.Never);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Never);
    }

    [Fact]
    public async Task DeleteArticleAsync_ShouldDelegateToRepository()
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteArticleAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }
}
