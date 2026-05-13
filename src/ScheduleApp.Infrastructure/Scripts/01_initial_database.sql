CREATE DATABASE ScheduleAppDb_Dev;
GO

USE ScheduleAppDb_Dev;
GO

CREATE TABLE Roles
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE Users
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    IdentityDocument NVARCHAR(20) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleId)
        REFERENCES Roles(Id)
);
GO

INSERT INTO Roles (Id, Name)
VALUES
('11111111-1111-1111-1111-111111111111', 'Administrador'),
('22222222-2222-2222-2222-222222222222', 'Coordinador');
GO

INSERT INTO Users
(
    Id, FullName, Email, Username, IdentityDocument,
    PasswordHash, RoleId, IsActive, CreatedAt
)
VALUES
(
    NEWID(),
    'Lina Maria Lopez',
    'llopezu@autonoma.edu.co',
    'llopezu',
    '1054929981',
    '$2a$11$jUB4GV3iGKPYVCbMmKsSjumuHyugqB5Gi4iGcwIIKDGoXhI4h7Yjy',
    '22222222-2222-2222-2222-222222222222',
    1,
    GETUTCDATE()
),
(
    NEWID(),
    'Heily Rios',
    'heilyy.riosa@autonoma.edu.co',
    'heilyy.riosa',
    '1000000001',
    '$2a$11$3gI0k635xfdd7utoQd88CO9Z52tGllZeoT6v/L/1Eh0Zk3e.FHkHy',
    '11111111-1111-1111-1111-111111111111',
    1,
    GETUTCDATE()
);
GO