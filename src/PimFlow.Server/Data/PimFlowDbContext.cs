using Microsoft.EntityFrameworkCore;
using PimFlow.Domain.Entities;
using PimFlow.Domain.Enums;

namespace PimFlow.Server.Data;

public class PimFlowDbContext : DbContext
{
    public PimFlowDbContext(DbContextOptions<PimFlowDbContext> options) : base(options)
    {
    }

    // PIM Entities
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CustomAttribute> CustomAttributes { get; set; }
    public DbSet<ArticleAttributeValue> ArticleAttributeValues { get; set; }
    public DbSet<ArticleVariant> ArticleVariants { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Article configuration
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Type)
                .HasConversion(
                    v => v.ToString(),                    // Enum → String
                    v => (ArticleType)Enum.Parse(typeof(ArticleType), v)  // String → Enum
                );
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.SKU).IsUnique();

            // Relationships
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Articles)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Supplier)
                  .WithMany(u => u.SuppliedArticles)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();

            // Self-referencing relationship for hierarchical categories
            entity.HasOne(e => e.ParentCategory)
                  .WithMany(c => c.SubCategories)
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CustomAttribute configuration
        modelBuilder.Entity<CustomAttribute>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Type)
                .HasConversion(
                    v => v.ToString(),                    // Enum → String
                    v => (AttributeType)Enum.Parse(typeof(AttributeType), v)  // String → Enum
                );
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // ArticleAttributeValue configuration (EAV pattern)
        modelBuilder.Entity<ArticleAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();

            // Composite index for performance
            entity.HasIndex(e => new { e.ArticleId, e.CustomAttributeId }).IsUnique();
            entity.HasIndex(e => new { e.CustomAttributeId, e.Value });

            // Relationships
            entity.HasOne(e => e.Article)
                  .WithMany(a => a.AttributeValues)
                  .HasForeignKey(e => e.ArticleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CustomAttribute)
                  .WithMany(ca => ca.AttributeValues)
                  .HasForeignKey(e => e.CustomAttributeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ArticleVariant configuration
        modelBuilder.Entity<ArticleVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Size).HasMaxLength(20);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Stock).HasPrecision(18, 2);
            entity.Property(e => e.WholesalePrice).HasPrecision(18, 2);
            entity.Property(e => e.RetailPrice).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.SKU).IsUnique();

            // Relationships
            entity.HasOne(e => e.Article)
                  .WithMany(a => a.Variants)
                  .HasForeignKey(e => e.ArticleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // User configuration (adapted for suppliers)
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var now = DateTime.UtcNow;

        // Seed Users (Suppliers)
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Admin Cruzado", Email = "admin@cruzado.com", CreatedAt = now, IsActive = true },
            new User { Id = 2, Name = "Nike Supplier", Email = "supplier@nike.com", CreatedAt = now, IsActive = true },
            new User { Id = 3, Name = "Adidas Supplier", Email = "supplier@adidas.com", CreatedAt = now, IsActive = true }
        );

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Calzado", Description = "Calzado deportivo", CreatedAt = now, IsActive = true },
            new Category { Id = 2, Name = "Ropa", Description = "Ropa deportiva", CreatedAt = now, IsActive = true },
            new Category { Id = 3, Name = "Zapatillas Running", Description = "Zapatillas para correr", CreatedAt = now, IsActive = true, ParentCategoryId = 1 },
            new Category { Id = 4, Name = "Zapatillas Fútbol", Description = "Zapatillas de fútbol", CreatedAt = now, IsActive = true, ParentCategoryId = 1 },
            new Category { Id = 5, Name = "Remeras", Description = "Remeras deportivas", CreatedAt = now, IsActive = true, ParentCategoryId = 2 }
        );

        // Seed Custom Attributes
        modelBuilder.Entity<CustomAttribute>().HasData(
            new CustomAttribute { Id = 1, Name = "talle", DisplayName = "Talle", Type = AttributeType.Select, IsRequired = true, CreatedAt = now, SortOrder = 1 },
            new CustomAttribute { Id = 2, Name = "color", DisplayName = "Color", Type = AttributeType.Color, IsRequired = true, CreatedAt = now, SortOrder = 2 },
            new CustomAttribute { Id = 3, Name = "material", DisplayName = "Material", Type = AttributeType.Text, IsRequired = false, CreatedAt = now, SortOrder = 3 },
            new CustomAttribute { Id = 4, Name = "temporada", DisplayName = "Temporada", Type = AttributeType.Select, IsRequired = false, CreatedAt = now, SortOrder = 4 },
            new CustomAttribute { Id = 5, Name = "genero", DisplayName = "Género", Type = AttributeType.Select, IsRequired = true, CreatedAt = now, SortOrder = 5 },
            new CustomAttribute { Id = 6, Name = "resistencia_agua", DisplayName = "Resistente al Agua", Type = AttributeType.Boolean, IsRequired = false, CreatedAt = now, SortOrder = 6 },
            new CustomAttribute { Id = 7, Name = "tipo_suela", DisplayName = "Tipo de Suela", Type = AttributeType.Text, IsRequired = false, CreatedAt = now, SortOrder = 7 }
        );
    }
}
