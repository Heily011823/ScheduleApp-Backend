-- ==========================================================================
-- SCRIPT COMPLETO CORREGIDO

IF @@TRANCOUNT > 0
BEGIN
    ROLLBACK TRANSACTION;
END
GO

-- ==========================================
-- NIVEL 1: TABLAS INDEPENDIENTES
-- ==========================================

-- 1. Tabla: Roles
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE Roles (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL UNIQUE
    );
END
GO

-- 2. Tabla: Classrooms
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Classrooms]') AND type in (N'U'))
BEGIN
    CREATE TABLE Classrooms (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(100) NOT NULL,
        Building NVARCHAR(100) NOT NULL,
        Floor INT NOT NULL,
        Capacity INT NOT NULL,
        Type NVARCHAR(100) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

-- 3. Tabla: Subjects
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subjects]') AND type in (N'U'))
BEGIN
    CREATE TABLE Subjects (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Semester INT NOT NULL,
        Credits INT NOT NULL,
        WeeklyHours INT NOT NULL,
        IsTapsi BIT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

-- 4. Tabla: Teachers
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Teachers]') AND type in (N'U'))
BEGIN
    CREATE TABLE Teachers (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        IdentityDocument NVARCHAR(10) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(10) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT CHK_Teacher_Email
        CHECK (Email LIKE '%@autonoma.edu.co'),

        CONSTRAINT CHK_Teacher_IdentityDocument
        CHECK (
            IdentityDocument NOT LIKE '%[^0-9]%'
            AND LEN(IdentityDocument) BETWEEN 8 AND 10
        ),

        CONSTRAINT CHK_Teacher_PhoneNumber
        CHECK (
            PhoneNumber LIKE '3[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'
        )
    );
END
GO

-- 5. Tabla: AcademicPrograms
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AcademicPrograms]') AND type in (N'U'))
BEGIN
    CREATE TABLE AcademicPrograms (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Shift NVARCHAR(50) NOT NULL,
        TotalSemesters INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

-- 6. Tabla: TapsiRules
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TapsiRules]') AND type in (N'U'))
BEGIN
    CREATE TABLE TapsiRules (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        RuleType NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        [Value] NVARCHAR(500) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

-- Si TapsiRules ya existia con Value NVARCHAR(200), se amplia a NVARCHAR(500)
IF COL_LENGTH('dbo.TapsiRules', 'Value') IS NOT NULL
   AND COL_LENGTH('dbo.TapsiRules', 'Value') < 1000
BEGIN
    ALTER TABLE TapsiRules
    ALTER COLUMN [Value] NVARCHAR(500) NOT NULL;
END
GO

-- ==========================================
-- NIVEL 2: TABLAS CON DEPENDENCIAS SIMPLES
-- ==========================================

-- 7. Tabla: Users
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        IdentityDocument NVARCHAR(10) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleId) REFERENCES Roles(Id),

        CONSTRAINT CHK_User_Email
        CHECK (Email LIKE '%@autonoma.edu.co'),

        CONSTRAINT CHK_User_IdentityDocument
        CHECK (
            IdentityDocument NOT LIKE '%[^0-9]%'
            AND LEN(IdentityDocument) BETWEEN 8 AND 10
        )
    );
END
GO

-- 8. Tabla: ProgramSemesters
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProgramSemesters]') AND type in (N'U'))
BEGIN
    CREATE TABLE ProgramSemesters (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        AcademicProgramId UNIQUEIDENTIFIER NOT NULL,
        SemesterNumber INT NOT NULL,
        MaxCredits INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_ProgramSemesters_AcademicPrograms
        FOREIGN KEY (AcademicProgramId) REFERENCES AcademicPrograms(Id)
    );
END
GO

-- 9. Tabla: TeacherAvailabilities
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeacherAvailabilities]') AND type in (N'U'))
BEGIN
    CREATE TABLE TeacherAvailabilities (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        [Day] INT NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        MaxTeachingHours INT NOT NULL,

        CONSTRAINT FK_TeacherAvailabilities_Teachers
        FOREIGN KEY (TeacherId) REFERENCES Teachers(Id)
    );
END
GO

-- 10. Tabla: ClassroomAssignments
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClassroomAssignments]') AND type in (N'U'))
BEGIN
    CREATE TABLE ClassroomAssignments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ClassroomId UNIQUEIDENTIFIER NOT NULL,
        [Date] DATETIME2 NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,

        CONSTRAINT FK_ClassroomAssignments_Classrooms
        FOREIGN KEY (ClassroomId) REFERENCES Classrooms(Id)
    );
END
GO

-- ==========================================
-- NIVEL 3: TABLAS RELACIONALES COMPLEJAS
-- ==========================================

-- 11. Tabla: TeacherSubjects
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeacherSubjects]') AND type in (N'U'))
BEGIN
    CREATE TABLE TeacherSubjects (
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        SubjectId UNIQUEIDENTIFIER NOT NULL,
        ContractType NVARCHAR(100) NOT NULL,

        PRIMARY KEY (TeacherId, SubjectId),

        CONSTRAINT FK_TeacherSubjects_Teachers
        FOREIGN KEY (TeacherId) REFERENCES Teachers(Id),

        CONSTRAINT FK_TeacherSubjects_Subjects
        FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
    );
END
GO

-- 12. Tabla: Schedules
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Schedules]') AND type in (N'U'))
BEGIN
    CREATE TABLE Schedules (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        SubjectId UNIQUEIDENTIFIER NOT NULL,
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        ClassroomId UNIQUEIDENTIFIER NOT NULL,
        [Day] INT NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        AcademicProgram NVARCHAR(200) NOT NULL,
        Shift NVARCHAR(50) NOT NULL,
        Semester INT NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_Schedules_Subjects
        FOREIGN KEY (SubjectId) REFERENCES Subjects(Id),

        CONSTRAINT FK_Schedules_Teachers
        FOREIGN KEY (TeacherId) REFERENCES Teachers(Id),

        CONSTRAINT FK_Schedules_Classrooms
        FOREIGN KEY (ClassroomId) REFERENCES Classrooms(Id)
    );
END
GO

-- ==========================================================================
-- DATOS SEMILLA
-- ==========================================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Variables de roles
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @CoordRoleId UNIQUEIDENTIFIER = NEWID();

-- Variables de profesores
DECLARE @IdErnesto UNIQUEIDENTIFIER = NEWID();
DECLARE @IdHarold UNIQUEIDENTIFIER = NEWID();
DECLARE @IdMarcela UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSebastian UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAlejandra UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSimon UNIQUEIDENTIFIER = NEWID();
DECLARE @IdBeatriz UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAdriana UNIQUEIDENTIFIER = NEWID();
DECLARE @IdJulian UNIQUEIDENTIFIER = NEWID();
DECLARE @IdMauricio UNIQUEIDENTIFIER = NEWID();

-- Variables de materias
DECLARE @IdPOO UNIQUEIDENTIFIER = NEWID();
DECLARE @IdFundamentosPOO UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSistemasOperativos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdLogica UNIQUEIDENTIFIER = NEWID();
DECLARE @IdParadigmas UNIQUEIDENTIFIER = NEWID();
DECLARE @IdIngSoftware UNIQUEIDENTIFIER = NEWID();
DECLARE @IdTecnicasProg UNIQUEIDENTIFIER = NEWID();
DECLARE @IdElectronica UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSistemasEmbebidos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdTeoriaSistemas UNIQUEIDENTIFIER = NEWID();
DECLARE @IdBackend UNIQUEIDENTIFIER = NEWID();
DECLARE @IdRedes UNIQUEIDENTIFIER = NEWID();
DECLARE @IdBasesDatos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCalculoDiferencial UNIQUEIDENTIFIER = NEWID();
DECLARE @IdIngles UNIQUEIDENTIFIER = NEWID();
DECLARE @IdEtica UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCultura UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCompetencias UNIQUEIDENTIFIER = NEWID();

-- Variables de aulas que se usan en horarios
DECLARE @IdF401 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF402 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdF405 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdSAC101 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCB101 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCB201 UNIQUEIDENTIFIER = NEWID();

DECLARE @ProgramDiurnoId UNIQUEIDENTIFIER = NEWID();
DECLARE @ProgramNocturnoId UNIQUEIDENTIFIER = NEWID();

-- ==========================================
-- 1. ROLES
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Administrador')
    INSERT INTO Roles (Id, Name) VALUES (@AdminRoleId, 'Administrador');
ELSE
    SELECT @AdminRoleId = Id FROM Roles WHERE Name = 'Administrador';

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Coordinador')
    INSERT INTO Roles (Id, Name) VALUES (@CoordRoleId, 'Coordinador');
ELSE
    SELECT @CoordRoleId = Id FROM Roles WHERE Name = 'Coordinador';

-- ==========================================
-- 2. USUARIOS
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Id, FullName, Email, Username, IdentityDocument, PasswordHash, RoleId, IsActive, CreatedAt)
    VALUES (NEWID(), 'Administrador del Sistema', 'admin@autonoma.edu.co', 'admin', '11111111', 'AQAAAAIAAYagAAAAEIs9XvshbXOnwG01Z6vQCpXvXp1C7uG6bWscN8FvYmJvK6A==', @AdminRoleId, 1, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'llopezu')
BEGIN
    INSERT INTO Users (Id, FullName, Email, Username, IdentityDocument, PasswordHash, RoleId, IsActive, CreatedAt)
    VALUES (NEWID(), 'Lina Maria Lopez Uribe', 'llopezu@autonoma.edu.co', 'llopezu', '1024567890', 'AQAAAAIAAYagAAAAEIs9XvshbXOnwG01Z6vQCpXvXp1C7uG6bWscN8FvYmJvK6A==', @CoordRoleId, 1, GETUTCDATE());
END

-- ==========================================
-- 3. PROGRAMAS ACADEMICOS
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM AcademicPrograms WHERE Code = '1020')
    INSERT INTO AcademicPrograms (Id, Code, Name, Shift, TotalSemesters, IsActive, CreatedAt)
    VALUES (@ProgramDiurnoId, '1020', 'Ingeniería de Sistemas - Diurna', 'Diurna', 10, 1, GETUTCDATE());
ELSE
    SELECT @ProgramDiurnoId = Id FROM AcademicPrograms WHERE Code = '1020';

IF NOT EXISTS (SELECT 1 FROM AcademicPrograms WHERE Code = '1030')
    INSERT INTO AcademicPrograms (Id, Code, Name, Shift, TotalSemesters, IsActive, CreatedAt)
    VALUES (@ProgramNocturnoId, '1030', 'Ingeniería de Sistemas - Nocturna', 'Nocturna', 12, 1, GETUTCDATE());
ELSE
    SELECT @ProgramNocturnoId = Id FROM AcademicPrograms WHERE Code = '1030';

-- ==========================================
-- 4. SEMESTRES DEL PROGRAMA
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM ProgramSemesters WHERE AcademicProgramId = @ProgramDiurnoId)
BEGIN
    INSERT INTO ProgramSemesters (Id, AcademicProgramId, SemesterNumber, MaxCredits, CreatedAt)
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
    INSERT INTO ProgramSemesters (Id, AcademicProgramId, SemesterNumber, MaxCredits, CreatedAt)
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
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdPOO, '103018', 'Programación Orientada a Objetos', 3, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdPOO = Id FROM Subjects WHERE Code = '103018';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103008')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdFundamentosPOO, '103008', 'Fundamentos de POO', 2, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdFundamentosPOO = Id FROM Subjects WHERE Code = '103008';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103027')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdSistemasOperativos, '103027', 'Sistemas Operativos', 6, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdSistemasOperativos = Id FROM Subjects WHERE Code = '103027';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103002')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdLogica, '103002', 'Lógica de Programación', 1, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdLogica = Id FROM Subjects WHERE Code = '103002';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109182')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdParadigmas, '109182', 'Paradigmas de Lenguajes', 6, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdParadigmas = Id FROM Subjects WHERE Code = '109182';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103093')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdIngSoftware, '103093', 'Ingeniería de Software II', 7, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdIngSoftware = Id FROM Subjects WHERE Code = '103093';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103007')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdTecnicasProg, '103007', 'Técnicas de Programación', 2, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdTecnicasProg = Id FROM Subjects WHERE Code = '103007';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109184')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdElectronica, '109184', 'Electrónica Digital y Arquitectura de Computadores', 6, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdElectronica = Id FROM Subjects WHERE Code = '109184';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109104')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdSistemasEmbebidos, '109104', 'Sistemas Embebidos', 7, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdSistemasEmbebidos = Id FROM Subjects WHERE Code = '109104';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103004')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdTeoriaSistemas, '103004', 'Teoría de Sistemas', 2, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdTeoriaSistemas = Id FROM Subjects WHERE Code = '103004';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109183')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdBackend, '109183', 'Programación Back End', 6, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdBackend = Id FROM Subjects WHERE Code = '109183';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103126')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdRedes, '103126', 'Redes LAN', 6, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdRedes = Id FROM Subjects WHERE Code = '103126';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109180')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdBasesDatos, '109180', 'Bases de Datos I', 5, 3, 4, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdBasesDatos = Id FROM Subjects WHERE Code = '109180';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '104030')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdCalculoDiferencial, '104030', 'Cálculo Diferencial', 2, 3, 4, 1, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdCalculoDiferencial = Id FROM Subjects WHERE Code = '104030';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '151601')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdIngles, '151601', 'Inglés I', 1, 1, 2, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdIngles = Id FROM Subjects WHERE Code = '151601';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '105038')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdEtica, '105038', 'Ética', 1, 2, 2, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdEtica = Id FROM Subjects WHERE Code = '105038';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '105042')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdCultura, '105042', 'Cultura Política', 8, 2, 2, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdCultura = Id FROM Subjects WHERE Code = '105042';

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '100059')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdCompetencias, '100059', 'Competencias Comunicativas', 2, 3, 3, 0, 1, GETUTCDATE(), NULL);
ELSE SELECT @IdCompetencias = Id FROM Subjects WHERE Code = '100059';

-- ==========================================
-- 6. PROFESORES
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'ernesto@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdErnesto, 'Ernesto', 'Pérez', 'ernesto@autonoma.edu.co', '10000001', '3000000001', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdErnesto = Id FROM Teachers WHERE Email = 'ernesto@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'harold@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdHarold, 'Harold', 'Gómez', 'harold@autonoma.edu.co', '10000002', '3000000002', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdHarold = Id FROM Teachers WHERE Email = 'harold@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'marcela@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdMarcela, 'Marcela', 'Ríos', 'marcela@autonoma.edu.co', '10000003', '3000000003', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdMarcela = Id FROM Teachers WHERE Email = 'marcela@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'sebastian@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdSebastian, 'Sebastián', 'Castro', 'sebastian@autonoma.edu.co', '10000004', '3000000004', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdSebastian = Id FROM Teachers WHERE Email = 'sebastian@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'alejandra@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdAlejandra, 'Alejandra', 'López', 'alejandra@autonoma.edu.co', '10000005', '3000000005', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdAlejandra = Id FROM Teachers WHERE Email = 'alejandra@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'simon@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdSimon, 'Simón', 'Restrepo', 'simon@autonoma.edu.co', '10000006', '3000000006', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdSimon = Id FROM Teachers WHERE Email = 'simon@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'beatriz@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdBeatriz, 'Beatriz', 'Mejía', 'beatriz@autonoma.edu.co', '10000007', '3000000007', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdBeatriz = Id FROM Teachers WHERE Email = 'beatriz@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'adriana@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdAdriana, 'Adriana', 'Muñoz', 'adriana@autonoma.edu.co', '10000008', '3000000008', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdAdriana = Id FROM Teachers WHERE Email = 'adriana@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'julian@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdJulian, 'Julián', 'Giraldo', 'julian@autonoma.edu.co', '10000009', '3000000009', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdJulian = Id FROM Teachers WHERE Email = 'julian@autonoma.edu.co';

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'mauricio@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@IdMauricio, 'Mauricio', 'Mejía', 'mauricio@autonoma.edu.co', '10000010', '3000000010', 1, GETUTCDATE(), NULL);
ELSE SELECT @IdMauricio = Id FROM Teachers WHERE Email = 'mauricio@autonoma.edu.co';

-- ==========================================
-- 7. AULAS / CLASSROOMS
-- Fundadores y Edificio F son el mismo edificio.
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Classrooms WHERE Code = 'F-101')
BEGIN
    INSERT INTO Classrooms (Id, Code, Name, Building, Floor, Capacity, Type, IsActive, CreatedAt, UpdatedAt)
    VALUES
    (NEWID(), 'F-101', 'FUNDADORES 101', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-102', 'FUNDADORES 102', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-103', 'FUNDADORES 103', 'Fundadores / Edificio F', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-201', 'FUNDADORES 201', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-202', 'FUNDADORES 202', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-203', 'FUNDADORES 203', 'Fundadores / Edificio F', 2, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-301', 'FUNDADORES 301', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-302', 'FUNDADORES 302', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-303', 'FUNDADORES 303', 'Fundadores / Edificio F', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-401', 'INFORMÁTICA F-401', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-402', 'INFORMÁTICA F-402', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-403', 'INFORMÁTICA F-403', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-404', 'INFORMÁTICA F-404', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-405', 'INFORMÁTICA F-405', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-406', 'INFORMÁTICA F-406', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-407', 'INFORMÁTICA F-407', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-408', 'INFORMÁTICA F-408', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-409', 'INFORMÁTICA F-409', 'Fundadores / Edificio F', 4, 30, 'Sala de informática', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-501', 'FUNDADORES 501', 'Fundadores / Edificio F', 5, 30, 'Aula de maestría', 1, GETUTCDATE(), NULL),
    (NEWID(), 'F-502', 'FUNDADORES 502', 'Fundadores / Edificio F', 5, 30, 'Aula de maestría', 1, GETUTCDATE(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM Classrooms WHERE Code = 'SAC-101')
BEGIN
    INSERT INTO Classrooms (Id, Code, Name, Building, Floor, Capacity, Type, IsActive, CreatedAt, UpdatedAt)
    VALUES
    (NEWID(), 'SAC-101', 'SACATÍN 101', 'Sacatín', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'SAC-102', 'SACATÍN 102', 'Sacatín', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'SAC-103', 'SACATÍN 103', 'Sacatín', 1, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'SAC-LAB-ELEC', 'LABORATORIO DE ELECTRÓNICA', 'Sacatín', 1, 25, 'Laboratorio de electrónica', 1, GETUTCDATE(), NULL),
    (NEWID(), 'SAC-301', 'SACATÍN 301', 'Sacatín', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'SAC-302', 'SACATÍN 302', 'Sacatín', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'SAC-303', 'SACATÍN 303', 'Sacatín', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'SAC-304', 'SACATÍN 304', 'Sacatín', 3, 35, 'Aula teórica general', 1, GETUTCDATE(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM Classrooms WHERE Code = 'CB-101')
BEGIN
    INSERT INTO Classrooms (Id, Code, Name, Building, Floor, Capacity, Type, IsActive, CreatedAt, UpdatedAt)
    VALUES
    (NEWID(), 'CB-101', 'CASA BAVARIA 101', 'Casa Bavaria', 1, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'CB-102', 'CASA BAVARIA 102', 'Casa Bavaria', 1, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'CB-103', 'CASA BAVARIA 103', 'Casa Bavaria', 1, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'CB-201', 'CASA BAVARIA 201', 'Casa Bavaria', 2, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'CB-202', 'CASA BAVARIA 202', 'Casa Bavaria', 2, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'CB-203', 'CASA BAVARIA 203', 'Casa Bavaria', 2, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL),
    (NEWID(), 'CB-204', 'CASA BAVARIA 204', 'Casa Bavaria', 2, 30, 'Aula teórica general', 1, GETUTCDATE(), NULL);
END

-- Obtener IDs de aulas necesarias para horarios
SELECT @IdF401 = Id FROM Classrooms WHERE Code = 'F-401';
SELECT @IdF402 = Id FROM Classrooms WHERE Code = 'F-402';
SELECT @IdF405 = Id FROM Classrooms WHERE Code = 'F-405';
SELECT @IdSAC101 = Id FROM Classrooms WHERE Code = 'SAC-101';
SELECT @IdCB101 = Id FROM Classrooms WHERE Code = 'CB-101';
SELECT @IdCB201 = Id FROM Classrooms WHERE Code = 'CB-201';

-- ==========================================
-- 8. REGLAS TAPSI
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM TapsiRules WHERE RuleType = 'MateriasFijas')
BEGIN
    INSERT INTO TapsiRules (Id, RuleType, Description, [Value], IsActive, CreatedAt, UpdatedAt)
    VALUES (
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
    INSERT INTO TapsiRules (Id, RuleType, Description, [Value], IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'CreditosDiurna', 'Máximo de créditos permitidos para jornada diurna.', '18', 1, GETUTCDATE(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM TapsiRules WHERE RuleType = 'CreditosExtendida')
BEGIN
    INSERT INTO TapsiRules (Id, RuleType, Description, [Value], IsActive, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'CreditosExtendida', 'Máximo de créditos permitidos para jornada extendida/nocturna.', '15', 1, GETUTCDATE(), NULL);
END

-- ==========================================
-- 9. ASIGNAR MATERIAS A PROFESORES
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdMarcela AND SubjectId = @IdPOO)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdMarcela, @IdPOO, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdSebastian AND SubjectId = @IdTecnicasProg)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdSebastian, @IdTecnicasProg, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdSimon AND SubjectId = @IdBackend)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdSimon, @IdBackend, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdAlejandra AND SubjectId = @IdCalculoDiferencial)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdAlejandra, @IdCalculoDiferencial, 'Tiempo Completo');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdBeatriz AND SubjectId = @IdIngles)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdBeatriz, @IdIngles, 'Cátedra');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdAdriana AND SubjectId = @IdEtica)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdAdriana, @IdEtica, 'Cátedra');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdJulian AND SubjectId = @IdCultura)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdJulian, @IdCultura, 'Cátedra');

IF NOT EXISTS (SELECT 1 FROM TeacherSubjects WHERE TeacherId = @IdMauricio AND SubjectId = @IdBasesDatos)
    INSERT INTO TeacherSubjects (TeacherId, SubjectId, ContractType) VALUES (@IdMauricio, @IdBasesDatos, 'Tiempo Completo');

-- ==========================================
-- 10. DISPONIBILIDADES DE PROFESORES
-- Day: 1=Lunes, 2=Martes, 3=Miercoles, 4=Jueves, 5=Viernes, 6=Sabado
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdMarcela AND [Day] = 1 AND StartTime = '08:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, [Day], StartTime, EndTime, MaxTeachingHours)
    VALUES (NEWID(), @IdMarcela, 1, '08:00:00', '12:00:00', 4);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdSebastian AND [Day] = 2 AND StartTime = '10:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, [Day], StartTime, EndTime, MaxTeachingHours)
    VALUES (NEWID(), @IdSebastian, 2, '10:00:00', '12:00:00', 2);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdSimon AND [Day] = 3 AND StartTime = '14:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, [Day], StartTime, EndTime, MaxTeachingHours)
    VALUES (NEWID(), @IdSimon, 3, '14:00:00', '16:00:00', 2);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdAlejandra AND [Day] = 4 AND StartTime = '08:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, [Day], StartTime, EndTime, MaxTeachingHours)
    VALUES (NEWID(), @IdAlejandra, 4, '08:00:00', '12:00:00', 4);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdBeatriz AND [Day] = 2 AND StartTime = '08:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, [Day], StartTime, EndTime, MaxTeachingHours)
    VALUES (NEWID(), @IdBeatriz, 2, '08:00:00', '10:00:00', 2);

IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE TeacherId = @IdAdriana AND [Day] = 3 AND StartTime = '10:00:00')
    INSERT INTO TeacherAvailabilities (Id, TeacherId, [Day], StartTime, EndTime, MaxTeachingHours)
    VALUES (NEWID(), @IdAdriana, 3, '10:00:00', '12:00:00', 2);

-- ==========================================
-- 11. ASIGNACIONES DE AULAS
-- ==========================================
IF NOT EXISTS (
    SELECT 1 FROM ClassroomAssignments
    WHERE ClassroomId = @IdF401
      AND CAST([Date] AS DATE) = '2026-02-02'
      AND StartTime = '08:00:00'
)
BEGIN
    INSERT INTO ClassroomAssignments (ClassroomId, [Date], StartTime, EndTime)
    VALUES
    (@IdF401, '2026-02-02', '08:00:00', '10:00:00'),
    (@IdF402, '2026-02-02', '10:00:00', '12:00:00'),
    (@IdSAC101, '2026-02-03', '08:00:00', '10:00:00'),
    (@IdCB101, '2026-02-04', '10:00:00', '12:00:00');
END

-- ==========================================
-- 12. HORARIOS
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Schedules WHERE SubjectId = @IdPOO AND TeacherId = @IdMarcela AND [Day] = 1 AND StartTime = '08:00:00')
BEGIN
    INSERT INTO Schedules (Id, SubjectId, TeacherId, ClassroomId, [Day], StartTime, EndTime, AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt)
    VALUES (NEWID(), @IdPOO, @IdMarcela, @IdF401, 1, '08:00:00', '10:00:00', 'Ingeniería de Sistemas', 'Diurna', 3, 'Draft', GETUTCDATE(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM Schedules WHERE SubjectId = @IdTecnicasProg AND TeacherId = @IdSebastian AND [Day] = 2 AND StartTime = '10:00:00')
BEGIN
    INSERT INTO Schedules (Id, SubjectId, TeacherId, ClassroomId, [Day], StartTime, EndTime, AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt)
    VALUES (NEWID(), @IdTecnicasProg, @IdSebastian, @IdF402, 2, '10:00:00', '12:00:00', 'Ingeniería de Sistemas', 'Diurna', 2, 'Draft', GETUTCDATE(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM Schedules WHERE SubjectId = @IdBackend AND TeacherId = @IdSimon AND [Day] = 3 AND StartTime = '14:00:00')
BEGIN
    INSERT INTO Schedules (Id, SubjectId, TeacherId, ClassroomId, [Day], StartTime, EndTime, AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt)
    VALUES (NEWID(), @IdBackend, @IdSimon, @IdF405, 3, '14:00:00', '16:00:00', 'Ingeniería de Sistemas', 'Diurna', 6, 'Draft', GETUTCDATE(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM Schedules WHERE SubjectId = @IdCalculoDiferencial AND TeacherId = @IdAlejandra AND [Day] = 4 AND StartTime = '08:00:00')
BEGIN
    INSERT INTO Schedules (Id, SubjectId, TeacherId, ClassroomId, [Day], StartTime, EndTime, AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt)
    VALUES (NEWID(), @IdCalculoDiferencial, @IdAlejandra, @IdSAC101, 4, '08:00:00', '10:00:00', 'Ingeniería de Sistemas', 'Diurna', 2, 'Draft', GETUTCDATE(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM Schedules WHERE SubjectId = @IdIngles AND TeacherId = @IdBeatriz AND [Day] = 2 AND StartTime = '08:00:00')
BEGIN
    INSERT INTO Schedules (Id, SubjectId, TeacherId, ClassroomId, [Day], StartTime, EndTime, AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt)
    VALUES (NEWID(), @IdIngles, @IdBeatriz, @IdCB101, 2, '08:00:00', '10:00:00', 'Ingeniería de Sistemas', 'Diurna', 1, 'Draft', GETUTCDATE(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM Schedules WHERE SubjectId = @IdEtica AND TeacherId = @IdAdriana AND [Day] = 3 AND StartTime = '10:00:00')
BEGIN
    INSERT INTO Schedules (Id, SubjectId, TeacherId, ClassroomId, [Day], StartTime, EndTime, AcademicProgram, Shift, Semester, Status, CreatedAt, UpdatedAt)
    VALUES (NEWID(), @IdEtica, @IdAdriana, @IdCB201, 3, '10:00:00', '12:00:00', 'Ingeniería de Sistemas', 'Diurna', 1, 'Draft', GETUTCDATE(), NULL);
END

COMMIT TRANSACTION;

PRINT 'Script completo ejecutado correctamente: 12 tablas revisadas/creadas y datos semilla cargados.';
GO

-- Consulta de verificacion
SELECT COUNT(*) AS TotalRoles FROM Roles;
SELECT COUNT(*) AS TotalUsuarios FROM Users;
SELECT COUNT(*) AS TotalProgramas FROM AcademicPrograms;
SELECT COUNT(*) AS TotalSemestres FROM ProgramSemesters;
SELECT COUNT(*) AS TotalMaterias FROM Subjects;
SELECT COUNT(*) AS TotalProfesores FROM Teachers;
SELECT COUNT(*) AS TotalAulas FROM Classrooms;
SELECT COUNT(*) AS TotalReglasTapsi FROM TapsiRules;
SELECT COUNT(*) AS TotalProfesorMateria FROM TeacherSubjects;
SELECT COUNT(*) AS TotalDisponibilidades FROM TeacherAvailabilities;
SELECT COUNT(*) AS TotalAsignacionesAulas FROM ClassroomAssignments;
SELECT COUNT(*) AS TotalHorarios FROM Schedules;
