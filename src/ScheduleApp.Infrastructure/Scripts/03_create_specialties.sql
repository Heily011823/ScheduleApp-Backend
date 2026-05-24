-- ==========================================================================
-- SCRIPT: 03_create_specialties.sql
-- OBJETIVO: Crear tabla Specialties y cargar especialidades base
-- FECHA: 2026-05-24
-- AUTOR: Mateo Quintero
-- ==========================================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ==========================================
-- 1. CREAR TABLA SPECIALTIES (si no existe)
-- ==========================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Specialties')
BEGIN
    CREATE TABLE [Specialties] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Icon] nvarchar(50) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Specialties] PRIMARY KEY ([Id])
    );
    
    CREATE UNIQUE INDEX [IX_Specialties_Name] ON [Specialties] ([Name]);
    
    PRINT '✅ Tabla Specialties creada exitosamente';
END
ELSE
    PRINT '⚠️ La tabla Specialties ya existe';
GO

-- ==========================================
-- 2. CARGAR ESPECIALIDADES BASE
-- ==========================================

-- Eliminar especialidades anteriores si las hay (opcional, para limpiar)
-- DELETE FROM Specialties;
-- DBCC CHECKIDENT ('Specialties', RESEED, 0);

-- Insertar especialidades solo si no existen
INSERT INTO Specialties (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
SELECT Id, Name, Description, IsActive, DisplayOrder, CreatedAt FROM (VALUES
    (NEWID(), 'Lenguas Extranjeras', 'Idiomas y lingüística', 1, 1, GETUTCDATE()),
    (NEWID(), 'Matemáticas', 'Matemáticas puras y aplicadas', 1, 2, GETUTCDATE()),
    (NEWID(), 'Humanísticas', 'Ciencias humanas y sociales', 1, 3, GETUTCDATE()),
    (NEWID(), 'Física', 'Física teórica y experimental', 1, 4, GETUTCDATE()),
    (NEWID(), 'Ética', 'Ética y moral', 1, 5, GETUTCDATE()),
    (NEWID(), 'Arquitectura de Software', 'Diseño de software', 1, 6, GETUTCDATE()),
    (NEWID(), 'Electrónica Digital', 'Circuitos digitales y sistemas embebidos', 1, 7, GETUTCDATE()),
    (NEWID(), 'Ingeniería de Software', 'Desarrollo de software a gran escala', 1, 8, GETUTCDATE()),
    (NEWID(), 'Backend', 'Desarrollo del lado del servidor', 1, 9, GETUTCDATE()),
    (NEWID(), 'Frontend', 'Desarrollo del lado del cliente', 1, 10, GETUTCDATE()),
    (NEWID(), 'Ingeniería de Datos', 'Procesamiento y análisis de datos', 1, 11, GETUTCDATE()),
    (NEWID(), 'Programación', 'Lógica, algoritmos y fundamentos de programación', 1, 12, GETUTCDATE()),
    (NEWID(), 'Bases de Datos', 'Diseño y administración de bases de datos', 1, 13, GETUTCDATE()),
    (NEWID(), 'Redes y Sistemas', 'Redes, sistemas operativos y sistemas embebidos', 1, 14, GETUTCDATE()),
    (NEWID(), 'Teoría de Sistemas', 'Fundamentos teóricos de sistemas', 1, 15, GETUTCDATE())
) AS SpecialtiesData(Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM Specialties WHERE Name = SpecialtiesData.Name);

PRINT '✅ Especialidades base cargadas exitosamente';

-- ==========================================
-- 3. AGREGAR COLUMNA SPECIALTYID A TEACHERS (si no existe)
-- ==========================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Teachers') AND name = 'SpecialtyId')
BEGIN
    ALTER TABLE Teachers ADD SpecialtyId uniqueidentifier NULL;
    
    ALTER TABLE Teachers ADD CONSTRAINT FK_Teachers_Specialties_SpecialtyId 
        FOREIGN KEY (SpecialtyId) REFERENCES Specialties(Id) ON DELETE SET NULL;
    
    CREATE INDEX IX_Teachers_SpecialtyId ON Teachers(SpecialtyId);
    
    PRINT '✅ Columna SpecialtyId agregada a Teachers';
END
ELSE
    PRINT '⚠️ La columna SpecialtyId ya existe en Teachers';

-- ==========================================
-- 4. AGREGAR COLUMNA SPECIALTYID A SUBJECTS (si no existe)
-- ==========================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Subjects') AND name = 'SpecialtyId')
BEGIN
    ALTER TABLE Subjects ADD SpecialtyId uniqueidentifier NULL;
    
    ALTER TABLE Subjects ADD CONSTRAINT FK_Subjects_Specialties_SpecialtyId 
        FOREIGN KEY (SpecialtyId) REFERENCES Specialties(Id) ON DELETE SET NULL;
    
    CREATE INDEX IX_Subjects_SpecialtyId ON Subjects(SpecialtyId);
    
    PRINT '✅ Columna SpecialtyId agregada a Subjects';
END
ELSE
    PRINT '⚠️ La columna SpecialtyId ya existe en Subjects';

-- ==========================================
-- 5. VERIFICAR QUE TODO ESTÁ CORRECTO
-- ==========================================
SELECT 
    'Specialties' AS Tabla,
    COUNT(*) AS CantidadRegistros
FROM Specialties

UNION ALL

SELECT 
    'Teachers con SpecialtyId' AS Tabla,
    COUNT(*) AS CantidadRegistros
FROM Teachers
WHERE SpecialtyId IS NOT NULL

UNION ALL

SELECT 
    'Subjects con SpecialtyId' AS Tabla,
    COUNT(*) AS CantidadRegistros
FROM Subjects
WHERE SpecialtyId IS NOT NULL;

COMMIT TRANSACTION;

PRINT '✅ Script 03_create_specialties.sql ejecutado correctamente';