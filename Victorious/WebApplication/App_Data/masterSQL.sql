SET IDENTITY_INSERT [dbo].[Accounts] ON
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(1, N'Tyler', N'Yeary', N'test@test.test', N'test', N'test', NULL, NULL, N'2017-05-03 15:31:06', 1)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(101, N'Player', N'1', N'PlayerTesting', N'Player1', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(102, N'Player', N'2', N'PlayerTesting', N'Player2', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(103, N'Player', N'3', N'PlayerTesting', N'Player3', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(104, N'Player', N'4', N'PlayerTesting', N'Player4', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(105, N'Player', N'5', N'PlayerTesting', N'Player5', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(106, N'Player', N'6', N'PlayerTesting', N'Player6', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(107, N'Player', N'7', N'PlayerTesting', N'Player7', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(108, N'Player', N'8', N'PlayerTesting', N'Player8', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(109, N'Player', N'9', N'PlayerTesting', N'Player9', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(110, N'Player', N'10', N'PlayerTesting', N'Player10', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(111, N'Player', N'11', N'PlayerTesting', N'Player11', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(112, N'Player', N'12', N'PlayerTesting', N'Player12', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
INSERT INTO [dbo].[Accounts] ([AccountID], [FirstName], [LastName], [Email], [Username], [Password], [PhoneNumber], [CreatedOn], [LastLogin], [PermissionLevel]) VALUES 
(113, N'Player', N'13', N'PlayerTesting', N'Player13', N'test', NULL, NULL, N'2017-05-03 15:31:06', 2)
SET IDENTITY_INSERT [dbo].[Accounts] OFF
SET IDENTITY_INSERT [dbo].[GameTypes] ON
INSERT INTO [dbo].[GameTypes] ([GameTypeID], [Title]) VALUES (1, N'Test Game')
SET IDENTITY_INSERT [dbo].[GameTypes] OFF
SET IDENTITY_INSERT [dbo].[Tournaments] ON
INSERT INTO [dbo].[Tournaments] ([TournamentID], [GameTypeID], [Title], [Description], [CreatedOn], [CreatedByID], [WinnerID], [LastEditedOn], [LastEditedByID], [EntryFee], [PrizePurse], [IsPublic], [RegistrationStartDate], [RegistrationEndDate], [TournamentStartDate], [TournamentEndDate], [CheckInBegins], [CheckInEnds], [Platform], [InProgress]) VALUES 
(1, 1, N'test', N'test', N'2017-05-03 14:58:16', 1, -1, N'2017-05-03 14:58:16', 0, NULL, CAST(0.0000 AS Money), 1, N'2017-05-03 00:00:00', N'2017-05-04 00:00:00', N'2017-05-05 00:00:00', N'2017-05-05 00:00:00', N'2017-05-05 00:00:00', N'2017-05-05 00:00:00', 0, 0)
INSERT INTO [dbo].[Tournaments] ([TournamentID], [GameTypeID], [Title], [Description], [CreatedOn], [CreatedByID], [WinnerID], [LastEditedOn], [LastEditedByID], [EntryFee], [PrizePurse], [IsPublic], [RegistrationStartDate], [RegistrationEndDate], [TournamentStartDate], [TournamentEndDate], [CheckInBegins], [CheckInEnds], [Platform], [InProgress]) VALUES 
(2, 1, N'testing', N'Omg this workgin', N'2017-05-03 15:01:52', 1, -1, N'2017-05-03 17:29:08', 1, NULL, CAST(0.0000 AS Money), 1, N'2017-05-03 00:00:00', N'2017-05-04 00:00:00', N'2017-05-05 00:00:00', N'2017-05-05 00:00:00', N'2017-05-05 00:00:00', N'2017-05-05 00:00:00', 0, 0)
INSERT INTO [dbo].[Tournaments] ([TournamentID], [GameTypeID], [Title], [Description], [CreatedOn], [CreatedByID], [WinnerID], [LastEditedOn], [LastEditedByID], [EntryFee], [PrizePurse], [IsPublic], [RegistrationStartDate], [RegistrationEndDate], [TournamentStartDate], [TournamentEndDate], [CheckInBegins], [CheckInEnds], [Platform], [InProgress]) VALUES 
(1001, 1, N'Double Elim Bracket', N'Double Elim', N'2017-05-04 17:22:45', 1, -1, N'2017-05-04 17:22:45', 0, NULL, CAST(0.0000 AS Money), 1, N'2017-05-04 00:00:00', N'2017-05-05 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', 0, 0)
INSERT INTO [dbo].[Tournaments] ([TournamentID], [GameTypeID], [Title], [Description], [CreatedOn], [CreatedByID], [WinnerID], [LastEditedOn], [LastEditedByID], [EntryFee], [PrizePurse], [IsPublic], [RegistrationStartDate], [RegistrationEndDate], [TournamentStartDate], [TournamentEndDate], [CheckInBegins], [CheckInEnds], [Platform], [InProgress]) VALUES 
(1002, 1, N'Round Robin', N'Round Robin', N'2017-05-04 17:23:08', 1, -1, N'2017-05-04 17:23:08', 0, NULL, CAST(0.0000 AS Money), 1, N'2017-05-04 00:00:00', N'2017-05-05 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', 0, 0)
INSERT INTO [dbo].[Tournaments] ([TournamentID], [GameTypeID], [Title], [Description], [CreatedOn], [CreatedByID], [WinnerID], [LastEditedOn], [LastEditedByID], [EntryFee], [PrizePurse], [IsPublic], [RegistrationStartDate], [RegistrationEndDate], [TournamentStartDate], [TournamentEndDate], [CheckInBegins], [CheckInEnds], [Platform], [InProgress]) VALUES 
(1003, 1, N'Swiss', NULL, N'2017-05-04 17:23:44', 1, -1, N'2017-05-04 17:53:31', 1, NULL, CAST(0.0000 AS Money), 1, N'2017-05-04 00:00:00', N'2017-05-05 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', N'2017-05-06 00:00:00', 0, 0)
SET IDENTITY_INSERT [dbo].[Tournaments] OFF
SET IDENTITY_INSERT [dbo].[Brackets] ON
INSERT INTO [dbo].[Brackets] ([BracketID], [BracketTitle], [BracketTypeID], [Finalized], [NumberOfGroups], [TournamentID], [MaxRounds]) VALUES (2, N'Single', 1, 1, 0, 2, 3)
INSERT INTO [dbo].[Brackets] ([BracketID], [BracketTitle], [BracketTypeID], [Finalized], [NumberOfGroups], [TournamentID], [MaxRounds]) VALUES (1002, NULL, 2, 0, 0, 1001, 0)
INSERT INTO [dbo].[Brackets] ([BracketID], [BracketTitle], [BracketTypeID], [Finalized], [NumberOfGroups], [TournamentID], [MaxRounds]) VALUES (1003, NULL, 3, 0, 0, 1002, 0)
INSERT INTO [dbo].[Brackets] ([BracketID], [BracketTitle], [BracketTypeID], [Finalized], [NumberOfGroups], [TournamentID], [MaxRounds]) VALUES (1004, NULL, 6, 0, 0, 1003, 0)
SET IDENTITY_INSERT [dbo].[Brackets] OFF
SET IDENTITY_INSERT [dbo].[TournamentUsers] ON
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (1, 1, NULL, 2, 100, N'test')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (99991, 101, NULL, 2, 102, N'Player1')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (99992, 102, NULL, 2, 102, N'Player2')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (99993, 103, NULL, 2, 102, N'Player3')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (99994, 104, NULL, 2, 102, N'Player4')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (99995, 105, NULL, 2, 102, N'Player5')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (99996, 106, NULL, 2, 102, N'Player6')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (99997, 107, NULL, 2, 102, N'Player7')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (99998, 108, NULL, 2, 102, N'Player8')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (100991, 1, NULL, 1001, 100, N'test')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (100992, 1, NULL, 1002, 100, N'test')
INSERT INTO [dbo].[TournamentUsers] ([TournamentUserID], [AccountID], [UniformNumber], [TournamentID], [PermissionLevel], [Name]) VALUES (100993, 1, NULL, 1003, 100, N'test')
SET IDENTITY_INSERT [dbo].[TournamentUsers] OFF
