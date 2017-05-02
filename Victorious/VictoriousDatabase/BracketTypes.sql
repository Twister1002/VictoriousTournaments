CREATE TABLE [dbo].[BracketTypes]
(
	[BracketTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [TypeName]      NVARCHAR (MAX) NULL,
    [Type]          INT            NOT NULL, 
    CONSTRAINT [PK_BracketTypes] PRIMARY KEY ([BracketTypeId]),
)
