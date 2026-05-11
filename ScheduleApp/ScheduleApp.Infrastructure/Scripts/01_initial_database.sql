-- =============================================
-- DATABASE
-- =============================================

CREATE DATABASE ScheduleAppDb_Dev;
GO

USE ScheduleAppDb_Dev;
GO

-- =============================================
-- TABLE: Users
-- =============================================

CREATE TABLE Users
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,

    FullName NVARCHAR(100) NOT NULL,

    Email NVARCHAR(150) NOT NULL UNIQUE,

    Username NVARCHAR(50) NOT NULL UNIQUE,

    IdentityDocument NVARCHAR(20) NOT NULL UNIQUE,

    PasswordHash NVARCHAR(MAX) NOT NULL,

    Role NVARCHAR(50) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- =============================================
-- INITIAL USER
-- Password: 123456
-- =============================================

INSERT INTO Users
(
    Id,
    FullName,
    Email,
    Username,
    IdentityDocument,
    PasswordHash,
    Role,
    IsActive,
    CreatedAt
)
VALUES
(
    NEWID(),
    'Lina Maria Lopez',
    'llopezu@autonoma.edu.co',
    'llopezu',
    '1054929981',

   '$2a$11$jUB4GV3iGKPYVCbMmKsSjumuHyugqB5Gi4iGcwIIKDGoXhI4h7Yjy',

    'Coordinador',

    1,

    GETUTCDATE()
);
GO
-- =============================================
-- ADMIN USER
-- Password: 123456
-- =============================================

INSERT INTO Users
(
    Id,
    FullName,
    Email,
    Username,
    IdentityDocument,
    PasswordHash,
    Role,
    IsActive,
    CreatedAt
)
VALUES
(
    NEWID(),
    'Heily Rios',
    'heilyy.riosa@autonoma.edu.co',
    'heilyy.riosa',
    '1000000001',

    '$2a$11$3gI0k635xfdd7utoQd88CO9Z52tGllZeoT6v/L/1Eh0Zk3e.FHkHy',

    'Administrador',

    1,

    GETUTCDATE()
);
GO