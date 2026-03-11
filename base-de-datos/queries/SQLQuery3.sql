

CREATE DATABASE DiscordClone;
GO

USE DiscordClone;
GO

CREATE TABLE Roles (
	RoleId INT IDENTITY(1, 1) NOT NULL,
	Code NVARCHAR(10) NOT NULL,
	ShowName NVARCHAR(100) NOT NULL,
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
);
GO

CREATE TABLE UserStatusType (
	UserStatusTypeId INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	Code NVARCHAR(10) NOT NULL,
	ShowName NVARCHAR(11) NOT NULL,
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

INSERT INTO UserStatusType (Code, ShowName)
VALUES
('online',		 'En línea'),
('not_disturb',  'No molestar'),
('idle',		 'Ausente'),
('ghost',		 'Invisible');
GO

CREATE TABLE Users (
	UserId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	Username NVARCHAR(32) NOT NULL,
	DisplayName NVARCHAR(100) NOT NULL,
	Description NVARCHAR(255) NULL,
	StatusType INT NOT NULL REFERENCES UserStatusType(UserStatusTypeId) DEFAULT (1), -- online
	StatusTime INT NULL,
	StatusContent NVARCHAR(50) NULL DEFAULT ('Hi, there!'),
	AvatarURL NVARCHAR(255) NULL,
	BannerURL NVARCHAR(255) NULL,
	-- RoleId INT NOT NULL REFERENCES Roles (RoleId),
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
	-- CONSTRAINT FK_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (RoleId)
);
GO

CREATE TABLE Collections (
	CollectionId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	Name NVARCHAR(50) NOT NULL,
	Description NVARCHAR(100) NOT NULL DEFAULT('This is my collection!'),
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
	DeletedAt DATETIME2,
);
GO

CREATE TABLE Items (
	ItemId INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(50) NOT NULL UNIQUE,
	CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
)
GO

INSERT INTO Items (Name)
VALUES
('Hollow Knight'), -- Me pago 10$
('osu!'); -- Este tambien me pago 10$

CREATE TABLE CollectionsItems (
	CollectionId UNIQUEIDENTIFIER NOT NULL REFERENCES Collections (CollectionId),
	ItemId INT NOT NULL REFERENCES Items (ItemId) ON DELETE CASCADE,
	CONSTRAINT PK_CollectionsItems_CollectionId_ItemId_1 PRIMARY KEY (CollectionId, ItemId)
)

INSERT INTO Collections (Name, Description)
VALUES
('Mis juegos', 'Juegos');

select * from Collections
where DeletedAt is null

select * from Items;


DECLARE @CollectionId UNIQUEIDENTIFIER = '54211901-34C2-42A9-88FA-34B7B08C7ABD'
DECLARE @ItemHollowKnightId INT = (SELECT ItemId FROM Items WHERE ItemId = 1);
DECLARE @ItemOsuId INT = (SELECT ItemId FROM Items WHERE ItemId = 2);

INSERT INTO CollectionsItems (CollectionId, ItemId)
VALUES
(@CollectionId, @ItemOsuId)
GO

select * from Items

SELECT * FROM Items
WHERE Name = 'Hollow Knight' OR Name = 'osu!';


DECLARE @ItemABuscar NVARCHAR(50) = 'Hollow Knight'

select * from Items 
where Name = @ItemABuscar

CREATE INDEX IX_Items_Name ON Items (Name)