using System.Text.RegularExpressions;

namespace PimFlow.Domain.User.ValueObjects;

/// <summary>
/// Value Object para direcciones de email
/// Encapsula las reglas de validación para emails
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailPattern = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea un nuevo Email validando el formato
    /// </summary>
    /// <param name="value">Valor del email</param>
    /// <returns>Email válido</returns>
    /// <exception cref="ArgumentException">Si el email no es válido</exception>
    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email no puede estar vacío", nameof(value));

        var normalizedValue = value.Trim().ToLowerInvariant();

        if (normalizedValue.Length > 200)
            throw new ArgumentException("Email no puede exceder 200 caracteres", nameof(value));

        if (!EmailPattern.IsMatch(normalizedValue))
            throw new ArgumentException("Formato de email inválido", nameof(value));

        return new Email(normalizedValue);
    }

    /// <summary>
    /// Intenta crear un Email sin lanzar excepción
    /// </summary>
    /// <param name="value">Valor del email</param>
    /// <param name="email">Email creado si es válido</param>
    /// <returns>True si el email es válido</returns>
    public static bool TryCreate(string value, out Email? email)
    {
        try
        {
            email = Create(value);
            return true;
        }
        catch
        {
            email = null;
            return false;
        }
    }

    /// <summary>
    /// Valida si un string es un email válido sin crear el objeto
    /// </summary>
    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalizedValue = value.Trim().ToLowerInvariant();
        return normalizedValue.Length <= 200 && EmailPattern.IsMatch(normalizedValue);
    }

    public bool Equals(Email? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Email other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    public static bool operator ==(Email? left, Email? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Email? left, Email? right)
    {
        return !Equals(left, right);
    }
}
