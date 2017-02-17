CREATE TABLE [dbo].[Matches] (
    [MatchID]         INT      IDENTITY (1, 1) NOT NULL,
    [ChallengerID]    INT      NOT NULL,
    [DefenderID]      INT      NOT NULL,
    [TournamentID]    INT      NOT NULL,
    [ChallengerScore] INT      NULL,
    [DefenderScore]   INT      NULL,
    [WinnerID]        INT      NULL,
    [RoundNumber]     INT      NOT NULL,
    [StartDateTime]   DATETIME NOT NULL,
    [EndDateTime]     DATETIME NOT NULL,
    [MatchDuration]   TIME (7) NULL,
    PRIMARY KEY CLUSTERED ([MatchID] ASC),
    CONSTRAINT [FK_MatchesChallengerID_ToUsers] FOREIGN KEY ([ChallengerID]) REFERENCES [dbo].[Users] ([UserID]),
    CONSTRAINT [FK_MatchesDefenderID_ToUsers] FOREIGN KEY ([DefenderID]) REFERENCES [dbo].[Users] ([UserID]),
    CONSTRAINT [FK_MatchesTournamentID_ToTournaments] FOREIGN KEY ([TournamentID]) REFERENCES [dbo].[Tournaments] ([TournamentID]),
    CONSTRAINT [FK_MatchesWinnerID_ToUsers] FOREIGN KEY ([WinnerID]) REFERENCES [dbo].[Users] ([UserID])
);

