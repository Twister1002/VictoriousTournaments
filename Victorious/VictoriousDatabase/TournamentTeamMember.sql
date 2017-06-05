CREATE TABLE [dbo].[TournamentTeamMember]
(
	[TournamentTeamMemberID] INT NOT NULL IDENTITY, 
    [TournamentTeamID] INT NOT NULL, 
    [SiteTeamMemberID] INT NOT NULL, 
    CONSTRAINT [PK_TournamentTeamMember] PRIMARY KEY ([TournamentTeamMemberID]),
	CONSTRAINT FK_TournamentTeamMember_TournamentTeams_TournamentTeamID FOREIGN KEY (TournamentTeamID) REFERENCES TournamentTeams(TournamentTeamID),
	CONSTRAINT FK_TournamentTeamMember_SiteTeamMembers_SiteTeamMemberID FOREIGN KEY (SiteTeamMemberID) REFERENCES SiteTeamMembers(SiteTeamMemberID),


)
