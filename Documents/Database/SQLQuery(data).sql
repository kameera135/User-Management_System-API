--SELECT * FROM Users;

INSERT INTO dbo.Users (UserName,PasswordHash,PasswordSalt,FirstName,LastName,Email,Phone,FirstLogin) VALUES ('Steave.Smith',125365897553,125365897553542845856,'Steave','Smith','steavesmith@yoyo.com','02178595564','True');
INSERT INTO dbo.Users (UserName,PasswordHash,PasswordSalt,FirstName,LastName,Email,Phone,FirstLogin) VALUES ('Nolan.Sullivan',523698524251,52369852425154255662,'Nolan','Sullivan','nolansullivan@yoyo.com','0589562365','True');
INSERT INTO dbo.Users (UserName,PasswordHash,PasswordSalt,FirstName,LastName,Email,Phone,FirstLogin) VALUES ('Joe.Root',895368953254,5236985242515845521,'Joe','Root','joeroot@yoyo.com','02566875531','False');

--SELECT * FROM dbo.Platforms;

INSERT INTO dbo.Platforms (PlatformName,PlatformUrl,Description) VALUES ('Airecon Extention System','http://aes.marinaonebms.com/','Extention System');
INSERT INTO dbo.Platforms (PlatformName,PlatformUrl,Description) VALUES ('Tenant Billing System','http://tbs.marinaonebms.com/','Billing System');
INSERT INTO dbo.Platforms (PlatformName,PlatformUrl,Description) VALUES ('Configuration Management System','http://cms.marinaonebms.com/','Manage configurations of platforms');

--SELECT * FROM dbo.Roles;

INSERT INTO dbo.Roles (Role) VALUES ('Facility Manager');
INSERT INTO dbo.Roles (Role,PlatformID) VALUES ('Tenant Manager',2);
INSERT INTO dbo.Roles (Role,PlatformID) VALUES ('Tenant',2);
INSERT INTO dbo.Roles (Role,PlatformID) VALUES ('Facility Manager',1);
INSERT INTO dbo.Roles (Role,PlatformID) VALUES ('Facility Manager',2);

--SELECT * FROM dbo.Permissions;

INSERT INTO dbo.Permissions (Permission,PlatformID) VALUES ('Add extention',1);
INSERT INTO dbo.Permissions(Permission,PlatformID) VALUES ('View reports',1);

--SELECT * FROM dbo.UserPlatforms;

INSERT INTO dbo.UserPlatforms (UserID,PlatformID) VALUES (3,1);	
INSERT INTO dbo.UserPlatforms (UserID,PlatformID) VALUES (2,2);
--INSERT INTO dbo.UserPlatforms (UserID,PlatformID) VALUES (4,1);

--SELECT * FROM dbo.UserRoles;

INSERT INTO dbo.UserRoles (UserID,RoleID) VALUES (3,2);
INSERT INTO dbo.UserRoles (UserID,RoleID) VALUES (2,2);

--SELECT * FROM dbo.RolePermissions;

--INSERT INTO dbo.RolePermissions (RoleID,PermissionID) VALUES (3,3);
INSERT INTO dbo.RolePermissions (RoleID,PermissionID) VALUES (3,2);