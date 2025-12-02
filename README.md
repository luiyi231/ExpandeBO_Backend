# ExpandeBO Backend

Backend del sistema ExpandeBO desarrollado con ASP.NET Core Web API y MongoDB.

## Descripción

Sistema de gestión de pedidos y comercio electrónico que permite:
- Gestión de empresas y perfiles comerciales
- Catálogo de productos con categorías y subcategorías
- Sistema de pedidos
- Chat entre clientes y empresas
- Gestión de planes y suscripciones

## Tecnologías

- **.NET 8.0**
- **ASP.NET Core Web API**
- **MongoDB**
- **JWT Authentication**
- **BCrypt** para hash de contraseñas

## Requisitos Previos

- .NET 8.0 SDK
- MongoDB (local o remoto)
- Visual Studio 2022 / VS Code / Rider

## Configuración

1. **Configurar MongoDB**

   Edita `appsettings.json` y configura la cadena de conexión:
   ```json
   "MongoDB": {
     "ConnectionString": "mongodb://localhost:27017",
     "DatabaseName": "ExpandeBO"
   }
   ```

2. **Configurar JWT**

   En `appsettings.json`, configura la clave secreta JWT (mínimo 32 caracteres):
   ```json
   "Jwt": {
     "SecretKey": "TuClaveSecretaSuperSeguraParaJWT_Minimo32Caracteres",
     "Issuer": "ExpandeBO",
     "Audience": "ExpandeBOUsers",
     "ExpirationMinutes": 1440
   }
   ```

3. **Configurar CORS**

   Agrega los orígenes permitidos en `appsettings.json`:
   ```json
   "Cors": {
     "AllowedOrigins": [
       "http://localhost:3000",
       "http://localhost:5173"
     ]
   }
   ```

## Instalación

1. Clona el repositorio
2. Restaura las dependencias:
   ```bash
   dotnet restore
   ```
3. Ejecuta el proyecto:
   ```bash
   dotnet run
   ```

El API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Estructura del Proyecto

```
ExpandeBO_Backend/
├── Controllers/          # Controladores API
├── Models/              # Modelos de datos y DTOs
├── Repositories/         # Repositorios para acceso a datos
├── Services/            # Lógica de negocio
├── Program.cs           # Configuración de la aplicación
└── appsettings.json     # Configuración
```

## Endpoints Principales

### Autenticación
- `POST /api/auth/registro` - Registro de usuario
- `POST /api/auth/registro-empresa` - Registro de empresa
- `POST /api/auth/login` - Inicio de sesión
- `GET /api/auth/me` - Información del usuario actual

### Perfiles Comerciales
- `GET /api/perfiles` - Listar perfiles activos
- `GET /api/perfiles/mis-perfiles` - Mis perfiles (empresa)
- `GET /api/perfiles/{id}` - Obtener perfil
- `POST /api/perfiles` - Crear perfil
- `PUT /api/perfiles/{id}` - Actualizar perfil

### Productos
- `GET /api/productos` - Listar productos
- `GET /api/productos/perfil/{idPerfil}` - Productos por perfil
- `GET /api/productos/categoria/{categoriaId}` - Productos por categoría
- `GET /api/productos/disponibles` - Productos disponibles
- `POST /api/productos` - Crear producto
- `PUT /api/productos/{id}` - Actualizar producto
- `DELETE /api/productos/{id}` - Eliminar producto

### Pedidos
- `POST /api/pedidos` - Crear pedido
- `GET /api/pedidos/mis-pedidos` - Mis pedidos
- `GET /api/pedidos/perfil/{idPerfil}` - Pedidos por perfil
- `GET /api/pedidos/{id}` - Obtener pedido
- `PUT /api/pedidos/{id}/estado` - Actualizar estado

### Categorías y Subcategorías
- `GET /api/categorias` - Listar categorías
- `POST /api/categorias` - Crear categoría (Admin)
- `GET /api/subcategorias` - Listar subcategorías
- `POST /api/subcategorias` - Crear subcategoría (Admin)

### Chat
- `POST /api/chat/crear-o-obtener` - Crear o obtener chat
- `POST /api/chat/{chatId}/mensaje` - Enviar mensaje
- `GET /api/chat/mis-chats` - Mis chats
- `GET /api/chat/{id}` - Obtener chat

## Roles del Sistema

1. **Cliente**: Usa la app móvil para ver productos, hacer pedidos y chatear
2. **Empresa**: Gestiona perfiles comerciales y productos desde la web
3. **Administrador**: Gestiona categorías, subcategorías y supervisa el sistema

## Autenticación

El sistema usa JWT (JSON Web Tokens) para autenticación. Incluye el token en el header:
```
Authorization: Bearer {token}
```

## Base de Datos

El sistema utiliza MongoDB con las siguientes colecciones principales:
- `usuarios`
- `empresas`
- `planes`
- `suscripciones_empresa`
- `ciudades`
- `perfiles_comerciales`
- `categorias`
- `subcategorias`
- `productos`
- `pedidos`
- `chats`

## Notas Importantes

- Las contraseñas se hashean con BCrypt antes de almacenarse
- Los endpoints protegidos requieren autenticación JWT
- Algunos endpoints requieren roles específicos (Admin, Empresa, etc.)
- El sistema valida permisos antes de permitir operaciones

## Desarrollo

Para desarrollo, asegúrate de:
1. Tener MongoDB corriendo
2. Configurar correctamente `appsettings.json`
3. Usar Swagger para probar los endpoints
4. Verificar los logs en la consola para debugging

## Licencia

Este proyecto es parte de un proyecto integrador académico.


