CREATE TABLE [dbo].[TournamentRules] (
    [TournamnetRulesID] INT      IDENTITY (1, 1) NOT NULL,
    [TournamentID]      INT      NOT NULL,
    [NumberOfRounds]    INT      NOT NULL,
    [HasByes]           BIT      NOT NULL,
    [HasEntryFee]       BIT      NOT NULL,
    [EntryFee]          MONEY    NULL,
    [PrizePurse]        MONEY    NULL,
    [NumberOfPlayers]   INT      NULL,
    [IsPublic]          BIT      NOT NULL,
    [BracketID]         INT      NOT NULL,
    [CutoffDate]        DATETIME NOT NULL,
    [StartDate]         DATETIME NOT NULL,
    [EndDate]           DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([TournamnetRulesID] ASC),
    CONSTRAINT [FK_TournamentRulesBracketID_ToBrackets] FOREIGN KEY ([BracketID]) REFERENCES [dbo].[Brackets] ([BracketID]),
    CONSTRAINT [FK_TournamentRulesTournamentID_ToTournaments] FOREIGN KEY ([TournamentID]) REFERENCES [dbo].[Tournaments] ([TournamentID])
);

