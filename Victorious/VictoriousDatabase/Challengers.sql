CREATE TABLE [dbo].[Challengers]
(
	[MatchID] INT NOT NULL, 
    [TournamentUserID] INT NOT NULL 
	CONSTRAINT FK_Challengers_Matches FOREIGN KEY(MatchID) REFERENCES Matches(MatchID),
	CONSTRAINT FK_Challengers_TournamentUsers FOREIGN KEY(TournamentUserID) REFERENCES TournamentUsers(TournamentUserID), 
    CONSTRAINT [PK_Challengers] PRIMARY KEY ([MatchID], [TournamentUserID])
)
