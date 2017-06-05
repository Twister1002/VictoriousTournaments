CREATE TABLE [dbo].[TournamentInvites]
(
	[TournamentInviteID] INT NOT NULL IDENTITY,
	[TournamentInviteCode] NVARCHAR(256) NOT NULL , 
    [TournamentID] INT NOT NULL,
    [DateExpires] DATE NULL, 
    [IsExpired] BIT NOT NULL, 
    [DateCreated] DATE NOT NULL, 
    [NumberOfUses] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_TournamentInvites] PRIMARY KEY ([TournamentInviteCode]),
	CONSTRAINT FK_TournamentInvites_Tournaments_TournamentID FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID) 
)
