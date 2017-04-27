CREATE TABLE [dbo].[Games]
(
	[GameID]          INT IDENTITY (1, 1) NOT NULL,
    [ChallengerID]    INT NOT NULL,
    [DefenderID]      INT NOT NULL,
    [WinnerID]        INT NOT NULL,
    [MatchID]         INT NULL,
    [GameNumber]      INT NOT NULL,
    [ChallengerScore] INT NOT NULL,
    [DefenderScore]   INT NOT NULL, 
    CONSTRAINT [PK_Games] PRIMARY KEY ([GameID]),
)
