using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Services;

/// <summary>
/// Service interface for Article commands (CQRS - Command side)
/// Follows Single Responsibility Principle by handling only write operations
/// </summary>
public interface IArticleCommandService
{
    /// <summary>
    /// Create a new article
    /// </summary>
    Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto);

    /// <summary>
    /// Update an existing article
    /// </summary>
    Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto updateArticleDto);

    /// <summary>
    /// Delete an article
    /// </summary>
    Task<bool> DeleteArticleAsync(int id);
}
