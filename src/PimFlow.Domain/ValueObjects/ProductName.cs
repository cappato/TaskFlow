namespace PimFlow.Domain.ValueObjects;

/// <summary>
/// Value Object para nombres de productos
/// Encapsula las reglas de validación para nombres de artículos, categorías, etc.
/// </summary>
public sealed class ProductName : IEquatable<ProductName>
{
    public string Value { get; }

    private ProductName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea un nuevo ProductName validando el formato
    /// </summary>
    /// <param name="value">Valor del nombre</param>
    /// <returns>ProductName válido</returns>
    /// <exception cref="ArgumentException">Si el nombre no es válido</exception>
    public static ProductName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(value));

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < 2)
            throw new ArgumentException("El nombre debe tener al menos 2 caracteres", nameof(value));

        if (trimmedValue.Length > 200)
            throw new ArgumentException("El nombre no puede exceder 200 caracteres", nameof(value));

        // Validar que no contenga solo espacios o caracteres especiales
        if (string.IsNullOrWhiteSpace(trimmedValue.Replace("-", "").Replace("_", "")))
            throw new ArgumentException("El nombre debe contener caracteres alfanuméricos", nameof(value));

        return new ProductName(trimmedValue);
    }

    /// <summary>
    /// Intenta crear un ProductName sin lanzar excepción
    /// </summary>
    /// <param name="value">Valor del nombre</param>
    /// <param name="productName">ProductName creado si es válido</param>
    /// <returns>True si el nombre es válido</returns>
    public static bool TryCreate(string value, out ProductName? productName)
    {
        try
        {
            productName = Create(value);
            return true;
        }
        catch
        {
            productName = null;
            return false;
        }
    }

    /// <summary>
    /// Valida si un string es un nombre válido sin crear el objeto
    /// </summary>
    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmedValue = value.Trim();
        
        return trimmedValue.Length >= 2 && 
               trimmedValue.Length <= 200 && 
               !string.IsNullOrWhiteSpace(trimmedValue.Replace("-", "").Replace("_", ""));
    }

    public bool Equals(ProductName? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is ProductName other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(ProductName productName)
    {
        return productName.Value;
    }

    public static bool operator ==(ProductName? left, ProductName? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ProductName? left, ProductName? right)
    {
        return !Equals(left, right);
    }
}
