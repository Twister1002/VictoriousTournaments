CREATE TABLE [dbo].[GameTypes]
(
	[GameTypeID] INT NOT NULL IDENTITY,
    [Title]      NVARCHAR (MAX) NULL, 
    CONSTRAINT [PK_GameTypes] PRIMARY KEY ([GameTypeID]),
)
