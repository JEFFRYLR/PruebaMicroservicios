-- Script de Creación de Base de Datos para Microservicio de Personas
-- SQL Server 2016+

USE master;
GO

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PersonasDb')
BEGIN
    CREATE DATABASE PersonasDb;
    PRINT 'Base de datos PersonasDb creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La base de datos PersonasDb ya existe';
END
GO

USE PersonasDb;
GO

-- Tabla de Personas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Personas')
BEGIN
    CREATE TABLE Personas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Apellido NVARCHAR(100) NOT NULL,
        TipoPersona INT NOT NULL, -- 1 = Paciente, 2 = Médico
        Documento_TipoDocumento INT NOT NULL, -- 1 = Cedula, 2 = Pasaporte, 3 = TarjetaIdentidad
        Documento_Numero NVARCHAR(50) NOT NULL,
        CONSTRAINT CK_TipoPersona CHECK (TipoPersona IN (1, 2)),
        CONSTRAINT CK_TipoDocumento CHECK (Documento_TipoDocumento IN (1, 2, 3))
    );

    PRINT 'Tabla Personas creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla Personas ya existe';
END
GO

-- Índices para mejorar el rendimiento
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Personas_TipoPersona')
BEGIN
    CREATE INDEX IX_Personas_TipoPersona ON Personas(TipoPersona);
    PRINT 'Índice IX_Personas_TipoPersona creado exitosamente';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Personas_Documento')
BEGIN
    CREATE UNIQUE INDEX IX_Personas_Documento ON Personas(Documento_TipoDocumento, Documento_Numero);
    PRINT 'Índice IX_Personas_Documento creado exitosamente';
END
GO

-- Datos de prueba
PRINT 'Insertando datos de prueba...';

-- Pacientes de prueba
IF NOT EXISTS (SELECT * FROM Personas WHERE Documento_Numero = '1234567890')
BEGIN
    INSERT INTO Personas (Nombre, Apellido, TipoPersona, Documento_TipoDocumento, Documento_Numero)
    VALUES 
        ('Juan', 'Pérez', 1, 1, '1234567890'),
        ('María', 'González', 1, 1, '0987654321'),
        ('Carlos', 'Rodríguez', 1, 2, 'AB123456'),
        ('Ana', 'Martínez', 1, 1, '1122334455');
    
    PRINT 'Pacientes de prueba insertados';
END
GO

-- Médicos de prueba
IF NOT EXISTS (SELECT * FROM Personas WHERE Documento_Numero = 'M001234567')
BEGIN
    INSERT INTO Personas (Nombre, Apellido, TipoPersona, Documento_TipoDocumento, Documento_Numero)
    VALUES 
        ('Dr. Roberto', 'Sánchez', 2, 1, 'M001234567'),
        ('Dra. Laura', 'Fernández', 2, 1, 'M009876543'),
        ('Dr. Miguel', 'Torres', 2, 2, 'MP234567'),
        ('Dra. Carmen', 'López', 2, 1, 'M005544332');
    
    PRINT 'Médicos de prueba insertados';
END
GO

-- Verificar los datos insertados
PRINT 'Resumen de datos insertados:';
SELECT 
    TipoPersona AS Tipo,
    CASE TipoPersona 
        WHEN 1 THEN 'Paciente'
        WHEN 2 THEN 'Médico'
    END AS Descripcion,
    COUNT(*) AS Cantidad
FROM Personas
GROUP BY TipoPersona;
GO

PRINT 'Script completado exitosamente';
