using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.Article;

/// <summary>
/// Specifications específicas para la entidad Article
/// </summary>
public static class ArticleSpecifications
{
    /// <summary>
    /// Specification que valida que un artículo tenga SKU único
    /// </summary>
    public class UniqueSKUSpecification : Specification<Article>
    {
        private readonly Func<string, Task<bool>> _skuExistsAsync;
        private readonly int? _excludeId;

        public UniqueSKUSpecification(Func<string, Task<bool>> skuExistsAsync, int? excludeId = null)
        {
            _skuExistsAsync = skuExistsAsync;
            _excludeId = excludeId;
        }

        protected override bool IsSatisfiedByCore(Article entity)
        {
            // Para validación síncrona, usamos la versión async
            return IsSatisfiedByAsync(entity).GetAwaiter().GetResult();
        }

        public async Task<bool> IsSatisfiedByAsync(Article entity)
        {
            if (string.IsNullOrEmpty(entity.SKU))
                return false;

            var exists = await _skuExistsAsync(entity.SKU);
            
            // Si estamos actualizando, excluir el ID actual
            if (_excludeId.HasValue && entity.Id == _excludeId.Value)
                return true;

            return !exists;
        }

        public override string ErrorMessage => "El SKU debe ser único en el sistema";
    }

    /// <summary>
    /// Specification que valida que un artículo esté activo
    /// </summary>
    public class ActiveArticleSpecification : Specification<Article>
    {
        protected override bool IsSatisfiedByCore(Article entity)
        {
            return entity.IsActive;
        }

        public override string ErrorMessage => "El artículo debe estar activo";
    }

    /// <summary>
    /// Specification que valida que un artículo tenga datos básicos completos
    /// </summary>
    public class CompleteArticleDataSpecification : Specification<Article>
    {
        protected override bool IsSatisfiedByCore(Article entity)
        {
            return !string.IsNullOrWhiteSpace(entity.SKU) &&
                   !string.IsNullOrWhiteSpace(entity.Name) &&
                   !string.IsNullOrWhiteSpace(entity.Brand);
        }

        public override string ErrorMessage => "El artículo debe tener SKU, nombre y marca";
    }

    /// <summary>
    /// Specification que valida que un artículo tenga un SKU válido
    /// </summary>
    public class ValidSKUFormatSpecification : Specification<Article>
    {
        protected override bool IsSatisfiedByCore(Article entity)
        {
            return SKU.IsValid(entity.SKU);
        }

        public override string ErrorMessage => "El SKU debe tener un formato válido (3-50 caracteres alfanuméricos)";
    }

    /// <summary>
    /// Specification que valida que un artículo tenga un nombre válido
    /// </summary>
    public class ValidNameFormatSpecification : Specification<Article>
    {
        protected override bool IsSatisfiedByCore(Article entity)
        {
            return ProductName.IsValid(entity.Name);
        }

        public override string ErrorMessage => "El nombre debe tener un formato válido (2-200 caracteres)";
    }

    /// <summary>
    /// Specification que valida que un artículo tenga una marca válida
    /// </summary>
    public class ValidBrandFormatSpecification : Specification<Article>
    {
        protected override bool IsSatisfiedByCore(Article entity)
        {
            return Brand.IsValid(entity.Brand);
        }

        public override string ErrorMessage => "La marca debe tener un formato válido (2-100 caracteres)";
    }

    /// <summary>
    /// Specification compuesta que valida que un artículo sea válido para creación
    /// </summary>
    public class ValidForCreationSpecification : Specification<Article>
    {
        private readonly ISpecification<Article> _compositeSpec;

        public ValidForCreationSpecification(Func<string, Task<bool>> skuExistsAsync)
        {
            _compositeSpec = new CompleteArticleDataSpecification()
                .And(new ValidSKUFormatSpecification())
                .And(new ValidNameFormatSpecification())
                .And(new ValidBrandFormatSpecification())
                .And(new UniqueSKUSpecification(skuExistsAsync));
        }

        protected override bool IsSatisfiedByCore(Article entity)
        {
            return _compositeSpec.IsSatisfiedBy(entity);
        }

        public override string ErrorMessage => _compositeSpec.ErrorMessage;
    }

    /// <summary>
    /// Specification compuesta que valida que un artículo sea válido para actualización
    /// </summary>
    public class ValidForUpdateSpecification : Specification<Article>
    {
        private readonly ISpecification<Article> _compositeSpec;

        public ValidForUpdateSpecification(Func<string, Task<bool>> skuExistsAsync, int articleId)
        {
            _compositeSpec = new CompleteArticleDataSpecification()
                .And(new ValidSKUFormatSpecification())
                .And(new ValidNameFormatSpecification())
                .And(new ValidBrandFormatSpecification())
                .And(new UniqueSKUSpecification(skuExistsAsync, articleId));
        }

        protected override bool IsSatisfiedByCore(Article entity)
        {
            return _compositeSpec.IsSatisfiedBy(entity);
        }

        public override string ErrorMessage => _compositeSpec.ErrorMessage;
    }
}
