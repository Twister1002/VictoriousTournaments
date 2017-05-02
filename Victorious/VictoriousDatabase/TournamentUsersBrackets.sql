CREATE TABLE [dbo].[TournamentUsersBrackets]
(
	[TournamentUserId] INT NOT NULL, 
    [BracketId] INT NOT NULL,
	[Seed] INT NULL, 
    CONSTRAINT FK_UsersBrackets_Tournaments FOREIGN KEY([BracketId]) REFERENCES Brackets([BracketId]) ON DELETE CASCADE,
	CONSTRAINT FK_UserBrackets_TournamentUsers FOREIGN KEY([TournamentUserId]) REFERENCES TournamentUsers([TournamentUserId]) ON DELETE CASCADE, 

    CONSTRAINT [PK_TournamentUsersBrackets] PRIMARY KEY ([TournamentUserId], [BracketId])

)
