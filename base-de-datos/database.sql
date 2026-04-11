-- ============================================================
--  TalentInsights - Base de datos SQL Server
--  Reconoce el crecimiento de los colaboradores en una empresa
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TalentInsights')
    CREATE DATABASE TalentInsights;
GO

USE TalentInsights;
GO

-- ============================================================
--  TABLA: Collaborators
-- ============================================================
CREATE TABLE Collaborators (
    Id            UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    Email         NVARCHAR(100)      NOT NULL,
    FullName      NVARCHAR(150)      NOT NULL,
    GitlabProfile NVARCHAR(255)      NULL,
    Position      NVARCHAR(100)      NOT NULL,
    Password      NVARCHAR(255)      NOT NULL,
    JoinedAt      DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive      BIT                NOT NULL DEFAULT 1,
    CreatedAt     DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    DeletedAt     DATETIME2          NULL,
    UpdatedAt     DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Collaborators PRIMARY KEY (Id)
);
GO

-- ============================================================
--  TABLA: Skills
-- ============================================================
CREATE TABLE Skills (
    Id        UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    Name      NVARCHAR(100)      NOT NULL,
    Category  NVARCHAR(100)      NULL,

    CONSTRAINT PK_Skills PRIMARY KEY (Id),
    CONSTRAINT UQ_Skills_Name UNIQUE (Name)
);
GO

-- ============================================================
--  TABLA: CollaboratorSkills  (relación N:M Collaborator <-> Skill)
-- ============================================================
CREATE TABLE CollaboratorSkills (
    CollaboratorId  UNIQUEIDENTIFIER   NOT NULL,
    SkillId         UNIQUEIDENTIFIER   NOT NULL,
    AcquiredAt      DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_CollaboratorSkills PRIMARY KEY (CollaboratorId, SkillId),
    CONSTRAINT FK_CollaboratorSkills_Collaborator FOREIGN KEY (CollaboratorId)
        REFERENCES Collaborators (Id) ON DELETE CASCADE,
    CONSTRAINT FK_CollaboratorSkills_Skill FOREIGN KEY (SkillId)
        REFERENCES Skills (Id) ON DELETE CASCADE
);
GO

-- ============================================================
--  TABLA: Teams
-- ============================================================
CREATE TABLE Teams (
    Id          UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    Name        NVARCHAR(150)      NOT NULL,
    Description NVARCHAR(500)      NULL,
    IsPublic    BIT                NOT NULL DEFAULT 1,
    CreatedBy   UNIQUEIDENTIFIER   NOT NULL,
    CreatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Teams PRIMARY KEY (Id),
    CONSTRAINT FK_Teams_CreatedBy FOREIGN KEY (CreatedBy)
        REFERENCES Collaborators (Id)
);
GO

-- ============================================================
--  TABLA: TeamMembers  (relación N:M Team <-> Collaborator)
-- ============================================================
CREATE TABLE TeamMembers (
    TeamId         UNIQUEIDENTIFIER   NOT NULL,
    CollaboratorId UNIQUEIDENTIFIER   NOT NULL,
    JoinedAt       DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_TeamMembers PRIMARY KEY (TeamId, CollaboratorId),
    CONSTRAINT FK_TeamMembers_Team FOREIGN KEY (TeamId)
        REFERENCES Teams (Id) ON DELETE CASCADE,
    CONSTRAINT FK_TeamMembers_Collaborator FOREIGN KEY (CollaboratorId)
        REFERENCES Collaborators (Id)
);
GO

-- ============================================================
--  TABLA: Projects
-- ============================================================
CREATE TABLE Projects (
    Id          UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    Name        NVARCHAR(200)      NOT NULL,
    Description NVARCHAR(1000)     NULL,
    Status      NVARCHAR(50)       NOT NULL DEFAULT 'Active',  -- Active | Completed | Archived
    CreatedBy   UNIQUEIDENTIFIER   NOT NULL,
    CreatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Projects PRIMARY KEY (Id),
    CONSTRAINT FK_Projects_CreatedBy FOREIGN KEY (CreatedBy)
        REFERENCES Collaborators (Id),
    CONSTRAINT CK_Projects_Status CHECK (Status IN ('Active', 'Completed', 'Archived'))
);
GO

-- ============================================================
--  TABLA: ProjectCollaborators  (colaboradores asignados a un proyecto)
-- ============================================================
CREATE TABLE ProjectCollaborators (
    ProjectId      UNIQUEIDENTIFIER   NOT NULL,
    CollaboratorId UNIQUEIDENTIFIER   NOT NULL,
    AssignedAt     DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    RemovedAt      DATETIME2          NULL,        -- NULL = activo en el proyecto

    CONSTRAINT PK_ProjectCollaborators PRIMARY KEY (ProjectId, CollaboratorId),
    CONSTRAINT FK_ProjectCollaborators_Project FOREIGN KEY (ProjectId)
        REFERENCES Projects (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ProjectCollaborators_Collaborator FOREIGN KEY (CollaboratorId)
        REFERENCES Collaborators (Id)
);
GO

-- ============================================================
--  TABLA: ProjectTeams  (equipos relacionados a un proyecto)
-- ============================================================
CREATE TABLE ProjectTeams (
    ProjectId  UNIQUEIDENTIFIER   NOT NULL,
    TeamId     UNIQUEIDENTIFIER   NOT NULL,

    CONSTRAINT PK_ProjectTeams PRIMARY KEY (ProjectId, TeamId),
    CONSTRAINT FK_ProjectTeams_Project FOREIGN KEY (ProjectId)
        REFERENCES Projects (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ProjectTeams_Team FOREIGN KEY (TeamId)
        REFERENCES Teams (Id)
);
GO

-- ============================================================
--  TABLA: ProjectMessages
-- ============================================================
CREATE TABLE ProjectMessages (
    Id             UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    ProjectId      UNIQUEIDENTIFIER   NOT NULL,
    CollaboratorId UNIQUEIDENTIFIER   NOT NULL,
    Content        NVARCHAR(MAX)      NOT NULL,
    SentAt         DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    DeletedAt      DATETIME2          NULL,        -- Soft delete

    CONSTRAINT PK_ProjectMessages PRIMARY KEY (Id),
    CONSTRAINT FK_ProjectMessages_Project FOREIGN KEY (ProjectId)
        REFERENCES Projects (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ProjectMessages_Collaborator FOREIGN KEY (CollaboratorId)
        REFERENCES Collaborators (Id)
);
GO

-- ============================================================
--  TABLA: Posts
-- ============================================================
CREATE TABLE Posts (
    Id             UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    CollaboratorId UNIQUEIDENTIFIER   NOT NULL,
    Title          NVARCHAR(300)      NOT NULL,
    Content        NVARCHAR(MAX)      NOT NULL,
    CreatedAt      DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt      DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    DeletedAt      DATETIME2          NULL,        -- Soft delete

    CONSTRAINT PK_Posts PRIMARY KEY (Id),
    CONSTRAINT FK_Posts_Collaborator FOREIGN KEY (CollaboratorId)
        REFERENCES Collaborators (Id)
);
GO

-- ============================================================
--  TABLA: Permissions  (catálogo de permisos disponibles)
-- ============================================================
CREATE TABLE Permissions (
    Id          UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    Code        NVARCHAR(100)      NOT NULL,   -- e.g. COLLABORATORS/CREATE
    Module      NVARCHAR(50)       NOT NULL,   -- e.g. COLLABORATORS
    Action      NVARCHAR(50)       NOT NULL,   -- e.g. CREATE
    Name        NVARCHAR(150)      NOT NULL,
    Description NVARCHAR(500)      NULL,
    -- Specificity: Own | ByAssignment | Creator
    Specificity NVARCHAR(20)       NOT NULL DEFAULT 'ByAssignment',

    CONSTRAINT PK_Permissions PRIMARY KEY (Id),
    CONSTRAINT UQ_Permissions_Code UNIQUE (Code),
    CONSTRAINT CK_Permissions_Specificity CHECK (Specificity IN ('Own', 'ByAssignment', 'Creator'))
);
GO

-- ============================================================
--  TABLA: Roles  (catálogo de roles del sistema)
-- ============================================================
CREATE TABLE Roles (
    Id          UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    Name        NVARCHAR(100)      NOT NULL,
    Description NVARCHAR(500)      NULL,
    IsActive    BIT                NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Roles PRIMARY KEY (Id),
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);
GO

-- ============================================================
--  TABLA: RolePermissions  (relación N:M Role <-> Permission)
-- ============================================================
CREATE TABLE RolePermissions (
    RoleId       UNIQUEIDENTIFIER   NOT NULL,
    PermissionId UNIQUEIDENTIFIER   NOT NULL,
    AssignedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_RolePermissions PRIMARY KEY (RoleId, PermissionId),
    CONSTRAINT FK_RolePermissions_Role FOREIGN KEY (RoleId)
        REFERENCES Roles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_RolePermissions_Permission FOREIGN KEY (PermissionId)
        REFERENCES Permissions (Id) ON DELETE CASCADE
);
GO

-- ============================================================
--  TABLA: CollaboratorRoles  (relación N:M Collaborator <-> Role)
-- ============================================================
CREATE TABLE CollaboratorRoles (
    CollaboratorId UNIQUEIDENTIFIER   NOT NULL,
    RoleId         UNIQUEIDENTIFIER   NOT NULL,
    AssignedAt     DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    AssignedBy     UNIQUEIDENTIFIER   NULL,    -- NULL = sistema

    CONSTRAINT PK_CollaboratorRoles PRIMARY KEY (CollaboratorId, RoleId),
    CONSTRAINT FK_CollaboratorRoles_Collaborator FOREIGN KEY (CollaboratorId)
        REFERENCES Collaborators (Id) ON DELETE CASCADE,
    CONSTRAINT FK_CollaboratorRoles_Role FOREIGN KEY (RoleId)
        REFERENCES Roles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_CollaboratorRoles_AssignedBy FOREIGN KEY (AssignedBy)
        REFERENCES Collaborators (Id)
);
GO

-- ============================================================
--  TABLA: CollaboratorHistory  (histórico de colaboradores)
-- ============================================================
CREATE TABLE CollaboratorHistory (
    Id             UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    CollaboratorId UNIQUEIDENTIFIER   NOT NULL,
    EntityType     NVARCHAR(20)       NOT NULL,   -- Project | Team | Skill
    EntityId       UNIQUEIDENTIFIER   NOT NULL,
    EntityName     NVARCHAR(200)      NOT NULL,
    StartedAt      DATETIME2          NOT NULL,
    EndedAt        DATETIME2          NULL,        -- NULL = aún activo
    RecordedAt     DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_CollaboratorHistory PRIMARY KEY (Id),
    CONSTRAINT FK_CollaboratorHistory_Collaborator FOREIGN KEY (CollaboratorId)
        REFERENCES Collaborators (Id) ON DELETE CASCADE,
    CONSTRAINT CK_CollaboratorHistory_EntityType CHECK (EntityType IN ('Project', 'Team', 'Skill'))
);
GO

-- ============================================================
--  SEED: Catálogo de permisos del sistema
-- ============================================================
INSERT INTO Permissions (Id, Code, Module, Action, Name, Description, Specificity)
VALUES
    (NEWID(), 'COLLABORATORS/CREATE',                    'COLLABORATORS', 'CREATE',                    'Crear colaborador',                         'Permite la creación de un colaborador',                        'ByAssignment'),
    (NEWID(), 'COLLABORATORS/UPDATE',                    'COLLABORATORS', 'UPDATE',                    'Actualizar colaborador',                    'Permite la actualización de un colaborador',                   'ByAssignment'),
    (NEWID(), 'COLLABORATORS/UPDATE_PERSONAL',           'COLLABORATORS', 'UPDATE_PERSONAL',           'Actualización personal de colaborador',     'Permite la actualización personal del colaborador',            'Own'),
    (NEWID(), 'COLLABORATORS/DISABLE',                   'COLLABORATORS', 'DISABLE',                   'Deshabilitar colaborador',                  'Permite la deshabilitación de un colaborador',                 'ByAssignment'),
    (NEWID(), 'COLLABORATORS/VIEW_HISTORICAL',           'COLLABORATORS', 'VIEW_HISTORICAL',           'Ver histórico de un colaborador',           'Permite la visualización del histórico de un colaborador',     'ByAssignment'),
    (NEWID(), 'TEAMS/CREATE',                            'TEAMS',         'CREATE',                    'Crear equipo público',                      'Permite la creación de equipos públicos',                      'ByAssignment'),
    (NEWID(), 'TEAMS/UPDATE',                            'TEAMS',         'UPDATE',                    'Actualizar equipo público',                 'Permite la actualización de equipos públicos',                 'ByAssignment'),
    (NEWID(), 'TEAMS/DELETE',                            'TEAMS',         'DELETE',                    'Eliminar equipo público',                   'Permite la eliminación de equipos públicos',                   'ByAssignment'),
    (NEWID(), 'POSTS/CREATE',                            'POSTS',         'CREATE',                    'Crear publicación',                         'Permite la creación de publicaciones',                         'Own'),
    (NEWID(), 'POSTS/UPDATE',                            'POSTS',         'UPDATE',                    'Actualizar publicación',                    'Permite la actualización de publicaciones',                    'Own'),
    (NEWID(), 'POSTS/DELETE',                            'POSTS',         'DELETE',                    'Eliminar publicación',                      'Permite la eliminación de publicaciones',                      'Own'),
    (NEWID(), 'PROJECTS/CREATE',                         'PROJECTS',      'CREATE',                    'Crear proyecto',                            'Permite la creación de proyectos',                             'ByAssignment'),
    (NEWID(), 'PROJECTS/UPDATE',                         'PROJECTS',      'UPDATE',                    'Actualizar proyecto',                       'Permite la actualización de proyectos',                        'ByAssignment'),
    (NEWID(), 'PROJECTS/ADD_COLABORATORS',               'PROJECTS',      'ADD_COLABORATORS',          'Añadir colaboradores a un proyecto',        'Permite la adición de colaboradores a un proyecto',            'Creator'),
    (NEWID(), 'PROJECTS/DELETE_COLABORATORS',            'PROJECTS',      'DELETE_COLABORATORS',       'Eliminar colaboradores de un proyecto',     'Permite la eliminación de colaboradores de un proyecto',       'Creator'),
    (NEWID(), 'PROJECTS/SEND_MESSAGES',                  'PROJECTS',      'SEND_MESSAGES',             'Enviar mensajes en un proyecto',            'Permite el envío de mensajes en un proyecto',                  'Own'),
    (NEWID(), 'PROJECTS/DELETE_SENDING_OWN_MESSAGES',    'PROJECTS',      'DELETE_SENDING_OWN_MESSAGES','Eliminar mensajes propios enviados',       'Permite la eliminación de mensajes propios enviados',          'Own');
GO

-- ============================================================
--  SEED: Catálogo de roles del sistema
-- ============================================================
DECLARE @RoleAdmin       UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleHR          UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleTeamLeader  UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleDeveloper   UNIQUEIDENTIFIER = NEWID();

INSERT INTO Roles (Id, Name, Description)
VALUES
    (@RoleAdmin,      'Admin',       'Acceso total al sistema. Gestiona colaboradores, equipos, proyectos y publicaciones.'),
    (@RoleHR,         'HR',          'Recursos Humanos. Gestiona el ciclo de vida de los colaboradores.'),
    (@RoleTeamLeader, 'TeamLeader',  'Lidera equipos y proyectos. Puede gestionar miembros dentro de sus proyectos.'),
    (@RoleDeveloper,  'Developer',   'Colaborador técnico. Gestiona su perfil, publicaciones y mensajería en proyectos.');
GO

-- ============================================================
--  SEED: Asignación de permisos a roles
-- ============================================================

-- Admin: todos los permisos
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM   Roles r
CROSS JOIN Permissions p
WHERE  r.Name = 'Admin';
GO

-- HR: gestión de colaboradores
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM   Roles r
JOIN   Permissions p ON p.Code IN (
    'COLLABORATORS/CREATE',
    'COLLABORATORS/UPDATE',
    'COLLABORATORS/DISABLE',
    'COLLABORATORS/VIEW_HISTORICAL'
)
WHERE  r.Name = 'HR';
GO

-- TeamLeader: gestión de equipos y proyectos
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM   Roles r
JOIN   Permissions p ON p.Code IN (
    'TEAMS/CREATE',
    'TEAMS/UPDATE',
    'TEAMS/DELETE',
    'PROJECTS/CREATE',
    'PROJECTS/UPDATE',
    'PROJECTS/ADD_COLABORATORS',
    'PROJECTS/DELETE_COLABORATORS'
)
WHERE  r.Name = 'TeamLeader';
GO

-- Developer: perfil propio, publicaciones y mensajería
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM   Roles r
JOIN   Permissions p ON p.Code IN (
    'COLLABORATORS/UPDATE_PERSONAL',
    'POSTS/CREATE',
    'POSTS/UPDATE',
    'POSTS/DELETE',
    'PROJECTS/SEND_MESSAGES',
    'PROJECTS/DELETE_SENDING_OWN_MESSAGES'
)
WHERE  r.Name = 'Developer';
GO

-- ============================================================
--  TABLA: Menus
-- ============================================================
CREATE TABLE Menus (
    Id          UNIQUEIDENTIFIER   NOT NULL DEFAULT NEWID(),
    Code        NVARCHAR(100)      NOT NULL,
    Name        NVARCHAR(150)      NOT NULL,
    Path        NVARCHAR(300)      NOT NULL,
    IconName    NVARCHAR(100)      NOT NULL,
    ParentId    UNIQUEIDENTIFIER   NULL,
    SortOrder   INT                NOT NULL DEFAULT 0,
    IsVisible   BIT                NOT NULL DEFAULT 1,
    IsActive    BIT                NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Menus          PRIMARY KEY (Id),
    CONSTRAINT UQ_Menus_Code     UNIQUE (Code),
    CONSTRAINT FK_Menus_ParentId FOREIGN KEY (ParentId)
        REFERENCES Menus (Id)
);
GO

-- ============================================================
--  TABLA: MenuPermissions  (relación N:M Menu <-> Permission)
--  Define qué permisos son necesarios para ver/acceder a un ítem de menú.
--  Los permisos del colaborador se derivan de sus roles asignados.
-- ============================================================
CREATE TABLE MenuPermissions (
    MenuId       UNIQUEIDENTIFIER   NOT NULL,
    PermissionId UNIQUEIDENTIFIER   NOT NULL,
    -- ALL = el colaborador debe tener TODOS los permisos del menú para verlo
    -- ANY = basta con tener AL MENOS UNO para verlo
    MatchMode    NVARCHAR(10)       NOT NULL DEFAULT 'ANY',

    CONSTRAINT PK_MenuPermissions            PRIMARY KEY (MenuId, PermissionId),
    CONSTRAINT FK_MenuPermissions_Menu       FOREIGN KEY (MenuId)
        REFERENCES Menus (Id) ON DELETE CASCADE,
    CONSTRAINT FK_MenuPermissions_Permission FOREIGN KEY (PermissionId)
        REFERENCES Permissions (Id) ON DELETE CASCADE,
    CONSTRAINT CK_MenuPermissions_MatchMode  CHECK (MatchMode IN ('ANY', 'ALL'))
);
GO

-- ============================================================
--  SEED: Menús del sistema TalentInsights
-- ============================================================
DECLARE @IdCollaborators  UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCollabList     UNIQUEIDENTIFIER = NEWID();
DECLARE @IdCollabHistory  UNIQUEIDENTIFIER = NEWID();
DECLARE @IdTeams          UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProjects       UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProjList       UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProjMessages   UNIQUEIDENTIFIER = NEWID();
DECLARE @IdPosts          UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfile        UNIQUEIDENTIFIER = NEWID();

INSERT INTO Menus (Id, Code, Name, Path, IconName, ParentId, SortOrder, IsVisible, IsActive)
VALUES
    -- Menús raíz
    (@IdCollaborators, 'collaborators',         'Colaboradores',    '/collaborators',           'users',          NULL,              1,  1, 1),
    (@IdTeams,         'teams',                 'Equipos',          '/teams',                   'users-group',    NULL,              2,  1, 1),
    (@IdProjects,      'projects',              'Proyectos',        '/projects',                'folder',         NULL,              3,  1, 1),
    (@IdPosts,         'posts',                 'Publicaciones',    '/posts',                   'newspaper',      NULL,              4,  1, 1),
    (@IdProfile,       'profile',               'Mi Perfil',        '/profile',                 'user-circle',    NULL,              5,  1, 1),
    -- Submenús de Colaboradores
    (@IdCollabList,    'collaborators.list',    'Listado',          '/collaborators/list',      'list',           @IdCollaborators,  1,  1, 1),
    (@IdCollabHistory, 'collaborators.history', 'Histórico',        '/collaborators/history',   'clock-history',  @IdCollaborators,  2,  1, 1),
    -- Submenús de Proyectos
    (@IdProjList,      'projects.list',         'Listado',          '/projects/list',           'list',           @IdProjects,       1,  1, 1),
    (@IdProjMessages,  'projects.messages',     'Mensajes',         '/projects/messages',       'chat',           @IdProjects,       2,  1, 1);
GO

-- ============================================================
--  SEED: Relación Menús <-> Permisos
-- ============================================================
INSERT INTO MenuPermissions (MenuId, PermissionId, MatchMode)
SELECT m.Id, p.Id, 'ANY'
FROM   Menus m
CROSS JOIN Permissions p
WHERE
    (m.Code = 'collaborators.list'    AND p.Code IN ('COLLABORATORS/CREATE', 'COLLABORATORS/UPDATE'))
    OR (m.Code = 'collaborators.history' AND p.Code = 'COLLABORATORS/VIEW_HISTORICAL')
    OR (m.Code = 'teams'              AND p.Code IN ('TEAMS/CREATE', 'TEAMS/UPDATE', 'TEAMS/DELETE'))
    OR (m.Code = 'projects.list'      AND p.Code IN ('PROJECTS/CREATE', 'PROJECTS/UPDATE'))
    OR (m.Code = 'projects.messages'  AND p.Code IN ('PROJECTS/SEND_MESSAGES', 'PROJECTS/DELETE_SENDING_OWN_MESSAGES'))
    OR (m.Code = 'posts'              AND p.Code IN ('POSTS/CREATE', 'POSTS/UPDATE', 'POSTS/DELETE'));
GO

-- ============================================================
--  ÍNDICES para mejorar el rendimiento de consultas frecuentes
-- ============================================================
CREATE INDEX IX_CollaboratorSkills_SkillId        ON CollaboratorSkills (SkillId);
CREATE INDEX IX_TeamMembers_CollaboratorId         ON TeamMembers (CollaboratorId);
CREATE INDEX IX_ProjectCollaborators_Collaborator  ON ProjectCollaborators (CollaboratorId);
CREATE INDEX IX_ProjectMessages_ProjectId          ON ProjectMessages (ProjectId);
CREATE INDEX IX_ProjectMessages_CollaboratorId     ON ProjectMessages (CollaboratorId);
CREATE INDEX IX_Posts_CollaboratorId               ON Posts (CollaboratorId);
CREATE INDEX IX_CollaboratorHistory_Collaborator   ON CollaboratorHistory (CollaboratorId);
CREATE INDEX IX_CollaboratorHistory_EntityType     ON CollaboratorHistory (EntityType, EntityId);
CREATE INDEX IX_Menus_ParentId                     ON Menus (ParentId);
CREATE INDEX IX_Menus_SortOrder                    ON Menus (SortOrder);
CREATE INDEX IX_MenuPermissions_PermissionId       ON MenuPermissions (PermissionId);
-- Índices nuevos para el sistema de roles
CREATE INDEX IX_RolePermissions_PermissionId       ON RolePermissions (PermissionId);
CREATE INDEX IX_CollaboratorRoles_RoleId           ON CollaboratorRoles (RoleId);
CREATE INDEX IX_CollaboratorRoles_AssignedBy       ON CollaboratorRoles (AssignedBy);
GO