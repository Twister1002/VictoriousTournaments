CREATE TABLE [dbo].[Accounts]
(
	[AccountID]                    INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]                 NVARCHAR (50) NULL,
    [LastName]                  NVARCHAR (50) NULL,
    [Email]                     NVARCHAR (50) NULL,
    [Username]                  NVARCHAR (50) NULL,
    [Password]                  NVARCHAR (255) NULL,
    [PhoneNumber]               NCHAR (15)    NULL,
    [CreatedOn]                 DATETIME      NULL,
    [LastLogin]                 DATETIME      NULL, 
    [PermissionLevel] INT NOT NULL, 
    [InviteCode] NVARCHAR(256) NULL, 
    [InvitedByID] INT NULL, 
    [Salt] NVARCHAR(MAX) NULL, 
    [ReceiveTournamentUpdates] BIT NULL DEFAULT 0, 
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountID]),
	--CONSTRAINT FK_Accounts_Accounts_InvitedBy FOREIGN KEY (InvitedByID) REFERENCES Accounts(AccountID)
)
