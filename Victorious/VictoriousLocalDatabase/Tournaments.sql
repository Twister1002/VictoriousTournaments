CREATE TABLE [dbo].[Tournaments] (
    [TournamentID]   INT            IDENTITY (1, 1) NOT NULL,
    [Title]          NVARCHAR (MAX) NOT NULL,
    [Description]    TEXT           NULL,
    [CreatedOn]      DATETIME       NOT NULL,
    [CreatedByID]    INT            NOT NULL,
    [WinnerID]       INT            NULL,
    [LastEditedOn]   DATETIME       NOT NULL,
    [LastEditedByID] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([TournamentID] ASC)
);

