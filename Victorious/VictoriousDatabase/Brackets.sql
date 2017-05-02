CREATE TABLE [dbo].[Brackets]
(
	[BracketId]               INT            IDENTITY (1, 1) NOT NULL,
    [BracketTitle]            NVARCHAR (MAX) NULL,
    [BracketTypeId]           INT            NOT NULL,
    [Finalized]               BIT            NOT NULL,
    [NumberOfGroups]          INT            NOT NULL,
    [TournamentId]			  INT			 NULL, 
    CONSTRAINT [PK_Brackets] PRIMARY KEY ([BracketId]),
	CONSTRAINT FK_Brackets_Tournaments FOREIGN KEY([TournamentId]) REFERENCES Tournaments([TournamentId]),

)
