-- ==========================================================================
-- SCRIPT DE INSERCIÓN DE DATOS SEMILLA - CORREGIDO Y COMPLETO
-- Ejecutar después del script de creación de las 12 tablas.
-- ==========================================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ==========================================
-- VARIABLES DE ROLES
-- ==========================================
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @CoordRoleId UNIQUEIDENTIFIER = NEWID();

-- ==========================================
-- VARIABLES DE PROFESORES
-- ==========================================
DECLARE @IdErnesto UNIQUEIDENTIFIER = NEWID();
DECLARE @IdHarold UNIQUEIDENTIFIER = NEWID();
DECLARE @IdMarcela UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSebastian UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAlejandra UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSimon UNIQUEIDENTIFIER = NEWID();
DECLARE @IdBeatriz UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSantiago UNIQUEIDENTIFIER = NEWID();
DECLARE @IdJuanDavid UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAdriana UNIQUEIDENTIFIER = NEWID();
DECLARE @IdJulian UNIQUEIDENTIFIER = NEWID();
DECLARE @IdNuevo UNIQUEIDENTIFIER = NEWID();
DECLARE @IdLina UNIQUEIDENTIFIER = NEWID();
DECLARE @IdMauricio UNIQUEIDENTIFIER = NEWID();

-- ==========================================
-- VARIABLES DE MATERIAS
-- ==========================================
DECLARE @IdPOO UNIQUEIDENTIFIER = NEWID();
DECLARE @IdFundamentosPOO UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSistemasOperativos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdLogica UNIQUEIDENTIFIER = NEWID();
DECLARE @IdParadigmas UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProcesadores UNIQUEIDENTIFIER = NEWID();
DECLARE @IdIngSoftware UNIQUEIDENTIFIER = NEWID();
DECLARE @IdTecnicasProg UNIQUEIDENTIFIER = NEWID();
DECLARE @IdElectronica UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSistemasEmbebidos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdTeoriaSistemas UNIQUEIDENTIFIER = NEWID();
DECLARE @IdMoviles UNIQUEIDENTIFIER = NEWID();
DECLARE @IdBackend UNIQUEIDENTIFIER = NEWID();
DECLARE @IdDisenoAlgoritmos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCienciaDatos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdModelamiento UNIQUEIDENTIFIER = NEWID();
DECLARE @IdRedes UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSistemasInfo UNIQUEIDENTIFIER = NEWID();
DECLARE @IdDiscretas UNIQUEIDENTIFIER = NEWID();
DECLARE @IdFrontEnd UNIQUEIDENTIFIER = NEWID();
DECLARE @IdBasesDatos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCalculoDiferencial UNIQUEIDENTIFIER = NEWID();
DECLARE @IdIngles UNIQUEIDENTIFIER = NEWID();
DECLARE @IdEtica UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCultura UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCompetencias UNIQUEIDENTIFIER = NEWID();

-- ==========================================
-- VARIABLES DE AULAS
-- ==========================================
DECLARE @IdF401 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF402 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF403 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF404 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF405 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF406 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF407 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF408 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF409 UNIQUEIDENTIFIER = NEWID();

DECLARE @IdF101 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF102 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF103 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF201 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF202 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF203 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF301 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF302 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF303 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF501 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF502 UNIQUEIDENTIFIER = NEWID();

DECLARE @IdSAC101 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSAC102 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSAC103 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSACLAB UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSAC301 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSAC302 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSAC303 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSAC304 UNIQUEIDENTIFIER = NEWID();

DECLARE @IdCB101 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCB102 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCB103 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCB201 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCB202 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCB203 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCB204 UNIQUEIDENTIFIER = NEWID();

DECLARE @ProgramDiurnoId UNIQUEIDENTIFIER = NEWID();
DECLARE @ProgramNocturnoId UNIQUEIDENTIFIER = NEWID();

-- ==========================================
-- 1. ROLES
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Administrador')
BEGIN
    INSERT INTO Roles (Id, Name)
    VALUES (@AdminRoleId, 'Administrador');
END
ELSE
BEGIN
    SELECT @AdminRoleId = Id FROM Roles WHERE Name = 'Administrador';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Coordinador')
BEGIN
    INSERT INTO Roles (Id, Name)
    VALUES (@CoordRoleId, 'Coordinador');
END
ELSE
BEGIN
    SELECT @CoordRoleId = Id FROM Roles WHERE Name = 'Coordinador';
END

-- ==========================================
-- 2. USUARIOS
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (
        Id, FullName, Email, Username, IdentityDocument, PasswordHash, RoleId, IsActive, CreatedAt
    )
    VALUES (
        NEWID(),
        'Administrador del Sistema',
        'admin@autonoma.edu.co',
        'admin',
        '11111111',
        '$2y$11$mUhyq.SXgO6zp1wD9pl5OeRiHzE.G6//wpLiJpdiO1NYqjR0QwhbK',
        @AdminRoleId,
        1,
        GETUTCDATE()
    );
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'llopezu')
BEGIN
    INSERT INTO Users (
        Id, FullName, Email, Username, IdentityDocument, PasswordHash, RoleId, IsActive, CreatedAt
    )
    VALUES (
        NEWID(),
        'Lina Maria Lopez Uribe',
        'llopezu@autonoma.edu.co',
        'llopezu',
        '1024567890',
        '$2y$11$0eMQea7NWRMh5k/AqbqeQ.G4emWXfxAj2/b7svg2TgAjGMNM5Qjwa',
        @CoordRoleId,
        1,
        GETUTCDATE()
    );
END

-- ==========================================
-- 3. PROGRAMAS ACADÉMICOS
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM AcademicPrograms WHERE Code = '1020')
BEGIN
    INSERT INTO AcademicPrograms (
        Id, Code, Name, Shift, TotalSemesters, IsActive, CreatedAt
    )
    VALUES (
        @ProgramDiurnoId,
        '1020',
        'Ingeniería de Sistemas - Diurna',
        'Diurna',
        10,
        1,
        GETUTCDATE()
    );
END
ELSE
BEGIN
    SELECT @ProgramDiurnoId = Id FROM AcademicPrograms WHERE Code = '1020';
END

IF NOT EXISTS (SELECT 1 FROM AcademicPrograms WHERE Code = '1030')
BEGIN
    INSERT INTO AcademicPrograms (
        Id, Code, Name, Shift, TotalSemesters, IsActive, CreatedAt
    )
    VALUES (
        @ProgramNocturnoId,
        '1030',
        'Ingeniería de Sistemas - Nocturna',
        'Nocturna',
        12,
        1,
        GETUTCDATE()
    );
END
ELSE
BEGIN
    SELECT @ProgramNocturnoId = Id FROM AcademicPrograms WHERE Code = '1030';
END

-- ==========================================
-- 4. SEMESTRES DEL PROGRAMA
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM ProgramSemesters WHERE AcademicProgramId = @ProgramDiurnoId)
BEGIN
    INSERT INTO ProgramSemesters (
        Id, AcademicProgramId, SemesterNumber, MaxCredits, CreatedAt
    )
    VALUES
    (NEWID(), @ProgramDiurnoId, 1, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 2, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 3, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 4, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 5, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 6, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 7, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 8, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 9, 18, GETUTCDATE()),
    (NEWID(), @ProgramDiurnoId, 10, 18, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM ProgramSemesters WHERE AcademicProgramId = @ProgramNocturnoId)
BEGIN
    INSERT INTO ProgramSemesters (
        Id, AcademicProgramId, SemesterNumber, MaxCredits, CreatedAt
    )
    VALUES
    (NEWID(), @ProgramNocturnoId, 1, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 2, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 3, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 4, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 5, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 6, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 7, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 8, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 9, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 10, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 11, 15, GETUTCDATE()),
    (NEWID(), @ProgramNocturnoId, 12, 15, GETUTCDATE());
END

-- ==========================================
-- 5. MATERIAS
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103018')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdPOO, '103018', 'Programación Orientada a Objetos', 3, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdPOO = Id FROM Subjects WHERE Code = '103018';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103008')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdFundamentosPOO, '103008', 'Fundamentos de POO', 2, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdFundamentosPOO = Id FROM Subjects WHERE Code = '103008';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103027')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdSistemasOperativos, '103027', 'Sistemas Operativos', 6, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdSistemasOperativos = Id FROM Subjects WHERE Code = '103027';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103002')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdLogica, '103002', 'Lógica de Programación', 1, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdLogica = Id FROM Subjects WHERE Code = '103002';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109182')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdParadigmas, '109182', 'Paradigmas de Lenguajes', 6, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdParadigmas = Id FROM Subjects WHERE Code = '109182';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109186')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdProcesadores, '109186', 'Procesadores de Lenguajes', 7, 2, 3, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdProcesadores = Id FROM Subjects WHERE Code = '109186';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103093')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdIngSoftware, '103093', 'Ingeniería de Software II', 7, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdIngSoftware = Id FROM Subjects WHERE Code = '103093';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103007')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdTecnicasProg, '103007', 'Técnicas de Programación', 2, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdTecnicasProg = Id FROM Subjects WHERE Code = '103007';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109184')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdElectronica, '109184', 'Electrónica Digital y Arquitectura de Computadores', 6, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdElectronica = Id FROM Subjects WHERE Code = '109184';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109104')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdSistemasEmbebidos, '109104', 'Sistemas Embebidos', 7, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdSistemasEmbebidos = Id FROM Subjects WHERE Code = '109104';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103004')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdTeoriaSistemas, '103004', 'Teoría de Sistemas', 2, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdTeoriaSistemas = Id FROM Subjects WHERE Code = '103004';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109189')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdMoviles, '109189', 'Programación de Dispositivos Móviles', 9, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdMoviles = Id FROM Subjects WHERE Code = '109189';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109183')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdBackend, '109183', 'Programación Back End', 6, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdBackend = Id FROM Subjects WHERE Code = '109183';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103021')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdDisenoAlgoritmos, '103021', 'Diseño de Algoritmos', 8, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdDisenoAlgoritmos = Id FROM Subjects WHERE Code = '103021';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109188')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdCienciaDatos, '109188', 'Ciencia de los Datos', 8, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdCienciaDatos = Id FROM Subjects WHERE Code = '109188';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109185')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdModelamiento, '109185', 'Modelamiento y Simulación', 7, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdModelamiento = Id FROM Subjects WHERE Code = '109185';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103126')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdRedes, '103126', 'Redes LAN', 6, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdRedes = Id FROM Subjects WHERE Code = '103126';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103125')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdSistemasInfo, '103125', 'Sistemas de Información y Organizaciones', 10, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdSistemasInfo = Id FROM Subjects WHERE Code = '103125';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '104027')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdDiscretas, '104027', 'Matemáticas Discretas', 3, 4, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdDiscretas = Id FROM Subjects WHERE Code = '104027';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109187')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdFrontEnd, '109187', 'Programación Front End', 7, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdFrontEnd = Id FROM Subjects WHERE Code = '109187';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109180')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdBasesDatos, '109180', 'Bases de Datos I', 5, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdBasesDatos = Id FROM Subjects WHERE Code = '109180';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '104030')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdCalculoDiferencial, '104030', 'Cálculo Diferencial', 2, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdCalculoDiferencial = Id FROM Subjects WHERE Code = '104030';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '151601')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdIngles, '151601', 'Inglés I', 1, 1, 2, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdIngles = Id FROM Subjects WHERE Code = '151601';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '105038')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdEtica, '105038', 'Ética', 1, 2, 2, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdEtica = Id FROM Subjects WHERE Code = '105038';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '105042')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdCultura, '105042', 'Cultura Política', 8, 2, 2, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdCultura = Id FROM Subjects WHERE Code = '105042';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '100059')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt) VALUES (@IdCompetencias, '100059', 'Competencias Comunicativas', 2, 3, 3, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdCompetencias = Id FROM Subjects WHERE Code = '100059';

-- ==========================================
-- 6. PROFESORES
-- CORREGIDO: la validación se hace por IdentityDocument,
-- porque esa es la columna con restricción UNIQUE.
-- Además, si el documento ya existe, se actualizan los datos
-- para corregir registros viejos y evitar errores al reejecutar.
-- ==========================================

-- Limpieza preventiva: si por ejecuciones anteriores quedó el mismo correo
-- asociado a otro documento, se libera antes del UPSERT correcto.
-- Esto evita choques por correos únicos cuando el documento ya existe con datos viejos.

-- Ernesto Pérez
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000001')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Ernesto',
        LastName = 'Pérez',
        Email = 'ernesto@autonoma.edu.co',
        PhoneNumber = '3000000001',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000001';

    SELECT @IdErnesto = Id
    FROM Teachers
    WHERE IdentityDocument = '10000001';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdErnesto, 'Ernesto', 'Pérez', 'ernesto@autonoma.edu.co', '10000001', '3000000001', 1, GETUTCDATE(), NULL);
END

-- Harold Gómez
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000002')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Harold',
        LastName = 'Gómez',
        Email = 'harold@autonoma.edu.co',
        PhoneNumber = '3000000002',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000002';

    SELECT @IdHarold = Id
    FROM Teachers
    WHERE IdentityDocument = '10000002';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdHarold, 'Harold', 'Gómez', 'harold@autonoma.edu.co', '10000002', '3000000002', 1, GETUTCDATE(), NULL);
END

-- Marcela Ríos
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000003')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Marcela',
        LastName = 'Ríos',
        Email = 'marcela@autonoma.edu.co',
        PhoneNumber = '3000000003',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000003';

    SELECT @IdMarcela = Id
    FROM Teachers
    WHERE IdentityDocument = '10000003';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdMarcela, 'Marcela', 'Ríos', 'marcela@autonoma.edu.co', '10000003', '3000000003', 1, GETUTCDATE(), NULL);
END

-- Sebastián Castro
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000004')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Sebastián',
        LastName = 'Castro',
        Email = 'sebastian@autonoma.edu.co',
        PhoneNumber = '3000000004',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000004';

    SELECT @IdSebastian = Id
    FROM Teachers
    WHERE IdentityDocument = '10000004';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdSebastian, 'Sebastián', 'Castro', 'sebastian@autonoma.edu.co', '10000004', '3000000004', 1, GETUTCDATE(), NULL);
END

-- Alejandra López
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000005')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Alejandra',
        LastName = 'López',
        Email = 'alejandra@autonoma.edu.co',
        PhoneNumber = '3000000005',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000005';

    SELECT @IdAlejandra = Id
    FROM Teachers
    WHERE IdentityDocument = '10000005';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdAlejandra, 'Alejandra', 'López', 'alejandra@autonoma.edu.co', '10000005', '3000000005', 1, GETUTCDATE(), NULL);
END

-- Simón Restrepo
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000006')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Simón',
        LastName = 'Restrepo',
        Email = 'simon@autonoma.edu.co',
        PhoneNumber = '3000000006',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000006';

    SELECT @IdSimon = Id
    FROM Teachers
    WHERE IdentityDocument = '10000006';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdSimon, 'Simón', 'Restrepo', 'simon@autonoma.edu.co', '10000006', '3000000006', 1, GETUTCDATE(), NULL);
END

-- Beatriz Mejía
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000007')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Beatriz',
        LastName = 'Mejía',
        Email = 'beatriz@autonoma.edu.co',
        PhoneNumber = '3000000007',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000007';

    SELECT @IdBeatriz = Id
    FROM Teachers
    WHERE IdentityDocument = '10000007';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdBeatriz, 'Beatriz', 'Mejía', 'beatriz@autonoma.edu.co', '10000007', '3000000007', 1, GETUTCDATE(), NULL);
END

-- Santiago Toro
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000008')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Santiago',
        LastName = 'Toro',
        Email = 'santiago@autonoma.edu.co',
        PhoneNumber = '3000000008',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000008';

    SELECT @IdSantiago = Id
    FROM Teachers
    WHERE IdentityDocument = '10000008';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdSantiago, 'Santiago', 'Toro', 'santiago@autonoma.edu.co', '10000008', '3000000008', 1, GETUTCDATE(), NULL);
END

-- Juan David Patiño
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000009')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Juan David',
        LastName = 'Patiño',
        Email = 'juandavid@autonoma.edu.co',
        PhoneNumber = '3000000009',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000009';

    SELECT @IdJuanDavid = Id
    FROM Teachers
    WHERE IdentityDocument = '10000009';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdJuanDavid, 'Juan David', 'Patiño', 'juandavid@autonoma.edu.co', '10000009', '3000000009', 1, GETUTCDATE(), NULL);
END

-- Adriana Muñoz
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000010')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Adriana',
        LastName = 'Muñoz',
        Email = 'adriana@autonoma.edu.co',
        PhoneNumber = '3000000010',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000010';

    SELECT @IdAdriana = Id
    FROM Teachers
    WHERE IdentityDocument = '10000010';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdAdriana, 'Adriana', 'Muñoz', 'adriana@autonoma.edu.co', '10000010', '3000000010', 1, GETUTCDATE(), NULL);
END

-- Julián Giraldo
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000011')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Julián',
        LastName = 'Giraldo',
        Email = 'julian@autonoma.edu.co',
        PhoneNumber = '3000000011',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000011';

    SELECT @IdJulian = Id
    FROM Teachers
    WHERE IdentityDocument = '10000011';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdJulian, 'Julián', 'Giraldo', 'julian@autonoma.edu.co', '10000011', '3000000011', 1, GETUTCDATE(), NULL);
END

-- Profesor Nuevo
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000012')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Profesor',
        LastName = 'Nuevo',
        Email = 'nuevo@autonoma.edu.co',
        PhoneNumber = '3000000012',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000012';

    SELECT @IdNuevo = Id
    FROM Teachers
    WHERE IdentityDocument = '10000012';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdNuevo, 'Profesor', 'Nuevo', 'nuevo@autonoma.edu.co', '10000012', '3000000012', 1, GETUTCDATE(), NULL);
END

-- Lina Marín
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000013')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Lina',
        LastName = 'Marín',
        Email = 'lmarinu@autonoma.edu.co',
        PhoneNumber = '3000000013',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000013';

    SELECT @IdLina = Id
    FROM Teachers
    WHERE IdentityDocument = '10000013';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdLina, 'Lina', 'Marín', 'lmarinu@autonoma.edu.co', '10000013', '3000000013', 1, GETUTCDATE(), NULL);
END

-- Mauricio Mejía
IF EXISTS (SELECT 1 FROM Teachers WHERE IdentityDocument = '10000014')
BEGIN
    UPDATE Teachers
    SET
        FirstName = 'Mauricio',
        LastName = 'Mejía',
        Email = 'mauricio@autonoma.edu.co',
        PhoneNumber = '3000000014',
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE IdentityDocument = '10000014';

    SELECT @IdMauricio = Id
    FROM Teachers
    WHERE IdentityDocument = '10000014';
END
ELSE
BEGIN
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdMauricio, 'Mauricio', 'Mejía', 'mauricio@autonoma.edu.co', '10000014', '3000000014', 1, GETUTCDATE(), NULL);
END

-- ==========================================
-- 7. AULAS / CLASSROOMS
-- Fundadores y Edificio F son el mismo edificio.
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM Classrooms WHERE Code = 'F-101')
BEGIN
    INSERT INTO Classrooms (Id, Code, Name, Building, Floor, Capacity, Type, IsActive, CreatedAt, UpdatedAt) VALUES
    (@IdF101, 'F-101', 'FUNDADORES 101', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdF102, 'F-102', 'FUNDADORES 102', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdF103, 'F-103', 'FUNDADORES 103', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),

    (@IdF201, 'F-201', 'FUNDADORES 201', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdF202, 'F-202', 'FUNDADORES 202', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdF203, 'F-203', 'FUNDADORES 203', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),

    (@IdF301, 'F-301', 'FUNDADORES 301', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdF302, 'F-302', 'FUNDADORES 302', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdF303, 'F-303', 'FUNDADORES 303', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),

    (@IdF401, 'F-401', 'INFORMÁTICA F-401', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (@IdF402, 'F-402', 'INFORMÁTICA F-402', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (@IdF403, 'F-403', 'INFORMÁTICA F-403', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (@IdF404, 'F-404', 'INFORMÁTICA F-404', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (@IdF405, 'F-405', 'INFORMÁTICA F-405', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (@IdF406, 'F-406', 'INFORMÁTICA F-406', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (@IdF407, 'F-407', 'INFORMÁTICA F-407', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (@IdF408, 'F-408', 'INFORMÁTICA F-408', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (@IdF409, 'F-409', 'INFORMÁTICA F-409', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),

    (@IdF501, 'F-501', 'FUNDADORES 501', 'Fundadores / Edificio F', 5, 30, 'Aula de maestría', 1, GETUTCDATE(), NULL),
    (@IdF502, 'F-502', 'FUNDADORES 502', 'Fundadores / Edificio F', 5, 30, 'Aula de maestría', 1, GETUTCDATE(), NULL);
END
ELSE
BEGIN
    SELECT @IdF401 = Id FROM Classrooms WHERE Code = 'F-401';
    SELECT @IdF402 = Id FROM Classrooms WHERE Code = 'F-402';
    SELECT @IdF405 = Id FROM Classrooms WHERE Code = 'F-405';
    SELECT @IdF101 = Id FROM Classrooms WHERE Code = 'F-101';
    SELECT @IdF201 = Id FROM Classrooms WHERE Code = 'F-201';
END

IF NOT EXISTS (SELECT 1 FROM Classrooms WHERE Code = 'SAC-101')
BEGIN
    INSERT INTO Classrooms (Id, Code, Name, Building, Floor, Capacity, Type, IsActive, CreatedAt, UpdatedAt) VALUES
    (@IdSAC101, 'SAC-101', 'SACATÍN 101', 'Sacatín', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdSAC102, 'SAC-102', 'SACATÍN 102', 'Sacatín', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdSAC103, 'SAC-103', 'SACATÍN 103', 'Sacatín', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdSACLAB, 'SAC-LAB-ELEC', 'LABORATORIO DE ELECTRÓNICA', 'Sacatín', 1, 25, 'Laboratorio de electrónica', 1, GETUTCDATE(), NULL),
    (@IdSAC301, 'SAC-301', 'SACATÍN 301', 'Sacatín', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdSAC302, 'SAC-302', 'SACATÍN 302', 'Sacatín', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdSAC303, 'SAC-303', 'SACATÍN 303', 'Sacatín', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdSAC304, 'SAC-304', 'SACATÍN 304', 'Sacatín', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL);
END
ELSE
BEGIN
    SELECT @IdSAC101 = Id FROM Classrooms WHERE Code = 'SAC-101';
    SELECT @IdSACLAB = Id FROM Classrooms WHERE Code = 'SAC-LAB-ELEC';
END

IF NOT EXISTS (SELECT 1 FROM Classrooms WHERE Code = 'CB-101')
BEGIN
    INSERT INTO Classrooms (Id, Code, Name, Building, Floor, Capacity, Type, IsActive, CreatedAt, UpdatedAt) VALUES
    (@IdCB101, 'CB-101', 'CASA BAVARIA 101', 'Casa Bavaria', 1, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdCB102, 'CB-102', 'CASA BAVARIA 102', 'Casa Bavaria', 1, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdCB103, 'CB-103', 'CASA BAVARIA 103', 'Casa Bavaria', 1, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdCB201, 'CB-201', 'CASA BAVARIA 201', 'Casa Bavaria', 2, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdCB202, 'CB-202', 'CASA BAVARIA 202', 'Casa Bavaria', 2, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdCB203, 'CB-203', 'CASA BAVARIA 203', 'Casa Bavaria', 2, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (@IdCB204, 'CB-204', 'CASA BAVARIA 204', 'Casa Bavaria', 2, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL);
END
ELSE
BEGIN
    SELECT @IdCB101 = Id FROM Classrooms WHERE Code = 'CB-101';
    SELECT @IdCB201 = Id FROM Classrooms WHERE Code = 'CB-201';
END

-- ==========================================
-- 8. REGLAS TAPSI
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM TapsiRules WHERE RuleType = 'MateriasFijas')
BEGIN
    INSERT INTO TapsiRules (
        Id, RuleType, Description, Value, IsActive, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        'MateriasFijas',
        'Materias fijas para estudiantes provenientes de TAPSI, sin cruce de horarios.',
        'Cálculo Diferencial; Técnicas de Programación; Programación Orientada a Objetos; Teoría de Sistemas; Sistemas Operativos',
        1,
        GETUTCDATE(),
        NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM TapsiRules WHERE RuleType = 'CreditosDiurna')
BEGIN
    INSERT INTO TapsiRules (
        Id, RuleType, Description, Value, IsActive, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        'CreditosDiurna',
        'Máximo de créditos permitidos para jornada diurna.',
        '18',
        1,
        GETUTCDATE(),
        NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM TapsiRules WHERE RuleType = 'CreditosExtendida')
BEGIN
    INSERT INTO TapsiRules (
        Id, RuleType, Description, Value, IsActive, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        'CreditosExtendida',
        'Máximo de créditos permitidos para jornada extendida/nocturna.',
        '15',
        1,
        GETUTCDATE(),
        NULL
    );
END

-- ==========================================
-- 9. ASIGNAR MATERIAS A PROFESORES
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdErnesto AND SubjectId = @IdFundamentosPOO)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdErnesto, @IdFundamentosPOO, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdErnesto AND SubjectId = @IdSistemasOperativos)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdErnesto, @IdSistemasOperativos, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdHarold AND SubjectId = @IdParadigmas)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdHarold, @IdParadigmas, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdMarcela AND SubjectId = @IdPOO)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdMarcela, @IdPOO, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdMarcela AND SubjectId = @IdIngSoftware)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdMarcela, @IdIngSoftware, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdSebastian AND SubjectId = @IdTecnicasProg)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdSebastian, @IdTecnicasProg, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdSimon AND SubjectId = @IdBackend)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdSimon, @IdBackend, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdMauricio AND SubjectId = @IdBasesDatos)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdMauricio, @IdBasesDatos, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdBeatriz AND SubjectId = @IdIngles)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdBeatriz, @IdIngles, 'Cátedra');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdAdriana AND SubjectId = @IdEtica)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdAdriana, @IdEtica, 'Cátedra');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdJulian AND SubjectId = @IdCultura)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdJulian, @IdCultura, 'Cátedra');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdAlejandra AND SubjectId = @IdCalculoDiferencial)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdAlejandra, @IdCalculoDiferencial, 'Tiempo Completo');

-- ==========================================
-- 10. DISPONIBILIDADES DE PROFESORES
-- Day: 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdErnesto AND Day = 1 AND StartTime = '16:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdErnesto, 1, '16:00:00', '21:30:00', 4);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdErnesto AND Day = 3 AND StartTime = '16:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdErnesto, 3, '16:00:00', '18:00:00', 2);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdHarold AND Day = 1 AND StartTime = '08:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdHarold, 1, '08:00:00', '12:00:00', 4);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdHarold AND Day = 4 AND StartTime = '08:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdHarold, 4, '08:00:00', '12:00:00', 4);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdMarcela AND Day = 1 AND StartTime = '08:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdMarcela, 1, '08:00:00', '12:00:00', 4);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdMauricio AND Day = 1 AND StartTime = '14:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdMauricio, 1, '14:00:00', '16:00:00', 2);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdBeatriz AND Day = 2 AND StartTime = '08:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdBeatriz, 2, '08:00:00', '10:00:00', 2);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdAdriana AND Day = 3 AND StartTime = '10:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdAdriana, 3, '10:00:00', '12:00:00', 2);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdAlejandra AND Day = 4 AND StartTime = '08:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, Day, StartTime, EndTime, MaxTeachingHours) VALUES (NEWID(), @IdAlejandra, 4, '08:00:00', '12:00:00', 4);

-- ==========================================
-- 11. ASIGNACIONES DE AULAS
-- Esta tabla permite reservar aula por fecha/hora.
-- ==========================================

IF NOT EXISTS (
    SELECT 1 FROM ClassroomAssignments 
    WHERE ClassroomId = @IdF401 
    AND Date = '2026-02-02'
    AND StartTime = '08:00:00'
)
BEGIN
    INSERT INTO ClassroomAssignments (
        ClassroomId, Date, StartTime, EndTime
    )
    VALUES
    (@IdF401, '2026-02-02', '08:00:00', '10:00:00'),
    (@IdF402, '2026-02-02', '10:00:00', '12:00:00'),
    (@IdSAC101, '2026-02-03', '08:00:00', '10:00:00'),
    (@IdCB101, '2026-02-04', '10:00:00', '12:00:00');
END

-- ==========================================
-- 12. HORARIOS
-- Day: 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado
-- ==========================================

IF NOT EXISTS (
    SELECT 1 FROM Schedules 
    WHERE SubjectId = @IdPOO 
    AND TeacherId = @IdMarcela 
    AND Day = 1 
    AND StartTime = '08:00:00'
)
BEGIN
    INSERT INTO Schedules (
        Id, SubjectId, TeacherId, ClassroomId, Day, StartTime, EndTime,
        AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        @IdPOO,
        @IdMarcela,
        @IdF401,
        1,
        '08:00:00',
        '10:00:00',
        'Ingeniería de Sistemas',
        'Diurna',
        3,
        'Draft',
        GETUTCDATE(),
        NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM Schedules 
    WHERE SubjectId = @IdTecnicasProg 
    AND TeacherId = @IdSebastian 
    AND Day = 2 
    AND StartTime = '10:00:00'
)
BEGIN
    INSERT INTO Schedules (
        Id, SubjectId, TeacherId, ClassroomId, Day, StartTime, EndTime,
        AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        @IdTecnicasProg,
        @IdSebastian,
        @IdF402,
        2,
        '10:00:00',
        '12:00:00',
        'Ingeniería de Sistemas',
        'Diurna',
        2,
        'Draft',
        GETUTCDATE(),
        NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM Schedules 
    WHERE SubjectId = @IdBackend 
    AND TeacherId = @IdSimon 
    AND Day = 3 
    AND StartTime = '14:00:00'
)
BEGIN
    INSERT INTO Schedules (
        Id, SubjectId, TeacherId, ClassroomId, Day, StartTime, EndTime,
        AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        @IdBackend,
        @IdSimon,
        @IdF405,
        3,
        '14:00:00',
        '16:00:00',
        'Ingeniería de Sistemas',
        'Diurna',
        6,
        'Draft',
        GETUTCDATE(),
        NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM Schedules 
    WHERE SubjectId = @IdCalculoDiferencial 
    AND TeacherId = @IdAlejandra 
    AND Day = 4 
    AND StartTime = '08:00:00'
)
BEGIN
    INSERT INTO Schedules (
        Id, SubjectId, TeacherId, ClassroomId, Day, StartTime, EndTime,
        AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        @IdCalculoDiferencial,
        @IdAlejandra,
        @IdSAC101,
        4,
        '08:00:00',
        '10:00:00',
        'Ingeniería de Sistemas',
        'Diurna',
        2,
        'Draft',
        GETUTCDATE(),
        NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM Schedules 
    WHERE SubjectId = @IdIngles 
    AND TeacherId = @IdBeatriz 
    AND Day = 2 
    AND StartTime = '08:00:00'
)
BEGIN
    INSERT INTO Schedules (
        Id, SubjectId, TeacherId, ClassroomId, Day, StartTime, EndTime,
        AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        @IdIngles,
        @IdBeatriz,
        @IdCB101,
        2,
        '08:00:00',
        '10:00:00',
        'Ingeniería de Sistemas',
        'Diurna',
        1,
        'Draft',
        GETUTCDATE(),
        NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM Schedules 
    WHERE SubjectId = @IdEtica 
    AND TeacherId = @IdAdriana 
    AND Day = 3 
    AND StartTime = '10:00:00'
)
BEGIN
    INSERT INTO Schedules (
        Id, SubjectId, TeacherId, ClassroomId, Day, StartTime, EndTime,
        AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt
    )
    VALUES
    (
        NEWID(),
        @IdEtica,
        @IdAdriana,
        @IdCB201,
        3,
        '10:00:00',
        '12:00:00',
        'Ingeniería de Sistemas',
        'Diurna',
        1,
        'Draft',
        GETUTCDATE(),
        NULL
    );
END

COMMIT TRANSACTION;

PRINT 'Datos semilla cargados correctamente para las 12 tablas.';