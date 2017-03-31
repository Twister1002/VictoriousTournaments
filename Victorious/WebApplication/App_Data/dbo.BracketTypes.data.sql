SET IDENTITY_INSERT [dbo].[BracketTypes] ON
INSERT INTO [dbo].[BracketTypes] ([BracketTypeID], [TypeName], [Type]) VALUES (1, N'Single Elimination', 1)
INSERT INTO [dbo].[BracketTypes] ([BracketTypeID], [TypeName], [Type]) VALUES (2, N'Double Elimination', 2)
INSERT INTO [dbo].[BracketTypes] ([BracketTypeID], [TypeName], [Type]) VALUES (3, N'Round Robin', 3)
SET IDENTITY_INSERT [dbo].[BracketTypes] OFF
