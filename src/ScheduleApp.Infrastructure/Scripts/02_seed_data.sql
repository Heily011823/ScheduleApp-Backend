SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ==========================================
-- 0. LIMPIEZA CONTROLADA DE DATOS SEMILLA
-- ==========================================

IF OBJECT_ID(N'[dbo].[Schedules]', N'U') IS NOT NULL
BEGIN
    DELETE FROM [dbo].[Schedules];
END;

-- ==========================================
-- VARIABLES FIJAS
-- ==========================================

DECLARE @AdminRoleId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @CoordRoleId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

DECLARE @ProgramDiurnoId UNIQUEIDENTIFIER = 'D36A9D27-7C9B-4386-9E62-19ABC134546D';
DECLARE @ProgramNocturnoId UNIQUEIDENTIFIER = 'E2141636-257A-41B2-91A4-371859D33BA5';

DECLARE @OldProgramDiurnoId UNIQUEIDENTIFIER;
DECLARE @OldProgramNocturnoId UNIQUEIDENTIFIER;

SELECT @OldProgramDiurnoId = Id FROM [dbo].[AcademicPrograms] WHERE Code = '1020';
SELECT @OldProgramNocturnoId = Id FROM [dbo].[AcademicPrograms] WHERE Code = '1030';

-- ==========================================
-- 1. ROLES
-- ==========================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE Name = 'Administrador')
BEGIN
    INSERT INTO [dbo].[Roles] (Id, Name)
    VALUES (@AdminRoleId, 'Administrador');
END
ELSE
BEGIN
    SELECT @AdminRoleId = Id
    FROM [dbo].[Roles]
    WHERE Name = 'Administrador';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE Name = 'Coordinador')
BEGIN
    INSERT INTO [dbo].[Roles] (Id, Name)
    VALUES (@CoordRoleId, 'Coordinador');
END
ELSE
BEGIN
    SELECT @CoordRoleId = Id
    FROM [dbo].[Roles]
    WHERE Name = 'Coordinador';
END;

-- ==========================================
-- 2. USUARIOS
-- ==========================================

DECLARE @Users TABLE (
    FullName NVARCHAR(100),
    Email NVARCHAR(150),
    Username NVARCHAR(50),
    IdentityDocument NVARCHAR(10),
    PasswordHash NVARCHAR(MAX),
    RoleId UNIQUEIDENTIFIER
);

INSERT INTO @Users (FullName, Email, Username, IdentityDocument, PasswordHash, RoleId)
VALUES
('Administrador del Sistema', 'admin@autonoma.edu.co', 'admin', '11111111', '$2a$11$yqjcrTTVaOtcRrlnvyZu4etI5YNMITtWPXqOCbMvB4cP4AdO7Jox2', @AdminRoleId),
('Lina Maria Lopez Uribe', 'llopezu@autonoma.edu.co', 'llopezu', '1024567890', '$2y$11$0eMQea7NWRMh5k/AqbqeQ.G4emWXfxAj2/b7svg2TgAjGMNM5Qjwa', @CoordRoleId);

MERGE [dbo].[Users] AS target
USING @Users AS source
ON target.Username = source.Username
WHEN MATCHED THEN
    UPDATE SET
        target.FullName = source.FullName,
        target.Email = source.Email,
        target.IdentityDocument = source.IdentityDocument,
        target.PasswordHash = source.PasswordHash,
        target.RoleId = source.RoleId,
        target.IsActive = 1,
        target.IsDeleted = 0,
        target.UpdatedAt = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (
        Id, FullName, Email, Username, IdentityDocument, PasswordHash,
        RoleId, IsActive, IsDeleted, CreatedAt, UpdatedAt
    )
    VALUES (
        NEWID(), source.FullName, source.Email, source.Username,
        source.IdentityDocument, source.PasswordHash, source.RoleId,
        1, 0, GETUTCDATE(), NULL
    );

-- ==========================================
-- 3. PROGRAMAS ACADÉMICOS
-- ==========================================

DELETE FROM [dbo].[ProgramSemesters]
WHERE AcademicProgramId IN (
    @ProgramDiurnoId,
    @ProgramNocturnoId,
    @OldProgramDiurnoId,
    @OldProgramNocturnoId
);

DECLARE @Programs TABLE (
    Id UNIQUEIDENTIFIER,
    Code NVARCHAR(50),
    Name NVARCHAR(200),
    Shift NVARCHAR(50),
    TotalSemesters INT
);

INSERT INTO @Programs (Id, Code, Name, Shift, TotalSemesters)
VALUES
(@ProgramDiurnoId, '1020', 'Ingeniería de Sistemas', 'Diurna', 10),
(@ProgramNocturnoId, '1030', 'Ingeniería de Sistemas', 'Nocturna', 12);

MERGE [dbo].[AcademicPrograms] AS target
USING @Programs AS source
ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        target.Id = source.Id,
        target.Name = source.Name,
        target.Shift = source.Shift,
        target.TotalSemesters = source.TotalSemesters,
        target.IsActive = 1,
        target.IsDeleted = 0,
        target.UpdatedAt = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (
        Id, Code, Name, Shift, TotalSemesters,
        IsActive, IsDeleted, CreatedAt, UpdatedAt
    )
    VALUES (
        source.Id, source.Code, source.Name, source.Shift, source.TotalSemesters,
        1, 0, GETUTCDATE(), NULL
    );

-- ==========================================
-- 4. SEMESTRES DEL PROGRAMA
-- ==========================================

INSERT INTO [dbo].[ProgramSemesters] (
    Id, AcademicProgramId, SemesterNumber, MaxCredits, CreatedAt, UpdatedAt
)
VALUES
(NEWID(), @ProgramDiurnoId, 1, 18, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 2, 16, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 3, 18, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 4, 19, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 5, 18, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 6, 17, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 7, 18, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 8, 17, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 9, 17, GETUTCDATE(), NULL),
(NEWID(), @ProgramDiurnoId, 10, 17, GETUTCDATE(), NULL),
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
-- 5. ESPECIALIDADES
-- ==========================================

DECLARE @Specialties TABLE (
    Name NVARCHAR(100),
    Description NVARCHAR(500),
    Icon NVARCHAR(50),
    DisplayOrder INT
);

INSERT INTO @Specialties (Name, Description, Icon, DisplayOrder)
VALUES
('Lenguas Extranjeras', 'Idiomas y lingüística', NULL, 1),
('Matemáticas', 'Matemáticas puras y aplicadas', NULL, 2),
('Humanísticas', 'Ciencias humanas y sociales', NULL, 3),
('Física', 'Física teórica y experimental', NULL, 4),
('Ética', 'Ética y moral', NULL, 5),
('Arquitectura de Software', 'Diseño de software', NULL, 6),
('Electrónica Digital', 'Circuitos digitales y sistemas embebidos', NULL, 7),
('Ingeniería de Software', 'Desarrollo de software a gran escala', NULL, 8),
('Backend', 'Desarrollo del lado del servidor', NULL, 9),
('Frontend', 'Desarrollo del lado del cliente', NULL, 10),
('Ingeniería de Datos', 'Procesamiento y análisis de datos', NULL, 11),
('Programación', 'Lógica, algoritmos y fundamentos de programación', NULL, 12),
('Bases de Datos', 'Diseño y administración de bases de datos', NULL, 13),
('Redes y Sistemas', 'Redes, sistemas operativos y sistemas embebidos', NULL, 14),
('Teoría de Sistemas', 'Fundamentos teóricos de sistemas', NULL, 15);

MERGE [dbo].[Specialties] AS target
USING @Specialties AS source
ON target.Name = source.Name
WHEN MATCHED THEN
    UPDATE SET
        target.Description = source.Description,
        target.Icon = source.Icon,
        target.IsActive = 1,
        target.DisplayOrder = source.DisplayOrder,
        target.UpdatedAt = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (Id, Name, Description, Icon, IsActive, DisplayOrder, CreatedAt, UpdatedAt)
    VALUES (NEWID(), source.Name, source.Description, source.Icon, 1, source.DisplayOrder, GETUTCDATE(), NULL);

-- ==========================================
-- 6. MATERIAS COMPLETAS SEGÚN PLANES 1020 Y 1030
-- ==========================================

DECLARE @Subjects TABLE (
    Code NVARCHAR(50),
    Name NVARCHAR(200),
    Semester INT,
    Credits INT,
    WeeklyHours INT,
    IsTapsi BIT,
    SpecialtyName NVARCHAR(100)
);

INSERT INTO @Subjects (Code, Name, Semester, Credits, WeeklyHours, IsTapsi, SpecialtyName)
VALUES
-- Formación básica
('104066', 'Matemáticas Básicas', 1, 4, 4, 0, 'Matemáticas'),
('100010', 'Fundamentos de Ingeniería', 1, 2, 2, 0, 'Teoría de Sistemas'),
('104030', 'Cálculo Diferencial', 2, 3, 4, 1, 'Matemáticas'),
('104029', 'Álgebra Lineal', 2, 3, 4, 0, 'Matemáticas'),
('109178', 'Física Mecánica', 3, 4, 4, 0, 'Física'),
('104031', 'Cálculo Integral', 3, 3, 4, 0, 'Matemáticas'),
('109179', 'Electricidad y Magnetismo', 4, 4, 4, 0, 'Física'),
('104032', 'Cálculo Vectorial', 4, 3, 4, 0, 'Matemáticas'),
('104036', 'Estadística y Probabilidad', 4, 3, 4, 0, 'Matemáticas'),
('104033', 'Ecuaciones Diferenciales', 5, 3, 4, 0, 'Matemáticas'),
('109057', 'Métodos Numéricos', 5, 3, 4, 0, 'Matemáticas'),

-- Formación profesional
('103004', 'Teoría de Sistemas', 1, 3, 4, 1, 'Teoría de Sistemas'),
('103002', 'Lógica de Programación', 1, 3, 4, 0, 'Programación'),
('103007', 'Técnicas de Programación', 2, 3, 4, 1, 'Programación'),
('103008', 'Fundamentos de Programación Orientada a Objetos', 2, 3, 4, 1, 'Programación'),
('104027', 'Matemáticas Discretas', 3, 4, 4, 0, 'Matemáticas'),
('103011', 'Estructura de Datos', 3, 3, 4, 0, 'Programación'),
('103018', 'Programación Orientada a Objetos', 3, 3, 4, 1, 'Programación'),
('109180', 'Bases de Datos I', 4, 3, 4, 0, 'Bases de Datos'),
('103027', 'Sistemas Operativos', 4, 3, 4, 1, 'Redes y Sistemas'),
('103022', 'Ingeniería de Software I', 4, 3, 4, 0, 'Ingeniería de Software'),
('109182', 'Paradigmas de Lenguajes', 5, 3, 4, 0, 'Programación'),
('109183', 'Programación Back End', 5, 3, 4, 1, 'Backend'),
('103093', 'Ingeniería de Software II', 5, 3, 4, 1, 'Ingeniería de Software'),
('109184', 'Electrónica Digital y Arquitectura de Computadores', 6, 3, 4, 0, 'Electrónica Digital'),
('109185', 'Modelamiento y Simulación', 6, 3, 4, 0, 'Ingeniería de Software'),
('109186', 'Procesadores de Lenguajes', 6, 2, 3, 0, 'Programación'),
('103126', 'Redes LAN', 6, 3, 4, 1, 'Redes y Sistemas'),
('103021', 'Diseño de Algoritmos', 6, 3, 4, 0, 'Programación'),
('109104', 'Sistemas Embebidos', 7, 3, 4, 0, 'Electrónica Digital'),
('103127', 'Énfasis Profesional', 7, 3, 4, 0, 'Ingeniería de Software'),
('109188', 'Ciencia de los Datos', 7, 3, 4, 0, 'Ingeniería de Datos'),
('109181', 'Bases de Datos II', 7, 2, 3, 0, 'Bases de Datos'),
('109187', 'Programación Front End', 7, 3, 4, 0, 'Frontend'),
('103125', 'Sistemas de Información y Organizaciones', 8, 3, 4, 0, 'Backend'),
('103117', 'Inteligencia Artificial', 8, 3, 4, 0, 'Ingeniería de Datos'),
('109189', 'Programación de Dispositivos Móviles', 8, 3, 4, 0, 'Frontend'),
('103135', 'Proyecto de Desarrollo de Software', 8, 2, 3, 0, 'Ingeniería de Software'),
('103118', 'Gerencia de Proyectos Tecnológicos', 10, 2, 2, 0, 'Ingeniería de Software'),
('103136', 'Práctica Empresarial', 10, 9, 9, 0, 'Ingeniería de Software'),

-- Formación sociohumanística
('105038', 'Ética', 1, 2, 2, 0, 'Humanísticas'),
('100059', 'Competencias Comunicativas', 1, 3, 3, 0, 'Humanísticas'),
('101241', 'Emprendimiento', 2, 3, 3, 0, 'Humanísticas'),
('105036', 'Filosofía de la Ciencia', 6, 2, 2, 0, 'Humanísticas'),
('100030', 'Proceso de Investigación I', 7, 3, 3, 0, 'Humanísticas'),
('105042', 'Cultura Política', 8, 2, 2, 0, 'Humanísticas'),
('105041', 'Desarrollo Sostenible', 9, 2, 2, 0, 'Humanísticas'),
('100063', 'Paz y Competitividad', 9, 12, 12, 0, 'Humanísticas'),
('100031', 'Proceso de Investigación II', 10, 3, 3, 0, 'Humanísticas'),

-- Inglés
('151601', 'Inglés I', 1, 1, 2, 0, 'Lenguas Extranjeras'),
('151602', 'Inglés II', 2, 1, 2, 0, 'Lenguas Extranjeras'),
('151603', 'Inglés III', 3, 1, 2, 0, 'Lenguas Extranjeras'),
('151604', 'Inglés IV', 6, 1, 2, 0, 'Lenguas Extranjeras'),
('151605', 'Inglés V', 7, 1, 2, 0, 'Lenguas Extranjeras'),
('151606', 'Inglés VI', 8, 1, 2, 0, 'Lenguas Extranjeras');

MERGE [dbo].[Subjects] AS target
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
        NEWID(), source.Code, source.Name, source.Semester, source.Credits,
        source.WeeklyHours, source.IsTapsi, 1, 0, GETUTCDATE(), NULL
    );

UPDATE s
SET s.SpecialtyId = sp.Id,
    s.UpdatedAt = GETUTCDATE()
FROM [dbo].[Subjects] s
INNER JOIN @Subjects seed ON seed.Code = s.Code
INNER JOIN [dbo].[Specialties] sp ON sp.Name = seed.SpecialtyName;

-- ==========================================
-- 7. PROFESORES
-- ==========================================

DECLARE @Teachers TABLE (
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(150),
    IdentityDocument NVARCHAR(10),
    PhoneNumber NVARCHAR(10),
    PrimarySpecialtyName NVARCHAR(100)
);

INSERT INTO @Teachers (FirstName, LastName, Email, IdentityDocument, PhoneNumber, PrimarySpecialtyName)
VALUES
-- Programación / software / sistemas
('Ernesto', 'Pérez', 'ernesto@autonoma.edu.co', '10000001', '3000000001', 'Programación'),
('Harold', 'Gómez', 'harold@autonoma.edu.co', '10000002', '3000000002', 'Backend'),
('Marcela', 'Ríos', 'marcela@autonoma.edu.co', '10000003', '3000000003', 'Ingeniería de Software'),
('Sebastián', 'Castro', 'sebastian@autonoma.edu.co', '10000004', '3000000004', 'Frontend'),
('Simón', 'Restrepo', 'simon@autonoma.edu.co', '10000006', '3000000006', 'Backend'),
('Beatriz', 'Mejía', 'beatriz@autonoma.edu.co', '10000007', '3000000007', 'Programación'),
('Santiago', 'Toro', 'santiago@autonoma.edu.co', '10000008', '3000000008', 'Programación'),
('Juan David', 'Patiño', 'juandavid@autonoma.edu.co', '10000009', '3000000009', 'Programación'),
('Mauricio', 'Mejía', 'mauricio@autonoma.edu.co', '10000014', '3000000014', 'Bases de Datos'),

-- Inglés
('Adriana', 'Muñoz', 'adriana@autonoma.edu.co', '10000010', '3000000010', 'Lenguas Extranjeras'),
('Carolina', 'Torres', 'carolina.torres@autonoma.edu.co', '10000015', '3000000015', 'Lenguas Extranjeras'),

-- Matemáticas
('Alejandra', 'López', 'alejandra@autonoma.edu.co', '10000005', '3000000005', 'Matemáticas'),
('Carlos', 'Ramírez', 'carlos.ramirez@autonoma.edu.co', '10000016', '3000000016', 'Matemáticas'),
('Diana', 'Salazar', 'diana.salazar@autonoma.edu.co', '10000017', '3000000017', 'Matemáticas'),

-- Sociohumanísticas
('Julián', 'Giraldo', 'julian@autonoma.edu.co', '10000011', '3000000011', 'Humanísticas'),
('Lina', 'Marín', 'lmarinu@autonoma.edu.co', '10000013', '3000000013', 'Humanísticas'),
('Paula', 'Restrepo', 'paula.restrepo@autonoma.edu.co', '10000018', '3000000018', 'Humanísticas'),
('Camilo', 'Vargas', 'camilo.vargas@autonoma.edu.co', '10000019', '3000000019', 'Humanísticas'),

-- Física y electrónica
('Andrés', 'Quintero', 'andres.quintero@autonoma.edu.co', '10000020', '3000000020', 'Física'),
('Natalia', 'Vélez', 'natalia.velez@autonoma.edu.co', '10000021', '3000000021', 'Electrónica Digital');

MERGE [dbo].[Teachers] AS target
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
        NEWID(), source.FirstName, source.LastName, source.Email,
        source.IdentityDocument, source.PhoneNumber, 1, 0, GETUTCDATE(), NULL
    );

UPDATE t
SET t.SpecialtyId = sp.Id,
    t.UpdatedAt = GETUTCDATE()
FROM [dbo].[Teachers] t
INNER JOIN @Teachers seed ON seed.IdentityDocument = t.IdentityDocument
INNER JOIN [dbo].[Specialties] sp ON sp.Name = seed.PrimarySpecialtyName;

-- ==========================================
-- 8. TEACHER SPECIALTIES
-- ==========================================

DECLARE @TeacherSpecialtySeed TABLE (
    TeacherDocument NVARCHAR(10),
    SpecialtyName NVARCHAR(100)
);

INSERT INTO @TeacherSpecialtySeed (TeacherDocument, SpecialtyName)
VALUES
('10000001', 'Programación'),
('10000001', 'Redes y Sistemas'),
('10000002', 'Backend'),
('10000002', 'Programación'),
('10000002', 'Ingeniería de Datos'),
('10000003', 'Ingeniería de Software'),
('10000003', 'Frontend'),
('10000004', 'Frontend'),
('10000005', 'Matemáticas'),
('10000006', 'Backend'),
('10000007', 'Programación'),
('10000008', 'Programación'),
('10000009', 'Programación'),
('10000010', 'Lenguas Extranjeras'),
('10000011', 'Humanísticas'),
('10000013', 'Humanísticas'),
('10000014', 'Bases de Datos'),
('10000014', 'Ingeniería de Datos'),
('10000015', 'Lenguas Extranjeras'),
('10000016', 'Matemáticas'),
('10000017', 'Matemáticas'),
('10000018', 'Humanísticas'),
('10000019', 'Humanísticas'),
('10000020', 'Física'),
('10000021', 'Electrónica Digital');

DELETE ts
FROM [dbo].[TeacherSpecialties] ts
INNER JOIN [dbo].[Teachers] t ON t.Id = ts.TeacherId
INNER JOIN @Teachers seed ON seed.IdentityDocument = t.IdentityDocument;

INSERT INTO [dbo].[TeacherSpecialties] (TeacherId, SpecialtyId, AssignedAt)
SELECT DISTINCT
    t.Id,
    sp.Id,
    GETUTCDATE()
FROM @TeacherSpecialtySeed seed
INNER JOIN [dbo].[Teachers] t ON t.IdentityDocument = seed.TeacherDocument
INNER JOIN [dbo].[Specialties] sp ON sp.Name = seed.SpecialtyName
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[TeacherSpecialties] existing
    WHERE existing.TeacherId = t.Id
      AND existing.SpecialtyId = sp.Id
);

-- ==========================================
-- 9. AULAS
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

MERGE [dbo].[Classrooms] AS target
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
        NEWID(), source.Code, source.Name, source.Building, source.Floor,
        source.Capacity, source.Type, 1, 0, GETUTCDATE(), NULL
    );

-- ==========================================
-- 10. REGLAS TAPSI
-- ==========================================

DECLARE @TapsiRules TABLE (
    RuleType NVARCHAR(100),
    Description NVARCHAR(MAX),
    Value NVARCHAR(MAX)
);

INSERT INTO @TapsiRules (RuleType, Description, Value)
VALUES
('MateriasFijas', 'Materias fijas para estudiantes provenientes de TAPSI, sin cruce de horarios.', 'Cálculo Diferencial; Técnicas de Programación; Programación Orientada a Objetos; Teoría de Sistemas; Sistemas Operativos'),
('CreditosDiurna', 'Máximo de créditos permitidos para jornada diurna.', '18'),
('CreditosExtendida', 'Máximo de créditos permitidos para jornada extendida/nocturna.', '15');

MERGE [dbo].[TapsiRules] AS target
USING @TapsiRules AS source
ON target.RuleType = source.RuleType
WHEN MATCHED THEN
    UPDATE SET
        target.Description = source.Description,
        target.Value = source.Value,
        target.IsActive = 1,
        target.IsDeleted = 0,
        target.UpdatedAt = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (Id, RuleType, Description, Value, IsActive, IsDeleted, CreatedAt, UpdatedAt)
    VALUES (NEWID(), source.RuleType, source.Description, source.Value, 1, 0, GETUTCDATE(), NULL);

-- ==========================================
-- 11. ASIGNAR MATERIAS A PROFESORES
-- ==========================================

DECLARE @TeacherSubjectsSeed TABLE (
    TeacherDocument NVARCHAR(10),
    SubjectCode NVARCHAR(50),
    ContractType NVARCHAR(100)
);

INSERT INTO @TeacherSubjectsSeed (TeacherDocument, SubjectCode, ContractType)
VALUES
-- Matemáticas
('10000016', '104066', 'Tiempo Completo'),
('10000005', '104030', 'Tiempo Completo'),
('10000016', '104030', 'Tiempo Completo'),
('10000017', '104029', 'Tiempo Completo'),
('10000016', '104031', 'Tiempo Completo'),
('10000016', '104032', 'Tiempo Completo'),
('10000017', '104033', 'Tiempo Completo'),
('10000017', '104036', 'Tiempo Completo'),
('10000017', '109057', 'Tiempo Completo'),
('10000005', '104027', 'Tiempo Completo'),

-- Inglés
('10000010', '151601', 'Cátedra'),
('10000010', '151602', 'Cátedra'),
('10000015', '151603', 'Cátedra'),
('10000015', '151604', 'Cátedra'),
('10000010', '151605', 'Cátedra'),
('10000015', '151606', 'Cátedra'),

-- Sociohumanísticas
('10000018', '105038', 'Cátedra'),
('10000013', '100059', 'Cátedra'),
('10000019', '101241', 'Cátedra'),
('10000018', '105036', 'Cátedra'),
('10000011', '100030', 'Cátedra'),
('10000011', '105042', 'Cátedra'),
('10000019', '105041', 'Cátedra'),
('10000018', '100063', 'Cátedra'),
('10000011', '100031', 'Cátedra'),

-- Física y electrónica
('10000020', '100010', 'Tiempo Completo'),
('10000020', '109178', 'Tiempo Completo'),
('10000020', '109179', 'Tiempo Completo'),
('10000021', '109184', 'Tiempo Completo'),
('10000021', '109104', 'Tiempo Completo'),

-- Programación / software / sistemas
('10000001', '103004', 'Tiempo Completo'),
('10000008', '103002', 'Tiempo Completo'),
('10000007', '103002', 'Tiempo Completo'),
('10000008', '103007', 'Tiempo Completo'),
('10000001', '103008', 'Tiempo Completo'),
('10000007', '103008', 'Tiempo Completo'),
('10000007', '103011', 'Tiempo Completo'),
('10000003', '103018', 'Tiempo Completo'),
('10000007', '103018', 'Tiempo Completo'),
('10000001', '103027', 'Tiempo Completo'),
('10000001', '103126', 'Tiempo Completo'),
('10000002', '109182', 'Tiempo Completo'),
('10000008', '109182', 'Tiempo Completo'),
('10000002', '109186', 'Tiempo Completo'),
('10000008', '103021', 'Tiempo Completo'),
('10000009', '103021', 'Tiempo Completo'),
('10000003', '103022', 'Tiempo Completo'),
('10000003', '103093', 'Tiempo Completo'),
('10000003', '109185', 'Tiempo Completo'),
('10000003', '103135', 'Tiempo Completo'),
('10000003', '103118', 'Tiempo Completo'),
('10000003', '103136', 'Tiempo Completo'),

-- Bases de datos / datos / IA
('10000014', '109180', 'Tiempo Completo'),
('10000014', '109181', 'Tiempo Completo'),
('10000014', '109188', 'Tiempo Completo'),
('10000002', '103117', 'Tiempo Completo'),

-- Backend / frontend
('10000006', '109183', 'Tiempo Completo'),
('10000002', '109183', 'Tiempo Completo'),
('10000004', '109187', 'Tiempo Completo'),
('10000003', '109187', 'Tiempo Completo'),
('10000004', '109189', 'Tiempo Completo'),
('10000006', '103125', 'Tiempo Completo'),
('10000003', '103127', 'Tiempo Completo');

DELETE ts
FROM [dbo].[TeacherSubjects] ts
INNER JOIN [dbo].[Teachers] t ON t.Id = ts.TeacherId
INNER JOIN @Teachers seed ON seed.IdentityDocument = t.IdentityDocument;

INSERT INTO [dbo].[TeacherSubjects] (TeacherId, SubjectId, ContractType)
SELECT DISTINCT
    t.Id,
    s.Id,
    ts.ContractType
FROM @TeacherSubjectsSeed ts
INNER JOIN [dbo].[Teachers] t ON t.IdentityDocument = ts.TeacherDocument
INNER JOIN [dbo].[Subjects] s ON s.Code = ts.SubjectCode
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[TeacherSubjects] existing
    WHERE existing.TeacherId = t.Id
      AND existing.SubjectId = s.Id
);

-- ==========================================
-- 12. DISPONIBILIDADES DE PROFESORES
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
-- Programación / software / sistemas
('10000001', 1, '16:00:00', '21:30:00', 4),
('10000001', 3, '16:00:00', '18:00:00', 2),
('10000002', 1, '08:00:00', '12:00:00', 4),
('10000002', 4, '08:00:00', '12:00:00', 4),
('10000003', 1, '08:00:00', '12:00:00', 4),
('10000003', 3, '08:00:00', '12:00:00', 4),
('10000004', 2, '14:00:00', '18:00:00', 4),
('10000006', 3, '14:00:00', '18:00:00', 4),
('10000007', 2, '08:00:00', '12:00:00', 4),
('10000008', 4, '14:00:00', '18:00:00', 4),
('10000009', 5, '08:00:00', '12:00:00', 4),
('10000014', 1, '14:00:00', '16:00:00', 2),
('10000014', 3, '14:00:00', '18:00:00', 4),

-- Inglés
('10000010', 1, '10:00:00', '12:00:00', 2),
('10000010', 3, '10:00:00', '12:00:00', 2),
('10000015', 2, '10:00:00', '12:00:00', 2),
('10000015', 4, '10:00:00', '12:00:00', 2),

-- Matemáticas
('10000005', 4, '08:00:00', '12:00:00', 4),
('10000016', 1, '08:00:00', '12:00:00', 4),
('10000016', 3, '08:00:00', '12:00:00', 4),
('10000017', 2, '08:00:00', '12:00:00', 4),
('10000017', 4, '08:00:00', '12:00:00', 4),

-- Sociohumanísticas
('10000011', 2, '16:00:00', '18:00:00', 2),
('10000013', 1, '14:00:00', '16:00:00', 2),
('10000018', 3, '10:00:00', '12:00:00', 2),
('10000018', 5, '10:00:00', '12:00:00', 2),
('10000019', 1, '14:00:00', '16:00:00', 2),
('10000019', 4, '14:00:00', '16:00:00', 2),

-- Física y electrónica
('10000020', 2, '14:00:00', '18:00:00', 4),
('10000020', 4, '14:00:00', '18:00:00', 4),
('10000021', 1, '14:00:00', '18:00:00', 4),
('10000021', 3, '14:00:00', '18:00:00', 4);

DELETE ta
FROM [dbo].[TeacherAvailabilities] ta
INNER JOIN [dbo].[Teachers] t ON t.Id = ta.TeacherId
INNER JOIN @Teachers seed ON seed.IdentityDocument = t.IdentityDocument;

INSERT INTO [dbo].[TeacherAvailabilities] (
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
INNER JOIN [dbo].[Teachers] t ON t.IdentityDocument = a.TeacherDocument
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[TeacherAvailabilities] existing
    WHERE existing.TeacherId = t.Id
      AND existing.[Day] = a.[Day]
      AND existing.StartTime = a.StartTime
      AND existing.EndTime = a.EndTime
);

-- ==========================================
-- 13. ASIGNACIONES DE AULAS
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

INSERT INTO [dbo].[ClassroomAssignments] (
    ClassroomId, [Date], StartTime, EndTime
)
SELECT
    c.Id,
    ca.[Date],
    ca.StartTime,
    ca.EndTime
FROM @ClassroomAssignmentsSeed ca
INNER JOIN [dbo].[Classrooms] c ON c.Code = ca.ClassroomCode
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[ClassroomAssignments] existing
    WHERE existing.ClassroomId = c.Id
      AND existing.[Date] = ca.[Date]
      AND existing.StartTime = ca.StartTime
      AND existing.EndTime = ca.EndTime
);

COMMIT TRANSACTION;

PRINT 'Datos semilla unificados insertados correctamente.';
