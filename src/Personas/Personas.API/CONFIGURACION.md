# Configuración del Microservicio de Personas

## Estructura Implementada

Se ha creado una arquitectura DDD (Domain-Driven Design) completa para el microservicio de Personas con las siguientes capas:

### 1. **Personas.Domain** (Capa de Dominio)
- **Entidades**: `Persona`
- **Value Objects**: `Documento`
- **Enumeraciones**: `TipoPersona`, `TipoDocumento`
- **Interfaces de Repositorio**: `IPersonaRepository`

### 2. **Personas.Application** (Capa de Aplicación)
- **Servicios**: `PersonaService`
- **DTOs**: `PersonaDto`

### 3. **Personas.Infrastructure** (Capa de Infraestructura)
- **Repositorios**: `PersonaRepository`
- **DbContext**: `PersonasDbContext`
- **Configuraciones EF**: `DocumentoConfiguration`

### 4. **Personas.API** (Capa de Presentación)
- **Controladores**: `PersonasController`
- **Configuración DI**: `DependencyConfig`

## Pasos para Completar la Configuración

### Paso 1: Agregar Referencias de Proyecto

Necesitas agregar las referencias entre los proyectos. En Visual Studio:

1. **Clic derecho** en el proyecto `Personas.API`
2. Seleccionar **Agregar > Referencia del proyecto...**
3. Marcar los siguientes proyectos:
   - ☑ Personas.Domain
   - ☑ Personas.Application
   - ☑ Personas.Infrastructure

4. Clic en **Aceptar**

### Paso 2: Verificar Entity Framework

Asegúrate de que todos los proyectos tengan Entity Framework instalado:

```bash
# En la Consola del Administrador de Paquetes NuGet
Install-Package EntityFramework -Version 6.4.4 -Project Personas.Domain
Install-Package EntityFramework -Version 6.4.4 -Project Personas.Application  
Install-Package EntityFramework -Version 6.4.4 -Project Personas.Infrastructure
Install-Package EntityFramework -Version 6.4.4 -Project Personas.API
```

### Paso 3: Crear la Base de Datos

Ejecuta en la Consola del Administrador de Paquetes NuGet:

```bash
# Asegúrate de que el proyecto Personas.API esté como proyecto de inicio
# Y que Personas.Infrastructure sea el proyecto predeterminado en la consola
Enable-Migrations -ProjectName Personas.Infrastructure -ContextTypeName PersonasDbContext
Add-Migration InitialCreate -ProjectName Personas.Infrastructure
Update-Database -ProjectName Personas.Infrastructure
```

### Paso 4: Verificar la Cadena de Conexión

En el archivo `Web.config` del proyecto `Personas.API`, verifica la cadena de conexión:

```xml
<connectionStrings>
  <add name="PersonasDbConnection" 
       connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonasDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Ajusta según tu configuración de SQL Server.

## Endpoints Implementados

Una vez configurado, el controlador `PersonasController` expondrá los siguientes endpoints:

### 1. **Obtener todas las personas**
```http
GET /api/personas
```

### 2. **Obtener persona por ID**
```http
GET /api/personas/{id}
```

### 3. **Obtener médicos**
```http
GET /api/personas/medicos
```

### 4. **Obtener pacientes**
```http
GET /api/personas/pacientes
```

### 5. **Crear una persona**
```http
POST /api/personas
Content-Type: application/json

{
  "nombre": "Juan",
  "apellido": "Pérez",
  "numeroDocumento": "1234567890",
  "tipoDocumento": 1,
  "tipoPersona": 2,
  "fechaNacimiento": "1990-01-01T00:00:00"
}
```

### 6. **Actualizar una persona**
```http
PUT /api/personas/{id}
Content-Type: application/json

{
  "nombre": "Juan",
  "apellido": "Pérez",
  "numeroDocumento": "1234567890",
  "tipoDocumento": 1,
  "tipoPersona": 2,
  "fechaNacimiento": "1990-01-01T00:00:00"
}
```

### 7. **Eliminar una persona**
```http
DELETE /api/personas/{id}
```

## Enumeraciones

### TipoPersona
- `Paciente = 1`
- `Medico = 2`

### TipoDocumento
- `Cedula = 1`
- `Pasaporte = 2`
- `TarjetaIdentidad = 3`

## Patrones Implementados

✅ **Repository Pattern**: `IPersonaRepository` y `PersonaRepository`
✅ **Dependency Injection**: Configurado en `DependencyConfig`
✅ **Domain-Driven Design**: Separación clara de responsabilidades
✅ **Value Objects**: `Documento` encapsula tipo y número
✅ **DTOs**: Separación entre modelo de dominio y API

## Próximos Pasos para Nivel 3

Para alcanzar los requisitos avanzados (Nivel 3), considera implementar:

1. **CQRS y Mediator**: Utiliza MediatR para separar comandos de consultas
2. **JWT Authentication**: Implementa autenticación basada en tokens
3. **Unit Tests**: Crea pruebas unitarias con cobertura del 35%
4. **Validaciones**: Agrega FluentValidation para validar DTOs
5. **Logging**: Implementa logging con Serilog o NLog
6. **Exception Handling**: Agrega un middleware de manejo de excepciones global

## Solución de Problemas

### Error: "El tipo o el nombre del espacio de nombres no existe"
- Verifica que hayas agregado las referencias de proyecto (Paso 1)
- Limpia y reconstruye la solución

### Error de conexión a la base de datos
- Verifica la cadena de conexión en Web.config
- Asegúrate de tener SQL Server LocalDB instalado
- Ejecuta `Update-Database` para crear la base de datos

### Los endpoints no responden
- Verifica que DependencyConfig esté registrado en Global.asax.cs
- Revisa las rutas en WebApiConfig.cs
