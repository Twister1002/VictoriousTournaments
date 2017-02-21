CREATE TABLE [dbo].[Matches] (
    [MatchID]         INT      IDENTITY (1, 1) NOT NULL,
    [ChallengerID]    INT      NULL,
    [DefenderID]      INT      NULL,
    [TournamentID]    INT      NOT NULL,
    [ChallengerScore] INT      NULL,
    [DefenderScore]   INT      NULL,
    [WinnerID]        INT      NULL,
    [RoundNumber]     INT      NULL,
    [StartDateTime]   DATETIME NULL,
    [EndDateTime]     DATETIME NULL,
    [MatchDuration]   TIME (7) NULL,
    [IsBye]           BIT      DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([MatchID] ASC),
    CONSTRAINT [FK_MatchesChallengerID_ToUsers] FOREIGN KEY ([ChallengerID]) REFERENCES [dbo].[Users] ([UserID]),
    CONSTRAINT [FK_MatchesDefenderID_ToUsers] FOREIGN KEY ([DefenderID]) REFERENCES [dbo].[Users] ([UserID]),
    CONSTRAINT [FK_MatchesTournamentID_ToTournaments] FOREIGN KEY ([TournamentID]) REFERENCES [dbo].[Tournaments] ([TournamentID]),
    CONSTRAINT [FK_MatchesWinnerID_ToUsers] FOREIGN KEY ([WinnerID]) REFERENCES [dbo].[Users] ([UserID])
);




GO
CREATE NONCLUSTERED INDEX [IX_DefenderID]
    ON [dbo].[Matches]([DefenderID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ChallengerID]
    ON [dbo].[Matches]([ChallengerID] ASC);

