CREATE TABLE [dbo].[Tournaments]
(
	[TournamentID]      INT            IDENTITY (1, 1) NOT NULL,
    [GameTypeID]        INT            NULL,
    [Title]             NVARCHAR (MAX) NOT NULL,
    [Description]       TEXT           NULL,
    [CreatedOn]         DATETIME       NOT NULL,
    [CreatedByID]       INT            NOT NULL,
    [WinnerID]          INT            NOT NULL,
    [LastEditedOn]      DATETIME       NOT NULL,
    [LastEditedByID]    INT            NOT NULL, 
    CONSTRAINT [PK_Tournaments] PRIMARY KEY ([TournamentID]), 
)
