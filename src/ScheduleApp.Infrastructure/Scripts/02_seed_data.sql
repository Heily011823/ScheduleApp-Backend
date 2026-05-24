-- ==========================================================================
-- SCRIPT DE INSERCIÓN DE DATOS SEMILLA 

-- ==========================================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ==========================================
-- 0. LIMPIEZA DE HORARIOS
-- ==========================================

IF OBJECT_ID(N'[dbo].[Schedules]', N'U') IS NOT NULL
BEGIN
    DELETE FROM Schedules;
END

-- ==========================================
-- VARIABLES FIJAS
-- ==========================================

DECLARE @AdminRoleId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @CoordRoleId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

DECLARE @ProgramDiurnoId UNIQUEIDENTIFIER = 'D36A9D27-7C9B-4386-9E62-19ABC134546D';
DECLARE @ProgramNocturnoId UNIQUEIDENTIFIER = 'E2141636-257A-41B2-91A4-371859D33BA5';

DECLARE @OldProgramDiurnoId UNIQUEIDENTIFIER;
DECLARE @OldProgramNocturnoId UNIQUEIDENTIFIER;

SELECT @OldProgramDiurnoId = Id FROM AcademicPrograms WHERE Code = '1020';
SELECT @OldProgramNocturnoId = Id FROM AcademicPrograms WHERE Code = '1030';

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

IF EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    UPDATE Users
    SET
        FullName = 'Administrador del Sistema',
        Email = 'admin@autonoma.edu.co',
        IdentityDocument = '11111111',
        PasswordHash = '$2a$11$yqjcrTTVaOtcRrlnvyZu4etI5YNMITtWPXqOCbMvB4cP4AdO7Jox2',
        RoleId = @AdminRoleId,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Username = 'admin';
END
ELSE
BEGIN
    INSERT INTO Users (
        Id, FullName, Email, Username, IdentityDocument, PasswordHash,
        RoleId, IsActive, IsDeleted, CreatedAt, UpdatedAt
    )
    VALUES (
        NEWID(),
        'Administrador del Sistema',
        'admin@autonoma.edu.co',
        'admin',
        '11111111',
        '$2a$11$yqjcrTTVaOtcRrlnvyZu4etI5YNMITtWPXqOCbMvB4cP4AdO7Jox2',
        @AdminRoleId,
        1,
        0,
        GETUTCDATE(),
        NULL
    );
END

IF EXISTS (SELECT 1 FROM Users WHERE Username = 'llopezu')
BEGIN
    UPDATE Users
    SET
        FullName = 'Lina Maria Lopez Uribe',
        Email = 'llopezu@autonoma.edu.co',
        IdentityDocument = '1024567890',
        PasswordHash = '$2y$11$0eMQea7NWRMh5k/AqbqeQ.G4emWXfxAj2/b7svg2TgAjGMNM5Qjwa',
        RoleId = @CoordRoleId,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Username = 'llopezu';
END
ELSE
BEGIN
    INSERT INTO Users (
        Id, FullName, Email, Username, IdentityDocument, PasswordHash,
        RoleId, IsActive, IsDeleted, CreatedAt, UpdatedAt
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
        0,
        GETUTCDATE(),
        NULL
    );
END

-- ==========================================
-- 3. PROGRAMAS ACADÉMICOS
-- ==========================================

DELETE FROM ProgramSemesters
WHERE AcademicProgramId IN (
    @ProgramDiurnoId,
    @ProgramNocturnoId,
    @OldProgramDiurnoId,
    @OldProgramNocturnoId
);

IF EXISTS (SELECT 1 FROM AcademicPrograms WHERE Code = '1020')
BEGIN
    UPDATE AcademicPrograms
    SET
        Id = @ProgramDiurnoId,
        Name = 'Ingeniería de Sistemas',
        Shift = 'Diurna',
        TotalSemesters = 10,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Code = '1020';
END
ELSE
BEGIN
    INSERT INTO AcademicPrograms (
        Id, Code, Name, Shift, TotalSemesters, IsActive, IsDeleted, CreatedAt, UpdatedAt
    )
    VALUES (
        @ProgramDiurnoId,
        '1020',
        'Ingeniería de Sistemas',
        'Diurna',
        10,
        1,
        0,
        GETUTCDATE(),
        NULL
    );
END

IF EXISTS (SELECT 1 FROM AcademicPrograms WHERE Code = '1030')
BEGIN
    UPDATE AcademicPrograms
    SET
        Id = @ProgramNocturnoId,
        Name = 'Ingeniería de Sistemas',
        Shift = 'Nocturna',
        TotalSemesters = 12,
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Code = '1030';
END
ELSE
BEGIN
    INSERT INTO AcademicPrograms (
        Id, Code, Name, Shift, TotalSemesters, IsActive, IsDeleted, CreatedAt, UpdatedAt
    )
    VALUES (
        @ProgramNocturnoId,
        '1030',
        'Ingeniería de Sistemas',
        'Nocturna',
        12,
        1,
        0,
        GETUTCDATE(),
        NULL
    );
END

-- ==========================================
-- 4. SEMESTRES DEL PROGRAMA
-- ==========================================

INSERT INTO ProgramSemesters (
    Id, AcademicProgramId, SemesterNumber, MaxCredits, CreatedAt, UpdatedAt
)
VALUES
(NEWID(), @ProgramDiurnoId, 1, 16, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 2, 18, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 3, 18, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 4, 19, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 5, 18, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 6, 17, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 7, 18, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 8, 17, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 9, 17, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 10, 17, GETUTCDATE(), NULL);

INSERT INTO ProgramSemesters (
    Id, AcademicProgramId, SemesterNumber, MaxCredits, CreatedAt, UpdatedAt
)
VALUES
(NEWID(), @ProgramNocturnoId, 1, 15, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 2, 15, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 3, 15, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 4, 15, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 5, 14, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 6, 15, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 7, 15, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 8, 14, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 9, 14, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 10, 15, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 11, 14, GETUTCDATE(), NULL),
(NEWID(), @ProgramNocturnoId, 12, 14, GETUTCDATE(), NULL);

-- ==========================================
-- 5. MATERIAS
-- ==========================================

DECLARE @Subjects TABLE (
    Code NVARCHAR(50),
    Name NVARCHAR(200),
    Semester INT,
    Credits INT,
    WeeklyHours INT,
    IsTapsi BIT
);

INSERT INTO @Subjects (Code, Name, Semester, Credits, WeeklyHours, IsTapsi)
VALUES
('103018', 'Programación Orientada a Objetos', 3, 3, 4, 1),
('103008', 'Fundamentos de POO', 2, 3, 4, 1),
('103027', 'Sistemas Operativos', 6, 3, 4, 1),
('103002', 'Lógica de Programación', 1, 3, 4, 0),
('109182', 'Paradigmas de Lenguajes', 6, 3, 4, 0),
('109186', 'Procesadores de Lenguajes', 7, 2, 3, 0),
('103093', 'Ingeniería de Software II', 7, 3, 4, 1),
('103007', 'Técnicas de Programación', 2, 3, 4, 1),
('109184', 'Electrónica Digital y Arquitectura de Computadores', 6, 3, 4, 0),
('109104', 'Sistemas Embebidos', 7, 3, 4, 0),
('103004', 'Teoría de Sistemas', 2, 3, 4, 1),
('109189', 'Programación de Dispositivos Móviles', 9, 3, 4, 0),
('109183', 'Programación Back End', 6, 3, 4, 1),
('103021', 'Diseño de Algoritmos', 8, 3, 4, 0),
('109188', 'Ciencia de los Datos', 8, 3, 4, 0),
('109185', 'Modelamiento y Simulación', 7, 3, 4, 0),
('103126', 'Redes LAN', 6, 3, 4, 1),
('103125', 'Sistemas de Información y Organizaciones', 10, 3, 4, 0),
('104027', 'Matemáticas Discretas', 3, 4, 4, 0),
('109187', 'Programación Front End', 7, 3, 4, 0),
('109180', 'Bases de Datos I', 5, 3, 4, 0),
('104030', 'Cálculo Diferencial', 2, 3, 4, 1),
('151601', 'Inglés I', 1, 1, 2, 0),
('105038', 'Ética', 1, 2, 2, 0),
('105042', 'Cultura Política', 8, 2, 2, 0),
('100059', 'Competencias Comunicativas', 2, 3, 3, 0);

MERGE Subjects AS target
USING @Subjects AS source
ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        target.Name = source.Name,
        target.Semester = source.Semester,
        target.Credits = source.Credits,
        target.WeeklyHours = source.WeeklyHours,
        target.IsTapsi = source.IsTapsi,
        target.IsActive = 1,
        target.IsDeleted = 0,
        target.UpdatedAt = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (
        Id, Code, Name, Semester, Credits, WeeklyHours,
        IsTapsi, IsActive, IsDeleted, CreatedAt, UpdatedAt
    )
    VALUES (
        NEWID(), source.Code, source.Name, source.Semester, source.Credits, source.WeeklyHours,
        source.IsTapsi, 1, 0, GETUTCDATE(), NULL
    );

-- ==========================================
-- 6. PROFESORES
-- ==========================================

DECLARE @Teachers TABLE (
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(150),
    IdentityDocument NVARCHAR(10),
    PhoneNumber NVARCHAR(10)
);

INSERT INTO @Teachers (FirstName, LastName, Email, IdentityDocument, PhoneNumber)
VALUES
('Ernesto', 'Pérez', 'ernesto@autonoma.edu.co', '10000001', '3000000001'),
('Harold', 'Gómez', 'harold@autonoma.edu.co', '10000002', '3000000002'),
('Marcela', 'Ríos', 'marcela@autonoma.edu.co', '10000003', '3000000003'),
('Sebastián', 'Castro', 'sebastian@autonoma.edu.co', '10000004', '3000000004'),
('Alejandra', 'López', 'alejandra@autonoma.edu.co', '10000005', '3000000005'),
('Simón', 'Restrepo', 'simon@autonoma.edu.co', '10000006', '3000000006'),
('Beatriz', 'Mejía', 'beatriz@autonoma.edu.co', '10000007', '3000000007'),
('Santiago', 'Toro', 'santiago@autonoma.edu.co', '10000008', '3000000008'),
('Juan David', 'Patiño', 'juandavid@autonoma.edu.co', '10000009', '3000000009'),
('Adriana', 'Muñoz', 'adriana@autonoma.edu.co', '10000010', '3000000010'),
('Julián', 'Giraldo', 'julian@autonoma.edu.co', '10000011', '3000000011'),
('Profesor', 'Nuevo', 'nuevo@autonoma.edu.co', '10000012', '3000000012'),
('Lina', 'Marín', 'lmarinu@autonoma.edu.co', '10000013', '3000000013'),
('Mauricio', 'Mejía', 'mauricio@autonoma.edu.co', '10000014', '3000000014');

MERGE Teachers AS target
USING @Teachers AS source
ON target.IdentityDocument = source.IdentityDocument
WHEN MATCHED THEN
    UPDATE SET
        target.FirstName = source.FirstName,
        target.LastName = source.LastName,
        target.Email = source.Email,
        target.PhoneNumber = source.PhoneNumber,
        target.IsActive = 1,
        target.IsDeleted = 0,
        target.UpdatedAt = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (
        Id, FirstName, LastName, Email, IdentityDocument,
        PhoneNumber, IsActive, IsDeleted, CreatedAt, UpdatedAt
    )
    VALUES (
        NEWID(), source.FirstName, source.LastName, source.Email, source.IdentityDocument,
        source.PhoneNumber, 1, 0, GETUTCDATE(), NULL
    );

-- ==========================================
-- 7. AULAS
-- ==========================================

DECLARE @Classrooms TABLE (
    Code NVARCHAR(50),
    Name NVARCHAR(100),
    Building NVARCHAR(100),
    Floor INT,
    Capacity INT,
    Type NVARCHAR(100)
);

INSERT INTO @Classrooms (Code, Name, Building, Floor, Capacity, Type)
VALUES
('F-101', 'FUNDADORES 101', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general'),
('F-102', 'FUNDADORES 102', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general'),
('F-103', 'FUNDADORES 103', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general'),
('F-201', 'FUNDADORES 201', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general'),
('F-202', 'FUNDADORES 202', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general'),
('F-203', 'FUNDADORES 203', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general'),
('F-301', 'FUNDADORES 301', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general'),
('F-302', 'FUNDADORES 302', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general'),
('F-303', 'FUNDADORES 303', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general'),
('F-401', 'INFORMÁTICA F-401', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-402', 'INFORMÁTICA F-402', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-403', 'INFORMÁTICA F-403', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-404', 'INFORMÁTICA F-404', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-405', 'INFORMÁTICA F-405', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-406', 'INFORMÁTICA F-406', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-407', 'INFORMÁTICA F-407', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-408', 'INFORMÁTICA F-408', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-409', 'INFORMÁTICA F-409', 'Fundadores / Edificio F', 4, 30, 'Sala de informática'),
('F-501', 'FUNDADORES 501', 'Fundadores / Edificio F', 5, 30, 'Aula de maestría'),
('F-502', 'FUNDADORES 502', 'Fundadores / Edificio F', 5, 30, 'Aula de maestría'),
('SAC-101', 'SACATÍN 101', 'Sacatín', 1, 35, 'Aula teórica general'),
('SAC-102', 'SACATÍN 102', 'Sacatín', 1, 35, 'Aula teórica general'),
('SAC-103', 'SACATÍN 103', 'Sacatín', 1, 35, 'Aula teórica general'),
('SAC-LAB-ELEC', 'LABORATORIO DE ELECTRÓNICA', 'Sacatín', 1, 25, 'Laboratorio de electrónica'),
('SAC-301', 'SACATÍN 301', 'Sacatín', 3, 35, 'Aula teórica general'),
('SAC-302', 'SACATÍN 302', 'Sacatín', 3, 35, 'Aula teórica general'),
('SAC-303', 'SACATÍN 303', 'Sacatín', 3, 35, 'Aula teórica general'),
('SAC-304', 'SACATÍN 304', 'Sacatín', 3, 35, 'Aula teórica general'),
('CB-101', 'CASA BAVARIA 101', 'Casa Bavaria', 1, 30, 'Aula teórica general'),
('CB-102', 'CASA BAVARIA 102', 'Casa Bavaria', 1, 30, 'Aula teórica general'),
('CB-103', 'CASA BAVARIA 103', 'Casa Bavaria', 1, 30, 'Aula teórica general'),
('CB-201', 'CASA BAVARIA 201', 'Casa Bavaria', 2, 30, 'Aula teórica general'),
('CB-202', 'CASA BAVARIA 202', 'Casa Bavaria', 2, 30, 'Aula teórica general'),
('CB-203', 'CASA BAVARIA 203', 'Casa Bavaria', 2, 30, 'Aula teórica general'),
('CB-204', 'CASA BAVARIA 204', 'Casa Bavaria', 2, 30, 'Aula teórica general');

MERGE Classrooms AS target
USING @Classrooms AS source
ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        target.Name = source.Name,
        target.Building = source.Building,
        target.Floor = source.Floor,
        target.Capacity = source.Capacity,
        target.Type = source.Type,
        target.IsActive = 1,
        target.IsDeleted = 0,
        target.UpdatedAt = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (
        Id, Code, Name, Building, Floor, Capacity,
        Type, IsActive, IsDeleted, CreatedAt, UpdatedAt
    )
    VALUES (
        NEWID(), source.Code, source.Name, source.Building, source.Floor, source.Capacity,
        source.Type, 1, 0, GETUTCDATE(), NULL
    );

-- ==========================================
-- 8. REGLAS TAPSI
-- ==========================================

IF EXISTS (SELECT 1 FROM TapsiRules WHERE RuleType = 'MateriasFijas')
BEGIN
    UPDATE TapsiRules
    SET
        Description = 'Materias fijas para estudiantes provenientes de TAPSI, sin cruce de horarios.',
        Value = 'Cálculo Diferencial; Técnicas de Programación; Programación Orientada a Objetos; Teoría de Sistemas; Sistemas Operativos',
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = GETUTCDATE()
    WHERE RuleType = 'MateriasFijas';
END
ELSE
BEGIN
    INSERT INTO TapsiRules (Id, RuleType, Description, Value, IsActive, IsDeleted, CreatedAt, UpdatedAt)
    VALUES (
        NEWID(),
        'MateriasFijas',
        'Materias fijas para estudiantes provenientes de TAPSI, sin cruce de horarios.',
        'Cálculo Diferencial; Técnicas de Programación; Programación Orientada a Objetos; Teoría de Sistemas; Sistemas Operativos',
        1,
        0,
        GETUTCDATE(),
        NULL
    );
END

IF EXISTS (SELECT 1 FROM TapsiRules WHERE RuleType = 'CreditosDiurna')
BEGIN
    UPDATE TapsiRules
    SET
        Description = 'Máximo de créditos permitidos para jornada diurna.',
        Value = '18',
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = GETUTCDATE()
    WHERE RuleType = 'CreditosDiurna';
END
ELSE
BEGIN
    INSERT INTO TapsiRules (Id, RuleType, Description, Value, IsActive, IsDeleted, CreatedAt, UpdatedAt)
    VALUES (
        NEWID(),
        'CreditosDiurna',
        'Máximo de créditos permitidos para jornada diurna.',
        '18',
        1,
        0,
        GETUTCDATE(),
        NULL
    );
END

IF EXISTS (SELECT 1 FROM TapsiRules WHERE RuleType = 'CreditosExtendida')
BEGIN
    UPDATE TapsiRules
    SET
        Description = 'Máximo de créditos permitidos para jornada extendida/nocturna.',
        Value = '15',
        IsActive = 1,
        IsDeleted = 0,
        UpdatedAt = GETUTCDATE()
    WHERE RuleType = 'CreditosExtendida';
END
ELSE
BEGIN
    INSERT INTO TapsiRules (Id, RuleType, Description, Value, IsActive, IsDeleted, CreatedAt, UpdatedAt)
    VALUES (
        NEWID(),
        'CreditosExtendida',
        'Máximo de créditos permitidos para jornada extendida/nocturna.',
        '15',
        1,
        0,
        GETUTCDATE(),
        NULL
    );
END

-- ==========================================
-- 9. ASIGNAR MATERIAS A PROFESORES
-- ==========================================

DECLARE @TeacherSubjectsSeed TABLE (
    TeacherDocument NVARCHAR(10),
    SubjectCode NVARCHAR(50),
    ContractType NVARCHAR(100)
);

INSERT INTO @TeacherSubjectsSeed (TeacherDocument, SubjectCode, ContractType)
VALUES
('10000001', '103008', 'Tiempo Completo'),
('10000001', '103027', 'Tiempo Completo'),
('10000002', '109182', 'Tiempo Completo'),
('10000003', '103018', 'Tiempo Completo'),
('10000003', '103093', 'Tiempo Completo'),
('10000004', '103007', 'Tiempo Completo'),
('10000006', '109183', 'Tiempo Completo'),
('10000014', '109180', 'Tiempo Completo'),
('10000007', '151601', 'Cátedra'),
('10000010', '105038', 'Cátedra'),
('10000011', '105042', 'Cátedra'),
('10000005', '104030', 'Tiempo Completo');

INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType)
SELECT
    t.Id,
    s.Id,
    ts.ContractType
FROM @TeacherSubjectsSeed ts
INNER JOIN Teachers t ON t.IdentityDocument = ts.TeacherDocument
INNER JOIN Subjects s ON s.Code = ts.SubjectCode
WHERE NOT EXISTS (
    SELECT 1
    FROM TeacherSubjects existing
    WHERE existing.TeacherId = t.Id
      AND existing.SubjectId = s.Id
);

-- ==========================================
-- 10. DISPONIBILIDADES DE PROFESORES
-- Day: 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado
-- ==========================================

DECLARE @Availabilities TABLE (
    TeacherDocument NVARCHAR(10),
    [Day] INT,
    StartTime TIME,
    EndTime TIME,
    MaxTeachingHours INT
);

INSERT INTO @Availabilities (TeacherDocument, [Day], StartTime, EndTime, MaxTeachingHours)
VALUES
('10000001', 1, '16:00:00', '21:30:00', 4),
('10000001', 3, '16:00:00', '18:00:00', 2),
('10000002', 1, '08:00:00', '12:00:00', 4),
('10000002', 4, '08:00:00', '12:00:00', 4),
('10000003', 1, '08:00:00', '12:00:00', 4),
('10000014', 1, '14:00:00', '16:00:00', 2),
('10000007', 2, '08:00:00', '10:00:00', 2),
('10000010', 3, '10:00:00', '12:00:00', 2),
('10000005', 4, '08:00:00', '12:00:00', 4);

INSERT INTO TeacherAvailabilities (
    Id, TeacherId, [Day], StartTime, EndTime, MaxTeachingHours
)
SELECT
    NEWID(),
    t.Id,
    a.[Day],
    a.StartTime,
    a.EndTime,
    a.MaxTeachingHours
FROM @Availabilities a
INNER JOIN Teachers t ON t.IdentityDocument = a.TeacherDocument
WHERE NOT EXISTS (
    SELECT 1
    FROM TeacherAvailabilities existing
    WHERE existing.TeacherId = t.Id
      AND existing.[Day] = a.[Day]
      AND existing.StartTime = a.StartTime
      AND existing.EndTime = a.EndTime
);

-- ==========================================
-- 11. ASIGNACIONES DE AULAS
-- ==========================================

DECLARE @ClassroomAssignmentsSeed TABLE (
    ClassroomCode NVARCHAR(50),
    [Date] DATETIME2,
    StartTime TIME,
    EndTime TIME
);

INSERT INTO @ClassroomAssignmentsSeed (ClassroomCode, [Date], StartTime, EndTime)
VALUES
('F-401', '2026-02-02', '08:00:00', '10:00:00'),
('F-402', '2026-02-02', '10:00:00', '12:00:00'),
('SAC-101', '2026-02-03', '08:00:00', '10:00:00'),
('CB-101', '2026-02-04', '10:00:00', '12:00:00');

INSERT INTO ClassroomAssignments (
    ClassroomId, [Date], StartTime, EndTime
)
SELECT
    c.Id,
    ca.[Date],
    ca.StartTime,
    ca.EndTime
FROM @ClassroomAssignmentsSeed ca
INNER JOIN Classrooms c ON c.Code = ca.ClassroomCode
WHERE NOT EXISTS (
    SELECT 1
    FROM ClassroomAssignments existing
    WHERE existing.ClassroomId = c.Id
      AND existing.[Date] = ca.[Date]
      AND existing.StartTime = ca.StartTime
      AND existing.EndTime = ca.EndTime
);

COMMIT TRANSACTION;

PRINT 'Datos semilla insertados correctamente.';