-- ==========================================================================
-- SCRIPT DE CREACIÃ“N DE TABLAS
-- ScheduleApp
-- ==========================================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ========================
-- ROLES
-- ========================
IF OBJECT_ID(N'[dbo].[Roles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Roles] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL UNIQUE
    );
END;

-- ========================
-- USERS
-- ========================
IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
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

        CONSTRAINT FK_Users_Roles 
            FOREIGN KEY (RoleId) 
            REFERENCES [dbo].[Roles](Id),

        CONSTRAINT CHK_User_Email 
            CHECK (Email LIKE '%@autonoma.edu.co'),

        CONSTRAINT CHK_User_IdentityDocument 
            CHECK (
                IdentityDocument NOT LIKE '%[^0-9]%' 
                AND LEN(IdentityDocument) BETWEEN 8 AND 10
            )
    );
END;

-- ========================
-- ACADEMIC PROGRAMS
-- ========================
IF OBJECT_ID(N'[dbo].[AcademicPrograms]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AcademicPrograms] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Shift NVARCHAR(50) NOT NULL,
        TotalSemesters INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END;

-- ========================
-- PROGRAM SEMESTERS
-- ========================
IF OBJECT_ID(N'[dbo].[ProgramSemesters]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProgramSemesters] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        AcademicProgramId UNIQUEIDENTIFIER NOT NULL,
        SemesterNumber INT NOT NULL,
        MaxCredits INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_ProgramSemesters_AcademicPrograms 
            FOREIGN KEY (AcademicProgramId) 
            REFERENCES [dbo].[AcademicPrograms](Id)
    );
END;

-- ========================
-- SPECIALTIES
-- ========================
IF OBJECT_ID(N'[dbo].[Specialties]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Specialties] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        Icon NVARCHAR(50) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        DisplayOrder INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX IX_Specialties_Name 
    ON [dbo].[Specialties](Name);
END;

-- ========================
-- SUBJECTS
-- ========================
IF OBJECT_ID(N'[dbo].[Subjects]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Subjects] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Semester INT NOT NULL,
        Credits INT NOT NULL,
        WeeklyHours INT NOT NULL,
        IsTapsi BIT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        SpecialtyId UNIQUEIDENTIFIER NULL,

        CONSTRAINT FK_Subjects_Specialties 
            FOREIGN KEY (SpecialtyId) 
            REFERENCES [dbo].[Specialties](Id)
            ON DELETE SET NULL
    );

    CREATE INDEX IX_Subjects_SpecialtyId 
    ON [dbo].[Subjects](SpecialtyId);
END;

-- ========================
-- TEACHERS
-- ========================
IF OBJECT_ID(N'[dbo].[Teachers]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Teachers] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        IdentityDocument NVARCHAR(10) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(10) NOT NULL,
        SpecialtyId UNIQUEIDENTIFIER NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_Teachers_Specialties 
            FOREIGN KEY (SpecialtyId) 
            REFERENCES [dbo].[Specialties](Id)
            ON DELETE SET NULL
    );

    CREATE INDEX IX_Teachers_SpecialtyId 
    ON [dbo].[Teachers](SpecialtyId);
END;

-- ========================
-- TEACHER SPECIALTIES
-- ========================
IF OBJECT_ID(N'[dbo].[TeacherSpecialties]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TeacherSpecialties] (
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        SpecialtyId UNIQUEIDENTIFIER NOT NULL,
        AssignedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT PK_TeacherSpecialties 
            PRIMARY KEY (TeacherId, SpecialtyId),

        CONSTRAINT FK_TeacherSpecialties_Teachers 
            FOREIGN KEY (TeacherId) 
            REFERENCES [dbo].[Teachers](Id)
            ON DELETE CASCADE,

        CONSTRAINT FK_TeacherSpecialties_Specialties 
            FOREIGN KEY (SpecialtyId) 
            REFERENCES [dbo].[Specialties](Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_TeacherSpecialties_TeacherId 
    ON [dbo].[TeacherSpecialties](TeacherId);

    CREATE INDEX IX_TeacherSpecialties_SpecialtyId 
    ON [dbo].[TeacherSpecialties](SpecialtyId);
END;
-- ========================
-- CLASSROOMS
-- ========================
IF OBJECT_ID(N'[dbo].[Classrooms]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Classrooms] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
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
END;

-- ========================
-- TAPSI RULES
-- ========================
IF OBJECT_ID(N'[dbo].[TapsiRules]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TapsiRules] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        RuleType NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Value NVARCHAR(MAX) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
END;

-- ========================
-- TEACHER SUBJECTS
-- ========================
IF OBJECT_ID(N'[dbo].[TeacherSubjects]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TeacherSubjects] (
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        SubjectId UNIQUEIDENTIFIER NOT NULL,
        ContractType NVARCHAR(100) NOT NULL,

        CONSTRAINT PK_TeacherSubjects 
            PRIMARY KEY (TeacherId, SubjectId),

        CONSTRAINT FK_TeacherSubjects_Teachers 
            FOREIGN KEY (TeacherId) 
            REFERENCES [dbo].[Teachers](Id)
            ON DELETE CASCADE,

        CONSTRAINT FK_TeacherSubjects_Subjects 
            FOREIGN KEY (SubjectId) 
            REFERENCES [dbo].[Subjects](Id)
            ON DELETE CASCADE
    );
END;

-- ========================
-- TEACHER AVAILABILITIES
-- ========================
IF OBJECT_ID(N'[dbo].[TeacherAvailabilities]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TeacherAvailabilities] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        TeacherId UNIQUEIDENTIFIER NOT NULL,
        [Day] INT NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        MaxTeachingHours INT NOT NULL,

        CONSTRAINT FK_TeacherAvailabilities_Teachers 
            FOREIGN KEY (TeacherId) 
            REFERENCES [dbo].[Teachers](Id)
            ON DELETE CASCADE
    );
END;

-- ========================
-- CLASSROOM ASSIGNMENTS
-- ========================
IF OBJECT_ID(N'[dbo].[ClassroomAssignments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ClassroomAssignments] (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClassroomId UNIQUEIDENTIFIER NOT NULL,
        [Date] DATETIME2 NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,

        CONSTRAINT FK_ClassroomAssignments_Classrooms 
            FOREIGN KEY (ClassroomId) 
            REFERENCES [dbo].[Classrooms](Id)
            ON DELETE CASCADE
    );
END;

-- ========================
-- SCHEDULES
-- ========================
IF OBJECT_ID(N'[dbo].[Schedules]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Schedules] (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
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
        SubjectId1 UNIQUEIDENTIFIER NULL,

        CONSTRAINT FK_Schedules_Subjects 
            FOREIGN KEY (SubjectId) 
            REFERENCES [dbo].[Subjects](Id),

        CONSTRAINT FK_Schedules_Teachers 
            FOREIGN KEY (TeacherId) 
            REFERENCES [dbo].[Teachers](Id),

        CONSTRAINT FK_Schedules_Classrooms 
            FOREIGN KEY (ClassroomId) 
            REFERENCES [dbo].[Classrooms](Id)
    );
END;

COMMIT TRANSACTION;
