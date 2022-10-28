INSERT INTO [dbo].[Role] (Id, [Name]) VALUES (newid(), 'admin');
INSERT INTO [dbo].[Resource] ([Id],[IsPublic],[Method],[Url]) VALUES (newid(), 1, 'get', '/');
INSERT INTO [dbo].[Resource] ([Id],[IsPublic],[Method],[Url]) VALUES (newid(), 1, 'post', '/login');
INSERT INTO [dbo].[Resource] ([Id],[IsPublic],[Method],[Url]) VALUES (newid(), 0, 'get', '/private');
INSERT INTO [dbo].[ResourceRole] ([ResourceId] ,[RoleId]) VALUES ((select id from [resource] where [url] = '/private' and method = 'get'), (select id from [role] where [name] = 'admin'));
