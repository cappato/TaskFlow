using PimFlow.Domain.User.ValueObjects;
using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.User;

/// <summary>
/// Validador específico para el agregado User
/// Centraliza todas las validaciones relacionadas con usuarios
/// </summary>
public static class UserValidator
{
    /// <summary>
    /// Validaciones para nombre de usuario
    /// </summary>
    public static class Name
    {
        public static bool IsValid(string value) => ProductName.IsValid(value);

        public static string GetValidationMessage() =>
            "El nombre debe tener entre 2 y 200 caracteres alfanuméricos";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para email de usuario
    /// </summary>
    public static class Email
    {
        public static bool IsValid(string value) => ValueObjects.Email.IsValid(value);

        public static string GetValidationMessage() =>
            "El email debe tener un formato válido y no exceder 200 caracteres";

        public static Result Validate(string value)
        {
            if (IsValid(value))
                return Result.Success();

            return Result.Failure(GetValidationMessage());
        }
    }

    /// <summary>
    /// Validaciones para estado del usuario
    /// </summary>
    public static class Status
    {
        public static bool IsValidForActivation(bool currentStatus)
        {
            // Un usuario inactivo puede ser activado
            return !currentStatus;
        }

        public static bool IsValidForDeactivation(bool currentStatus)
        {
            // Un usuario activo puede ser desactivado
            return currentStatus;
        }

        public static Result ValidateActivation(bool currentStatus)
        {
            if (IsValidForActivation(currentStatus))
                return Result.Success();

            return Result.Failure("El usuario ya está activo");
        }

        public static Result ValidateDeactivation(bool currentStatus)
        {
            if (IsValidForDeactivation(currentStatus))
                return Result.Success();

            return Result.Failure("El usuario ya está inactivo");
        }
    }

    /// <summary>
    /// Validación completa de un usuario para creación
    /// </summary>
    public static Result ValidateForCreation(string name, string email)
    {
        var nameResult = Name.Validate(name);
        if (nameResult.IsFailure)
            return nameResult;

        var emailResult = Email.Validate(email);
        if (emailResult.IsFailure)
            return emailResult;

        return Result.Success();
    }

    /// <summary>
    /// Validación completa de un usuario para actualización
    /// </summary>
    public static Result ValidateForUpdate(string name, string email)
    {
        var nameResult = Name.Validate(name);
        if (nameResult.IsFailure)
            return nameResult;

        var emailResult = Email.Validate(email);
        if (emailResult.IsFailure)
            return emailResult;

        return Result.Success();
    }

    /// <summary>
    /// Validación de datos completos del usuario
    /// </summary>
    public static Result ValidateCompleteness(string name, string email, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure("El email es requerido");

        var validationResult = ValidateForUpdate(name, email);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Success();
    }
}
