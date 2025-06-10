# 📋 Reglas de Onboarding para Agentes AI

## 🎯 Objetivo
Este documento establece las reglas fundamentales que debe seguir cualquier agente AI al trabajar en el proyecto PimFlow (Product Information Management).

## 📋 **Información del Proyecto PimFlow**

### **🏗️ Arquitectura**
- **Clean Architecture** + **DDD** + **SOLID Principles**
- **Frontend**: Blazor WebAssembly
- **Backend**: ASP.NET Core Web API
- **Base de datos**: SQLite (desarrollo), configurable para producción

### **📁 Estructura de Proyectos**
```
src/
├── PimFlow.Domain/          # Entidades, Value Objects, Interfaces
├── PimFlow.Server/          # API, Controllers, Services, Repositories
├── PimFlow.Client/          # Blazor WebAssembly frontend
├── PimFlow.Shared/          # DTOs, contratos compartidos
└── PimFlow.Contracts/       # Interfaces y contratos base

tests/
├── PimFlow.Domain.Tests/        # Tests de dominio
├── PimFlow.Server.Tests/        # Tests de API y servicios
├── PimFlow.Shared.Tests/        # Tests de DTOs y mappers
└── PimFlow.Architecture.Tests/  # Tests arquitectónicos
```

### **🚀 Scripts Disponibles**
```bash
./scripts/create-feature.sh         # Crear nueva feature branch
./scripts/git-flow-status.sh        # Ver estado de Git Flow
./scripts/validate-feature.sh       # Validar feature antes de merge
./scripts/pre-merge-check.sh        # Verificaciones pre-merge
./scripts/test-architecture.ps1     # Tests arquitectónicos
```

## 🌍 **Regla #1: Idioma**
- ✅ **SIEMPRE hablar en español**
- ✅ Usar terminología técnica en español cuando sea posible
- ✅ Explicaciones, comentarios y documentación en español
- ❌ No cambiar a inglés sin autorización explícita

## 🚫 **Regla #2: Control de Versiones y GitHub Flow**
- ❌ **NUNCA hacer commits automáticamente**
- ❌ **NUNCA hacer push sin autorización explícita**
- ❌ **NUNCA commitear directamente a main sin feature**
- ✅ **SIEMPRE pedir confirmación** antes de cualquier operación git
- ✅ **SIEMPRE usar GitHub Flow** para nuevas funcionalidades
- ✅ Mostrar los cambios propuestos antes de commitear
- ✅ Usar conventional commits cuando se autorice

### Flujo Correcto GitHub Flow:
1. **Para código nuevo**: Crear feature branch desde main
   ```bash
   git checkout main
   git pull origin main
   git checkout -b feature/nombre-funcionalidad
   ```
2. Hacer cambios en archivos
3. Mostrar qué se cambió
4. **PREGUNTAR** si se debe hacer commit
5. **PREGUNTAR** si se debe hacer push
6. **PREGUNTAR** si se debe mergear a main
7. Solo proceder con autorización explícita

### Ramas Permitidas para Commits:
- ✅ **feature/***: Para nuevas funcionalidades
- ✅ **hotfix/***: Para correcciones urgentes
- ⚠️ **main**: Solo para merges de features (nunca commits directos)

## 💡 **Regla #3: Código Nuevo**
- ⚠️ **SIEMPRE confirmar antes** de generar código nuevo
- ✅ Explicar la propuesta y beneficios
- ✅ Mostrar ejemplos de lo que se va a crear
- ✅ Esperar aprobación explícita del usuario
- ❌ No crear archivos nuevos sin autorización

### Ejemplos que Requieren Confirmación:
- Nuevos endpoints en controllers
- Nuevas entidades o modelos
- Nuevos servicios o repositorios
- Nuevos tests
- Nuevos componentes de UI
- Scripts de automatización
- Archivos de configuración

## 🧹 **Regla #4: Código Limpio**
- ✅ **SIEMPRE mantener el código limpio**
- ❌ No dejar código temporal o de debug en producción
- ✅ Eliminar inmediatamente cualquier código de prueba
- ✅ Seguir las convenciones del proyecto existente
- ✅ Respetar la arquitectura establecida

## 🔍 **Regla #5: Transparencia**
- ✅ **SIEMPRE explicar** qué se va a hacer antes de hacerlo
- ✅ Mostrar el plan paso a paso
- ✅ Informar sobre posibles riesgos o efectos secundarios
- ✅ Pedir feedback durante el proceso
- ❌ No hacer cambios "silenciosos"

## 🧪 **Regla #6: Testing y Verificación (ESTRICTA)**
- ✅ **OBLIGATORIO: Crear/actualizar tests** después de cada paso o tarea
- ✅ **OBLIGATORIO: Ejecutar tests** después de cada cierre de paso
- ✅ **OBLIGATORIO: Ejecutar tests completos** en cierre total de tarea
- ✅ **OBLIGATORIO: Levantar aplicación** cuando tenga sentido al final
- ✅ **OBLIGATORIO: Mostrar en browser** para verificar funcionamiento
- ✅ Verificar que no se rompió funcionalidad existente
- ❌ No asumir que algo funciona sin verificar
- ❌ No cerrar tareas sin verificación completa

### **Protocolo Obligatorio de Cierre:**

#### **🔄 Al Cerrar Cada Paso/Subtarea:**
1. **Crear/Actualizar Tests** correspondientes al cambio
2. **Ejecutar Tests** del módulo modificado
3. **Verificar** que todos los tests pasan
4. **Reportar** resultado de tests al usuario

#### **🎯 Al Cerrar Tarea Completa:**
1. **Ejecutar TODOS los tests** de la solución
2. **Compilar** toda la solución sin errores
3. **Levantar la aplicación** (cuando aplique)
4. **Abrir browser** y mostrar funcionamiento
5. **Verificar** funcionalidades principales
6. **Reportar** estado completo al usuario

#### **🚀 Comandos de Verificación Estándar:**
```bash
# Tests por módulo específico
dotnet test tests/PimFlow.Domain.Tests/
dotnet test tests/PimFlow.Server.Tests/
dotnet test tests/PimFlow.Shared.Tests/
dotnet test tests/PimFlow.Architecture.Tests/

# Tests completos
dotnet test --verbosity normal

# Compilación completa
dotnet build

# Levantar aplicación
dotnet run --project src/PimFlow.Server

# Verificar endpoints
curl -X GET "http://localhost:5001/api/articles"
curl -X GET "http://localhost:5001/api/categories"
```

## 📚 **Regla #7: Documentación**
- ✅ **SIEMPRE actualizar documentación** cuando sea relevante
- ✅ Mantener README y docs/ actualizados
- ✅ Documentar decisiones arquitectónicas importantes
- ✅ Explicar el "por qué" de los cambios, no solo el "qué"

## 🎯 **Regla #8: Enfoque en el Usuario**
- ✅ **SIEMPRE preguntar** si no está claro qué quiere el usuario
- ✅ Confirmar entendimiento antes de proceder
- ✅ Ofrecer alternativas cuando sea apropiado
- ❌ No asumir intenciones o requisitos

## ⚠️ **Regla #9: Gestión de Errores**
- ✅ **SIEMPRE informar** si algo sale mal
- ✅ Explicar qué pasó y por qué
- ✅ Ofrecer soluciones alternativas
- ✅ Pedir ayuda si es necesario
- ❌ No ocultar errores o problemas

## 🌊 **Regla #10: GitHub Flow Obligatorio**
- ✅ **SIEMPRE usar GitHub Flow** para cualquier código nuevo
- ✅ **SIEMPRE trabajar desde main** como base de desarrollo
- ✅ **SIEMPRE crear features** para nuevas funcionalidades
- ❌ **NUNCA saltarse el flujo** de GitHub Flow
- ✅ **SIEMPRE mergear features a main** correctamente

### Comandos GitHub Flow Esenciales:
```bash
# Verificar estado
git status
git branch --show-current

# Nueva funcionalidad desde main
git checkout main
git pull origin main
git checkout -b feature/nombre-feature

# Validar feature antes de merge
./scripts/validate-feature.sh

# Pre-merge check
./scripts/pre-merge-check.sh feature/nombre-feature
```

### Flujo Típico para Agentes:
1. **Verificar rama actual**: `git branch --show-current`
2. **Si no estás en feature**: Crear desde main
3. **Desarrollar**: Hacer cambios y commits
4. **Validar**: `./scripts/validate-feature.sh`
5. **Pre-merge check**: `./scripts/pre-merge-check.sh feature/nombre-feature`
6. **Finalizar**: Merge a main con autorización

## 🏗️ **Regla #11: Decisiones Arquitectónicas y Planificación**
- ✅ **Una feature = un objetivo específico**
- ✅ **Refactorings grandes = features separadas**
- ✅ **Planificación secuencial** para cambios complejos
- ✅ **Completar una funcionalidad** antes de empezar otra
- ❌ **No mezclar objetivos** en una sola feature
- ❌ **No hacer cambios invasivos** junto con funcionalidades nuevas

### Ejemplos de Planificación Correcta:
```bash
# ✅ CORRECTO: Desarrollo secuencial
Fase 1: feature/implement-hosted-architecture
Fase 2: feature/refactor-domain-agnostic-names

# ❌ INCORRECTO: Mezclar objetivos
feature/hosted-and-refactoring-together
```

### Principios de Planificación:
- **🎯 Enfoque**: 100% concentración en un objetivo
- **🧪 Testing**: Aislado por funcionalidad
- **🔄 Rollback**: Fácil reversión si hay problemas
- **📋 Commits**: Historia clara y trazable
- **🚀 Entrega**: Valor incremental por feature

## 🏗️ **Regla #12: Arquitectura Domain-Agnostic Obligatoria**
- ✅ **SIEMPRE usar configuración centralizada** con clases de settings
- ✅ **SIEMPRE usar nombres genéricos** en bases de datos y archivos
- ✅ **SIEMPRE implementar feature flags** para control granular
- ✅ **SIEMPRE estructurar configuración** por ambiente (Dev/Prod)
- ❌ **NUNCA hardcodear nombres específicos** de dominio en configuración
- ❌ **NUNCA usar configuración dispersa** sin centralización

### Estructura Obligatoria de Configuración:
```json
{
  "Application": {
    "Name": "ApplicationName",
    "Version": "1.0.0",
    "Environment": "Development"
  },
  "Database": {
    "Provider": "SQLite",
    "ConnectionString": "Data Source=App_Data/application.db"
  },
  "Features": {
    "EnableSwagger": true,
    "EnableDetailedErrors": true,
    "EnableSeedData": true
  }
}
```

### Clases de Configuración Requeridas:
- `ApplicationSettings` - Info general de la aplicación
- `DatabaseSettings` - Configuración de base de datos agnóstica
- `FeatureSettings` - Feature flags para control granular
- `SecuritySettings` - Configuración de seguridad

## 🌊 **Regla #13: Manejo de Feature Branches (Historia Preservada)**
- ✅ **SIEMPRE mantener branches remotas** como historia después del merge
- ✅ **SOLO eliminar branches locales** con `git branch -d` después del merge
- ✅ **NUNCA eliminar branches remotas** con `git push origin --delete`
- ✅ **NO usar Pull Requests** por defecto (merge directo preferido)
- ❌ **NUNCA seguir Git Flow "ortodoxo"** que elimina historia
- ❌ **NUNCA borrar feature branches remotas** automáticamente

### Flujo Correcto de Feature Branches:
```bash
# 1. Crear feature desde main
git checkout main
git pull origin main
git checkout -b feature/nueva-funcionalidad

# 2. Desarrollar y commitear
git add .
git commit -m "feat: implementar nueva funcionalidad"

# 3. Pushear feature branch (mantener historia)
git push origin feature/nueva-funcionalidad

# 4. Merge a main con --no-ff
git checkout main
git merge --no-ff feature/nueva-funcionalidad

# 5. Push main
git push origin main

# 6. SOLO eliminar branch local (mantener remota)
git branch -d feature/nueva-funcionalidad
# ❌ NO HACER: git push origin --delete feature/nueva-funcionalidad
```

### Beneficios de Mantener Historia:
- 🗂️ **Trazabilidad completa** de features desarrolladas
- 🔍 **Facilita debugging** y análisis retrospectivo
- 📊 **Mejor comprensión** del flujo de desarrollo
- 🔄 **Posibilidad de recuperar** trabajo específico

## 🔄 **Regla #14: Iteración y Mejora**
- ✅ **SIEMPRE estar abierto** a feedback y correcciones
- ✅ Aprender de los errores y ajustar comportamiento
- ✅ Sugerir mejoras cuando sea apropiado
- ✅ Mantener un enfoque de mejora continua

---

## 📝 **Frases Clave para Recordar**

### ✅ **Usar SIEMPRE:**
- "¿Te parece bien si...?"
- "¿Quieres que proceda con...?"
- "¿Debo hacer commit de estos cambios?"
- "¿Te gustaría que haga push al repositorio?"
- "Antes de crear código nuevo, ¿confirmas que...?"
- "¿Creo una nueva feature para esta funcionalidad?"
- "¿Cómo quieres que llame a la feature?"
- "¿Debo finalizar la feature y mergear a develop?"
- "¿Completamos esta funcionalidad antes de empezar la siguiente?"
- "¿Prefieres hacer esto en una feature separada?"
- "¿Te parece mejor planificación secuencial o todo junto?"
- **"Voy a crear/actualizar los tests correspondientes"**
- **"Ejecutando tests para verificar que todo funciona"**
- **"Levantando la aplicación para verificar funcionamiento"**
- **"Abriendo browser para mostrar que todo funciona correctamente"**

### ❌ **NUNCA usar:**
- "Voy a hacer commit automáticamente"
- "Haciendo push de los cambios"
- "Creando nuevo código sin confirmar"
- "Asumiendo que quieres..."
- "Commiteando directamente a main"
- "Saltándome Git Flow"
- "Voy a hacer ambas funcionalidades en una feature"
- "Mezclando refactoring con nueva funcionalidad"
- "Asumiendo que quieres todo junto"
- **"Cerrando tarea sin ejecutar tests"**
- **"Asumiendo que funciona sin verificar"**
- **"Terminando sin mostrar la aplicación funcionando"**

---

## 🎯 **Resumen Ejecutivo**

**Las 10 reglas de oro:**
1. 🌍 **Español siempre**
2. 🚫 **Nunca commit/push sin autorización**
3. 💡 **Siempre confirmar antes de código nuevo**
4. 🌊 **Siempre usar GitHub Flow (features desde main)**
5. 🔧 **Siempre crear features para funcionalidades nuevas**
6. 🏗️ **Una feature = un objetivo (planificación secuencial)**
7. 🏗️ **Arquitectura Domain-Agnostic obligatoria**
8. 🗂️ **Mantener feature branches remotas como historia**
9. 🧪 **OBLIGATORIO: Tests en cada paso y cierre de tarea**
10. 🚀 **OBLIGATORIO: Verificar funcionamiento con aplicación levantada**

**Flujo básico:** Confirmar → Crear feature desde main → Desarrollar → **Tests** → Commitear → **Verificar** → Mergear a main → Nueva feature

**Planificación:** Una funcionalidad completa → **Tests + Verificación** → Siguiente funcionalidad (no mezclar objetivos)

**Testing:** Cada paso → Tests módulo | Cierre total → Tests completos + Aplicación + Browser

**Recuerda:** Es mejor preguntar de más que asumir de menos.

---

## 🧠 **Lecciones Aprendidas del Usuario**

### **🧹 Código Limpio y Profesional**
- **Lección**: "No me convenció mucho eso que hiciste nuevo de debug... creo que eso va a ensuciar el código"
- **Aplicación**: Nunca dejar código temporal, debug endpoints, o elementos no productivos
- **Principio**: Mantener siempre código production-ready

### **🔄 Commits Atómicos y Conventional**
- **Lección**: "Tendríamos que hacer commits pequeños y bien definidos con conventional commit"
- **Aplicación**: Cada commit debe tener un propósito específico y claro
- **Principio**: Historial de git limpio y trazable

### **🚫 Control Estricto de Git**
- **Lección**: "Hagamos push", "osea vos ya pusheaste?"
- **Aplicación**: SIEMPRE preguntar explícitamente antes de cualquier operación git
- **Principio**: Control total del usuario sobre el repositorio

### **🏗️ Arquitectura y Mejores Prácticas**
- **Lección**: Conocimiento profundo de Entity Framework, Clean Architecture, DDD
- **Aplicación**: Implementar soluciones arquitectónicamente correctas
- **Principio**: Hacer las cosas "bien" aunque tome más tiempo

### **🤝 Validación Colaborativa**
- **Lección**: "¿Qué opinas?", "¿Estás de acuerdo?", "¿Te parece bien?"
- **Aplicación**: Buscar validación en decisiones técnicas importantes
- **Principio**: Decisiones colaborativas y bien fundamentadas

### **💬 Estilo de Comunicación**
- **Lección**: Feedback directo pero constructivo
- **Aplicación**: Aceptar correcciones y ajustar comportamiento inmediatamente
- **Principio**: Comunicación clara y orientada a soluciones

### **🏗️ Planificación Arquitectónica**
- **Lección**: "Una feature = un objetivo", "Completar Hosted primero, después refactoring"
- **Aplicación**: Desarrollo secuencial, no mezclar objetivos complejos
- **Principio**: Enfoque 100% en una funcionalidad, testing aislado, commits trazables

## 🎯 **Patrones de Comportamiento del Usuario**

### **✅ Frases Características:**
- "¿Qué opinas?"
- "¿Estás de acuerdo?"
- "Tendríamos que..."
- "No me convenció mucho..."
- "Creo que lo más prolijo es..."
- "¿Te parece bien?"

### **🔍 Señales de Alerta (Cuando el Usuario Dice):**
- **"No me convenció"** → Revisar y limpiar inmediatamente
- **"Tendríamos que"** → Está sugiriendo una mejora, implementar
- **"¿Qué opinas?"** → Busca validación técnica, dar opinión fundamentada
- **"¿Estás de acuerdo?"** → Quiere confirmación antes de proceder
- **"Una feature = un objetivo"** → Separar funcionalidades en features distintas
- **"Completar X primero"** → Planificación secuencial, no mezclar objetivos

### **🎯 Preferencias Técnicas Confirmadas:**
- ✅ Clean Architecture y DDD
- ✅ Entity Framework con migraciones
- ✅ Conventional commits
- ✅ Código sin elementos temporales
- ✅ Tests completos y funcionando
- ✅ Documentación actualizada
- ✅ Comunicación en español

## 📋 **Checklist Antes de Cualquier Acción:**

1. **🌍 ¿Estoy comunicando en español?**
2. **🧹 ¿El código que voy a crear es production-ready?**
3. **💡 ¿Confirmé antes de generar código nuevo?**
4. **🌊 ¿Estoy usando Git Flow correctamente?**
5. **🔄 ¿Estoy en la rama correcta (feature/*)? **
6. **🔄 ¿Planifiqué commits atómicos?**
7. **🚫 ¿Pedí autorización para git operations?**
8. **🤝 ¿Busqué validación en decisiones técnicas?**
9. **🎯 ¿Seguí las mejores prácticas arquitectónicas?**
10. **🏗️ ¿Esta feature tiene un solo objetivo específico?**
11. **📋 ¿Confirmé la planificación secuencial si hay múltiples cambios?**
12. **🧪 ¿Creé/actualicé tests para los cambios realizados?**
13. **🧪 ¿Ejecuté tests después de cada paso importante?**

## 🎯 **Checklist OBLIGATORIO de Cierre de Tarea:**

1. **🧪 ¿Ejecuté TODOS los tests de la solución?**
2. **🔧 ¿Compilé toda la solución sin errores?**
3. **🚀 ¿Levanté la aplicación para verificar funcionamiento?**
4. **🌐 ¿Abrí el browser para mostrar que todo funciona?**
5. **✅ ¿Verifiqué las funcionalidades principales?**
6. **📊 ¿Reporté el estado completo al usuario?**

## 🌊 **Checklist Específico Git Flow:**

### **Antes de Crear Código:**
1. **¿Estoy en una feature branch?** Si no: `./scripts/create-feature.sh nombre`
2. **¿La feature tiene un nombre descriptivo?**
3. **¿Confirmé con el usuario el nombre de la feature?**

### **Antes de Commitear:**
1. **¿Estoy en feature/* branch?** (NUNCA en main directamente)
2. **¿Uso conventional commits?**
3. **¿Pedí autorización para el commit?**

### **Antes de Finalizar Feature:**
1. **¿Todos los tests pasan?**
2. **¿El código está limpio y completo?**
3. **¿Pedí autorización para finalizar?**
4. **¿Confirmé que se debe mergear a main?**

**Recuerda:** Es mejor preguntar de más que asumir de menos.

---

## 🧪 **Protocolo Estricto de Testing y Verificación**

### **📋 Reglas OBLIGATORIAS:**

#### **🔄 En Cada Paso/Subtarea:**
1. **Identificar** qué componentes se modificaron
2. **Crear/Actualizar** tests correspondientes
3. **Ejecutar** tests del módulo específico
4. **Verificar** que todos pasan
5. **Informar** resultado al usuario

#### **🎯 En Cierre Total de Tarea:**
1. **Ejecutar** `dotnet test` (todos los tests)
2. **Ejecutar** `dotnet build` (compilación completa)
3. **Levantar** aplicación con `dotnet run`
4. **Abrir** browser en la URL correspondiente
5. **Verificar** funcionalidades principales manualmente
6. **Reportar** estado completo con números de tests

### **📊 Formato de Reporte Obligatorio:**

#### **Al Cerrar Paso:**
```
✅ Tests Actualizados: [Módulo] - [X] tests
✅ Ejecución: [X] correctos, [Y] errores
✅ Estado: [CORRECTO/REQUIERE ATENCIÓN]
```

#### **Al Cerrar Tarea Completa:**
```
🎯 VERIFICACIÓN COMPLETA:
✅ Tests Totales: [X] correctos, [Y] errores
✅ Compilación: [EXITOSA/CON ERRORES]
✅ Aplicación: [FUNCIONANDO/CON PROBLEMAS]
✅ Browser: [URL] - [VERIFICADO/PROBLEMAS]
✅ Funcionalidades: [LISTA DE VERIFICACIONES]
```

### **🚀 Comandos Estándar de Verificación:**
```bash
# Por módulo específico
dotnet test tests/PimFlow.Domain.Tests/
dotnet test tests/PimFlow.Shared.Tests/
dotnet test tests/PimFlow.Server.Tests/
dotnet test tests/PimFlow.Architecture.Tests/

# Todos los tests
dotnet test --verbosity normal

# Compilación completa
dotnet build

# Levantar aplicación
dotnet run --project src/PimFlow.Server

# Verificar API
curl -X GET "http://localhost:5001/api/articles"
curl -X GET "http://localhost:5001/api/categories"
curl -X GET "http://localhost:5001/api/customattributes"
Invoke-WebRequest -Uri "http://localhost:5001/api/articles"
```

### **🌐 URLs Estándar para Verificación:**
- **API Base**: `http://localhost:5001/api/`
- **Aplicación Blazor**: `http://localhost:5001/`
- **Swagger**: `http://localhost:5001/swagger`
- **Endpoints principales**:
  - Articles: `http://localhost:5001/api/articles`
  - Categories: `http://localhost:5001/api/categories`
  - Custom Attributes: `http://localhost:5001/api/customattributes`

### **❌ Errores Comunes a Evitar:**
- Cerrar tarea sin ejecutar tests
- Asumir que funciona sin verificar
- No reportar números específicos de tests
- No mostrar la aplicación funcionando
- Saltarse la verificación en browser
