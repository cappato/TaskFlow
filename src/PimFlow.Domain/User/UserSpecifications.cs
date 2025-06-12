using PimFlow.Domain.User.ValueObjects;
using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.User;

/// <summary>
/// Specifications específicas para la entidad User
/// </summary>
public static class UserSpecifications
{
    /// <summary>
    /// Specification que valida que un usuario tenga email único
    /// </summary>
    public class UniqueEmailSpecification : Specification<User>
    {
        private readonly Func<string, Task<bool>> _emailExistsAsync;
        private readonly int? _excludeId;

        public UniqueEmailSpecification(Func<string, Task<bool>> emailExistsAsync, int? excludeId = null)
        {
            _emailExistsAsync = emailExistsAsync;
            _excludeId = excludeId;
        }

        protected override bool IsSatisfiedByCore(User entity)
        {
            // Para validación síncrona, usamos la versión async
            return IsSatisfiedByAsync(entity).GetAwaiter().GetResult();
        }

        public async Task<bool> IsSatisfiedByAsync(User entity)
        {
            if (string.IsNullOrEmpty(entity.Email))
                return false;

            var exists = await _emailExistsAsync(entity.Email);

            // Si estamos actualizando, excluir el ID actual
            if (_excludeId.HasValue && entity.Id == _excludeId.Value)
                return true;

            return !exists;
        }

        public override string ErrorMessage => "El email debe ser único en el sistema";
    }

    /// <summary>
    /// Specification que valida que un usuario esté activo
    /// </summary>
    public class ActiveUserSpecification : Specification<User>
    {
        protected override bool IsSatisfiedByCore(User entity)
        {
            return entity.IsActive;
        }

        public override string ErrorMessage => "El usuario debe estar activo";
    }

    /// <summary>
    /// Specification que valida que un usuario tenga datos básicos completos
    /// </summary>
    public class CompleteUserDataSpecification : Specification<User>
    {
        protected override bool IsSatisfiedByCore(User entity)
        {
            return !string.IsNullOrWhiteSpace(entity.Name) &&
                   !string.IsNullOrWhiteSpace(entity.Email);
        }

        public override string ErrorMessage => "El usuario debe tener nombre y email";
    }

    /// <summary>
    /// Specification que valida que un usuario tenga un email válido
    /// </summary>
    public class ValidEmailFormatSpecification : Specification<User>
    {
        protected override bool IsSatisfiedByCore(User entity)
        {
            return Email.IsValid(entity.Email);
        }

        public override string ErrorMessage => "El email debe tener un formato válido";
    }

    /// <summary>
    /// Specification que valida que un usuario tenga un nombre válido
    /// </summary>
    public class ValidNameFormatSpecification : Specification<User>
    {
        protected override bool IsSatisfiedByCore(User entity)
        {
            return ProductName.IsValid(entity.Name);
        }

        public override string ErrorMessage => "El nombre debe tener un formato válido (2-200 caracteres)";
    }

    /// <summary>
    /// Specification compuesta que valida que un usuario sea válido para creación
    /// </summary>
    public class ValidForCreationSpecification : Specification<User>
    {
        private readonly ISpecification<User> _compositeSpec;

        public ValidForCreationSpecification(Func<string, Task<bool>> emailExistsAsync)
        {
            _compositeSpec = new CompleteUserDataSpecification()
                .And(new ValidNameFormatSpecification())
                .And(new ValidEmailFormatSpecification())
                .And(new UniqueEmailSpecification(emailExistsAsync));
        }

        protected override bool IsSatisfiedByCore(User entity)
        {
            return _compositeSpec.IsSatisfiedBy(entity);
        }

        public override string ErrorMessage => _compositeSpec.ErrorMessage;
    }

    /// <summary>
    /// Specification compuesta que valida que un usuario sea válido para actualización
    /// </summary>
    public class ValidForUpdateSpecification : Specification<User>
    {
        private readonly ISpecification<User> _compositeSpec;

        public ValidForUpdateSpecification(Func<string, Task<bool>> emailExistsAsync, int userId)
        {
            _compositeSpec = new CompleteUserDataSpecification()
                .And(new ValidNameFormatSpecification())
                .And(new ValidEmailFormatSpecification())
                .And(new UniqueEmailSpecification(emailExistsAsync, userId));
        }

        protected override bool IsSatisfiedByCore(User entity)
        {
            return _compositeSpec.IsSatisfiedBy(entity);
        }

        public override string ErrorMessage => _compositeSpec.ErrorMessage;
    }
}
