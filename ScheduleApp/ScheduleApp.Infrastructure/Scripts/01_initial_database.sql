USE ScheduleAppDb_Dev;
GO

INSERT INTO Users
(
    Id,
    FullName,
    Email,
    Username,
    IdentityDocument,
    PasswordHash,
    RoleId,
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
    '22222222-2222-2222-2222-222222222222',
    1,
    GETUTCDATE()
);
GO

INSERT INTO Users
(
    Id,
    FullName,
    Email,
    Username,
    IdentityDocument,
    PasswordHash,
    RoleId,
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
    '11111111-1111-1111-1111-111111111111',
    1,
    GETUTCDATE()
);
GO