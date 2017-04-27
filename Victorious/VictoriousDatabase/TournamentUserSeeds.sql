CREATE TABLE [dbo].[TournamentUserSeeds]
(
	[TournamentUserID]       INT NOT NULL,
    [BracketID]              INT NOT NULL,
    [Seed]                   INT NOT NULL,
	CONSTRAINT FK_TournamentUserSeeds_TournamentUsers FOREIGN KEY(TournamentUserID) REFERENCES TournamentUsers(TournamentUserID) ON DELETE CASCADE,
	CONSTRAINT FK_TournamentUserSeeds_Brackets FOREIGN KEY(BracketID) REFERENCES Brackets(BracketID) ON DELETE CASCADE

)
