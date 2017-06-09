CREATE TABLE [dbo].[TournamentTeams]
(
	[TournamentTeamID] INT NOT NULL IDENTITY, 
    [TournamentID] INT NOT NULL, 
    [SiteTeamID] INT NOT NULL, 
    [CreatedByID] INT NOT NULL, 
    [TeamName] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_TournamentTeams] PRIMARY KEY ([TournamentTeamID]),
	CONSTRAINT FK_SiteTeamMembers_Tournaments_TournamentID FOREIGN KEY (TournamentID) REFERENCES Tournaments(TournamentID),
	CONSTRAINT FK_TournamentTeams_SiteTeams_SiteTeamID FOREIGN KEY (SiteTeamID) REFERENCES SiteTeams(SiteTeamID),

)
