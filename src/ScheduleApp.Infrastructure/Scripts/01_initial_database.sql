-- ==========================================================================
-- NOTA: Asegúrate de seleccionar tu base de datos local antes de ejecutar.
-- ==

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
        IdentityDocument NVARCHAR(50) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(50) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        -- Regla: El correo debe terminar obligatoriamente en @autonoma.edu.co
        CONSTRAINT CHK_Teacher_Email CHECK (Email LIKE '%@autonoma.edu.co')
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
        Value NVARCHAR(200) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
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
        IdentityDocument NVARCHAR(20) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id),
        -- Regla: El correo debe terminar obligatoriamente en @autonoma.edu.co
        CONSTRAINT CHK_User_Email CHECK (Email LIKE '%@autonoma.edu.co')
    );
END
GO
-- 8. Tabla: ProgramSemesters (Depende de AcademicPrograms)
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
GO

-- 9. Tabla: TeacherAvailabilities (Depende de Teachers)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeacherAvailabilities]') AND type in (N'U'))
BEGIN
    CREATE TABLE TeacherAvailabilities (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        Day INT NOT NULL, -- Mapea DayOfWeek de C#
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        MaxTeachingHours INT NOT NULL,
        CONSTRAINT FK_TeacherAvailabilities_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id)
    );
END
GO

-- 10. Tabla: ClassroomAssignments (Depende de Classrooms)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClassroomAssignments]') AND type in (N'U'))
BEGIN
    CREATE TABLE ClassroomAssignments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ClassroomId UNIQUEIDENTIFIER NOT NULL,
        Date DATETIME2 NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        CONSTRAINT FK_ClassroomAssignments_Classrooms FOREIGN KEY (ClassroomId) REFERENCES Classrooms(Id)
    );
END
GO

-- ==========================================
-- NIVEL 3: TABLAS RELACIONALES COMPLEJAS
-- ==========================================

-- 11. Tabla: TeacherSubjects (Depende de Teachers y Subjects)
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
GO

-- 12. Tabla: Schedules (Depende de Subjects, Teachers y Classrooms)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Schedules]') AND type in (N'U'))
BEGIN
    CREATE TABLE Schedules (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        SubjectId UNIQUEIDENTIFIER NOT NULL,
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        ClassroomId UNIQUEIDENTIFIER NOT NULL,
        Day INT NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        AcademicProgram NVARCHAR(200) NOT NULL,
        Shift NVARCHAR(50) NOT NULL,
        Semester INT NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Schedules_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(Id),
        CONSTRAINT FK_Schedules_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id),
        CONSTRAINT FK_Schedules_Classrooms FOREIGN KEY (ClassroomId) REFERENCES Classrooms(Id)
    );
END
GO