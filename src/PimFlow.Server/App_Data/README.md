# 📊 Base de Datos de Ejemplo - PimFlow

## 🎯 **Propósito**
Esta base de datos SQLite (`application-dev.db`) contiene **datos de ejemplo** para facilitar el desarrollo y las demostraciones del sistema PimFlow.

## 📦 **Contenido de la Base de Datos**

### **🏷️ Categorías (1)**
- **Calzado** (ID: 1) - Categoría principal para todos los productos

### **🎨 Atributos Personalizados (6)**
1. **Color** - Tipo: Text
2. **Material** - Tipo: Text  
3. **Temporada** - Tipo: Text
4. **Género** - Tipo: Text
5. **Resistencia al Agua** - Tipo: Text
6. **Tipo de Suela** - Tipo: Text

### **👟 Artículos de Ejemplo (15)**

#### **Nike (2 productos)**
- **Nike Air Max 270** - Azul/Blanco, Mesh sintético, Unisex
- **Nike Air Force 1 Low** - Rosa/Blanco, Cuero sintético, Femenino

#### **Adidas (2 productos)**
- **Adidas Ultraboost 22** - Negro/Rojo, Primeknit, Masculino, Resistente al agua
- **Adidas Stan Smith** - Verde/Blanco, Cuero genuino, Unisex

#### **Puma (2 productos)**
- **Puma RS-X3** - Blanco/Azul, Cuero sintético, Femenino
- **Puma Suede Classic** - Azul/Blanco, Gamuza, Masculino

#### **Otras Marcas (9 productos)**
- **Reebok Classic Leather** - Blanco, Cuero genuino, Unisex
- **Under Armour HOVR Phantom** - Negro/Rojo, Knit engineered, Masculino, Resistente al agua
- **New Balance 990v5** - Gris, Mesh y gamuza, Unisex
- **Vans Old Skool** - Negro/Blanco, Canvas y gamuza, Unisex
- **Converse Chuck Taylor All Star** - Rojo, Canvas, Unisex
- **Asics Gel-Kayano 29** - Amarillo/Negro, Mesh técnico, Femenino, Resistente al agua
- **Skechers D'Lites** - Blanco/Rosa, Cuero sintético, Femenino

## 🔍 **Casos de Uso para Pruebas**

### **Búsqueda por Marca**
- Nike: 2 resultados
- Adidas: 2 resultados  
- Puma: 2 resultados
- Otras marcas: 1 resultado cada una

### **Filtros por Atributos**
- **Color Azul**: 3 productos (Nike Air Max, Puma RS-X3, Puma Suede)
- **Género Femenino**: 4 productos
- **Género Masculino**: 3 productos
- **Resistentes al agua**: 3 productos
- **Material Cuero**: 4 productos

### **Búsqueda General**
- "Nike" → 2 resultados
- "Azul" → 3 resultados
- "Running" → 2 resultados
- "Classic" → 3 resultados

## 🚀 **Uso**

### **Para Desarrollo**
1. La base de datos se carga automáticamente al iniciar el servidor
2. No necesitas ejecutar migraciones adicionales
3. Los datos están listos para usar inmediatamente

### **Para Demos**
- Perfecto para mostrar todas las funcionalidades del sistema
- Incluye variedad de marcas, colores y atributos
- Permite probar búsquedas avanzadas y filtros

### **Para Testing**
- Base de datos consistente para pruebas
- Datos conocidos para validar resultados
- Cobertura de diferentes casos de uso

## ⚠️ **Notas Importantes**

1. **Solo para Desarrollo**: Esta base de datos es solo para entorno de desarrollo
2. **No usar en Producción**: En producción se debe usar una base de datos vacía
3. **Datos de Ejemplo**: Todos los productos son ficticios para propósitos de demostración
4. **Actualización**: Si necesitas datos frescos, puedes eliminar este archivo y ejecutar las migraciones

## 🔄 **Regenerar la Base de Datos**

Si necesitas regenerar la base de datos desde cero:

```bash
# 1. Eliminar la base de datos actual
rm src/PimFlow.Server/App_Data/application-dev.db*

# 2. Ejecutar migraciones
dotnet ef database update --project src/PimFlow.Server

# 3. Los datos de ejemplo se pueden recrear usando la API
```

---
**Creado**: Junio 2025  
**Versión**: 1.0  
**Artículos**: 15 productos de ejemplo  
**Marcas**: 10 marcas diferentes
