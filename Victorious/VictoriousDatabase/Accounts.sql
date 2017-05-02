CREATE TABLE [dbo].[Accounts]
(
	[AccountId]                    INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]                 NVARCHAR (50) NULL,
    [LastName]                  NVARCHAR (50) NULL,
    [Email]                     NVARCHAR (50) NULL,
    [Username]                  NVARCHAR (50) NULL,
    [Password]                  NVARCHAR (50) NULL,
    [PhoneNumber]               NCHAR (15)    NULL,
    [CreatedOn]                 DATETIME      NULL,
    [LastLogin]                 DATETIME      NULL, 
    [PermissionLevel] INT NOT NULL, 
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountId]),
)
