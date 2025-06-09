using PimFlow.Shared.DTOs;

namespace PimFlow.Server.Validation.Article;

/// <summary>
/// Specific validation strategy interface for Article creation
/// Extends base validation strategy with Article-specific context
/// </summary>
public interface IArticleCreateValidationStrategy : IValidationStrategy<CreateArticleDto>
{
    /// <summary>
    /// Validation category for grouping related validations
    /// </summary>
    ValidationCategory Category { get; }
}

/// <summary>
/// Specific validation strategy interface for Article updates
/// </summary>
public interface IArticleUpdateValidationStrategy : IValidationStrategy<(int Id, UpdateArticleDto Dto)>
{
    /// <summary>
    /// Validation category for grouping related validations
    /// </summary>
    ValidationCategory Category { get; }
}

/// <summary>
/// Categories of validation for better organization and control
/// </summary>
public enum ValidationCategory
{
    /// <summary>
    /// Basic field validation (required fields, formats, etc.)
    /// </summary>
    Basic = 1,
    
    /// <summary>
    /// Business rules validation (SKU uniqueness, category exists, etc.)
    /// </summary>
    BusinessRules = 2,
    
    /// <summary>
    /// Data integrity validation (foreign keys, relationships, etc.)
    /// </summary>
    DataIntegrity = 3,
    
    /// <summary>
    /// Security validation (permissions, access control, etc.)
    /// </summary>
    Security = 4,
    
    /// <summary>
    /// Performance validation (data size limits, etc.)
    /// </summary>
    Performance = 5
}
