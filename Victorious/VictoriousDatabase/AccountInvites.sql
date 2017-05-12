CREATE TABLE [dbo].[AccountInvites]
(
	[AccountInviteCode] NVARCHAR(256) NOT NULL PRIMARY KEY, 
    [SentToEmail] NVARCHAR(50) NOT NULL, 
    [SentByID] INT NOT NULL, 
    [DateCreated] DATE NOT NULL, 
    [DateExpires] DATE NOT NULL, 
    [IsExpired] BIT NOT NULL,
	CONSTRAINT FK_UserInvites_Accounts_SentBy FOREIGN KEY (SentByID) REFERENCES Accounts(AccountID)
)
