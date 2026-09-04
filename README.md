# RealEstateApp

## Descripción del Proyecto

RealEstateApp es una aplicación web completa para la gestión de propiedades inmobiliarias, desarrollada con .NET 8 utilizando arquitectura limpia (Onion Architecture). La aplicación permite a usuarios con diferentes roles (Administradores, Agentes, Clientes y Desarrolladores) gestionar propiedades, ofertas, favoritos y más.

### Arquitectura

La aplicación sigue el patrón de Arquitectura Cebolla (Onion Architecture) con las siguientes capas:

- **Core.Domain**: Contiene las entidades del dominio, enumeraciones e interfaces de repositorios.
- **Core.Application**: Contiene la lógica de negocio, DTOs, validaciones y servicios de aplicación.
- **Infrastructure.Data**: Maneja el acceso a datos con Entity Framework Core y SQL Server.
- **Infrastructure.Identity**: Gestiona la autenticación y autorización con ASP.NET Core Identity.
- **Infrastructure.Shared**: Servicios compartidos como email y archivos.
- **WebApi**: API RESTful con JWT para integración.
- **WebApp**: Aplicación web MVC para usuarios finales.

### Tecnologías Utilizadas

- **.NET 8**
- **ASP.NET Core MVC**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server (LocalDB para desarrollo)**
- **ASP.NET Core Identity**
- **JWT Authentication**
- **Swagger/OpenAPI**
- **MediatR** para CQRS
- **AutoMapper** para mapeos
- **FluentValidation** para validaciones

### Características Principales

- Gestión de propiedades inmobiliarias
- Sistema de roles y permisos
- Autenticación JWT para API
- Autenticación basada en cookies para WebApp
- Gestión de agentes y desarrolladores
- Sistema de ofertas y favoritos
- Catálogos de tipos de propiedades y ventas
- Mejoras de propiedades
- Envío de correos electrónicos
- Subida de imágenes

## Configuración y Ejecución

### Prerrequisitos

- .NET 8 SDK
- SQL Server (LocalDB incluido en Visual Studio)
- Visual Studio 2022 o superior

### Instalación

1. Clona el repositorio:
   ```bash
   git clone https://github.com/GeraldGG10/RealEstateApp.git
   cd RealEstateApp
   ```

2. Restaura los paquetes NuGet:
   ```bash
   dotnet restore
   ```

3. Ejecuta las migraciones para crear la base de datos:
   ```bash
   dotnet ef database update --project RealEstateApp.Infrastructure.Data
   dotnet ef database update --project RealEstateApp.Infrastructure.Identity
   ```

4. Ejecuta la aplicación:
   - Para la WebApp: `dotnet run --project RealEstateApp.WebApp`
   - Para la API: `dotnet run --project RealEstateApp.WebApi`

### Configuración de Base de Datos

La aplicación utiliza SQL Server LocalDB por defecto. Las cadenas de conexión se configuran en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RealEstateApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;",
  "ConexionPorDefecto": "Server=(localdb)\\mssqllocaldb;Database=RealEstateAppIdentity;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
}
```

### Configuración de Email

Para el envío de correos, configura las credenciales en `appsettings.json`:

```json
"ConfiguracionEmail": {
  "EmailDesde": "noreply@realestateapp.com",
  "SmtpServidor": "smtp.gmail.com",
  "SmtpPuerto": 587,
  "SmtpUsuario": "tu-email@gmail.com",
  "SmtpClave": "tu-clave"
}
```

## Credenciales de Usuarios de Prueba

La aplicación incluye datos de semilla con usuarios de prueba para cada rol. Las credenciales son las siguientes:

### Administradores
- **SuperAdmin**
  - Email: super.admin@realestateapp.com
  - Contraseña: Admin123!
- **Pedro Ramírez**
  - Email: pedro.admin@realestateapp.com
  - Contraseña: Admin123!

### Agentes
- **Albertson Terrero**
  - Email: albertson.agente@realestateapp.com
  - Contraseña: Agente123!
- **Gerald Gomera**
  - Email: gerald.agente@realestateapp.com
  - Contraseña: Agente123!
- **Jesica de la Rosa**
  - Email: jesica.agente@realestateapp.com
  - Contraseña: Agente123!
- **Jhoan Paulino** (Inactivo)
  - Email: jhoan.agente@realestateapp.com
  - Contraseña: Agente123!
- **Luis Castillo**
  - Email: luis.agente@realestateapp.com
  - Contraseña: Agente123!

### Clientes
- **Leonardo Taváres**
  - Email: leonardo.cliente@realestateapp.com
  - Contraseña: Cliente123!
- **Sofía Torres**
  - Email: sofia.cliente@realestateapp.com
  - Contraseña: Cliente123!
- **Miguel Fernández**
  - Email: miguel.cliente@realestateapp.com
  - Contraseña: Cliente123!
- **Laura Núñez** (Inactivo, no confirmado)
  - Email: laura.cliente@realestateapp.com
  - Contraseña: Cliente123!

### Desarrolladores
- **Juan López**
  - Email: juan.dev@realestateapp.com
  - Contraseña: Dev123!
- **Raily Santos** (Inactivo)
  - Email: raily.dev@realestateapp.com
  - Contraseña: Dev123!

## API Endpoints

La API está documentada con Swagger. Accede a `/swagger` cuando ejecutes la aplicación WebApi.

Ejemplos de endpoints principales:
- `GET /api/propiedades` - Lista propiedades
- `POST /api/propiedades` - Crear propiedad
- `GET /api/agentes` - Lista agentes
- `POST /api/cuenta/login` - Login

## Estructura del Proyecto

```
RealEstateApp/
├── RealEstateApp.Core.Domain/          # Entidades y lógica de dominio
├── RealEstateApp.Core.Application/     # Servicios de aplicación y DTOs
├── RealEstateApp.Infrastructure.Data/  # Acceso a datos
├── RealEstateApp.Infrastructure.Identity/ # Autenticación
├── RealEstateApp.Infrastructure.Shared/ # Servicios compartidos
├── RealEstateApp.WebApi/               # API REST
├── RealEstateApp.WebApp/               # Aplicación web MVC
└── RealEstateApp.Tests/                # Pruebas unitarias e integración
```

## 👥 Contribuidores

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/AlbertsonTL">
        <img src="https://avatars.githubusercontent.com/AlbertsonTL?s=200" width="100" height="100" alt="AlbertsonTL">
        <br>
        <sub><b>AlbertsonTL</b></sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/GeraldGG10">
        <img src="https://avatars.githubusercontent.com/GeraldGG10?s=200" width="100" height="100" alt="GeraldGG10">
        <br>
        <sub><b>GeraldGG10</b></sub>
      </a>
    </td>
  </tr>
</table>

## Licencia

Este proyecto está bajo la Licencia MIT.
