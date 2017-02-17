CREATE TABLE [dbo].[Byes] (
    [ByeID]        INT IDENTITY (1, 1) NOT NULL,
    [TournamentID] INT NOT NULL,
    [UserID]       INT NOT NULL,
    [RoundNumber]  INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ByeID] ASC),
    CONSTRAINT [FK_ByesTournamentID_ToTournaments] FOREIGN KEY ([TournamentID]) REFERENCES [dbo].[Tournaments] ([TournamentID]),
    CONSTRAINT [FK_ByesUserID_ToUsers] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
);

