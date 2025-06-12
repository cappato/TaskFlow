using System.Text.RegularExpressions;

namespace PimFlow.Domain.Article.ValueObjects;

/// <summary>
/// Value Object para SKU (Stock Keeping Unit)
/// Encapsula las reglas de validación y formato para códigos SKU
/// </summary>
public sealed class SKU : IEquatable<SKU>
{
    private static readonly Regex SkuPattern = new(@"^[A-Z0-9]{3,50}$", RegexOptions.Compiled);
    
    public string Value { get; }

    private SKU(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea un nuevo SKU validando el formato
    /// </summary>
    /// <param name="value">Valor del SKU</param>
    /// <returns>SKU válido</returns>
    /// <exception cref="ArgumentException">Si el SKU no es válido</exception>
    public static SKU Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU no puede estar vacío", nameof(value));

        var normalizedValue = value.Trim().ToUpperInvariant();

        if (!SkuPattern.IsMatch(normalizedValue))
            throw new ArgumentException(
                "SKU debe contener solo letras mayúsculas y números, entre 3 y 50 caracteres", 
                nameof(value));

        return new SKU(normalizedValue);
    }

    /// <summary>
    /// Intenta crear un SKU sin lanzar excepción
    /// </summary>
    /// <param name="value">Valor del SKU</param>
    /// <param name="sku">SKU creado si es válido</param>
    /// <returns>True si el SKU es válido</returns>
    public static bool TryCreate(string value, out SKU? sku)
    {
        try
        {
            sku = Create(value);
            return true;
        }
        catch
        {
            sku = null;
            return false;
        }
    }

    /// <summary>
    /// Valida si un string es un SKU válido sin crear el objeto
    /// </summary>
    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalizedValue = value.Trim().ToUpperInvariant();
        return SkuPattern.IsMatch(normalizedValue);
    }

    public bool Equals(SKU? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is SKU other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(SKU sku)
    {
        return sku.Value;
    }

    public static bool operator ==(SKU? left, SKU? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SKU? left, SKU? right)
    {
        return !Equals(left, right);
    }
}
