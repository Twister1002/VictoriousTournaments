CREATE TABLE [dbo].[Tournaments] (
    [TournamentID]      INT            IDENTITY (1, 1) NOT NULL,
    [Title]             NVARCHAR (MAX) NOT NULL,
    [Description]       TEXT           NULL,
    [CreatedOn]         DATETIME       NULL,
    [CreatedByID]       INT            NULL,
    [WinnerID]          INT            NULL,
    [LastEditedOn]      DATETIME       NULL,
    [LastEditedByID]    INT            NULL,
    [TournamnetRulesID] INT            NULL,
    PRIMARY KEY CLUSTERED ([TournamentID] ASC),
    CONSTRAINT [FK_dbo.Tournaments_dbo.TournamentRules_TournamnetRulesID] FOREIGN KEY ([TournamnetRulesID]) REFERENCES [dbo].[TournamentRules] ([TournamnetRulesID])
);






GO
CREATE NONCLUSTERED INDEX [IX_TournamentRules_TournamnetRulesID]
    ON [dbo].[Tournaments]([TournamnetRulesID] ASC);

