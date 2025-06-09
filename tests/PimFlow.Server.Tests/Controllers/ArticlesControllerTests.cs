using Microsoft.AspNetCore.Mvc;
using Moq;
using PimFlow.Server.Controllers;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.Enums;
using PimFlow.Shared.Mappers;
using FluentAssertions;
using Xunit;

namespace PimFlow.Server.Tests.Controllers;

public class ArticlesControllerTests
{
    private readonly Mock<IArticleService> _mockService;
    private readonly ArticlesController _controller;

    public ArticlesControllerTests()
    {
        _mockService = new Mock<IArticleService>();
        _controller = new ArticlesController(_mockService.Object);
    }

    [Fact]
    public async Task GetArticles_ShouldReturnOkWithArticles()
    {
        // Arrange
        var articles = new List<ArticleDto>
        {
            new ArticleDto
            {
                Id = 1,
                SKU = "TEST-001",
                Name = "Test Article",
                Type = ArticleType.Footwear
            }
        };

        _mockService.Setup(x => x.GetAllArticlesAsync())
            .ReturnsAsync(articles);

        // Act
        var result = await _controller.GetArticles();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<ArticleDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedArticles = okResult.Value.Should().BeAssignableTo<IEnumerable<ArticleDto>>().Subject;
        returnedArticles.Should().HaveCount(1);
        returnedArticles.First().SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetArticle_WithValidId_ShouldReturnOkWithArticle()
    {
        // Arrange
        var article = new ArticleDto
        {
            Id = 1,
            SKU = "TEST-001",
            Name = "Test Article"
        };

        _mockService.Setup(x => x.GetArticleByIdAsync(1))
            .ReturnsAsync(article);

        // Act
        var result = await _controller.GetArticle(1);

        // Assert
        result.Should().BeOfType<ActionResult<ArticleDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedArticle = okResult.Value.Should().BeOfType<ArticleDto>().Subject;
        returnedArticle.Id.Should().Be(1);
        returnedArticle.SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetArticle_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(x => x.GetArticleByIdAsync(999))
            .ReturnsAsync((ArticleDto?)null);

        // Act
        var result = await _controller.GetArticle(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetArticleBySKU_WithValidSKU_ShouldReturnOkWithArticle()
    {
        // Arrange
        var article = new ArticleDto
        {
            Id = 1,
            SKU = "TEST-001",
            Name = "Test Article"
        };

        _mockService.Setup(x => x.GetArticleBySKUAsync("TEST-001"))
            .ReturnsAsync(article);

        // Act
        var result = await _controller.GetArticleBySKU("TEST-001");

        // Assert
        result.Should().BeOfType<ActionResult<ArticleDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedArticle = okResult.Value.Should().BeOfType<ArticleDto>().Subject;
        returnedArticle.SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetArticlesByAttribute_WithValidParameters_ShouldReturnOkWithArticles()
    {
        // Arrange
        var articles = new List<ArticleDto>
        {
            new ArticleDto { Id = 1, SKU = "RED-001", Name = "Red Shoes" }
        };

        _mockService.Setup(x => x.GetArticlesByAttributeAsync("color", "red"))
            .ReturnsAsync(articles);

        // Act
        var result = await _controller.GetArticlesByAttribute("color", "red");

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<ArticleDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedArticles = okResult.Value.Should().BeAssignableTo<IEnumerable<ArticleDto>>().Subject;
        returnedArticles.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetArticlesByAttribute_WithMissingParameters_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.GetArticlesByAttribute("", "red");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateArticle_WithValidData_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateArticleDto
        {
            SKU = "NEW-001",
            Name = "New Article",
            Type = ArticleType.Footwear
        };

        var createdArticle = new ArticleDto
        {
            Id = 1,
            SKU = "NEW-001",
            Name = "New Article",
            Type = ArticleType.Footwear
        };

        _mockService.Setup(x => x.CreateArticleAsync(createDto))
            .ReturnsAsync(createdArticle);

        // Act
        var result = await _controller.CreateArticle(createDto);

        // Assert
        result.Should().BeOfType<ActionResult<ArticleDto>>();
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ArticlesController.GetArticle));
        createdResult.RouteValues!["id"].Should().Be(1);

        var returnedArticle = createdResult.Value.Should().BeOfType<ArticleDto>().Subject;
        returnedArticle.SKU.Should().Be("NEW-001");
    }

    [Fact]
    public async Task CreateArticle_WithDuplicateSKU_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateArticleDto
        {
            SKU = "EXISTING-001",
            Name = "New Article"
        };

        _mockService.Setup(x => x.CreateArticleAsync(createDto))
            .ThrowsAsync(new InvalidOperationException("Ya existe un artículo con SKU: EXISTING-001"));

        // Act
        var result = await _controller.CreateArticle(createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result.As<BadRequestObjectResult>();
        badRequestResult.Value.Should().Be("Ya existe un artículo con SKU: EXISTING-001");
    }

    [Fact]
    public async Task UpdateArticle_WithValidData_ShouldReturnOkWithUpdatedArticle()
    {
        // Arrange
        var updateDto = new UpdateArticleDto
        {
            Name = "Updated Article",
            Brand = "Updated Brand"
        };

        var updatedArticle = new ArticleDto
        {
            Id = 1,
            SKU = "TEST-001",
            Name = "Updated Article",
            Brand = "Updated Brand"
        };

        _mockService.Setup(x => x.UpdateArticleAsync(1, updateDto))
            .ReturnsAsync(updatedArticle);

        // Act
        var result = await _controller.UpdateArticle(1, updateDto);

        // Assert
        result.Should().BeOfType<ActionResult<ArticleDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedArticle = okResult.Value.Should().BeOfType<ArticleDto>().Subject;
        returnedArticle.Name.Should().Be("Updated Article");
        returnedArticle.Brand.Should().Be("Updated Brand");
    }

    [Fact]
    public async Task UpdateArticle_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateArticleDto { Name = "Updated Article" };

        _mockService.Setup(x => x.UpdateArticleAsync(999, updateDto))
            .ReturnsAsync((ArticleDto?)null);

        // Act
        var result = await _controller.UpdateArticle(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteArticle_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteArticleAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteArticle(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteArticle_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteArticleAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteArticle(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task SearchArticles_WithValidTerm_ShouldReturnOkWithArticles()
    {
        // Arrange
        var articles = new List<ArticleDto>
        {
            new ArticleDto { Id = 1, SKU = "NIKE-001", Name = "Nike Shoes" }
        };

        _mockService.Setup(x => x.SearchArticlesAsync("Nike"))
            .ReturnsAsync(articles);

        // Act
        var result = await _controller.SearchArticles("Nike");

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<ArticleDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedArticles = okResult.Value.Should().BeAssignableTo<IEnumerable<ArticleDto>>().Subject;
        returnedArticles.Should().HaveCount(1);
        returnedArticles.First().Name.Should().Contain("Nike");
    }

    [Fact]
    public async Task SearchArticles_WithEmptyTerm_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.SearchArticles("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
