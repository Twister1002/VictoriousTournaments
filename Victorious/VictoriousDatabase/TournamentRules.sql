CREATE TABLE [dbo].[TournamentRules]
(
	[TournamentRulesID]     INT      IDENTITY (1, 1) NOT NULL,
    [TournamentID]          INT      NOT NULL,
    [NumberOfRounds]        INT      NOT NULL,
    [HasEntryFee]           BIT      NOT NULL,
    [EntryFee]              MONEY    NULL,
    [PrizePurse]            MONEY    NOT NULL,
    [IsPublic]              BIT      NOT NULL,
    [RegistrationStartDate] DATETIME NULL,
    [RegistrationEndDate]   DATETIME NULL,
    [TournamentStartDate]   DATETIME NULL,
    [TournamentEndDate]     DATETIME NULL,
    [CheckInBegins]         DATETIME NULL,
    [CheckInEnds]           DATETIME NULL,
    [Platform]              INT      NOT NULL, 
    CONSTRAINT [PK_TournamentRules] PRIMARY KEY ([TournamentRulesID]),
	CONSTRAINT FK_TournamentRules_Tournaments FOREIGN KEY(TournamentID) REFERENCES Tournaments(TournamentID)
)
