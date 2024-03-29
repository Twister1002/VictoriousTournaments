﻿CREATE TABLE [dbo].[AccountInvites]
(
	[AccountInviteID] INT NOT NULL IDENTITY, 
	[AccountInviteCode] NVARCHAR(255) NOT NULL , 
    [SentByID] INT NOT NULL, 
    [SentToEmail] NVARCHAR(50) NOT NULL, 
    [DateCreated] DATE NOT NULL, 
    [DateExpires] DATE NOT NULL, 
    [IsExpired] BIT NOT NULL,
    CONSTRAINT FK_UserInvites_Accounts_SentBy FOREIGN KEY (SentByID) REFERENCES Accounts(AccountID), 
    CONSTRAINT [PK_AccountInvites] PRIMARY KEY ([AccountInviteCode])
)
