-- ==========================================================================
-- SCRIPT DE CREACIÓN DE TABLAS CORREGIDO CON IsDeleted
-- ==========================================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ========================
-- ROLES
-- ========================
-- Reemplaza las líneas que definen @AdminRoleId y @CoordRoleId
DECLARE @AdminRoleId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @CoordRoleId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

-- Luego inserta los roles solo si no existen (usando esos GUIDs)
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Administrador')
    INSERT INTO Roles (Id, Name) VALUES (@AdminRoleId, 'Administrador');
ELSE
    UPDATE Roles SET Id = @AdminRoleId WHERE Name = 'Administrador';

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Coordinador')
    INSERT INTO Roles (Id, Name) VALUES (@CoordRoleId, 'Coordinador');
ELSE
    UPDATE Roles SET Id = @CoordRoleId WHERE Name = 'Coordinador';

-- ========================
-- USERS
-- ========================
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
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id),
        CONSTRAINT CHK_User_Email CHECK (Email LIKE '%@autonoma.edu.co'),
        CONSTRAINT CHK_User_IdentityDocument CHECK (IdentityDocument NOT LIKE '%[^0-9]%' AND LEN(IdentityDocument) BETWEEN 8 AND 10)
    );
END

-- ========================
-- ACADEMIC PROGRAMS
-- ========================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AcademicPrograms]') AND type in (N'U'))
BEGIN
    CREATE TABLE AcademicPrograms (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Shift NVARCHAR(50) NOT NULL,
        TotalSemesters INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END

-- ========================
-- PROGRAM SEMESTERS
-- ========================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProgramSemesters]') AND type in (N'U'))
BEGIN
    CREATE TABLE ProgramSemesters (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        AcademicProgramId UNIQUEIDENTIFIER NOT NULL,
        SemesterNumber INT NOT NULL,
        MaxCredits INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_ProgramSemesters_AcademicPrograms FOREIGN KEY (AcademicProgramId) REFERENCES AcademicPrograms(Id)
    );
END

-- ========================
-- SUBJECTS
-- ========================
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
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END

-- ========================
-- TEACHERS
-- ========================
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
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT CHK_Teacher_Email CHECK (Email LIKE '%@autonoma.edu.co'),
        CONSTRAINT CHK_Teacher_IdentityDocument CHECK (IdentityDocument NOT LIKE '%[^0-9]%' AND LEN(IdentityDocument) BETWEEN 8 AND 10),
        CONSTRAINT CHK_Teacher_PhoneNumber CHECK (PhoneNumber LIKE '3[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
    );
END

-- ========================
-- CLASSROOMS
-- ========================
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
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END

-- ========================
-- TAPSI RULES
-- ========================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TapsiRules]') AND type in (N'U'))
BEGIN
    CREATE TABLE TapsiRules (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        RuleType NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Value NVARCHAR(MAX) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END

-- ========================
-- TEACHER SUBJECTS (Many-to-Many)
-- ========================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeacherSubjects]') AND type in (N'U'))
BEGIN
    CREATE TABLE TeacherSubjects (
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        SubjectId UNIQUEIDENTIFIER NOT NULL,
        ContractType NVARCHAR(100) NOT NULL,
        PRIMARY KEY (TeacherId, SubjectId),
        CONSTRAINT FK_TeacherSubjects_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id),
        CONSTRAINT FK_TeacherSubjects_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
    );
END

-- ========================
-- TEACHER AVAILABILITIES
-- ========================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeacherAvailabilities]') AND type in (N'U'))
BEGIN
    CREATE TABLE TeacherAvailabilities (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        [Day] INT NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        MaxTeachingHours INT NOT NULL,
        CONSTRAINT FK_TeacherAvailabilities_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id)
    );
END

-- ========================
-- CLASSROOM ASSIGNMENTS
-- ========================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClassroomAssignments]') AND type in (N'U'))
BEGIN
    CREATE TABLE ClassroomAssignments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ClassroomId UNIQUEIDENTIFIER NOT NULL,
        [Date] DATETIME2 NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        CONSTRAINT FK_ClassroomAssignments_Classrooms FOREIGN KEY (ClassroomId) REFERENCES Classrooms(Id)
    );
END

-- ========================
-- SCHEDULES
-- ========================
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
        UpdatedAt DATETIME2 NULL
    );
END

COMMIT TRANSACTION;
PRINT 'Todas las tablas creadas correctamente';