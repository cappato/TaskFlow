using Microsoft.AspNetCore.Mvc;
using TaskFlow.Server.Services;
using TaskFlow.Shared.DTOs;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles()
    {
        var articles = await _articleService.GetAllArticlesAsync();
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetArticle(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article == null)
            return NotFound();

        return Ok(article);
    }

    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<ArticleDto>> GetArticleBySKU(string sku)
    {
        var article = await _articleService.GetArticleBySKUAsync(sku);
        if (article == null)
            return NotFound();

        return Ok(article);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticlesByCategory(int categoryId)
    {
        var articles = await _articleService.GetArticlesByCategoryIdAsync(categoryId);
        return Ok(articles);
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticlesByType(ArticleType type)
    {
        var articles = await _articleService.GetArticlesByTypeAsync(type);
        return Ok(articles);
    }

    [HttpGet("brand/{brand}")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticlesByBrand(string brand)
    {
        var articles = await _articleService.GetArticlesByBrandAsync(brand);
        return Ok(articles);
    }

    [HttpGet("attribute")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticlesByAttribute(
        [FromQuery] string attributeName,
        [FromQuery] string value)
    {
        if (string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(value))
            return BadRequest("AttributeName y value son requeridos");

        var articles = await _articleService.GetArticlesByAttributeAsync(attributeName, value);
        return Ok(articles);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> SearchArticles([FromQuery] string term)
    {
        if (string.IsNullOrEmpty(term))
            return BadRequest("Término de búsqueda es requerido");

        var articles = await _articleService.SearchArticlesAsync(term);
        return Ok(articles);
    }

    [HttpPost]
    public async Task<ActionResult<ArticleDto>> CreateArticle(CreateArticleDto createArticleDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var article = await _articleService.CreateArticleAsync(createArticleDto);
            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ArticleDto>> UpdateArticle(int id, UpdateArticleDto updateArticleDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var article = await _articleService.UpdateArticleAsync(id, updateArticleDto);
            if (article == null)
                return NotFound();

            return Ok(article);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var result = await _articleService.DeleteArticleAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
