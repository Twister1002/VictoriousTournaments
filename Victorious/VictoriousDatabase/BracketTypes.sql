CREATE TABLE [dbo].[BracketTypes]
(
	[BracketTypeID] INT NOT NULL,
    [TypeName]      NVARCHAR (MAX) NULL,
    [Type]          INT            NOT NULL, 
    CONSTRAINT [PK_BracketTypes] PRIMARY KEY ([BracketTypeID]),
)
