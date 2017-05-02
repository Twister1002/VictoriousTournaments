CREATE TABLE [dbo].[TournamentUsers]
(
	[TournamentUserId] INT NOT NULL IDENTITY , 
    [AccountId] INT NULL, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
	[Username] NVARCHAR(50) NULL,
    [Seed] INT NULL, 
    [UniformNumber] INT NULL, 
    [TournamentId] INT NOT NULL,
    CONSTRAINT [PK_TournamentUsers] PRIMARY KEY ([TournamentUserId]),
	CONSTRAINT FK_TournamentUser_Tournaments FOREIGN KEY([TournamentId]) REFERENCES Tournaments([TournamentId])

)
