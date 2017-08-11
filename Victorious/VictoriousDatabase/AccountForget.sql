CREATE TABLE [dbo].[AccountForget]
(
	[AccountForgetID] INT NOT NULL PRIMARY KEY, 
    [AccountID] INT NOT NULL, 
    [Token] VARCHAR(100) NOT NULL, 
    [DateIssued] DATETIME NOT NULL, 
    [DateUsed] DATETIME NULL, 
    CONSTRAINT [FK_AccountForget_ToTable_AccountModel] FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
)
