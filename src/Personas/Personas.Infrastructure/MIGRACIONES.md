# Guía de Migraciones de Entity Framework

## ✅ Configuración Completada

El DbContext ya está configurado correctamente para ejecutar migraciones:
- ✅ Cadena de conexión configurada en `App.config` y `Web.config`
- ✅ `Configuration.cs` creado en la carpeta `Migrations`
- ✅ DbContext configurado con mapeos completos

---

## 📋 Pasos para Ejecutar las Migraciones

### 1️⃣ Abrir la Consola del Administrador de Paquetes

En Visual Studio:
- **Herramientas** → **Administrador de paquetes NuGet** → **Consola del Administrador de paquetes**

### 2️⃣ Configurar el Proyecto Predeterminado

En la consola, selecciona en el dropdown:
```
Proyecto predeterminado: Personas.Infrastructure
```

### 3️⃣ Habilitar las Migraciones (Ya está hecho ✅)

```powershell
Enable-Migrations -ProjectName Personas.Infrastructure -ContextTypeName PersonasDbContext
```

> ⚠️ **Nota**: Esto ya se hizo al crear `Configuration.cs`, puedes omitir este paso.

### 4️⃣ Crear la Migración Inicial

```powershell
Add-Migration InitialCreate -ProjectName Personas.Infrastructure
```

Esto creará un archivo en `Personas.Infrastructure\Migrations\` con el código de migración.

### 5️⃣ Aplicar la Migración a la Base de Datos

**IMPORTANTE**: Asegúrate de que `Personas.API` sea el **proyecto de inicio** (en negrita en el Explorador de soluciones).

```powershell
Update-Database -ProjectName Personas.Infrastructure -Verbose
```

El flag `-Verbose` te mostrará el SQL que se está ejecutando.

---

## 🔍 Comandos Útiles

### Ver el SQL de la Migración (sin ejecutar)
```powershell
Update-Database -Script -ProjectName Personas.Infrastructure
```

### Revertir a una Migración Anterior
```powershell
Update-Database -TargetMigration:NombreDeLaMigracion -ProjectName Personas.Infrastructure
```

### Listar todas las Migraciones
```powershell
Get-Migrations -ProjectName Personas.Infrastructure
```

### Crear Migración con Datos Semilla
```powershell
Add-Migration SeedData -ProjectName Personas.Infrastructure
```

---

## ⚙️ Verificación de Configuración

### Verificar Cadena de Conexión

**En `Personas.Infrastructure\App.config`:**
```xml
<connectionStrings>
  <add name="PersonasDbConnection" 
       connectionString="Data Source=localhost\SQLEXPRESS;
                        Initial Catalog=PersonasDb;
                        User ID=admin;
                        Password=inetum;..." 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Verificar que SQL Server está corriendo

```powershell
# En PowerShell normal (no en la consola de paquetes)
Get-Service | Where-Object {$_.Name -like "*SQL*"}
```

O abre **SQL Server Management Studio** y conéctate a `localhost\SQLEXPRESS`.

---

## 🎯 Estructura Esperada de la Base de Datos

Después de ejecutar las migraciones, deberías tener:

```sql
PersonasDb
└── Tables
    └── dbo.Personas
        ├── Id (int, PK, Identity)
        ├── Nombre (nvarchar(100), NOT NULL)
        ├── Apellido (nvarchar(100), NOT NULL)
        ├── TipoPersona (int, NOT NULL)
        ├── Documento_TipoDocumento (int, NOT NULL)
        └── Documento_Numero (nvarchar(50), NOT NULL)
```

---

## ⚠️ Solución de Problemas

### Error: "No connection string named 'PersonasDbConnection' could be found"
**Solución**: Verifica que `App.config` en `Personas.Infrastructure` tenga la cadena de conexión.

### Error: "Login failed for user 'admin'"
**Solución**: 
1. Verifica usuario y contraseña en SQL Server
2. Asegúrate de que el usuario tenga permisos en la base de datos

### Error: "Could not load file or assembly 'EntityFramework'"
**Solución**: 
```powershell
Install-Package EntityFramework -Version 6.4.4 -ProjectName Personas.Infrastructure
```

### Error: "A network-related or instance-specific error"
**Solución**:
1. Verifica que SQL Server esté corriendo
2. Verifica el nombre de la instancia: `localhost\SQLEXPRESS`

---

## 🚀 Comandos Completos (Copiar y Pegar)

Ejecuta estos comandos en orden:

```powershell
# 1. Crear la migración inicial
Add-Migration InitialCreate -ProjectName Personas.Infrastructure

# 2. Aplicar la migración
Update-Database -ProjectName Personas.Infrastructure -Verbose

# 3. Verificar (opcional)
Get-Migrations -ProjectName Personas.Infrastructure
```

---

## ✅ Siguiente Paso

Una vez completadas las migraciones, puedes:
1. **Ejecutar el proyecto** (F5)
2. **Probar los endpoints** en `http://localhost:[puerto]/api/personas`
3. **Insertar datos de prueba** usando POST requests

---

## 📝 Notas Adicionales

- Las migraciones se versionan automáticamente
- Cada migración tiene un timestamp en el nombre
- Puedes hacer rollback a versiones anteriores
- El código de migración se puede editar antes de ejecutar `Update-Database`

---

¡Todo listo para crear la base de datos! 🎉
