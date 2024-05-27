--Get Platform Users
SELECT * 
FROM Users u
JOIN UserPlatforms up ON u.UserID = up.UserID
JOIN Platforms p ON up.PlatformID = p.PlatformID
where up.PlatformID = 2 AND up.DeletedAt<GETDATE()

--for assign users to platform get users who are not assigned to selected platform
SELECT *
FROM Users u
INNER JOIN UserPlatforms up ON u.UserID = up.UserID
WHERE up.PlatformID != 3

SELECT *
FROM Users u
WHERE NOT EXISTS (
    SELECT 1
    FROM UserPlatforms up
    WHERE u.UserID = up.UserID AND up.PlatformID = 3
);

SELECT u.*
FROM Users u
LEFT JOIN UserPlatforms up ON u.UserID = up.UserID AND up.PlatformID = 3
WHERE up.UserID IS NULL;

--get permissions in selected platform but not in selected role
SELECT p.*
FROM Permissions p
WHERE NOT EXISTS (
    SELECT 1
    FROM RolePermissions rp
    WHERE  rp.RoleId = 3 AND rp.PermissionId = p.PermissionId
) AND p.PlatformId = 2 


--get roles in selected platform but not not assigned to selected user
SELECT r.*
FROM Roles r
WHERE NOT EXISTS(
	SELECT 1
	FROM UserRoles ur
	WHERE ur.UserID = 2 AND ur.RoleID = r.RoleID
) AND r.PlatformID = 2

--get user roles for selected platform
/*SELECT *
FROM Users u 
INNER JOIN UserRoles ur ON u.UserID = ur.UserID AND ur.UserID = 3
INNER JOIN Roles r ON ur.RoleID = r.RoleID
WHERE EXISTS (
  SELECT *
  FROM Platforms p
  WHERE p.PlatformID = 2 AND p.PlatformID = r.PlatformID  
)*/

SELECT *
FROM Roles r 
INNER JOIN UserRoles ur ON r.RoleID = ur.RoleID
INNER JOIN Users u ON ur.UserID = u.UserID
INNER JOIN Platforms p ON r.PlatformID = p.PlatformID
WHERE 
  u.UserID = 3 
  AND p.PlatformID = 2


--get all roles of selected platform

/*SELECT *
FROM Roles r
LEFT JOIN UserRoles ur ON r.RoleID = ur.RoleID AND ur.UserID != 3 OR ur.UserID IS NULL 
WHERE PlatformID = 2*/

SELECT r.Role
FROM Roles r 
LEFT JOIN UserRoles ur ON r.RoleID = ur.RoleID AND ur.UserID !=3 OR ur.UserID IS NULL
WHERE PlatformID = 2
AND r.Role NOT IN (
  SELECT r2.Role
  FROM Roles r2
  LEFT JOIN UserRoles ur2 ON r2.RoleID = ur2.RoleID
  WHERE ur2.UserID = 3
)

--get platform details of the role
SELECT *
FROM Roles r
JOIN Platforms ON r.PlatformID = Platforms.PlatformID
--WHERE r.RoleID = 2

--get permissions of selected role in same platform
SELECT p.Permission,r.PlatformID 
FROM Roles r
JOIN Permissions p ON p.PlatformID = r.PlatformID
join RolePermissions rp on r.RoleID = rp.RoleID and rp.PermissionID = p.PermissionID
--INNER JOIN Permissions p ON rp.PermissionID = p.PermissionID
WHERE r.RoleID = 3 
AND r.PlatformID = 2

--get permissions for given role and platform
SELECT *
FROM Permissions p
JOIN RolePermissions rp ON p.PermissionID = rp.PermissionID 
--WHERE p.PlatformID = 1 

INSERT INTO dbo.RolePermissions(RoleID,PermissionID) VALUES(3,2)

--get platforms and permissions for given platform
SELECT *
FROM Permissions p
Full JOIN RolePermissions rp ON p.PermissionID = rp.PermissionID
Full JOIN Platforms pl ON p.PlatformID = pl.PlatformID
--WHERE RP.RoleID = 3 
--AND pl.PlatformID = 1
	
--get permissions that given platform have but selected role do not have
SELECT rp.RoleID,p.PlatformID,p.PermissionID
FROM Permissions p
JOIN RolePermissions rp ON rp.PermissionID = p.PermissionID


SELECT r.RoleID, r.PlatformID,rp.PermissionID
FROM Roles r
JOIN RolePermissions rp ON r.RoleID = rp.RoleID

--get roles and permissions when give userId and platformId
SELECT *
FROM Users u
JOIN UserRoles ur ON u.UserID = ur.UserID
JOIN Roles r ON ur.RoleID = r.RoleID
FULL JOIN RolePermissions rp ON r.RoleID = rp.RoleID
JOIN Permissions p on p.PermissionID = rp.PermissionID
WHERE r.PlatformID = 2;



