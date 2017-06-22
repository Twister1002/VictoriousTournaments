CREATE TABLE [dbo].[BracketTypes]
(
	[BracketTypeID] INT NOT NULL,
    [TypeName]      NVARCHAR (MAX) NULL,
    [Type]          INT            NOT NULL, 
    [IsActive] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_BracketTypes] PRIMARY KEY ([BracketTypeID]),
)
