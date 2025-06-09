namespace PimFlow.Domain.ValueObjects;

/// <summary>
/// Value Object para marcas de productos
/// Encapsula las reglas de validación para marcas
/// </summary>
public sealed class Brand : IEquatable<Brand>
{
    public string Value { get; }

    private Brand(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea una nueva Brand validando el formato
    /// </summary>
    /// <param name="value">Valor de la marca</param>
    /// <returns>Brand válida</returns>
    /// <exception cref="ArgumentException">Si la marca no es válida</exception>
    public static Brand Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La marca no puede estar vacía", nameof(value));

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < 2)
            throw new ArgumentException("La marca debe tener al menos 2 caracteres", nameof(value));

        if (trimmedValue.Length > 100)
            throw new ArgumentException("La marca no puede exceder 100 caracteres", nameof(value));

        // Validar que no contenga solo espacios o caracteres especiales
        if (string.IsNullOrWhiteSpace(trimmedValue.Replace("-", "").Replace("_", "").Replace("&", "")))
            throw new ArgumentException("La marca debe contener caracteres alfanuméricos", nameof(value));

        return new Brand(trimmedValue);
    }

    /// <summary>
    /// Intenta crear una Brand sin lanzar excepción
    /// </summary>
    /// <param name="value">Valor de la marca</param>
    /// <param name="brand">Brand creada si es válida</param>
    /// <returns>True si la marca es válida</returns>
    public static bool TryCreate(string value, out Brand? brand)
    {
        try
        {
            brand = Create(value);
            return true;
        }
        catch
        {
            brand = null;
            return false;
        }
    }

    /// <summary>
    /// Valida si un string es una marca válida sin crear el objeto
    /// </summary>
    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmedValue = value.Trim();
        
        return trimmedValue.Length >= 2 && 
               trimmedValue.Length <= 100 && 
               !string.IsNullOrWhiteSpace(trimmedValue.Replace("-", "").Replace("_", "").Replace("&", ""));
    }

    public bool Equals(Brand? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Brand other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Brand brand)
    {
        return brand.Value;
    }

    public static bool operator ==(Brand? left, Brand? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Brand? left, Brand? right)
    {
        return !Equals(left, right);
    }
}
