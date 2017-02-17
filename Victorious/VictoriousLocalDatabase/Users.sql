CREATE TABLE [dbo].[Users] (
    [UserID]      INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (50) NOT NULL,
    [LastName]    NVARCHAR (50) NOT NULL,
    [Email]       NVARCHAR (50) NOT NULL,
    [UserName]    NVARCHAR (50) NOT NULL,
    [Password]    NVARCHAR (50) NOT NULL,
    [PhoneNumber] NCHAR (10)    NULL,
    [CreatedOn]   DATETIME      NOT NULL,
    [LastLogin]   DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([UserID] ASC)
);

