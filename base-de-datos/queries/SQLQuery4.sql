-- LeaderBoard

CREATE DATABASE LeaderBoard;
GO

USE LeaderBoard;
GO

-- DDL
CREATE TABLE UsuarioTipos (
UsuarioTipoId INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
Descripcion NVARCHAR(20) NOT NULL,
FechaDeCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE ModuloTipos (
ModuloTipoId INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
Especialidad NVARCHAR(100) NOT NULL,
Tecnologia NVARCHAR(100) NOT NULL,
FechaDeCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE Usuarios (
UserId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID() ,
TipoId INT NOT NULL REFERENCES UsuarioTipos (UsuarioTipoId),
Nombre NVARCHAR(100) NOT NULL,
Edad INT NOT NULL,
Correo NVARCHAR(100) NOT NULL UNIQUE,
NumeroDeTelefono NVARCHAR(32) NOT NULL UNIQUE,
Cedula NVARCHAR(10) NOT NULL UNIQUE,
FechaDeCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE Modulos (
ModuloId INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
TipoId INT NOT NULL REFERENCES ModuloTipos(ModuloTipoId),
ProfesorId UNIQUEIDENTIFIER NOT NULL REFERENCES Usuarios(UserId), 
FechaDeCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE Participaciones (
ParticipacionId INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
EstudianteId UNIQUEIDENTIFIER NOT NULL REFERENCES Usuarios(UserId), 
ModuloId INT NOT NULL REFERENCES Modulos(ModuloId),
Puntos DECIMAL NOT NULL,
FechaDeCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- DML
INSERT INTO UsuarioTipos(Descripcion)
VALUES
('Profesor'),
('Estudiante');
GO

INSERT INTO ModuloTipos (Especialidad, Tecnologia)
VALUES
('Motor de base de datos', 'SQL SERVER'),
('Framework', 'Angular'),
('Framework', '.NET'),
('Entorno de ejecución', 'NodeJS');
GO

DECLARE @JohnDoeUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @JohnDoeSegundoUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @JohnDoeTerceroUserId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Usuarios (UserId, TipoId, Nombre, Edad, Correo, NumeroDeTelefono, Cedula)
VALUES
(@JohnDoeUserId, 1, 'John Doe', 78, 'john@doe.com', '0123123230923', '0123456789'),
(@JohnDoeSegundoUserId, 1, 'John Doe', 78, 'john@doe.com', '0123123230923', '0123456789'),
(@JohnDoeTerceroUserId, 1, 'John Doe', 78, 'john@doe.com', '0123123230923', '0123456789');

INSERT INTO Usuarios (TipoId, Nombre, Edad, Correo, NumeroDeTelefono, Cedula)
VALUES
(1, 'John Doe', 78, 'john@doe.com', '0123123230923', '0123456789');

DECLARE @SQLServerModuloTipo INT = (SELECT ModuloTipoId FROM ModuloTipos WHERE Tecnologia = 'SQL SERVER');
DECLARE @AngularModuloTipo INT = (SELECT ModuloTipoId FROM ModuloTipos WHERE Tecnologia = 'Angular');
DECLARE @DotNetModuloTipo INT = (SELECT ModuloTipoId FROM ModuloTipos WHERE Tecnologia = '.NET');
DECLARE @NodeJs INT = (SELECT ModuloTipoId FROM ModuloTipos WHERE Tecnologia = 'NodeJS');

INSERT INTO Modulos (TipoId, ProfesorId)
VALUES
(@SQLServerModuloTipo, @JohnDoeUserId),
(@DotNetModuloTipo, @JohnDoeUserId),
(@AngularModuloTipo, @JohnDoeSegundoUserId),
(@NodeJs, @JohnDoeTerceroUserId);
GO

ALTER TABLE Usuarios
ADD CONSTRAINT UQ_Usuario_CorreoElectronico UNIQUE (Correo);
GO

ALTER TABLE Usuarios
ADD CONSTRAINT UQ_Usuario_Cedula UNIQUE (Cedula);
GO

ALTER TABLE Usuarios
ADD CONSTRAINT UQ_Usuario_NumeroDeTelefono UNIQUE (NumeroDeTelefono);
GO

SELECT * FROM Usuarios
WHERE UserId != '911025E0-D028-4831-BC96-CAF09BB63EF8'

UPDATE Usuarios
SET Correo = 'john2@doe.com', Cedula = '0123456787', NumeroDeTelefono = '123123192731238'
WHERE UserId = '37FC072F-78B0-4A21-9835-4DC8E3FA7F4E'

UPDATE Usuarios
SET Correo = 'john3@doe.com', Cedula = '0123456788', NumeroDeTelefono = '19283123123123'
WHERE UserId = '5573226A-B1C5-46B6-A57D-9E0F57263081'
