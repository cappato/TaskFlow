# Reglas Estrictas del Repositorio TaskFlow

Este documento define las **reglas obligatorias** de trabajo colaborativo para este proyecto. Estas reglas se **aplican automáticamente** mediante triggers y no pueden ser evadidas.

---

## 🚫 **REGLAS AUTOMATIZADAS - NO NEGOCIABLES**

### 1. **Estructura del Proyecto**
```
✅ OBLIGATORIO:
src/
├── TaskFlow.Client/     # Blazor WebAssembly
├── TaskFlow.Server/     # ASP.NET Core API
└── TaskFlow.Shared/     # Modelos compartidos
tests/
└── TaskFlow.Server.Tests/
```

**🤖 Automatización**: GitHub Action valida estructura en cada PR.

### 2. **Estrategia de Ramas - FLUJO SIMPLIFICADO**

**✅ PERMITIDO:**
- `main` → Código estable y desplegable
- `feature/nombre-descriptivo` → Nueva funcionalidad
- `fix/descripcion-bug` → Corrección de errores
- `refactor/componente` → Refactoring de código

**❌ PROHIBIDO:**
- Commits directos a `main`
- Ramas sin prefijo (`feature/`, `fix/`, `refactor/`)
- Nombres de rama con espacios o caracteres especiales

**🤖 Automatización**: Branch Protection Rules + GitHub Action.

### 3. **Convención de Commits - OBLIGATORIA**

**✅ FORMATO REQUERIDO:**
```
tipo: descripción clara en español

feat: agregar formulario de inicio de sesión
fix: corregir error de referencia nula en TaskService
refactor: separar lógica de autenticación en AuthService
test: agregar pruebas unitarias para TaskController
docs: actualizar README con instrucciones de Docker
chore: actualizar dependencias de Entity Framework
```

**📏 LÍMITES ESTRICTOS:**
- Máximo **10 archivos** por commit
- Máximo **300 líneas** cambiadas por commit
- Título del commit: 50-72 caracteres
- Descripción obligatoria si supera 5 archivos

**🤖 Automatización**: GitHub Action valida cada commit.

### 4. **Pull Requests - PROCESO OBLIGATORIO**

**✅ REQUERIMIENTOS:**
- Título en inglés: `Add user authentication system`
- Descripción en español explicando QUÉ y POR QUÉ
- Al menos 1 reviewer aprobación
- Todos los checks de CI/CD en verde
- Tests que cubran el nuevo código

**❌ PROHIBIDO:**
- Merge sin revisión
- Merge con CI/CD fallando
- PRs sin descripción
- Squash de commits valiosos

**🤖 Automatización**: Branch Protection Rules + Required Reviews.

### 5. **Versionado Semántico - AUTOMÁTICO**

**✅ FORMATO:**
- `v1.0.0` → Release mayor
- `v1.1.0` → Nueva funcionalidad
- `v1.0.1` → Bugfix

**🤖 Automatización**: GitHub Action crea tags automáticamente.

### 6. **Integración Continua - OBLIGATORIA**

**✅ PIPELINE REQUERIDA:**
```yaml
1. dotnet restore
2. dotnet build --configuration Release
3. dotnet test --collect:"XPlat Code Coverage"
4. Análisis de código estático
5. Verificación de convenciones
```

**❌ MERGE BLOQUEADO SI:**
- Cualquier test falla
- Cobertura < 80%
- Build falla
- Violación de reglas de commit

**🤖 Automatización**: Required Status Checks.

### 7. **Pruebas - COBERTURA MÍNIMA**

**✅ OBLIGATORIO:**
- Cobertura mínima: **80%**
- Tests unitarios para nueva funcionalidad
- Tests de integración para controllers
- Usar xUnit + FluentAssertions

**🤖 Automatización**: GitHub Action bloquea merge si cobertura < 80%.

### 8. **Calidad de Código - ANÁLISIS AUTOMÁTICO**

**✅ HERRAMIENTAS:**
- SonarCloud para análisis estático
- CodeQL para seguridad
- Dependabot para vulnerabilidades
- EditorConfig para formato

**🤖 Automatización**: Quality Gates obligatorios.

---

## 🔧 **CONFIGURACIÓN DE ENFORCEMENT**

### Branch Protection Rules (GitHub Settings)
```json
{
  "required_status_checks": {
    "strict": true,
    "contexts": [
      "build-and-test",
      "code-quality",
      "commit-validation"
    ]
  },
  "enforce_admins": true,
  "required_pull_request_reviews": {
    "required_approving_review_count": 1,
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": false
  },
  "restrictions": null,
  "allow_force_pushes": false,
  "allow_deletions": false
}
```

### Rulesets Adicionales
- Bloquear push directo a `main`
- Validar nombres de ramas
- Requerir commits firmados (opcional)
- Limitar quién puede hacer merge

---

## ⚡ **TRIGGERS AUTOMÁTICOS**

### En cada Push:
1. ✅ Validar estructura de proyecto
2. ✅ Verificar convención de commits
3. ✅ Ejecutar tests y build
4. ✅ Análisis de calidad de código

### En cada PR:
1. ✅ Validar título y descripción
2. ✅ Verificar que viene de rama válida
3. ✅ Ejecutar suite completa de tests
4. ✅ Verificar cobertura de código
5. ✅ Análisis de seguridad

### En cada Merge:
1. ✅ Crear tag automático si es release
2. ✅ Actualizar CHANGELOG.md
3. ✅ Notificar en Slack/Discord (opcional)

---

## 🚨 **VIOLACIONES Y CONSECUENCIAS**

### Commit Inválido:
- ❌ **Bloqueado automáticamente**
- 📧 Notificación con instrucciones de corrección
- 🔄 Debe rehacer el commit con formato correcto

### PR Sin Cumplir Reglas:
- ❌ **Merge bloqueado**
- 🤖 Bot comenta qué falta corregir
- ✅ Se desbloquea automáticamente al corregir

### Tests Fallando:
- ❌ **Deploy bloqueado**
- 🔴 Status check en rojo
- 📊 Reporte detallado de fallos

---

## 🎯 **BENEFICIOS DEL SISTEMA**

✅ **Calidad Garantizada**: Código siempre funcional en `main`
✅ **Consistencia**: Todos siguen las mismas reglas
✅ **Automatización**: Sin intervención manual
✅ **Trazabilidad**: Historial claro de cambios
✅ **Seguridad**: Análisis automático de vulnerabilidades
✅ **Productividad**: Menos tiempo en reviews manuales

---

## 📚 **RECURSOS DE AYUDA**

- **Conventional Commits**: https://www.conventionalcommits.org/
- **Semantic Versioning**: https://semver.org/
- **GitHub Flow**: https://guides.github.com/introduction/flow/
- **Documentación interna**: Ver `/docs` en este repo

---

## 🔄 **ACTUALIZACIÓN DE REGLAS**

Las reglas pueden actualizarse mediante PR a este archivo, pero requieren:
- ✅ Aprobación de 2+ maintainers
- ✅ Discusión en GitHub Discussions
- ✅ Actualización de automatizaciones correspondientes

---

**⚠️ IMPORTANTE**: Estas reglas se aplican automáticamente. No hay excepciones ni overrides manuales. El sistema está diseñado para mantener la calidad y consistencia del proyecto sin intervención humana.
