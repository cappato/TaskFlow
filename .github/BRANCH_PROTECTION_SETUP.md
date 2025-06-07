# 🔒 Configuración de Branch Protection Rules

Este archivo contiene las instrucciones para configurar las **Branch Protection Rules** en GitHub que harán cumplir automáticamente las reglas del repositorio.

## 📋 Configuración Requerida

### 1. Acceder a Branch Protection Rules

1. Ir a **Settings** → **Branches** en tu repositorio de GitHub
2. Click en **Add rule** o editar la regla existente para `main`

### 2. Configuración de la Rama `main`

```json
{
  "branch_name_pattern": "main",
  "required_status_checks": {
    "strict": true,
    "contexts": [
      "🔒 Enforce Repository Rules / 📁 Validate Project Structure",
      "🔒 Enforce Repository Rules / 📝 Validate Commit Messages",
      "🔒 Enforce Repository Rules / 🔨 Build and Test",
      "🔍 Pull Request Validation / 📋 PR Rules Validation",
      "🔍 Pull Request Validation / 📝 Validate All Commits in PR",
      "🔍 Pull Request Validation / 📁 Validate File Changes"
    ]
  },
  "enforce_admins": true,
  "required_pull_request_reviews": {
    "required_approving_review_count": 1,
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": false,
    "require_last_push_approval": true
  },
  "restrictions": null,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "block_creations": false,
  "required_conversation_resolution": true
}
```

### 3. Configuración Manual en GitHub UI

#### **Branch name pattern**
```
main
```

#### **Protect matching branches**
- ✅ **Require a pull request before merging**
  - ✅ Require approvals: **1**
  - ✅ Dismiss stale pull request approvals when new commits are pushed
  - ✅ Require review from code owners (opcional)
  - ✅ Require approval of the most recent reviewable push

- ✅ **Require status checks to pass before merging**
  - ✅ Require branches to be up to date before merging
  - **Required status checks:**
    - `🔒 Enforce Repository Rules / 📁 Validate Project Structure`
    - `🔒 Enforce Repository Rules / 📝 Validate Commit Messages`
    - `🔒 Enforce Repository Rules / 🔨 Build and Test`
    - `🔍 Pull Request Validation / 📋 PR Rules Validation`
    - `🔍 Pull Request Validation / 📝 Validate All Commits in PR`
    - `🔍 Pull Request Validation / 📁 Validate File Changes`

- ✅ **Require conversation resolution before merging**

- ✅ **Require signed commits** (opcional pero recomendado)

- ✅ **Require linear history** (opcional)

- ✅ **Include administrators**

- ❌ **Allow force pushes** (DESHABILITADO)

- ❌ **Allow deletions** (DESHABILITADO)

## 🛡️ Rulesets Adicionales (GitHub Enterprise)

Si tienes GitHub Enterprise, puedes crear **Rulesets** más avanzados:

### Ruleset para Nombres de Ramas

```yaml
name: "Branch Naming Convention"
target: "branch"
enforcement: "active"
conditions:
  ref_name:
    include:
      - "refs/heads/feature/*"
      - "refs/heads/fix/*" 
      - "refs/heads/refactor/*"
    exclude:
      - "refs/heads/main"
rules:
  - type: "creation"
  - type: "update"
    parameters:
      update_allows_fetch_and_merge: false
```

### Ruleset para Commits

```yaml
name: "Commit Message Convention"
target: "branch"
enforcement: "active"
conditions:
  ref_name:
    include: ["refs/heads/*"]
rules:
  - type: "commit_message_pattern"
    parameters:
      pattern: "^(feat|fix|docs|style|refactor|test|chore)(\\(.+\\))?: .{1,50}$"
      operator: "regex"
```

## 🔧 Configuración Automática con GitHub CLI

Puedes usar GitHub CLI para configurar las reglas automáticamente:

```bash
# Instalar GitHub CLI si no lo tienes
# https://cli.github.com/

# Autenticarse
gh auth login

# Configurar branch protection
gh api repos/:owner/:repo/branches/main/protection \
  --method PUT \
  --field required_status_checks='{"strict":true,"contexts":["🔒 Enforce Repository Rules / 📁 Validate Project Structure","🔒 Enforce Repository Rules / 📝 Validate Commit Messages","🔒 Enforce Repository Rules / 🔨 Build and Test","🔍 Pull Request Validation / 📋 PR Rules Validation","🔍 Pull Request Validation / 📝 Validate All Commits in PR","🔍 Pull Request Validation / 📁 Validate File Changes"]}' \
  --field enforce_admins=true \
  --field required_pull_request_reviews='{"required_approving_review_count":1,"dismiss_stale_reviews":true,"require_code_owner_reviews":false,"require_last_push_approval":true}' \
  --field restrictions=null \
  --field allow_force_pushes=false \
  --field allow_deletions=false \
  --field required_conversation_resolution=true
```

## 📊 Verificación de Configuración

Para verificar que las reglas están funcionando:

1. **Crear una rama de prueba:**
   ```bash
   git checkout -b test/branch-protection
   echo "test" > test.txt
   git add test.txt
   git commit -m "test: verificar branch protection"
   git push origin test/branch-protection
   ```

2. **Intentar hacer push directo a main (debe fallar):**
   ```bash
   git checkout main
   echo "test" > direct-push.txt
   git add direct-push.txt
   git commit -m "test: push directo"
   git push origin main
   # Debe mostrar error: "remote: error: GH006: Protected branch update failed"
   ```

3. **Crear PR y verificar que los checks se ejecutan**

## 🚨 Troubleshooting

### Status Checks No Aparecen
- Asegúrate de que los workflows se hayan ejecutado al menos una vez
- Los nombres de los checks deben coincidir exactamente
- Verifica que los workflows estén en la rama `main`

### Reglas No Se Aplican
- Verifica que "Include administrators" esté habilitado
- Asegúrate de que no hay excepciones configuradas
- Revisa los permisos del repositorio

### Workflows Fallan
- Revisa los logs de GitHub Actions
- Verifica que los secrets necesarios estén configurados
- Asegúrate de que el token tiene los permisos correctos

## 🔄 Mantenimiento

Las reglas deben revisarse periódicamente:

- **Mensualmente**: Revisar efectividad de las reglas
- **Trimestralmente**: Actualizar según nuevas necesidades
- **Anualmente**: Revisión completa del sistema

## 📞 Soporte

Si tienes problemas configurando las reglas:

1. Revisa la [documentación oficial de GitHub](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/defining-the-mergeability-of-pull-requests/about-protected-branches)
2. Crea un issue en el repositorio
3. Consulta con el equipo de DevOps

---

**⚠️ IMPORTANTE**: Una vez configuradas estas reglas, **NO SE PUEDEN EVADIR**. Asegúrate de que todo el equipo entiende las reglas antes de activarlas.
