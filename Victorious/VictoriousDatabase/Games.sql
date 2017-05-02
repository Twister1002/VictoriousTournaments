CREATE TABLE [dbo].[Games]
(
	[GameId]          INT IDENTITY (1, 1) NOT NULL,
    [ChallengerId]    INT NOT NULL,
    [DefenderId]      INT NOT NULL,
    [WinnerId]        INT NOT NULL,
    [MatchId]         INT NULL,
    [GameNumber]      INT NOT NULL,
    [ChallengerScore] INT NOT NULL,
    [DefenderScore]   INT NOT NULL, 
    CONSTRAINT [PK_Games] PRIMARY KEY ([GameId]),
	CONSTRAINT FK_Games_Matches FOREIGN KEY (MatchId) REFERENCES Matches([MatchId])
)
