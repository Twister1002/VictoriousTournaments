SET IDENTITY_INSERT [dbo].[Tournaments] ON
INSERT INTO [dbo].[Tournaments] ([TournamentID], [TournamentRulesID], [Title], [Description], [CreatedOn], [CreatedByID], [WinnerID], [LastEditedOn], [LastEditedByID]) VALUES (1, 1, N'Double Elim Bracket', N'Testing', N'2017-03-30 15:05:04', 1, 0, N'2017-03-30 15:05:04', 0)
INSERT INTO [dbo].[Tournaments] ([TournamentID], [TournamentRulesID], [Title], [Description], [CreatedOn], [CreatedByID], [WinnerID], [LastEditedOn], [LastEditedByID]) VALUES (2, 2, N'Register only', NULL, N'2017-03-30 15:15:40', 1, 0, N'2017-03-30 15:15:40', 0)
INSERT INTO [dbo].[Tournaments] ([TournamentID], [TournamentRulesID], [Title], [Description], [CreatedOn], [CreatedByID], [WinnerID], [LastEditedOn], [LastEditedByID]) VALUES (3, 3, N'Round Robin', NULL, N'2017-03-30 15:24:36', 1, 0, N'2017-03-30 15:24:36', 0)
SET IDENTITY_INSERT [dbo].[Tournaments] OFF
