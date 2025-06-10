Perfecto, Tato. Acá te bajo todo el plan anterior a un **Roadmap técnico detallado por semanas**, con entregables concretos, foco por etapa y tareas priorizadas para que el equipo arranque con fuerza y claridad total.

---

# 🛣️ Roadmap Técnico – Evolución TaskFlow → PIM Dinámico

**Duración estimada**: 3 semanas intensivas (puede extenderse según validaciones y QA)

---

## 🔥 Semana 1 – Fundaciones del Modelo Dinámico

> Objetivo: transformar la base actual en un modelo flexible y desacoplado con soporte para atributos personalizados.

### 🧱 Backend - Infraestructura

- [ ] Renombrar `TaskItem` a `Article` (entidad principal)
- [ ] Crear modelos:

  - [ ] `Article`
  - [ ] `CustomAttribute` (define el esquema)
  - [ ] `ArticleAttributeValue` (instancia EAV)

- [ ] Agregar enum `AttributeType` (`Text`, `Number`, `Date`, `Boolean`, etc.)
- [ ] Configurar `DbContext` con relaciones y restricciones
- [ ] Generar migraciones iniciales con EF Core
- [ ] Crear seed de ejemplo para atributos y artículos

### 🧠 Arquitectura

- [ ] Definir `IArticleRepository` (y crear implementación SQLite con EF Core)
- [ ] Registrar repositorios en DI (`services.AddScoped<IArticleRepository, SqliteArticleRepository>()`)
- [ ] Crear servicio `ArticleService` para lógica de negocio (validación de atributos, etc.)

---

## 🎯 Semana 2 – Lógica de Negocio + Atributos Dinámicos

> Objetivo: exponer toda la lógica de atributos personalizados desde el backend (CRUD, validación, queries por atributos).

### 🧠 Backend - Dominio y Servicios

- [ ] CRUD completo:

  - [ ] `CustomAttribute` (admin)
  - [ ] `Article` (con atributos dinámicos)

- [ ] Validaciones dinámicas al guardar un artículo:

  - [ ] Obligatorios
  - [ ] Tipos correctos

- [ ] Query `GetArticlesByAttribute(attributeName, value)`
- [ ] Agregar tests unitarios para servicios y repositorios

### 🔄 API Endpoints

- [ ] `GET /attributes`
- [ ] `POST /attributes`
- [ ] `GET /articles`
- [ ] `POST /articles` con atributos
- [ ] `GET /articles?attr=color&value=red`

---

## 🧩 Semana 3 – UI Dinámica en Blazor

> Objetivo: lograr una interfaz administrable, con formularios que se adapten a los atributos definidos dinámicamente.

### 🖥️ Frontend

- [ ] Crear componente `DynamicForm` que:

  - [ ] Recibe definición de atributos
  - [ ] Renderiza inputs adecuados (`<InputText>`, `<InputDate>`, etc.)
  - [ ] Valida en base al tipo y requeridos

- [ ] Componente `AttributeAdminPage`

  - [ ] Agrega, edita, lista atributos personalizados

- [ ] Componente `ArticleFormPage`

  - [ ] Crea nuevo artículo con atributos dinámicos
  - [ ] Muestra valores existentes para editar

- [ ] Usar `HttpClient` para llamadas al backend

---

## 🧪 Semana 4 (Opcional / QA / Refinamiento)

> Objetivo: pruebas integrales, revisión de performance y preparar migrabilidad a Mongo (fase futura).

### 🔍 QA & Pruebas

- [ ] Pruebas end-to-end con creación y consulta de artículos
- [ ] Test de performance sobre tabla EAV
- [ ] Revisión de índice sobre `value` y `customAttributeId`
- [ ] Feedback del equipo y ajuste de UX

### 🔄 MongoDB Prep (no urgente)

- [ ] Esbozar `MongoArticleRepository` con estructura flexible
- [ ] Validar compatibilidad con interfaz actual
- [ ] Documentar diferencias clave vs SQLite

---

## 🧰 Herramientas sugeridas

| Área         | Herramienta                   | Motivo                            |
| ------------ | ----------------------------- | --------------------------------- |
| DB local     | SQLite + EF Core              | Rápido para prototipos            |
| Tests        | xUnit + Moq                   | Para lógica de negocio            |
| UI           | Blazor (WebAssembly o Server) | Ya lo están usando                |
| Future-proof | Interface + DI                | Permite migrar a Mongo sin romper |

---

## 📁 Estructura de carpetas sugerida

```
/Domain
  Article.cs
  CustomAttribute.cs
  ArticleAttributeValue.cs
  AttributeType.cs

/Infrastructure
  Repositories/
    IArticleRepository.cs
    SqliteArticleRepository.cs

/Application
  Services/
    ArticleService.cs
    AttributeService.cs

/Api
  Controllers/
    ArticlesController.cs
    AttributesController.cs
  DTOs/
    ArticleDto.cs
    AttributeDto.cs
```

---

Sí, Tato. Lo del **patrón de repositorio para permitir migrar de SQLite a MongoDB** **fue tenido en cuenta desde el principio**, y **sí, es viable**, pero con algunas **consideraciones técnicas importantes** que te explico claramente para que puedas tomar decisiones con fundamentos.

---

## ✅ ¿Es viable migrar de SQLite a Mongo usando patrón de repositorio?

### ✔️ **Sí, siempre que:**

1. Tengas **interfaces bien definidas** (`IArticleRepository`, `IAttributeRepository`, etc.)
2. No tengas lógica específica de EF Core (o Linq-to-SQL) mezclada en servicios o controladores
3. Estructures bien tus entidades para soportar un modelo _document-oriented_ en Mongo

---

## 🧱 Ejemplo de Abstracción

```csharp
public interface IArticleRepository
{
    Task<IEnumerable<Article>> GetAllAsync();
    Task<Article> GetByIdAsync(Guid id);
    Task CreateAsync(Article article);
    Task<IEnumerable<Article>> FindByAttributeAsync(string attributeName, string value);
}
```

Tu implementación actual sería algo como:

```csharp
public class SqliteArticleRepository : IArticleRepository
{
    private readonly AppDbContext _db;

    public SqliteArticleRepository(AppDbContext db) { _db = db; }

    public Task<IEnumerable<Article>> GetAllAsync() => _db.Articles.Include(a => a.Attributes)...;
    // etc.
}
```

Cuando quieras pasar a Mongo:

```csharp
public class MongoArticleRepository : IArticleRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoArticleRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<BsonDocument>("articles");
    }

    public Task<IEnumerable<Article>> GetAllAsync() => _collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();
    // etc.
}
```

---

## ⚠️ Consideraciones Importantes

| Tema         | SQLite (EF Core)               | MongoDB                                                     |
| ------------ | ------------------------------ | ----------------------------------------------------------- |
| Modelo       | Relacional (EAV)               | Documental (embeddear atributos)                            |
| Queries      | Join y filtrado fácil con LINQ | Requiere mapear propiedades dinámicas o usar `BsonDocument` |
| Validaciones | Fuertes con tipos              | Hay que manejarlas manualmente en los servicios             |
| Migraciones  | EF Core Migrations             | Manuales o con MongoMigrations                              |
| Performance  | Buena si hay pocos atributos   | Muy buena para modelos embebidos grandes                    |

---

## 🔄 ¿Qué tenés que prever hoy si pensás migrar a Mongo mañana?

1. **Diseñar los repositorios desde el día 1** (lo estamos haciendo).
2. **Evitar LINQ complejo en la capa de aplicación** (que solo EF Core entiende).
3. **Tener mapeos DTO <-> modelo de dominio bien separados**.
4. **Tener tests unitarios y de integración independientes del proveedor de datos.**
5. (Opcional) Crear una implementación MongoMock más adelante para ir testeando.

---

## 🧠 Recomendación práctica

Para tu caso actual:

- ✅ Empezá con SQLite + EF Core porque te permite iterar muy rápido
- ✅ Diseñá desde el día 1 con `IArticleRepository` y `IAttributeRepository`
- 🕐 Cuando quieras migrar, solo cambiás la implementación + configuración de DI
- 🧪 Antes de migrar, podés clonar algunos datos reales en Mongo y probar queries equivalentes
