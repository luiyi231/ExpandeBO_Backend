# Guía de Instalación - ExpandeBO Backend

## ✅ Lo que SÍ necesitas instalar manualmente:

### 1. .NET 8.0 SDK (OBLIGATORIO)
- **Descarga**: https://dotnet.microsoft.com/download/dotnet/8.0
- **Verifica instalación**: 
  ```bash
  dotnet --version
  ```
  Debe mostrar: `8.0.x` o superior

### 2. MongoDB (OBLIGATORIO - elige una opción)

#### Opción A: MongoDB Local (Recomendado para desarrollo)
- **Windows**: https://www.mongodb.com/try/download/community
- **Instalación**: Descarga e instala MongoDB Community Server
- **Inicia el servicio**: MongoDB se ejecuta automáticamente como servicio en Windows
- **Verifica**: Debe estar corriendo en `mongodb://localhost:27017`

#### Opción B: MongoDB Atlas (Cloud - Gratis)
- **Registro**: https://www.mongodb.com/cloud/atlas/register
- **Crea un cluster gratuito** (M0 - Free tier)
- **Obtén la cadena de conexión** y actualiza `appsettings.json`:
  ```json
  "MongoDB": {
    "ConnectionString": "mongodb+srv://usuario:password@cluster.mongodb.net",
    "DatabaseName": "ExpandeBO"
  }
  ```

## ✅ Lo que se instala AUTOMÁTICAMENTE (no necesitas hacer nada):

Cuando ejecutes `dotnet restore`, se descargarán automáticamente estos paquetes NuGet:

1. **Microsoft.AspNetCore.Authentication.JwtBearer** (v8.0.0) - Autenticación JWT
2. **MongoDB.Driver** (v2.22.0) - Driver de MongoDB
3. **BCrypt.Net-Next** (v4.0.3) - Hash de contraseñas
4. **Swashbuckle.AspNetCore** (v6.5.0) - Swagger/OpenAPI

## 📋 Pasos de Instalación Completos:

### Paso 1: Instalar .NET 8.0 SDK
```bash
# Verifica si ya lo tienes
dotnet --version

# Si no lo tienes, descárgalo de:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### Paso 2: Instalar MongoDB (elige una opción)

**Opción A - Local:**
1. Descarga MongoDB Community Server
2. Instálalo (sigue el asistente)
3. El servicio se inicia automáticamente

**Opción B - Atlas (Cloud):**
1. Crea cuenta en MongoDB Atlas
2. Crea un cluster gratuito
3. Obtén la cadena de conexión
4. Actualiza `appsettings.json` con la cadena de conexión

### Paso 3: Restaurar paquetes NuGet (AUTOMÁTICO)
```bash
cd ExpandeBO_Backend
dotnet restore
```
Esto descargará automáticamente todos los paquetes necesarios.

### Paso 4: Configurar appsettings.json
Edita `appsettings.json` y configura:
- **MongoDB ConnectionString**: Tu cadena de conexión
- **JWT SecretKey**: Cambia por una clave segura (mínimo 32 caracteres)

### Paso 5: Ejecutar el proyecto
```bash
dotnet run
```

## 🔍 Verificación:

### Verificar .NET:
```bash
dotnet --version
# Debe mostrar: 8.0.x
```

### Verificar MongoDB Local:
```bash
# En Windows, verifica que el servicio esté corriendo:
# Servicios de Windows > MongoDB Server
# O intenta conectarte con MongoDB Compass: mongodb://localhost:27017
```

### Verificar que los paquetes se descargaron:
```bash
dotnet list package
# Debe mostrar los 4 paquetes instalados
```

## ⚠️ Problemas Comunes:

### Error: "No se puede encontrar .NET SDK"
- **Solución**: Instala .NET 8.0 SDK desde el sitio oficial

### Error: "No se puede conectar a MongoDB"
- **Solución Local**: Asegúrate de que el servicio MongoDB esté corriendo
- **Solución Atlas**: Verifica la cadena de conexión y el whitelist de IPs

### Error: "dotnet restore falla"
- **Solución**: Verifica tu conexión a internet (necesita descargar paquetes)
- **Solución alternativa**: `dotnet restore --verbosity detailed` para ver el error específico

## 📝 Resumen:

**Instalar manualmente:**
- ✅ .NET 8.0 SDK
- ✅ MongoDB (local o Atlas)

**Se instala automáticamente:**
- ✅ Todos los paquetes NuGet (con `dotnet restore`)
- ✅ Dependencias del proyecto

**No necesitas instalar:**
- ❌ Visual Studio (puedes usar VS Code, Rider, o cualquier editor)
- ❌ Extensiones especiales
- ❌ Herramientas adicionales

## 🚀 Comando Rápido (después de instalar .NET y MongoDB):

```bash
cd ExpandeBO_Backend
dotnet restore    # Descarga paquetes automáticamente
dotnet run         # Ejecuta el proyecto
```

¡Eso es todo! Los paquetes NuGet se gestionan automáticamente con .NET.


