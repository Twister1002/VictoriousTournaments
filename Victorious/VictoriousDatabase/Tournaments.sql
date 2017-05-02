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
	[EntryFee]              MONEY    NULL,
    [PrizePurse]            MONEY    NOT NULL,
    [IsPublic]              BIT      NOT NULL,
    [RegistrationStartDate] DATETIME NOT NULL,
    [RegistrationEndDate]   DATETIME NOT NULL,
    [TournamentStartDate]   DATETIME NOT NULL,
    [TournamentEndDate]     DATETIME NOT NULL,
    [CheckInBegins]         DATETIME NOT NULL,
    [CheckInEnds]           DATETIME NOT NULL,
    [Platform]              INT      NOT NULL, 
    CONSTRAINT [PK_Tournaments] PRIMARY KEY ([TournamentID]), 
	CONSTRAINT FK_Tournaments_GameTypes FOREIGN KEY(GameTypeID) REFERENCES GameTypes(GameTypeID)
)
