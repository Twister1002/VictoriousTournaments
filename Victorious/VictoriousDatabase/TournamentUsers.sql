CREATE TABLE [dbo].[TournamentUsers]
(
	[TournamentUserID] INT NOT NULL IDENTITY , 
    [AccountID] INT NULL, 
    [UniformNumber] INT NULL, 
    [TournamentID] INT NOT NULL,
    [PermissionLevel] INT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [InviteCode] NVARCHAR(255) NULL, 
    [IsCheckedIn] BIT NULL, 
    [CheckInTime] DATETIME NULL, 
    CONSTRAINT [PK_TournamentUsers] PRIMARY KEY ([TournamentUserID]),
	CONSTRAINT FK_TournamentUser_Tournaments FOREIGN KEY(TournamentID) REFERENCES Tournaments(TournamentID)

)
