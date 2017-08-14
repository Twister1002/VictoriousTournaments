CREATE TABLE [dbo].[Accounts]
(
	[AccountID]                    INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]                 NVARCHAR (50) NOT NULL,
    [LastName]                  NVARCHAR (50) NOT NULL,
    [Email]                     NVARCHAR (50) NOT NULL,
    [Username]                  NVARCHAR (50) NOT NULL,
    [Password]                  NVARCHAR (255) NOT NULL,
    [PhoneNumber]               NCHAR (15)    NULL,
    [CreatedOn]                 DATETIME      NOT NULL,
    [LastLogin]                 DATETIME      NOT NULL, 
    [PermissionLevel] INT NOT NULL, 
    [InviteCode] NVARCHAR(256) NULL, 
    [InvitedByID] INT NULL, 
    [Salt] NVARCHAR(MAX) NOT NULL, 
    [ReceiveTournamentUpdates] BIT NULL DEFAULT 0, 
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountID]), 
    CONSTRAINT [CK_Accounts_Username] UNIQUE(Username),
	--CONSTRAINT FK_Accounts_Accounts_InvitedBy FOREIGN KEY (InvitedByID) REFERENCES Accounts(AccountID)
)
