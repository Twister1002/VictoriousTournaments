CREATE TABLE [dbo].[Defenders]
(
	[MatchID] INT NOT NULL, 
    [TournamentUserID] INT NOT NULL,
	--CONSTRAINT FK_Defenders_Matches FOREIGN KEY(MatchID) REFERENCES Matches(MatchID),
	--CONSTRAINT FK_Defenders_TournamentUsers FOREIGN KEY(TournamentUserID) REFERENCES TournamentUsers(TournamentUserID), 
    CONSTRAINT [PK_Defenders] PRIMARY KEY ([MatchID], [TournamentUserID])


)
