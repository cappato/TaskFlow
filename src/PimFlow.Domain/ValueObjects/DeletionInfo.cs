namespace PimFlow.Domain.ValueObjects;

/// <summary>
/// Value Object que encapsula información sobre la eliminación de una entidad
/// Proporciona detalles sobre por qué una entidad puede o no puede ser eliminada
/// </summary>
public sealed class DeletionInfo : IEquatable<DeletionInfo>
{
    public int ActiveSubCategories { get; }
    public int ActiveArticles { get; }
    public bool CanBeDeleted => ActiveSubCategories == 0 && ActiveArticles == 0;
    public string Summary => CanBeDeleted
        ? "La categoría puede ser eliminada"
        : $"La categoría no puede ser eliminada: {ActiveSubCategories} subcategorías activas, {ActiveArticles} artículos activos";

    public DeletionInfo(int activeSubCategories, int activeArticles)
    {
        ActiveSubCategories = activeSubCategories;
        ActiveArticles = activeArticles;
    }

    public bool Equals(DeletionInfo? other)
    {
        return other is not null &&
               ActiveSubCategories == other.ActiveSubCategories &&
               ActiveArticles == other.ActiveArticles;
    }

    public override bool Equals(object? obj)
    {
        return obj is DeletionInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ActiveSubCategories, ActiveArticles);
    }

    public override string ToString() => Summary;

    public static bool operator ==(DeletionInfo? left, DeletionInfo? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DeletionInfo? left, DeletionInfo? right)
    {
        return !Equals(left, right);
    }
}
