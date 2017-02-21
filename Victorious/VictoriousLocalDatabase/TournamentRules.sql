CREATE TABLE [dbo].[TournamentRules] (
    [TournamnetRulesID] INT      IDENTITY (1, 1) NOT NULL,
    [TournamentID]      INT      NULL,
    [NumberOfRounds]    INT      NULL,
    [HasEntryFee]       BIT      NULL,
    [EntryFee]          MONEY    NULL,
    [PrizePurse]        MONEY    NULL,
    [NumberOfPlayers]   INT      NULL,
    [IsPublic]          BIT      NULL,
    [CutoffDate]        DATETIME NULL,
    [StartDate]         DATETIME NULL,
    [EndDate]           DATETIME NULL,
    [BracketID]         INT      NULL,
    PRIMARY KEY CLUSTERED ([TournamnetRulesID] ASC),
    CONSTRAINT [FK_dbo.TournamentRules_dbo.Brackets_BracketID] FOREIGN KEY ([BracketID]) REFERENCES [dbo].[Brackets] ([BracketID]),
    CONSTRAINT [FK_TournamentRulesTournamentID_ToTournaments] FOREIGN KEY ([TournamentID]) REFERENCES [dbo].[Tournaments] ([TournamentID])
);






GO
CREATE NONCLUSTERED INDEX [IX_BracketID]
    ON [dbo].[TournamentRules]([BracketID] ASC);

