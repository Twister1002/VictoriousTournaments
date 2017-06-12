CREATE TABLE [dbo].[Accounts]
(
	[AccountID]                    INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]                 NVARCHAR (50) NULL,
    [LastName]                  NVARCHAR (50) NULL,
    [Email]                     NVARCHAR (50) NULL,
    [Username]                  NVARCHAR (50) NULL,
    [Password]                  NVARCHAR (50) NULL,
    [PhoneNumber]               NCHAR (15)    NULL,
    [CreatedOn]                 DATETIME      NULL,
    [LastLogin]                 DATETIME      NULL, 
    [PermissionLevel] INT NOT NULL, 
    [InviteCode] NVARCHAR(256) NULL, 
    [InvitedByID] INT NULL, 
    [Salt] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountID]),
	--CONSTRAINT FK_Accounts_Accounts_InvitedBy FOREIGN KEY (InvitedByID) REFERENCES Accounts(AccountID)
)
