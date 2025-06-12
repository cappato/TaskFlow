using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.Category;

/// <summary>
/// Specifications específicas para la entidad Category
/// </summary>
public static class CategorySpecifications
{
    /// <summary>
    /// Specification que valida que una categoría tenga nombre único
    /// </summary>
    public class UniqueNameSpecification : Specification<Category>
    {
        private readonly Func<string, int?, Task<bool>> _nameExistsAsync;
        private readonly int? _excludeId;

        public UniqueNameSpecification(Func<string, int?, Task<bool>> nameExistsAsync, int? excludeId = null)
        {
            _nameExistsAsync = nameExistsAsync;
            _excludeId = excludeId;
        }

        protected override bool IsSatisfiedByCore(Category entity)
        {
            // Para validación síncrona, usamos la versión async
            return IsSatisfiedByAsync(entity).GetAwaiter().GetResult();
        }

        public async Task<bool> IsSatisfiedByAsync(Category entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
                return false;

            var exists = await _nameExistsAsync(entity.Name, _excludeId);
            return !exists;
        }

        public override string ErrorMessage => "El nombre de la categoría debe ser único";
    }

    /// <summary>
    /// Specification que valida que una categoría esté activa
    /// </summary>
    public class ActiveCategorySpecification : Specification<Category>
    {
        protected override bool IsSatisfiedByCore(Category entity)
        {
            return entity.IsActive;
        }

        public override string ErrorMessage => "La categoría debe estar activa";
    }

    /// <summary>
    /// Specification que valida que una categoría tenga datos completos
    /// </summary>
    public class CompleteCategoryDataSpecification : Specification<Category>
    {
        protected override bool IsSatisfiedByCore(Category entity)
        {
            return !string.IsNullOrWhiteSpace(entity.Name);
        }

        public override string ErrorMessage => "La categoría debe tener un nombre";
    }

    /// <summary>
    /// Specification que valida que una categoría tenga un nombre válido
    /// </summary>
    public class ValidNameFormatSpecification : Specification<Category>
    {
        protected override bool IsSatisfiedByCore(Category entity)
        {
            return ProductName.IsValid(entity.Name);
        }

        public override string ErrorMessage => "El nombre debe tener un formato válido (2-200 caracteres)";
    }

    /// <summary>
    /// Specification que valida que una categoría no tenga referencias circulares
    /// </summary>
    public class NoCircularReferenceSpecification : Specification<Category>
    {
        private readonly Func<int, Category?> _getCategoryById;

        public NoCircularReferenceSpecification(Func<int, Category?> getCategoryById)
        {
            _getCategoryById = getCategoryById;
        }

        protected override bool IsSatisfiedByCore(Category entity)
        {
            if (!entity.ParentCategoryId.HasValue)
                return true; // Categoría raíz, no hay problema

            return !entity.WouldCreateCircularReference(entity.ParentCategoryId.Value, _getCategoryById);
        }

        public override string ErrorMessage => "La categoría no puede crear una referencia circular";
    }

    /// <summary>
    /// Specification que valida que una categoría pueda ser eliminada
    /// </summary>
    public class CanBeDeletedSpecification : Specification<Category>
    {
        protected override bool IsSatisfiedByCore(Category entity)
        {
            var canDelete = entity.CanBeDeleted();
            return canDelete.IsSuccess;
        }

        public override string ErrorMessage => "La categoría no puede ser eliminada porque tiene subcategorías o artículos activos";
    }

    /// <summary>
    /// Specification compuesta que valida que una categoría sea válida para creación
    /// </summary>
    public class ValidForCreationSpecification : Specification<Category>
    {
        private readonly ISpecification<Category> _compositeSpec;

        public ValidForCreationSpecification(
            Func<string, int?, Task<bool>> nameExistsAsync,
            Func<int, Category?> getCategoryById)
        {
            _compositeSpec = new CompleteCategoryDataSpecification()
                .And(new ValidNameFormatSpecification())
                .And(new UniqueNameSpecification(nameExistsAsync))
                .And(new NoCircularReferenceSpecification(getCategoryById));
        }

        protected override bool IsSatisfiedByCore(Category entity)
        {
            return _compositeSpec.IsSatisfiedBy(entity);
        }

        public override string ErrorMessage => _compositeSpec.ErrorMessage;
    }

    /// <summary>
    /// Specification compuesta que valida que una categoría sea válida para actualización
    /// </summary>
    public class ValidForUpdateSpecification : Specification<Category>
    {
        private readonly ISpecification<Category> _compositeSpec;

        public ValidForUpdateSpecification(
            Func<string, int?, Task<bool>> nameExistsAsync,
            Func<int, Category?> getCategoryById,
            int categoryId)
        {
            _compositeSpec = new CompleteCategoryDataSpecification()
                .And(new ValidNameFormatSpecification())
                .And(new UniqueNameSpecification(nameExistsAsync, categoryId))
                .And(new NoCircularReferenceSpecification(getCategoryById));
        }

        protected override bool IsSatisfiedByCore(Category entity)
        {
            return _compositeSpec.IsSatisfiedBy(entity);
        }

        public override string ErrorMessage => _compositeSpec.ErrorMessage;
    }
}
