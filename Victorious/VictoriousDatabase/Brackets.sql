CREATE TABLE [dbo].[Brackets]
(
	[BracketID]               INT            IDENTITY (1, 1) NOT NULL,
    [BracketTypeID]           INT            NOT NULL,
    [Finalized]               BIT            NOT NULL,
    [NumberOfGroups]          INT            NOT NULL,
    [TournamentID]			  INT			 NULL, 
    [MaxRounds] INT NOT NULL, 
    CONSTRAINT [PK_Brackets] PRIMARY KEY ([BracketID]),
	CONSTRAINT FK_Brackets_Tournaments FOREIGN KEY(TournamentID) REFERENCES Tournaments(TournamentID),
	CONSTRAINT FK_Brackets_BracketTypes FOREIGN KEY(BracketTypeID) REFERENCES BracketTypes(BracketTypeID)

)
