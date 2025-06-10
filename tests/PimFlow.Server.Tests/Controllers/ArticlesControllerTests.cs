using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PimFlow.Server.Controllers;
using PimFlow.Server.Services;
using PimFlow.Shared.DTOs;
using PimFlow.Shared.Enums;
using PimFlow.Shared.Mappers;
using PimFlow.Shared.Common;
using FluentAssertions;
using Xunit;

namespace PimFlow.Server.Tests.Controllers;

public class ArticlesControllerTests
{
    private readonly Mock<IArticleService> _mockService;
    private readonly Mock<ILogger<ArticlesController>> _mockLogger;
    private readonly Mock<IDomainEventService> _mockDomainEventService;
    private readonly ArticlesController _controller;

    public ArticlesControllerTests()
    {
        _mockService = new Mock<IArticleService>();
        _mockLogger = new Mock<ILogger<ArticlesController>>();
        _mockDomainEventService = new Mock<IDomainEventService>();
        _controller = new ArticlesController(_mockService.Object, _mockLogger.Object, _mockDomainEventService.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithApiResponseAndArticles()
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
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ArticleDto>>>().Subject;

        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().HaveCount(1);
        apiResponse.Data!.First().SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOkWithApiResponseAndArticle()
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
        var result = await _controller.GetById(1);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<ArticleDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<ArticleDto>>().Subject;

        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data!.Id.Should().Be(1);
        apiResponse.Data.SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnBadRequestWithApiResponse()
    {
        // Arrange
        _mockService.Setup(x => x.GetArticleByIdAsync(999))
            .ReturnsAsync((ArticleDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<ArticleDto>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<ArticleDto>>().Subject;

        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("ArticleDto no encontrado");
        apiResponse.ErrorCode.Should().Be("INVALID_OPERATION");
    }

    [Fact]
    public async Task GetArticleBySKU_WithValidSKU_ShouldReturnOkWithApiResponseAndArticle()
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
        result.Should().BeOfType<ActionResult<ApiResponse<ArticleDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<ArticleDto>>().Subject;

        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data!.SKU.Should().Be("TEST-001");
    }

    [Fact]
    public async Task GetArticlesByAttribute_WithValidParameters_ShouldReturnOkWithApiResponseAndArticles()
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
        result.Should().BeOfType<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ArticleDto>>>().Subject;

        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetArticlesByAttribute_WithMissingParameters_ShouldReturnBadRequestWithApiResponse()
    {
        // Act
        var result = await _controller.GetArticlesByAttribute("", "red");

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ArticleDto>>>().Subject;

        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("Errores de validación");
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnOkWithApiResponseAndArticle()
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
        var result = await _controller.Create(createDto);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<ArticleDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<ArticleDto>>().Subject;

        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data!.SKU.Should().Be("NEW-001");
        apiResponse.Data.Id.Should().Be(1);
    }

    [Fact]
    public async Task Create_WithDuplicateSKU_ShouldReturnBadRequestWithApiResponse()
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
        var result = await _controller.Create(createDto);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<ArticleDto>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<ArticleDto>>().Subject;

        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("Ya existe un artículo con SKU: EXISTING-001");
        apiResponse.ErrorCode.Should().Be("INVALID_OPERATION");
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOkWithApiResponseAndUpdatedArticle()
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
        var result = await _controller.Update(1, updateDto);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<ArticleDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<ArticleDto>>().Subject;

        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data!.Name.Should().Be("Updated Article");
        apiResponse.Data.Brand.Should().Be("Updated Brand");
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldReturnBadRequestWithApiResponse()
    {
        // Arrange
        var updateDto = new UpdateArticleDto { Name = "Updated Article" };

        _mockService.Setup(x => x.UpdateArticleAsync(999, updateDto))
            .ReturnsAsync((ArticleDto?)null);

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<ArticleDto>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<ArticleDto>>().Subject;

        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("ArticleDto no encontrado");
        apiResponse.ErrorCode.Should().Be("INVALID_OPERATION");
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnOkWithApiResponse()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteArticleAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse>().Subject;

        apiResponse.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnBadRequestWithApiResponse()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteArticleAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse>().Subject;

        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("ArticleDto no encontrado");
        apiResponse.ErrorCode.Should().Be("INVALID_OPERATION");
    }

    [Fact]
    public async Task SearchArticles_WithValidTerm_ShouldReturnOkWithApiResponseAndArticles()
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
        result.Should().BeOfType<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ArticleDto>>>().Subject;

        apiResponse.IsSuccess.Should().BeTrue();
        apiResponse.Data.Should().HaveCount(1);
        apiResponse.Data!.First().Name.Should().Contain("Nike");
    }

    [Fact]
    public async Task SearchArticles_WithEmptyTerm_ShouldReturnBadRequestWithApiResponse()
    {
        // Act
        var result = await _controller.SearchArticles("");

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<IEnumerable<ArticleDto>>>>();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ArticleDto>>>().Subject;

        apiResponse.IsSuccess.Should().BeFalse();
        apiResponse.ErrorMessage.Should().Be("Errores de validación");
    }
}
