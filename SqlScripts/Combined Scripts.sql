INSERT INTO SecurityEntityType(Id, [Name], [Description])
VALUES (1, 'Administrator', 'Administrator'),
	   (2, 'User', 'User');


INSERT INTO [Permission](Id, [Name], [Description])
VALUES (1, 'SuperUser', 'Super User'),
	   (2, 'UpdatePrivilegedPermissions', 'Update Privileged Permissions'),
	   (3, 'ManageSettings', 'Manage Settings'),
	   (4, 'ManageUsers', 'Manage Users'),
	   (5, 'ManageRoles', 'Manage Roles'),
	   (6, 'ViewOrders', 'View Orders'),
	   (7, 'ProcessOrder', 'Process Order'),
	   (8, 'ManageProduct', 'Manage Product'),
	   (9, 'ViewOrderHistory', 'View Order History')


INSERT INTO [PermissionSecurityEntityType](PermissionId, SecurityEntityTypeId)
VALUES (1, 1),
	   (2, 1),
	   (3, 1),
	   (4, 1), --1 -> Admin Entity (Section)
	   (5, 1),
	   (6, 1),
	   (7, 1),
	   (8, 1),
	   (9, 1),
	   (9, 2); -- User Entity (Section)


INSERT INTO [User] (Username, FirstName, LastName, MobileNumber, ImageReferenceId, CreationDate, ActivatedDate, ActivationEmailSentDate, LastLoginDate, PasswordHash, PasswordSalt, ValidatedEmail, Deleted)
VALUES ('superuser@khumalocraft.co.za', 'Super', 'User', '0821345679', NULL, GETDATE(), GETDATE(), GETDATE(), NULL, 0x0000000000000000, 0x6BC735A23F6CB069, 1, 0)


INSERT INTO [Role] (SecurityEntityTypeId, [Name], UpdatedDate, LastEditedByUserId)
VALUES (1, 'Super User', GETDATE(), 1),
	   (1, 'Receptionist', GETDATE(), 1),
	   (1, 'Operational Support', GETDATE(), 1),
	   (2, 'KC User', GETDATE(), 1)


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

INSERT INTO [UserRole] (RoleId, SecurityEntityTypeId, UserId, LastEditedByUserId)
VALUES (1, 1, 1, 1) --Super User Role Assigned to Super User


INSERT INTO Setting ([Name], [Value], LastEditedByUserId, UpdatedDate) VALUES
('ConcurrentEmailProcessCount', '1', 1, GETDATE()),
('EmailQueueSmtpFaultTolerance', '3', 1, GETDATE()),
('EnableEmailDomainChecking', 'True', 1, GETDATE()),
('InterestRate', '11.5', 1, GETDATE()),
('MaximumImagePayloadSizeMB', '10', 1, GETDATE()),
('MaximumImageSizeMB', '5', 1, GETDATE()),
('MinimumImageSizeB', '-128', 1, GETDATE()),
('NoReplyEmailEddress', 'KhumaloCraft noreply <no-reply@example.com>', 1, GETDATE()),
('Version', '1.0.0.0', 1, GETDATE()),
('PortalBaseUri', '', 1, GETDATE()),
('ImageServerBaseUri', 'https://kc-img-server.azurewebsites.net', 1, GETDATE()),
('ImageServerFallbackServer', 'https://kc-img-server-fallback.azurewebsites.net', 1, GETDATE()),
('BranchName', 'main', 1, GETDATE()),
('EnableTaskScheduler', 'True', 1, GETDATE()),
('SmtpServer', 'smtp.office365.com', 1, GETDATE()),
('SmtpLoginUsername', '', 1, GETDATE()),
('SmtpLoginPassword', '', 1, GETDATE()),
('VatRate', '0.15', 1, GETDATE()),
('AzureBlobStoragePrimary', '', 1, GETDATE()),
('RateLimitRequestsPerSecond', '30', 1, GETDATE()),
('DataProtectionKey', '', 1, GETDATE()),
('CookieProtectionKey', '', 1, GETDATE()),
('AzureAdCertThumbprint', '', 1, GETDATE()),
('AzureAdApplicationId', '', 1, GETDATE()),
('AzureAdDirectoryId', '', 1, GETDATE()),
('AzureKCResourceConnectionString', '', 1, GETDATE()),
('UseHostNameForCookies', 'True', 1, GETDATE());




INSERT INTO CraftworkCategory (Id, [Name], [Description])
VALUES	(1, 'Woodworking', 'Woodworking'),
		(2, 'Embroidery', 'Embroidery'),
		(3, 'Pottery', 'Pottery'),
		(4, 'Metalworking', 'Metalworking'),
		(5, 'Weaving', 'Weaving'),
		(6, 'Glassblowing', 'Glassblowing'),
		(7, 'TieDye', 'Tie Dye'),
		(8, 'Sculpture', 'Sculpture'),
		(9, 'Sewing', 'Sewing'),
		(10, 'Printmaking', 'Printmaking'),
		(11, 'Beadwork', 'Beadwork'),
		(12, 'RecycledCrafts', 'Recycled Crafts'),
		(13, 'Applique', 'Appliqué'),
		(14, 'Modelmaking', 'Modelmaking'),
		(15, 'Leatherworking', 'Leatherworking'),
		(16, 'Knitting', 'Knitting'),
		(17, 'TraditionalCrafts', 'Traditional Crafts'),
		(18, 'DigitalArt', 'Digital Art'),
		(19, 'MixedMedia', 'Mixed Media'),
		(20, 'Needlepoint', 'Needlepoint'),
		(21, 'StoneCarving', 'Stone Carving'),
		(22, 'PaperCrafts', 'Paper Crafts'),
		(23, 'Collage', 'Collage'),
		(24, 'Textiles', 'Textiles'),
		(25, 'Misc', 'Misc');



INSERT INTO ImageType (Id, [Name])
VALUES (0, 'Undefined'),
	   (1, 'Jpeg'),
	   (2, 'Gif'),
	   (3, 'Png'),
	   (4, 'Tiff'),
	   (5, 'Bmp')


INSERT INTO OrderStatus (Id, [Name], [Description])
VALUES  (1, 'AwaitingPayment', 'Awaiting payment'),
		(2, 'Cancelled', 'Cancelled'),
		(3, 'Completed', 'Completed'),
		(4, 'Declined', 'Declined'),
		(5, 'Pending', 'Pending'),
		(6, 'Shipped', 'Shipped');