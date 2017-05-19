CREATE TABLE [dbo].[TournamentInvites]
(
	[TournamentInviteCode] NVARCHAR(256) NOT NULL , 
    [TournamentID] INT NOT NULL,
    [DateExpires] DATE NULL, 
    [IsExpired] BIT NOT NULL, 
    [DateCreated] DATE NOT NULL, 
    [NumberOfUses] INT NOT NULL DEFAULT 0, 
	--CONSTRAINT FK_TournamentInvites_Tournament FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID), 
    CONSTRAINT [PK_TournamentInvites] PRIMARY KEY ([TournamentInviteCode])

)
