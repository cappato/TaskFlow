# 🏗️ Mejoras Arquitectónicas Propuestas - PimFlow

## 🎯 Objetivo

Identificar y resolver acoplamientos arquitectónicos que causaron la modificación de 90+ archivos durante el rename del proyecto.

## 🔍 Problemas Identificados

### **1. 🔄 Mapeo Duplicado y Hardcodeado**

**Problema Actual:**
- Cada servicio tiene su propio método `MapToDto` privado
- Lógica de mapeo duplicada en múltiples lugares
- Cambios en entidades requieren modificar múltiples servicios

**Impacto en Rename:**
- 15+ archivos de servicios modificados
- Mappers duplicados actualizados manualmente

**Solución Propuesta:**
```csharp
// 1. AutoMapper centralizado
public class ArticleMappingProfile : Profile
{
    public ArticleMappingProfile()
    {
        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => EnumMapper.ToShared(src.Type)))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
}

// 2. Servicio de mapeo genérico
public interface IMappingService
{
    TDestination Map<TSource, TDestination>(TSource source);
    IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source);
}
```

### **2. 🔗 Violación de Single Responsibility Principle**

**Problema Actual:**
- ArticleService hace: validación, mapeo, coordinación, manejo de atributos
- Servicios con múltiples responsabilidades

**Solución Propuesta: CQRS + Mediator Pattern**
```csharp
// Commands y Queries separados
public class CreateArticleCommand : IRequest<ArticleDto>
{
    public CreateArticleDto Article { get; set; }
}

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, ArticleDto>
{
    // Solo se encarga de crear artículos
}

public class GetArticleByIdQuery : IRequest<ArticleDto>
{
    public int Id { get; set; }
}
```

### **3. 🏗️ Falta de Unit of Work Pattern**

**Problema Actual:**
- Transacciones no coordinadas
- Operaciones pueden fallar parcialmente

**Solución Propuesta:**
```csharp
public interface IUnitOfWork : IDisposable
{
    IArticleRepository Articles { get; }
    ICustomAttributeRepository CustomAttributes { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### **4. 🔄 Dependencia Circular entre Capas**

**Problema Actual:**
- Shared depende de Domain (viola Clean Architecture)
- Client depende de Domain directamente

**Solución Propuesta:**
```
Antes:
Domain ← Shared ← Server
   ↑        ↑
   └─ Client ─┘

Después:
Domain → Application → Infrastructure
   ↓         ↓            ↓
   └─ Shared ← Server ← Client
```

### **5. 📝 Validaciones Duplicadas**

**Problema Actual:**
- Validaciones en Domain (Value Objects)
- Validaciones en Shared (FluentValidation)
- Lógica duplicada y desincronizada

**Solución Propuesta:**
```csharp
// Validaciones centralizadas
public interface IValidationService<T>
{
    ValidationResult Validate(T entity);
    Task<ValidationResult> ValidateAsync(T entity);
}

// Reutilizar validaciones de Domain en Shared
public class ArticleValidator : AbstractValidator<CreateArticleDto>
{
    public ArticleValidator()
    {
        RuleFor(x => x.SKU).Must(SKU.IsValid).WithMessage("SKU inválido");
        RuleFor(x => x.Name).Must(ProductName.IsValid).WithMessage("Nombre inválido");
    }
}
```

## 🎯 Plan de Implementación

### **Fase 1: Mapeo Centralizado**
1. Implementar AutoMapper
2. Crear perfiles de mapeo
3. Refactorizar servicios para usar mapper centralizado

### **Fase 2: CQRS + Mediator**
1. Instalar MediatR
2. Crear Commands y Queries
3. Implementar Handlers
4. Refactorizar controladores

### **Fase 3: Unit of Work**
1. Implementar IUnitOfWork
2. Refactorizar repositorios
3. Coordinar transacciones

### **Fase 4: Reestructuración de Capas**
1. Crear Application Layer
2. Mover lógica de servicios a Application
3. Eliminar dependencia circular

### **Fase 5: Validaciones Centralizadas**
1. Crear servicio de validación
2. Reutilizar Value Objects en validadores
3. Eliminar duplicación

## 🧪 Test de Acoplamiento Propuesto

### **Test de Rename Automatizado**
```csharp
[Fact]
public void Architecture_ShouldSupportProjectRename_WithMinimalChanges()
{
    // Simular rename de "PimFlow" a "NewName"
    var affectedFiles = AnalyzeRenameImpact("PimFlow", "NewName");
    
    // Debería afectar máximo 30 archivos (configuración y namespaces)
    affectedFiles.Should().HaveCountLessThan(30);
    
    // No debería afectar lógica de negocio
    affectedFiles.Should().NotContain(f => f.Contains("Services"));
    affectedFiles.Should().NotContain(f => f.Contains("Repositories"));
}
```

### **Test de Dependencias**
```csharp
[Fact]
public void Architecture_ShouldNotHaveCircularDependencies()
{
    var result = Types.InCurrentDomain()
        .Should()
        .NotHaveDependencyOnAny("PimFlow.Domain")
        .GetResult();
        
    result.IsSuccessful.Should().BeTrue();
}
```

## 📊 Beneficios Esperados

### **Reducción de Acoplamiento:**
- Rename: 90+ archivos → ~25 archivos
- Cambios en entidades: Automáticos via AutoMapper
- Nuevas funcionalidades: Aisladas en Commands/Queries

### **Mejor Mantenibilidad:**
- Responsabilidades claras
- Transacciones coordinadas
- Validaciones centralizadas
- Testing más fácil

### **Escalabilidad:**
- Fácil agregar nuevas funcionalidades
- Patrones consistentes
- Mejor separación de concerns

## 🎯 Próximos Pasos

1. **Crear feature branch** para mejoras arquitectónicas
2. **Implementar AutoMapper** como primera mejora
3. **Crear tests de acoplamiento** para medir progreso
4. **Refactorizar gradualmente** siguiendo el plan

¿Quieres que procedamos con la implementación de alguna de estas mejoras?
