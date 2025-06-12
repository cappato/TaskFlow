using PimFlow.Domain.User.ValueObjects;
using PimFlow.Domain.Article.ValueObjects;
using PimFlow.Domain.Common;

namespace PimFlow.Domain.User;

public class User
{
    public int Id { get; set; }

    // Value Objects para encapsular validaciones
    private string _name = string.Empty;
    private string _email = string.Empty;

    public string Name
    {
        get => _name;
        set => _name = value; // Setter simple para Entity Framework
    }

    public string Email
    {
        get => _email;
        set => _email = value; // Setter simple para Entity Framework
    }

    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<PimFlow.Domain.Article.Article> SuppliedArticles { get; set; } = new List<PimFlow.Domain.Article.Article>();

    /// <summary>
    /// Métodos de negocio que usan Value Objects para validación
    /// </summary>
    public Result SetName(string name)
    {
        if (!ProductName.IsValid(name))
            return Result.Failure("El nombre debe tener entre 2 y 200 caracteres");

        _name = ProductName.Create(name).Value;
        return Result.Success();
    }

    public Result SetEmail(string email)
    {
        if (!ValueObjects.Email.IsValid(email))
            return Result.Failure("El email debe tener un formato válido");

        try
        {
            _email = ValueObjects.Email.Create(email);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Factory method para crear un usuario válido
    /// </summary>
    public static Result<User> Create(string name, string email)
    {
        var user = new User
        {
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var nameResult = user.SetName(name);
        if (nameResult.IsFailure)
            return Result.Failure<User>(nameResult.Error);

        var emailResult = user.SetEmail(email);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);

        return Result.Success(user);
    }
}
