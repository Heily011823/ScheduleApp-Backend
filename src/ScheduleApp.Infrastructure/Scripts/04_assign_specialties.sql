-- ==========================================================================
-- SCRIPT: 04_assign_specialties.sql
-- OBJETIVO: Asignar especialidades a materias y docentes
-- FECHA: 2026-05-24
-- AUTOR: Mateo Quintero
-- DEPENDE DE: 03_create_specialties.sql (ejecutar primero)
-- ==========================================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

PRINT 'Iniciando asignación de especialidades...';
GO

-- ==========================================
-- 1. ASIGNAR ESPECIALIDADES A MATERIAS
-- ==========================================

-- Matemáticas
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Matemáticas')
WHERE Name IN ('Cálculo Diferencial', 'Matemáticas Discretas');

-- Humanísticas
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Humanísticas')
WHERE Name IN ('Competencias Comunicativas', 'Cultura Política', 'Ética');

-- Lenguas Extranjeras
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Lenguas Extranjeras')
WHERE Name = 'Inglés I';

-- Física y Electrónica
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Electrónica Digital')
WHERE Name IN ('Electrónica Digital y Arquitectura de Computadores', 'Sistemas Embebidos');

-- Programación
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Programación')
WHERE Name IN (
    'Lógica de Programación',
    'Fundamentos de POO',
    'Programación Orientada a Objetos',
    'Técnicas de Programación',
    'Diseño de Algoritmos',
    'Paradigmas de Lenguajes',
    'Procesadores de Lenguajes'
);

-- Backend
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Backend')
WHERE Name IN ('Programación Back End', 'Sistemas de Información y Organizaciones');

-- Frontend
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Frontend')
WHERE Name IN ('Programación Front End', 'Programación de Dispositivos Móviles');

-- Bases de Datos
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Bases de Datos')
WHERE Name IN ('Bases de Datos I', 'Ciencia de los Datos', 'Ingeniería de Datos');

-- Redes y Sistemas
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Redes y Sistemas')
WHERE Name IN ('Redes LAN', 'Sistemas Operativos');

-- Ingeniería de Software
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Ingeniería de Software')
WHERE Name IN ('Ingeniería de Software II', 'Modelamiento y Simulación');

-- Arquitectura de Software
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Arquitectura de Software')
WHERE Name = 'Arquitectura de Software';

-- Teoría de Sistemas
UPDATE Subjects 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Teoría de Sistemas')
WHERE Name = 'Teoría de Sistemas';

PRINT '✅ Especialidades asignadas a materias';
GO

-- ==========================================
-- 2. ASIGNAR ESPECIALIDADES A DOCENTES
-- ==========================================

-- Lenguas Extranjeras
UPDATE Teachers 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Lenguas Extranjeras')
WHERE FirstName = 'Adriana' AND LastName = 'Muñoz';

-- Matemáticas
UPDATE Teachers 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Matemáticas')
WHERE FirstName = 'Alejandra' AND LastName = 'López';

-- Backend
UPDATE Teachers 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Backend')
WHERE FirstName IN ('Harold', 'Ernesto', 'Simón');

-- Frontend
UPDATE Teachers 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Frontend')
WHERE FirstName IN ('Marcela', 'Sebastián');

-- Bases de Datos
UPDATE Teachers 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Bases de Datos')
WHERE FirstName = 'Mauricio' AND LastName = 'Mejía';

-- Programación
UPDATE Teachers 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Programación')
WHERE FirstName IN ('Beatriz', 'Santiago', 'Juan David');

-- Humanísticas
UPDATE Teachers 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Humanísticas')
WHERE FirstName IN ('Julián', 'Lina');

-- Ingeniería de Software
UPDATE Teachers 
SET SpecialtyId = (SELECT Id FROM Specialties WHERE Name = 'Ingeniería de Software')
WHERE FirstName = 'Marcela' AND LastName = 'Ríos';

PRINT '✅ Especialidades asignadas a docentes';
GO

-- ==========================================
-- 3. VERIFICAR RESULTADOS
-- ==========================================

-- Ver materias sin especialidad (estas pueden ser dadas por cualquier docente)
SELECT 
    'Materias sin especialidad' AS Tipo,
    Name AS Nombre
FROM Subjects
WHERE SpecialtyId IS NULL
ORDER BY Name;

-- Ver docentes sin especialidad (pueden dar materias sin requisito)
SELECT 
    'Docentes sin especialidad' AS Tipo,
    FirstName + ' ' + LastName AS Nombre
FROM Teachers
WHERE SpecialtyId IS NULL AND IsActive = 1
ORDER BY FirstName;

-- Ver docentes que pueden dar qué materias (resumen)
SELECT 
    t.FirstName + ' ' + t.LastName AS Docente,
    sp.Name AS EspecialidadDocente,
    COUNT(DISTINCT s.Id) AS MateriasQuePuedeDar
FROM Teachers t
INNER JOIN Specialties sp ON t.SpecialtyId = sp.Id
CROSS JOIN Subjects s
WHERE s.SpecialtyId IS NULL OR s.SpecialtyId = t.SpecialtyId
GROUP BY t.FirstName, t.LastName, sp.Name
ORDER BY t.FirstName;

PRINT '✅ Verificación completada';
GO

COMMIT TRANSACTION;

PRINT '✅ Script 04_assign_specialties.sql ejecutado correctamente';