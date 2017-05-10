SET IDENTITY_INSERT [dbo].[Brackets] ON
INSERT INTO [dbo].[Brackets] ([BracketID], [BracketTitle], [BracketTypeID], [Finalized], [NumberOfGroups], [TournamentID], [MaxRounds]) VALUES (2, N'Single', 1, 1, 0, 2, 3)
INSERT INTO [dbo].[Brackets] ([BracketID], [BracketTitle], [BracketTypeID], [Finalized], [NumberOfGroups], [TournamentID], [MaxRounds]) VALUES (1002, NULL, 2, 0, 0, 1001, 0)
INSERT INTO [dbo].[Brackets] ([BracketID], [BracketTitle], [BracketTypeID], [Finalized], [NumberOfGroups], [TournamentID], [MaxRounds]) VALUES (1003, NULL, 3, 0, 0, 1002, 0)
INSERT INTO [dbo].[Brackets] ([BracketID], [BracketTitle], [BracketTypeID], [Finalized], [NumberOfGroups], [TournamentID], [MaxRounds]) VALUES (1004, NULL, 6, 0, 0, 1003, 0)
SET IDENTITY_INSERT [dbo].[Brackets] OFF
