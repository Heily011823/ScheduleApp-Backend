-- ==========================================================================
-- SCRIPT DE INSERCIÓN DE DATOS SEMILLA (SEED DATA) - TOTALMENTE CORREGIDO
-- Selecciona tu base de datos local antes de ejecutar.
-- ==========================================================================

-- Variables para mapear Roles
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @CoordRoleId UNIQUEIDENTIFIER = NEWID();

-- Variables para mapear Profesores
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

-- Variables para mapear Materias Clave
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

-- ==========================================
-- 1. INSERTAR ROLES
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Administrador')
BEGIN
    INSERT INTO Roles (Id, Name) VALUES (@AdminRoleId, 'Administrador');
END
ELSE BEGIN SELECT @AdminRoleId = Id FROM Roles WHERE Name = 'Administrador'; END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Coordinador')
BEGIN
    INSERT INTO Roles (Id, Name) VALUES (@CoordRoleId, 'Coordinador');
END
ELSE BEGIN SELECT @CoordRoleId = Id FROM Roles WHERE Name = 'Coordinador'; END

-- ==========================================
-- 2. INSERTAR USUARIOS (ADMIN Y COORDINADORA LINA MARÍA)
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
-- 3. INSERTAR MATERIAS (SUBJECTS)
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103018')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdPOO, '103018', 'Programación Orientada a Objetos', 3, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdPOO = Id FROM Subjects WHERE Code = '103018'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103008')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdFundamentosPOO, '103008', 'Fundamentos de POO', 2, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdFundamentosPOO = Id FROM Subjects WHERE Code = '103008'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103127')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdSistemasOperativos, '103127', 'Sistemas Operativos', 4, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdSistemasOperativos = Id FROM Subjects WHERE Code = '103127'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103002')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdLogica, '103002', 'Lógica de Programación', 1, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdLogica = Id FROM Subjects WHERE Code = '103002'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109182')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdParadigmas, '109182', 'Paradigmas de Lenguajes', 5, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdParadigmas = Id FROM Subjects WHERE Code = '109182'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109186')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdProcesadores, '109186', 'Procesadores de Lenguajes', 6, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdProcesadores = Id FROM Subjects WHERE Code = '109186'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103093')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdIngSoftware, '103093', 'Ingeniería de Software II', 5, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdIngSoftware = Id FROM Subjects WHERE Code = '103093'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103007')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdTecnicasProg, '103007', 'Técnicas de Programación', 2, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdTecnicasProg = Id FROM Subjects WHERE Code = '103007'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109184')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdElectronica, '109184', 'Electrónica Digital y Arq. De Computadores', 4, 4, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdElectronica = Id FROM Subjects WHERE Code = '109184'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109104')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdSistemasEmbebidos, '109104', 'Sistemas Embebidos', 6, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdSistemasEmbebidos = Id FROM Subjects WHERE Code = '109104'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103004')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdTeoriaSistemas, '103004', 'Teoría de Sistemas', 3, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdTeoriaSistemas = Id FROM Subjects WHERE Code = '103004'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109189')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdMoviles, '109189', 'Programación de Dispositivos Móviles', 7, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdMoviles = Id FROM Subjects WHERE Code = '109189'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109183')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdBackend, '109183', 'Programación Back End', 6, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdBackend = Id FROM Subjects WHERE Code = '109183'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103021')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdDisenoAlgoritmos, '103021', 'Diseño de Algoritmos', 4, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdDisenoAlgoritmos = Id FROM Subjects WHERE Code = '103021'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109188')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdCienciaDatos, '109188', 'Ciencia de los Datos', 8, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdCienciaDatos = Id FROM Subjects WHERE Code = '109188'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109185')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdModelamiento, '109185', 'Modelamiento y Simulación', 7, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdModelamiento = Id FROM Subjects WHERE Code = '109185'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103126')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdRedes, '103126', 'Redes LAN / Enrutamiento', 5, 3, 4, 1, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdRedes = Id FROM Subjects WHERE Code = '103126'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103125')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdSistemasInfo, '103125', 'Sistemas de Información', 5, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdSistemasInfo = Id FROM Subjects WHERE Code = '103125'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '104003')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdDiscretas, '104003', 'Matemáticas Discretas', 2, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdDiscretas = Id FROM Subjects WHERE Code = '104003'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '109181')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdFrontEnd, '109181', 'Programación Front End', 6, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdFrontEnd = Id FROM Subjects WHERE Code = '109181'; END

IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Code = '103029')
    INSERT INTO Subjects (Id, Code, Name, Semester, Credits, WeeklyHours, IsTapsi, IsActive, CreatedAt) VALUES (@IdBasesDatos, '103029', 'Bases de Datos I', 4, 3, 4, 0, 1, GETUTCDATE());
ELSE BEGIN SELECT @IdBasesDatos = Id FROM Subjects WHERE Code = '103029'; END


-- ==========================================
-- 4. INSERTAR PROFESORES (TEACHERS)
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'ernesto@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdErnesto, 'Ernesto', 'Pérez', 'ernesto@autonoma.edu.co', '10001', '3001', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdErnesto = Id FROM Teachers WHERE Email = 'ernesto@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'harold@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdHarold, 'Harold', 'Gómez', 'harold@autonoma.edu.co', '10002', '3002', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdHarold = Id FROM Teachers WHERE Email = 'harold@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'marcela@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdMarcela, 'Marcela', 'Ríos', 'marcela@autonoma.edu.co', '10003', '3003', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdMarcela = Id FROM Teachers WHERE Email = 'marcela@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'sebastian@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdSebastian, 'Sebastián', 'Castro', 'sebastian@autonoma.edu.co', '10004', '3004', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdSebastian = Id FROM Teachers WHERE Email = 'sebastian@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'alejandra@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdAlejandra, 'Alejandra', 'López', 'alejandra@autonoma.edu.co', '10005', '3005', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdAlejandra = Id FROM Teachers WHERE Email = 'alejandra@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'simon@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdSimon, 'Simón', 'Restrepo', 'simon@autonoma.edu.co', '10006', '3006', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdSimon = Id FROM Teachers WHERE Email = 'simon@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'beatriz@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdBeatriz, 'Beatriz', 'Mejía', 'beatriz@autonoma.edu.co', '10007', '3007', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdBeatriz = Id FROM Teachers WHERE Email = 'beatriz@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'santiago@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdSantiago, 'Santiago', 'Toro', 'santiago@autonoma.edu.co', '10008', '3008', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdSantiago = Id FROM Teachers WHERE Email = 'santiago@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'juandavid@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdJuanDavid, 'Juan David', 'Patiño', 'juandavid@autonoma.edu.co', '10009', '3009', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdJuanDavid = Id FROM Teachers WHERE Email = 'juandavid@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'adriana@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdAdriana, 'Adriana', 'Muñoz', 'adriana@autonoma.edu.co', '10010', '3010', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdAdriana = Id FROM Teachers WHERE Email = 'adriana@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'julian@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdJulian, 'Julián', 'Giraldo', 'julian@autonoma.edu.co', '10011', '3011', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdJulian = Id FROM Teachers WHERE Email = 'julian@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'nuevo@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdNuevo, 'Profesor', 'Nuevo', 'nuevo@autonoma.edu.co', '10012', '3012', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdNuevo = Id FROM Teachers WHERE Email = 'nuevo@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'lmarinu@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdLina, 'Lina', 'Marín', 'lmarinu@autonoma.edu.co', '10013', '3013', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdLina = Id FROM Teachers WHERE Email = 'lmarinu@autonoma.edu.co'; END

IF NOT EXISTS (SELECT 1 FROM Teachers WHERE Email = 'mauricio@autonoma.edu.co')
    INSERT INTO Teachers (Id, FirstName, LastName, Email, IdentityDocument, PhoneNumber, IsActive, CreatedAt) VALUES (@IdMauricio, 'Mauricio', 'Mejía', 'mauricio@autonoma.edu.co', '10014', '3014', 1, GETUTCDATE());
ELSE BEGIN SELECT @IdMauricio = Id FROM Teachers WHERE Email = 'mauricio@autonoma.edu.co'; END


-- ==========================================
-- 5. ASIGNAR MATERIAS A PROFESORES (TEACHER_SUBJECTS)
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


-- ==========================================
-- 6. INSERTAR DISPONIBILIDADES (TEACHER_AVAILABILITIES)
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


-- ==========================================
-- 7. PROGRAMAS ACADÉMICOS BASE
-- ==========================================
IF NOT EXISTS (SELECT 1 FROM AcademicPrograms WHERE Code = '1020')
    INSERT INTO AcademicPrograms (Id, Code, Name, Shift, TotalSemesters, IsActive, CreatedAt) VALUES (NEWID(), '1020', 'Ingeniería de Sistemas - Diurna', 'Diurna', 10, 1, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM AcademicPrograms WHERE Code = '1030')
    INSERT INTO AcademicPrograms (Id, Code, Name, Shift, TotalSemesters, IsActive, CreatedAt) VALUES (NEWID(), '1030', 'Ingeniería de Sistemas - Nocturna', 'Nocturna', 12, 1, GETUTCDATE());

PRINT '¡Datos semilla cargados perfectamente sin conflictos de nulos ni llaves foráneas!';