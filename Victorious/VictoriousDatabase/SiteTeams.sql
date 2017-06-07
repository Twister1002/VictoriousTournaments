CREATE TABLE [dbo].[SiteTeams]
(
	[SiteTeamID] INT NOT NULL  IDENTITY, 
    [TeamName] NVARCHAR(255) NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [CreatedByID] INT NOT NULL, 
    CONSTRAINT [PK_SiteTeam] PRIMARY KEY ([SiteTeamID]),
	--CONSTRAINT FK_SiteTeam_Accounts_CreatedBy FOREIGN KEY (CreatedByID) REFERENCES Accounts(AccountID)
)
