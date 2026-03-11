================================================================================
SCRIPT BÁSICO DE CREACIÓN DE BASES DE DATOS
Sistema de Citas Médicas - Microservicios
================================================================================

Proyecto: CitasMedicas
Ubicación: C:\Proyectos\CitasMedicas\scripts\database\
SQL Server: 2016 o superior

NOTA IMPORTANTE:
Este script crea solo las bases de datos vacías.
Las tablas se pueden crear de dos formas:

OPCIÓN 1: Usar Migraciones de Entity Framework (RECOMENDADO)
   - Desde Package Manager Console en Visual Studio
   - PM> Update-Database -Project Personas.Infrastructure -StartupProject Personas.API
   - PM> Update-Database -Project Citas.Infrastructure -StartupProject Citas.API
   - PM> Update-Database -Project Recetas.Infrastructure -StartupProject Recetas.API

OPCIÓN 2: Ejecutar scripts SQL manualmente (ver archivo completo)
   - Ejecutar scripts de creación de tablas
   - Ejecutar scripts de índices
   - Ejecutar scripts de datos de prueba

================================================================================

USE master;
GO

-- =============================================
-- 1. CREAR BASE DE DATOS: PersonasDB
-- =============================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PersonasDB')
BEGIN
    CREATE DATABASE PersonasDB;
    PRINT 'Base de datos PersonasDB creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Base de datos PersonasDB ya existe.';
END
GO

-- =============================================
-- 2. CREAR BASE DE DATOS: CitasDB
-- =============================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CitasDB')
BEGIN
    CREATE DATABASE CitasDB;
    PRINT 'Base de datos CitasDB creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Base de datos CitasDB ya existe.';
END
GO

-- =============================================
-- 3. CREAR BASE DE DATOS: RecetasDB
-- =============================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'RecetasDB')
BEGIN
    CREATE DATABASE RecetasDB;
    PRINT 'Base de datos RecetasDB creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Base de datos RecetasDB ya existe.';
END
GO

-- =============================================
-- 4. CONFIGURAR OPCIONES DE BASES DE DATOS
-- =============================================

ALTER DATABASE PersonasDB SET RECOVERY SIMPLE;
ALTER DATABASE CitasDB SET RECOVERY SIMPLE;
ALTER DATABASE RecetasDB SET RECOVERY SIMPLE;
GO

PRINT '';
PRINT '==================================================';
PRINT 'BASES DE DATOS CREADAS EXITOSAMENTE';
PRINT '==================================================';
PRINT 'PersonasDB - Para gestión de médicos y pacientes';
PRINT 'CitasDB    - Para gestión de citas médicas';
PRINT 'RecetasDB  - Para gestión de recetas';
PRINT '';
PRINT 'PRÓXIMOS PASOS:';
PRINT '1. Configurar connection strings en los archivos Web.config';
PRINT '2. Ejecutar migraciones de Entity Framework:';
PRINT '   PM> Update-Database -Project Personas.Infrastructure';
PRINT '   PM> Update-Database -Project Citas.Infrastructure';
PRINT '   PM> Update-Database -Project Recetas.Infrastructure';
PRINT '';
PRINT 'O ejecutar scripts SQL manualmente para crear tablas.';
PRINT '==================================================';
GO

-- =============================================
-- VERIFICAR CREACIÓN
-- =============================================

SELECT 
    name AS [Base de Datos],
    create_date AS [Fecha Creación],
    recovery_model_desc AS [Modelo Recuperación]
FROM sys.databases
WHERE name IN ('PersonasDB', 'CitasDB', 'RecetasDB')
ORDER BY name;
GO

================================================================================
FIN DEL SCRIPT
================================================================================

INSTRUCCIONES DE USO:

1. EJECUTAR ESTE SCRIPT:
   - Abrir SQL Server Management Studio (SSMS)
   - Conectar al servidor SQL Server
   - Abrir este archivo .sql
   - Presionar F5 para ejecutar
   - Verificar mensajes de éxito

2. CONFIGURAR CONNECTION STRINGS:
   Editar Web.config de cada microservicio:
   
   📁 C:\Proyectos\CitasMedicas\src\Personas\Personas.API\Web.config
   <connectionStrings>
     <add name="PersonasDB" 
          connectionString="Server=localhost;Database=PersonasDB;Integrated Security=true;" 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   
   📁 C:\Proyectos\CitasMedicas\src\Citas\Citas.API\Web.config
   <connectionStrings>
     <add name="CitasDB" 
          connectionString="Server=localhost;Database=CitasDB;Integrated Security=true;" 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   
   📁 C:\Proyectos\CitasMedicas\src\Recetas\Recetas.API\Web.config
   <connectionStrings>
     <add name="RecetasDB" 
          connectionString="Server=localhost;Database=RecetasDB;Integrated Security=true;" 
          providerName="System.Data.SqlClient" />
   </connectionStrings>

3. CREAR TABLAS CON ENTITY FRAMEWORK (RECOMENDADO):
   Abrir Package Manager Console en Visual Studio:
   
   PM> Update-Database -Project Personas.Infrastructure -StartupProject Personas.API
   PM> Update-Database -Project Citas.Infrastructure -StartupProject Citas.API
   PM> Update-Database -Project Recetas.Infrastructure -StartupProject Recetas.API
   
   Esto creará automáticamente todas las tablas basándose en las entidades del dominio.

4. ALTERNATIVA - CREAR TABLAS MANUALMENTE:
   Si prefieres SQL directo en lugar de migraciones EF, solicita el script completo
   que incluye:
   - Creación de tablas
   - Índices
   - Stored Procedures
   - Datos de prueba

NOTAS:
- Recovery Model SIMPLE: No requiere backups de log de transacciones
- Integrated Security: Usa autenticación de Windows
- Si usas SQL Server Authentication, cambiar connection string:
  "Server=localhost;Database=PersonasDB;User Id=sa;Password=TuPassword;"

================================================================================