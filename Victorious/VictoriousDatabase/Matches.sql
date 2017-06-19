CREATE TABLE [dbo].[Matches]
(
	[MatchID]                   INT      IDENTITY (1, 1) NOT NULL,
    [ChallengerID]              INT      NOT NULL,
    [DefenderID]                INT      NOT NULL,
    [WinnerID]                  INT      NULL,
    [ChallengerScore]           INT      NULL,
    [DefenderScore]             INT      NULL,
    [RoundIndex]                INT      NULL,
    [MatchNumber]               INT      NOT NULL,
    [StartDateTime]             DATETIME NULL,
    [EndDateTime]               DATETIME NULL,
    [MatchDuration]             TIME (7) NULL,
    [MatchIndex]                INT      NULL,
    [NextMatchNumber]           INT      NULL,
    [PrevMatchIndex]            INT      NULL,
    [NextLoserMatchNumber]      INT      NULL,
    [PrevDefenderMatchNumber]   INT      NULL,
    [PrevChallengerMatchNumber] INT      NULL,
    [MaxGames]                  INT      NULL, 
    [BracketID] INT NULL, 
    [IsManualWin] BIT NOT NULL DEFAULT 0, 
    [GroupNumber] INT NULL, 
    CONSTRAINT [PK_Matches] PRIMARY KEY ([MatchID]),
	CONSTRAINT FK_Matches_Brackets FOREIGN KEY(BracketID) REFERENCES Brackets(BracketID)

)
