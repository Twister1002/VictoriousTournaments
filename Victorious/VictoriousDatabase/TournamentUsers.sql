CREATE TABLE [dbo].[TournamentUsers]
(
	[TournamentUserID] INT NOT NULL , 
    [AccountID] INT NULL, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
	[Username] NVARCHAR(50) NULL,
    [Seed] INT NULL, 
    [UniformNumber] INT NULL, 
    [TournamentID] INT NOT NULL,
    CONSTRAINT [PK_TournamentUsers] PRIMARY KEY ([TournamentUserID]),
	CONSTRAINT FK_TournamentUser_Tournaments FOREIGN KEY(TournamentID) REFERENCES Tournaments(TournamentID)

)
