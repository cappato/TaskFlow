# 🚀 Azure Deployment Guide - TaskFlow

## ✅ **¿Por qué Azure?**
- **🆓 Completamente GRATIS** (Free tier F1)
- **🔒 SSL automático** (HTTPS)
- **🌐 Dominio personalizado** disponible
- **📊 Monitoreo** incluido
- **🔄 CI/CD** con GitHub Actions
- **⚡ Perfecto para .NET** (es de Microsoft)

---

## 🎯 **Método 1: Visual Studio (MÁS FÁCIL)**

### **Paso 1: Preparar Azure**
1. **Crear cuenta gratuita:** https://azure.microsoft.com/free/
2. **Verificar email** y completar registro

### **Paso 2: Deploy desde Visual Studio**
1. **Abrir** `TaskFlow.sln` en Visual Studio 2022
2. **Right-click** en `TaskFlow.Server` → **Publish**
3. **Target:** Azure → **Azure App Service (Windows)**
4. **Create New** → Configurar:
   - **App name:** `taskflow-tuusuario` (debe ser único)
   - **Subscription:** Azure for Students/Free Trial
   - **Resource Group:** Create new → `taskflow-rg`
   - **Hosting Plan:** Create new → **Free F1**
5. **Create** → **Publish**

### **Resultado:**
- **URL:** `https://taskflow-tuusuario.azurewebsites.net`
- **Deploy automático** configurado
- **SSL** habilitado automáticamente

---

## 🎯 **Método 2: GitHub Actions (AUTOMÁTICO)**

### **Paso 1: Crear App Service**
1. **Portal Azure:** https://portal.azure.com
2. **Create Resource** → **Web App**
3. **Configurar:**
   ```
   App name: taskflow-tuusuario
   Runtime: .NET 8 (LTS)
   Operating System: Windows
   Pricing: Free F1
   ```

### **Paso 2: Configurar GitHub Secrets**
1. **Azure Portal** → Tu App → **Get publish profile**
2. **GitHub repo** → Settings → Secrets → Actions
3. **Agregar secrets:**
   ```
   AZURE_WEBAPP_NAME = taskflow-tuusuario
   AZURE_WEBAPP_PUBLISH_PROFILE = <contenido del archivo descargado>
   ```

### **Paso 3: Deploy automático**
- **Push a main** → Deploy automático
- **Ver logs** en GitHub Actions tab
- **App live** en ~3-5 minutos

---

## 🔧 **Configuración incluida**

### **✅ Ya configurado en el proyecto:**
- **SQLite** para base de datos (no requiere setup)
- **CORS** para dominios Azure
- **Health check** en `/health`
- **Logging** optimizado para producción
- **Compresión** habilitada
- **Security headers** configurados

### **✅ Datos de prueba incluidos:**
- **Usuario:** Admin Cruzado (admin@cruzado.com)
- **Proyecto:** Alejandro Cruzado Project
- **Interfaz** completamente en español

---

## 🌐 **URLs después del deploy**

- **App principal:** `https://taskflow-tuusuario.azurewebsites.net`
- **API Swagger:** `https://taskflow-tuusuario.azurewebsites.net/swagger`
- **Health check:** `https://taskflow-tuusuario.azurewebsites.net/health`

---

## 🔍 **Verificación post-deploy**

### **✅ Checklist:**
- [ ] App carga correctamente
- [ ] Navegación funciona (Inicio, Tareas, Proyectos)
- [ ] Se puede crear una tarea nueva
- [ ] Datos en español visibles
- [ ] Health check responde OK
- [ ] HTTPS funciona (candado verde)

### **🔧 Si algo falla:**
1. **Ver logs:** Azure Portal → App Service → Log stream
2. **Restart app:** Azure Portal → Overview → Restart
3. **Check GitHub Actions:** Repo → Actions tab

---

## 💰 **Costos y límites**

### **🆓 Azure Free Tier (F1):**
- **Costo:** $0 USD/mes
- **Compute:** 60 minutos/día
- **Storage:** 1 GB
- **Bandwidth:** 165 MB/día
- **Custom domains:** ✅ Sí
- **SSL:** ✅ Gratis

### **📊 Suficiente para:**
- ✅ Desarrollo y testing
- ✅ Demos a clientes
- ✅ Portfolio personal
- ✅ Proyectos pequeños

---

## 🚀 **Deploy en 5 minutos**

```bash
1. Abrir Visual Studio
2. Right-click TaskFlow.Server → Publish
3. Azure → App Service → Create New
4. Free F1 → Create → Publish
5. ¡Listo! App live en Azure
```

**🎉 ¡Tu TaskFlow estará online y accesible desde cualquier lugar!**
