-- Script SQL para modelo RBAC

-- Tabla de Usuarios
CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Tabla de Roles
CREATE TABLE Roles (
    RoleId INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(100) NOT NULL UNIQUE
);

-- Tabla de Permisos
CREATE TABLE Permissions (
    PermissionId INT IDENTITY PRIMARY KEY,
    Resource NVARCHAR(255) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    CONSTRAINT UQ_Permissions UNIQUE (Resource, Action)
);

-- Tabla de Relación: RolePermissions
CREATE TABLE RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId) ON DELETE CASCADE
);

-- Tabla de Relación: UserRoles
CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE
);

-- Tabla de Ítems de Menú
CREATE TABLE MenuItems (
    MenuItemId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Resource NVARCHAR(255) NOT NULL UNIQUE
);

-- Insertar datos de ejemplo

-- Roles
INSERT INTO Roles (RoleName) VALUES ('Admin');
INSERT INTO Roles (RoleName) VALUES ('Operador');
INSERT INTO Roles (RoleName) VALUES ('Auditor');

-- Permisos
INSERT INTO Permissions (Resource, Action) VALUES ('menu:dashboard', 'Read');
INSERT INTO Permissions (Resource, Action) VALUES ('menu:users', 'Create');
INSERT INTO Permissions (Resource, Action) VALUES ('menu:users', 'Read');
INSERT INTO Permissions (Resource, Action) VALUES ('menu:users', 'Update');
INSERT INTO Permissions (Resource, Action) VALUES ('menu:users', 'Delete');

-- RolePermissions
-- Admin tiene acceso total
INSERT INTO RolePermissions (RoleId, PermissionId) SELECT 1, PermissionId FROM Permissions;
-- Operador puede Crear, Leer y Actualizar
INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (2, 2), (2, 3), (2, 4);
-- Auditor solo puede Leer
INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (3, 3);

-- Usuarios
INSERT INTO Users (UserName, Email) VALUES ('admin', 'admin@example.com');
INSERT INTO Users (UserName, Email) VALUES ('operador', 'operador@example.com');
INSERT INTO Users (UserName, Email) VALUES ('auditor', 'auditor@example.com');

-- UserRoles
INSERT INTO UserRoles (UserId, RoleId) VALUES (1, 1); -- Admin
INSERT INTO UserRoles (UserId, RoleId) VALUES (2, 2); -- Operador
INSERT INTO UserRoles (UserId, RoleId) VALUES (3, 3); -- Auditor;