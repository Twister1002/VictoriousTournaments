﻿CREATE TABLE [dbo].[Tournaments]
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
    [PlatformID]              INT      NOT NULL, 
    [InProgress] BIT NOT NULL, 
    [InviteCode] NVARCHAR(256) NULL, 
    [PublicRegistration] BIT NOT NULL DEFAULT 0, 
    [PublicViewing] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Tournaments] PRIMARY KEY ([TournamentID]), 
	CONSTRAINT FK_Tournaments_GameTypes FOREIGN KEY(GameTypeID) REFERENCES GameTypes(GameTypeID),
	CONSTRAINT FK_Tournaments_Platforms FOREIGN KEY([PlatformID]) REFERENCES Platforms([PlatformID]),
	CONSTRAINT FK_Tournaments_TournamentInvites FOREIGN KEY(InviteCode) REFERENCES TournamentInvites([TournamentInviteCode]),

	
)
