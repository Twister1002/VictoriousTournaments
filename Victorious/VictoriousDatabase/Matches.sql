CREATE TABLE [dbo].[Matches]
(
	[MatchId]                   INT      IDENTITY (1, 1) NOT NULL,
    [ChallengerId]              INT      NOT NULL,
    [DefenderId]                INT      NOT NULL,
    [WinnerId]                  INT      NULL,
    [ChallengerScore]           INT      NULL,
    [DefenderScore]             INT      NULL,
    [RoundIndex]                INT      NULL,
    [MatchNumber]               INT      NOT NULL,
    [IsBye]                     BIT      NULL,
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
    [BracketId] INT NULL, 
    CONSTRAINT [PK_Matches] PRIMARY KEY ([MatchId]),
	CONSTRAINT FK_Matches_Brackets FOREIGN KEY([BracketId]) REFERENCES Brackets([BracketId])

)
