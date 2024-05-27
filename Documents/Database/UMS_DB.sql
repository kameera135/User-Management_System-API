/*
Wrote Date: 10 Jan 2024
Wrote by: Kameera Hemachandra, Thushara Makuloluwa
Modified Date: 18 Mar 2024
Modified By: Kameera Hemachandra

DataBase Name: UMS (User Management System)
*/

BEGIN TRAN

USE UMS

IF OBJECT_ID('RolePermissions') IS NOT NULL BEGIN DROP TABLE RolePermissions END
IF OBJECT_ID('UserRoles') IS NOT NULL BEGIN DROP TABLE UserRoles END
IF OBJECT_ID('UserPlatforms') IS NOT NULL BEGIN DROP TABLE UserPlatforms END
IF OBJECT_ID('ActivityLogs') IS NOT NULL BEGIN DROP TABLE ActivityLogs END
IF OBJECT_ID('APITokens') IS NOT NULL BEGIN DROP TABLE APITokens END
IF OBJECT_ID('AuthTokens') IS NOT NULL BEGIN DROP TABLE AuthTokens END
IF OBJECT_ID('Permissions') IS NOT NULL BEGIN DROP TABLE Permissions END
IF OBJECT_ID('Roles') IS NOT NULL BEGIN DROP TABLE Roles END
IF OBJECT_ID('Platforms') IS NOT NULL BEGIN DROP TABLE Platforms END
IF OBJECT_ID('Users') IS NOT NULL BEGIN DROP TABLE Users END

CREATE TABLE Users
(
    UserID bigint IDENTITY(1,1) NOT NULL,
	UserName nvarchar(100) NOT NULL,
	EmployeeID nvarchar(100) NULL,
	PasswordHash varbinary(MAX) NOT NULL,
	PasswordSalt varbinary(MAX) NOT NULL,
	FirstName nvarchar(100) NULL,
	LastName nvarchar(100) NULL,
	Email nvarchar(150) NULL,
	Phone nvarchar(100) NULL,
	FirstLogin nvarchar(100) NOT NULL,
	_2FA_authenticate bit DEFAULT 0 NOT NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);


CREATE TABLE Platforms
(
    PlatformID bigint IDENTITY(1,1) NOT NULL,
	PlatformName nvarchar(100) NOT NULL,
	PlatformCode nvarchar(100) NULL,
	PlatformUrl nvarchar(MAX) NULL,
	Description nvarchar(150) NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

CREATE TABLE Roles
(
    RoleID bigint IDENTITY(1,1) NOT NULL,
	Role nvarchar(100) NOT NULL,
	PlatformID bigint NULL,
	Status bit DEFAULT 1 NOT NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

CREATE TABLE Permissions
(
    PermissionID bigint IDENTITY(1,1) NOT NULL,
	Permission nvarchar(100) NOT NULL,
	PlatformID bigint NOT NULL,
	Status bit DEFAULT 1 NOT NULL,
	Is_licence bit DEFAULT 1 NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

CREATE TABLE AuthTokens
(
    TokenID bigint IDENTITY(1,1) NOT NULL,
	Token nvarchar(MAX) NULL,
	UserID bigint NOT NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	ExpireDate datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

CREATE TABLE APITokens
(
    TokenID bigint IDENTITY(1,1) NOT NULL,
	Token nvarchar(MAX) NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	ExpireDate datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

CREATE TABLE ActivityLogs
(
    LogID bigint IDENTITY(1,1) NOT NULL,
	UserID bigint NOT NULL,
	PlatformID bigint NULL,
	RoleID bigint NULL,
	ActivityType nvarchar(100) NULL,
	Description text NULL,
	Details text NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

CREATE TABLE UserPlatforms
(
    Id bigint IDENTITY(1,1) NOT NULL,
	UserID bigint NOT NULL,
	PlatformID bigint NOT NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

CREATE TABLE UserRoles
(
    Id bigint IDENTITY(1,1) NOT NULL,
	UserID bigint NOT NULL,
	RoleID bigint NOT NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

CREATE TABLE RolePermissions
(
    Id bigint IDENTITY(1,1) NOT NULL,
	RoleID bigint NOT NULL,
	PermissionID bigint NOT NULL,
	CreatedAt datetime NULL,
	UpdatedAt datetime NULL,
	DeletedAt datetime NULL,
	CreatedBy bigint NULL,
	UpdatedBy bigint NULL,
	DeletedBy bigint NULL
);

ALTER TABLE Users ADD CONSTRAINT PK_Users PRIMARY KEY (UserID);
ALTER TABLE Platforms ADD CONSTRAINT PK_Platforms PRIMARY KEY (PlatformID);
ALTER TABLE Roles ADD CONSTRAINT PK_Roles PRIMARY KEY (RoleID);
ALTER TABLE Permissions ADD CONSTRAINT PK_Permissions PRIMARY KEY (PermissionID);
ALTER TABLE AuthTokens ADD CONSTRAINT PK_AuthTokens PRIMARY KEY (TokenID);
ALTER TABLE APITokens ADD CONSTRAINT PK_APITokens PRIMARY KEY (TokenID);
ALTER TABLE ActivityLogs ADD CONSTRAINT PK_ActivityLogs PRIMARY KEY (LogID);
ALTER TABLE UserPlatforms ADD CONSTRAINT PK_UserPlatforms PRIMARY KEY (Id);
ALTER TABLE UserRoles ADD CONSTRAINT PK_UserRoles PRIMARY KEY (Id);
ALTER TABLE RolePermissions ADD CONSTRAINT PK_RolePermissions PRIMARY KEY (Id);

ALTER TABLE Roles ADD FOREIGN KEY (PlatformID) REFERENCES Platforms(PlatformID);
ALTER TABLE Permissions ADD FOREIGN KEY (PlatformID) REFERENCES Platforms(PlatformID);
ALTER TABLE AuthTokens ADD FOREIGN KEY (UserID) REFERENCES Users(UserID);
ALTER TABLE ActivityLogs ADD FOREIGN KEY (UserID) REFERENCES Users(UserID);
ALTER TABLE ActivityLogs ADD FOREIGN KEY (PlatformID) REFERENCES Platforms(PlatformID);
ALTER TABLE ActivityLogs ADD FOREIGN KEY (RoleID) REFERENCES Roles(RoleID);
ALTER TABLE UserPlatforms ADD FOREIGN KEY (UserID) REFERENCES Users(UserID);
ALTER TABLE UserPlatforms ADD FOREIGN KEY (PlatformID) REFERENCES Platforms(PlatformID);
ALTER TABLE UserRoles ADD FOREIGN KEY (UserID) REFERENCES Users(UserID);
ALTER TABLE UserRoles ADD FOREIGN KEY (RoleID) REFERENCES Roles(RoleID);
ALTER TABLE RolePermissions ADD FOREIGN KEY (RoleID) REFERENCES Roles(RoleID);
ALTER TABLE RolePermissions ADD FOREIGN KEY (PermissionID) REFERENCES Permissions(PermissionID);

COMMIT TRAN