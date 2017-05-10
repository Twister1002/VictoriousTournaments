CREATE TABLE [dbo].[TournamentUsersBrackets]
(
	[TournamentUserID] INT NOT NULL, 
    [BracketID] INT NOT NULL,
	[Seed] INT NULL, 
    [TournamentID] INT NOT NULL, 
    CONSTRAINT FK_UsersBrackets_Tournaments FOREIGN KEY(BracketID) REFERENCES Brackets(BracketID) ON DELETE CASCADE,
	CONSTRAINT FK_UserBrackets_TournamentUsers FOREIGN KEY(TournamentUserID) REFERENCES TournamentUsers(TournamentUserID) ON DELETE CASCADE, 

    CONSTRAINT [PK_TournamentUsersBrackets] PRIMARY KEY ([TournamentUserID], [BracketID])

)
