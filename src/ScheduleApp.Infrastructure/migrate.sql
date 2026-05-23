IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511193213_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Username] nvarchar(50) NOT NULL,
        [IdentityDocument] nvarchar(20) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Role] nvarchar(50) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511193213_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511193213_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_IdentityDocument] ON [Users] ([IdentityDocument]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511193213_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260511193213_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260511193213_InitialCreate', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512002503_CreateRolesTable'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Role');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Users] DROP COLUMN [Role];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512002503_CreateRolesTable'
)
BEGIN
    ALTER TABLE [Users] ADD [RoleId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512002503_CreateRolesTable'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512002503_CreateRolesTable'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([Id], [Name])
    VALUES (''11111111-1111-1111-1111-111111111111'', N''Administrador''),
    (''22222222-2222-2222-2222-222222222222'', N''Coordinador'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512002503_CreateRolesTable'
)
BEGIN
    CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512002503_CreateRolesTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512002503_CreateRolesTable'
)
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512002503_CreateRolesTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260512002503_CreateRolesTable', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513222127_CreateSubjectsTable'
)
BEGIN
    CREATE TABLE [Subjects] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(20) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Semester] int NOT NULL,
        [Credits] int NOT NULL,
        [WeeklyHours] int NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513222127_CreateSubjectsTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Subjects_Code] ON [Subjects] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260513222127_CreateSubjectsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260513222127_CreateSubjectsTable', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515145823_CreateAssignmentsTable'
)
BEGIN
    CREATE TABLE [Assignments] (
        [Id] int NOT NULL IDENTITY,
        [Teacher] nvarchar(100) NOT NULL,
        [Subject] nvarchar(100) NOT NULL,
        [Classroom] nvarchar(100) NOT NULL,
        [Day] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515145823_CreateAssignmentsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260515145823_CreateAssignmentsTable', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516144445_AddTapsiRules'
)
BEGIN
    ALTER TABLE [Subjects] ADD [IsTapsi] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516144445_AddTapsiRules'
)
BEGIN
    CREATE TABLE [TapsiRules] (
        [Id] uniqueidentifier NOT NULL,
        [RuleType] nvarchar(50) NOT NULL,
        [Description] nvarchar(300) NOT NULL,
        [Value] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_TapsiRules] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516144445_AddTapsiRules'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Description', N'IsActive', N'RuleType', N'UpdatedAt', N'Value') AND [object_id] = OBJECT_ID(N'[TapsiRules]'))
        SET IDENTITY_INSERT [TapsiRules] ON;
    EXEC(N'INSERT INTO [TapsiRules] ([Id], [CreatedAt], [Description], [IsActive], [RuleType], [UpdatedAt], [Value])
    VALUES (''aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'', ''2025-01-01T00:00:00.0000000Z'', N''Las materias TAPSI no pueden superar 4 horas diarias.'', CAST(1 AS bit), N''MAX_DAILY_HOURS'', NULL, N''4''),
    (''bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'', ''2025-01-01T00:00:00.0000000Z'', N''Las materias TAPSI solo pueden asignarse en días hábiles.'', CAST(1 AS bit), N''ALLOWED_DAYS'', NULL, N''Lunes,Martes,Miércoles,Jueves,Viernes''),
    (''cccccccc-cccc-cccc-cccc-cccccccccccc'', ''2025-01-01T00:00:00.0000000Z'', N''Las materias TAPSI deben dictarse entre 7:00 y 18:00.'', CAST(1 AS bit), N''TIME_RANGE'', NULL, N''07:00-18:00'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Description', N'IsActive', N'RuleType', N'UpdatedAt', N'Value') AND [object_id] = OBJECT_ID(N'[TapsiRules]'))
        SET IDENTITY_INSERT [TapsiRules] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516144445_AddTapsiRules'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260516144445_AddTapsiRules', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516200805_CreateTeachersTable'
)
BEGIN
    CREATE TABLE [Teachers] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [IdentityDocument] nvarchar(20) NOT NULL,
        [PhoneNumber] nvarchar(20) NULL,
        [Specialty] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Teachers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516200805_CreateTeachersTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Teachers_Email] ON [Teachers] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516200805_CreateTeachersTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Teachers_IdentityDocument] ON [Teachers] ([IdentityDocument]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516200805_CreateTeachersTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260516200805_CreateTeachersTable', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'FullName');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Teachers] DROP COLUMN [FullName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Specialty');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Teachers] DROP COLUMN [Specialty];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    ALTER TABLE [Teachers] ADD [FirstName] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    ALTER TABLE [Teachers] ADD [LastName] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    CREATE TABLE [TeacherAvailabilities] (
        [Id] uniqueidentifier NOT NULL,
        [TeacherId] uniqueidentifier NOT NULL,
        [Day] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [MaxTeachingHours] int NOT NULL,
        CONSTRAINT [PK_TeacherAvailabilities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeacherAvailabilities_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    CREATE TABLE [TeacherSubjects] (
        [TeacherId] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [ContractType] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_TeacherSubjects] PRIMARY KEY ([TeacherId], [SubjectId]),
        CONSTRAINT [FK_TeacherSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeacherSubjects_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    CREATE INDEX [IX_TeacherAvailabilities_TeacherId] ON [TeacherAvailabilities] ([TeacherId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    CREATE INDEX [IX_TeacherSubjects_SubjectId] ON [TeacherSubjects] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516222606_NormalizacionTablaDocentes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260516222606_NormalizacionTablaDocentes', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516224957_CrearTablaAulas'
)
BEGIN
    CREATE TABLE [Classrooms] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(20) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Building] nvarchar(100) NOT NULL,
        [Floor] int NOT NULL,
        [Capacity] int NOT NULL,
        [Type] nvarchar(50) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Classrooms] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516224957_CrearTablaAulas'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Classrooms_Code] ON [Classrooms] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516224957_CrearTablaAulas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260516224957_CrearTablaAulas', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516230752_CrearTablaHorarios'
)
BEGIN
    CREATE TABLE [Schedules] (
        [Id] uniqueidentifier NOT NULL,
        [SubjectId] uniqueidentifier NOT NULL,
        [TeacherId] uniqueidentifier NOT NULL,
        [ClassroomId] uniqueidentifier NOT NULL,
        [Day] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [AcademicProgram] nvarchar(100) NOT NULL,
        [Shift] nvarchar(50) NOT NULL,
        [Semester] int NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Schedules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Schedules_Classrooms_ClassroomId] FOREIGN KEY ([ClassroomId]) REFERENCES [Classrooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Schedules_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Schedules_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516230752_CrearTablaHorarios'
)
BEGIN
    CREATE INDEX [IX_Schedules_ClassroomId] ON [Schedules] ([ClassroomId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516230752_CrearTablaHorarios'
)
BEGIN
    CREATE INDEX [IX_Schedules_SubjectId] ON [Schedules] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516230752_CrearTablaHorarios'
)
BEGIN
    CREATE INDEX [IX_Schedules_TeacherId] ON [Schedules] ([TeacherId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516230752_CrearTablaHorarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260516230752_CrearTablaHorarios', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516232232_CrearTablaProgramasAcademicos'
)
BEGIN
    CREATE TABLE [AcademicPrograms] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(20) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Shift] nvarchar(50) NOT NULL,
        [TotalSemesters] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_AcademicPrograms] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516232232_CrearTablaProgramasAcademicos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AcademicPrograms_Code] ON [AcademicPrograms] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516232232_CrearTablaProgramasAcademicos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260516232232_CrearTablaProgramasAcademicos', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516233549_CrearTablaProgramasSemestres'
)
BEGIN
    CREATE TABLE [ProgramSemesters] (
        [Id] uniqueidentifier NOT NULL,
        [AcademicProgramId] uniqueidentifier NOT NULL,
        [SemesterNumber] int NOT NULL,
        [MaxCredits] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ProgramSemesters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProgramSemesters_AcademicPrograms_AcademicProgramId] FOREIGN KEY ([AcademicProgramId]) REFERENCES [AcademicPrograms] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516233549_CrearTablaProgramasSemestres'
)
BEGIN
    CREATE INDEX [IX_ProgramSemesters_AcademicProgramId] ON [ProgramSemesters] ([AcademicProgramId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516233549_CrearTablaProgramasSemestres'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260516233549_CrearTablaProgramasSemestres', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520005511_AddIsDeletedToAcademicPrograms'
)
BEGIN
    ALTER TABLE [AcademicPrograms] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520005511_AddIsDeletedToAcademicPrograms'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520005511_AddIsDeletedToAcademicPrograms', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520014220_AddIsDeletedToUsers'
)
BEGIN
    ALTER TABLE [Users] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520014220_AddIsDeletedToUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520014220_AddIsDeletedToUsers', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173718_AddIsDeletedColumnToUsers'
)
BEGIN
    ALTER TABLE [Subjects] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520173718_AddIsDeletedColumnToUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520173718_AddIsDeletedColumnToUsers', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520183349_AddIsDeletedToSubjects'
)
BEGIN
    ALTER TABLE [Subjects] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520183349_AddIsDeletedToSubjects'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520183349_AddIsDeletedToSubjects', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    ALTER TABLE [TeacherSubjects] DROP CONSTRAINT [FK_TeacherSubjects_Subjects_SubjectId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'IsDeleted');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Subjects] DROP COLUMN [IsDeleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    ALTER TABLE [Subjects] ADD [SpecialtyId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    CREATE TABLE [Specialties] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [Icon] nvarchar(50) NULL,
        [IsActive] bit NOT NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Specialties] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    CREATE INDEX [IX_Subjects_SpecialtyId] ON [Subjects] ([SpecialtyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Specialties_Name] ON [Specialties] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    ALTER TABLE [Subjects] ADD CONSTRAINT [FK_Subjects_Specialties_SpecialtyId] FOREIGN KEY ([SpecialtyId]) REFERENCES [Specialties] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    ALTER TABLE [TeacherSubjects] ADD CONSTRAINT [FK_TeacherSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523230706_AddSpecialtyEntityToDatabase'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260523230706_AddSpecialtyEntityToDatabase', N'10.0.7');
END;

COMMIT;
GO

