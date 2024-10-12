DECLARE 
@superUserRoleId INT,
@receptionistRoleId INT,
@supportRoleId INT,
@kcUserRoleId INT;

SELECT @superUserRoleId = Id FROM [Role] WHERE [Name] = 'Super User';
SELECT @receptionistRoleId = Id FROM [Role] WHERE [Name] = 'Receptionist';
SELECT @supportRoleId = Id FROM [Role] WHERE [Name] = 'Operational Support';
SELECT @kcUserRoleId = Id FROM [Role] WHERE [Name] = 'KC User';

INSERT INTO [RolePermission](RoleId, PermissionId, SecurityEntityTypeId, LastEditedByUserId)
VALUES (@superUserRoleId, 1, 1, 1), --Super User
	   (@superUserRoleId, 2, 1, 1),
	   (@superUserRoleId, 3, 1, 1),
	   (@superUserRoleId, 4, 1, 1),
	   (@superUserRoleId, 5, 1, 1),
	   (@superUserRoleId, 6, 1, 1),
	   (@superUserRoleId, 7, 1, 1),
	   (@superUserRoleId, 8, 1, 1),
	   (@superUserRoleId, 9, 1, 1),
	   --Receptionist
	   (@receptionistRoleId, 4, 1, 1),
	   (@receptionistRoleId, 6, 1, 1),
	   --Operational Support
	   (@supportRoleId, 4, 1, 1),
	   (@supportRoleId, 5, 1, 1),
	   (@supportRoleId, 6, 1, 1),
	   (@supportRoleId, 7, 1, 1),
	   (@supportRoleId, 8, 1, 1),
	   (@supportRoleId, 9, 1, 1),
	   --KC User
	   (@kcUserRoleId, 9, 2, 1);