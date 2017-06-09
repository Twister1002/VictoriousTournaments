CREATE TABLE [dbo].[SiteTeamMembers]
(
	[SiteTeamMemberID] INT NOT NULL IDENTITY, 
    [AccountID] INT NOT NULL, 
    [SiteTeamID] INT NOT NULL, 
    [Role] INT NOT NULL, 
    CONSTRAINT [PK_SiteTeamMember] PRIMARY KEY ([SiteTeamMemberID]),
	CONSTRAINT FK_SiteTeamMembers_Accounts_AccountID FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID),
	CONSTRAINT FK_SiteTeamMembers_SiteTeams_SiteTeamID FOREIGN KEY (SiteTeamID) REFERENCES SiteTeams(SiteTeamID),

)
