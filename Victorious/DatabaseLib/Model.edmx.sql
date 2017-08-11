
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/10/2017 21:05:34
-- Generated from EDMX file: H:\Data\Full Sail\Final Project\TournamentBracket\Victorious\DatabaseLib\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [VictoriousDatabase];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Brackets_BracketTypes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Brackets] DROP CONSTRAINT [FK_Brackets_BracketTypes];
GO
IF OBJECT_ID(N'[dbo].[FK_Brackets_Tournaments]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Brackets] DROP CONSTRAINT [FK_Brackets_Tournaments];
GO
IF OBJECT_ID(N'[dbo].[FK_Games_Matches]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Games] DROP CONSTRAINT [FK_Games_Matches];
GO
IF OBJECT_ID(N'[dbo].[FK_Matches_Brackets]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Matches] DROP CONSTRAINT [FK_Matches_Brackets];
GO
IF OBJECT_ID(N'[dbo].[FK_SiteTeamMembers_Accounts_AccountID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SiteTeamMembers] DROP CONSTRAINT [FK_SiteTeamMembers_Accounts_AccountID];
GO
IF OBJECT_ID(N'[dbo].[FK_SiteTeamMembers_SiteTeams_SiteTeamID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SiteTeamMembers] DROP CONSTRAINT [FK_SiteTeamMembers_SiteTeams_SiteTeamID];
GO
IF OBJECT_ID(N'[dbo].[FK_SiteTeamMembers_Tournaments_TournamentID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentTeams] DROP CONSTRAINT [FK_SiteTeamMembers_Tournaments_TournamentID];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentInvites_Tournaments_TournamentID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentInvites] DROP CONSTRAINT [FK_TournamentInvites_Tournaments_TournamentID];
GO
IF OBJECT_ID(N'[dbo].[FK_Tournaments_GameTypes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tournaments] DROP CONSTRAINT [FK_Tournaments_GameTypes];
GO
IF OBJECT_ID(N'[dbo].[FK_Tournaments_Platforms]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tournaments] DROP CONSTRAINT [FK_Tournaments_Platforms];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentTeamMember_SiteTeamMembers_SiteTeamMemberID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentTeamMembers] DROP CONSTRAINT [FK_TournamentTeamMember_SiteTeamMembers_SiteTeamMemberID];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentTeamMember_TournamentTeams_TournamentTeamID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentTeamMembers] DROP CONSTRAINT [FK_TournamentTeamMember_TournamentTeams_TournamentTeamID];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentTeams_SiteTeams_SiteTeamID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentTeams] DROP CONSTRAINT [FK_TournamentTeams_SiteTeams_SiteTeamID];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentTeamsBrackets_Brackets_BracketID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentTeamBrackets] DROP CONSTRAINT [FK_TournamentTeamsBrackets_Brackets_BracketID];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentTeamsBrackets_TournamentTeam_TournamentTeamID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentTeamBrackets] DROP CONSTRAINT [FK_TournamentTeamsBrackets_TournamentTeam_TournamentTeamID];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentUser_Tournaments]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentUsers] DROP CONSTRAINT [FK_TournamentUser_Tournaments];
GO
IF OBJECT_ID(N'[dbo].[FK_UserBrackets_TournamentUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentUsersBrackets] DROP CONSTRAINT [FK_UserBrackets_TournamentUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_UserInvites_Accounts_SentBy]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccountInvites] DROP CONSTRAINT [FK_UserInvites_Accounts_SentBy];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersBrackets_Tournaments]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentUsersBrackets] DROP CONSTRAINT [FK_UsersBrackets_Tournaments];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[__RefactorLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[__RefactorLog];
GO
IF OBJECT_ID(N'[dbo].[AccountInvites]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AccountInvites];
GO
IF OBJECT_ID(N'[dbo].[Accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Accounts];
GO
IF OBJECT_ID(N'[dbo].[Brackets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Brackets];
GO
IF OBJECT_ID(N'[dbo].[BracketTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BracketTypes];
GO
IF OBJECT_ID(N'[dbo].[Games]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Games];
GO
IF OBJECT_ID(N'[dbo].[GameTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GameTypes];
GO
IF OBJECT_ID(N'[dbo].[MailingList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MailingList];
GO
IF OBJECT_ID(N'[dbo].[Matches]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Matches];
GO
IF OBJECT_ID(N'[dbo].[Platforms]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Platforms];
GO
IF OBJECT_ID(N'[dbo].[SiteTeamMembers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SiteTeamMembers];
GO
IF OBJECT_ID(N'[dbo].[SiteTeams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SiteTeams];
GO
IF OBJECT_ID(N'[dbo].[TournamentInvites]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TournamentInvites];
GO
IF OBJECT_ID(N'[dbo].[Tournaments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tournaments];
GO
IF OBJECT_ID(N'[dbo].[TournamentTeamBrackets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TournamentTeamBrackets];
GO
IF OBJECT_ID(N'[dbo].[TournamentTeamMembers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TournamentTeamMembers];
GO
IF OBJECT_ID(N'[dbo].[TournamentTeams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TournamentTeams];
GO
IF OBJECT_ID(N'[dbo].[TournamentUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TournamentUsers];
GO
IF OBJECT_ID(N'[dbo].[TournamentUsersBrackets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TournamentUsersBrackets];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'C__RefactorLog'
CREATE TABLE [dbo].[C__RefactorLog] (
    [OperationKey] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'BracketModels'
CREATE TABLE [dbo].[BracketModels] (
    [BracketID] int IDENTITY(1,1) NOT NULL,
    [BracketTypeID] int  NOT NULL,
    [Finalized] bit  NOT NULL,
    [NumberOfGroups] int  NOT NULL,
    [TournamentID] int  NULL,
    [MaxRounds] int  NOT NULL,
    [NumberPlayersAdvance] int  NOT NULL,
    [IsLocked] bit  NOT NULL
);
GO

-- Creating table 'BracketTypeModels'
CREATE TABLE [dbo].[BracketTypeModels] (
    [BracketTypeID] int  NOT NULL,
    [TypeName] nvarchar(max)  NULL,
    [Type] int  NOT NULL,
    [IsActive] bit  NOT NULL
);
GO

-- Creating table 'TournamentUserModels'
CREATE TABLE [dbo].[TournamentUserModels] (
    [TournamentUserID] int  NOT NULL,
    [AccountID] int  NULL,
    [UniformNumber] int  NULL,
    [TournamentID] int  NOT NULL,
    [PermissionLevel] int  NULL,
    [Name] nvarchar(50)  NOT NULL,
    [InviteCode] nvarchar(255)  NULL,
    [IsCheckedIn] bit  NOT NULL,
    [CheckInTime] datetime  NULL
);
GO

-- Creating table 'TournamentUsersBracketModels'
CREATE TABLE [dbo].[TournamentUsersBracketModels] (
    [TournamentUserID] int  NOT NULL,
    [BracketID] int  NOT NULL,
    [Seed] int  NULL,
    [TournamentID] int  NOT NULL
);
GO

-- Creating table 'GameModels'
CREATE TABLE [dbo].[GameModels] (
    [GameID] int IDENTITY(1,1) NOT NULL,
    [ChallengerID] int  NOT NULL,
    [DefenderID] int  NOT NULL,
    [WinnerID] int  NOT NULL,
    [MatchID] int  NULL,
    [GameNumber] int  NOT NULL,
    [ChallengerScore] int  NOT NULL,
    [DefenderScore] int  NOT NULL
);
GO

-- Creating table 'PlatformModels'
CREATE TABLE [dbo].[PlatformModels] (
    [PlatformID] int IDENTITY(1,1) NOT NULL,
    [PlatformName] nvarchar(50)  NULL
);
GO

-- Creating table 'AccountModels'
CREATE TABLE [dbo].[AccountModels] (
    [AccountID] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(50)  NULL,
    [LastName] nvarchar(50)  NULL,
    [Email] nvarchar(50)  NULL,
    [Username] nvarchar(50)  NULL,
    [Password] nvarchar(255)  NULL,
    [PhoneNumber] nchar(15)  NULL,
    [CreatedOn] datetime  NULL,
    [LastLogin] datetime  NULL,
    [PermissionLevel] int  NOT NULL,
    [InviteCode] nvarchar(256)  NULL,
    [InvitedByID] int  NULL,
    [Salt] nvarchar(max)  NULL,
    [ReceiveTournamentUpdates] bit  NULL
);
GO

-- Creating table 'MatchModels'
CREATE TABLE [dbo].[MatchModels] (
    [MatchID] int IDENTITY(1,1) NOT NULL,
    [ChallengerID] int  NOT NULL,
    [DefenderID] int  NOT NULL,
    [WinnerID] int  NULL,
    [ChallengerScore] int  NULL,
    [DefenderScore] int  NULL,
    [RoundIndex] int  NULL,
    [MatchNumber] int  NOT NULL,
    [StartDateTime] datetime  NULL,
    [EndDateTime] datetime  NULL,
    [MatchDuration] time  NULL,
    [MatchIndex] int  NULL,
    [NextMatchNumber] int  NULL,
    [PrevMatchIndex] int  NULL,
    [NextLoserMatchNumber] int  NULL,
    [PrevDefenderMatchNumber] int  NULL,
    [PrevChallengerMatchNumber] int  NULL,
    [MaxGames] int  NULL,
    [BracketID] int  NULL,
    [IsManualWin] bit  NOT NULL,
    [GroupNumber] int  NULL
);
GO

-- Creating table 'GameTypeModels'
CREATE TABLE [dbo].[GameTypeModels] (
    [GameTypeID] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NULL
);
GO

-- Creating table 'AccountInviteModels'
CREATE TABLE [dbo].[AccountInviteModels] (
    [AccountInviteID] int IDENTITY(1,1) NOT NULL,
    [AccountInviteCode] nvarchar(255)  NOT NULL,
    [SentToEmail] nvarchar(50)  NOT NULL,
    [SentByID] int  NOT NULL,
    [DateCreated] datetime  NOT NULL,
    [DateExpires] datetime  NOT NULL,
    [IsExpired] bit  NOT NULL
);
GO

-- Creating table 'TournamentModels'
CREATE TABLE [dbo].[TournamentModels] (
    [TournamentID] int IDENTITY(1,1) NOT NULL,
    [GameTypeID] int  NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] varchar(max)  NULL,
    [CreatedOn] datetime  NOT NULL,
    [CreatedByID] int  NOT NULL,
    [WinnerID] int  NOT NULL,
    [LastEditedOn] datetime  NOT NULL,
    [LastEditedByID] int  NOT NULL,
    [EntryFee] decimal(19,4)  NULL,
    [PrizePurse] decimal(19,4)  NOT NULL,
    [RegistrationStartDate] datetime  NOT NULL,
    [RegistrationEndDate] datetime  NOT NULL,
    [TournamentStartDate] datetime  NOT NULL,
    [TournamentEndDate] datetime  NOT NULL,
    [CheckInBegins] datetime  NOT NULL,
    [CheckInEnds] datetime  NOT NULL,
    [PlatformID] int  NOT NULL,
    [InProgress] bit  NOT NULL,
    [InviteCode] nvarchar(256)  NULL,
    [PublicRegistration] bit  NOT NULL,
    [PublicViewing] bit  NOT NULL
);
GO

-- Creating table 'TournamentInviteModels'
CREATE TABLE [dbo].[TournamentInviteModels] (
    [TournamentInviteID] int IDENTITY(1,1) NOT NULL,
    [TournamentInviteCode] nvarchar(256)  NOT NULL,
    [TournamentID] int  NOT NULL,
    [DateExpires] datetime  NULL,
    [IsExpired] bit  NOT NULL,
    [DateCreated] datetime  NOT NULL,
    [NumberOfUses] int  NOT NULL
);
GO

-- Creating table 'SiteTeamMemberModels'
CREATE TABLE [dbo].[SiteTeamMemberModels] (
    [SiteTeamMemberID] int IDENTITY(1,1) NOT NULL,
    [AccountID] int  NOT NULL,
    [SiteTeamID] int  NOT NULL,
    [Role] int  NOT NULL
);
GO

-- Creating table 'SiteTeamModels'
CREATE TABLE [dbo].[SiteTeamModels] (
    [SiteTeamID] int IDENTITY(1,1) NOT NULL,
    [TeamName] nvarchar(255)  NOT NULL,
    [DateCreated] datetime  NOT NULL,
    [CreatedByID] int  NOT NULL
);
GO

-- Creating table 'TournamentTeamBracketModels'
CREATE TABLE [dbo].[TournamentTeamBracketModels] (
    [TournamentTeamID] int  NOT NULL,
    [BracketID] int  NOT NULL,
    [Seed] int  NOT NULL
);
GO

-- Creating table 'TournamentTeamModels'
CREATE TABLE [dbo].[TournamentTeamModels] (
    [TournamentTeamID] int IDENTITY(1,1) NOT NULL,
    [TournamentID] int  NOT NULL,
    [SiteTeamID] int  NOT NULL,
    [CreatedByID] int  NOT NULL,
    [TeamName] nvarchar(255)  NULL
);
GO

-- Creating table 'TournamentTeamMemberModels'
CREATE TABLE [dbo].[TournamentTeamMemberModels] (
    [TournamentTeamMemberID] int IDENTITY(1,1) NOT NULL,
    [TournamentTeamID] int  NOT NULL,
    [SiteTeamMemberID] int  NOT NULL
);
GO

-- Creating table 'MailingLists'
CREATE TABLE [dbo].[MailingLists] (
    [EmailAddress] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'AccountForgetModels'
CREATE TABLE [dbo].[AccountForgetModels] (
    [AccountForgetID] int IDENTITY(1,1) NOT NULL,
    [AccountID] int  NOT NULL,
    [Token] nvarchar(max)  NOT NULL,
    [RequestedDate] datetime  NOT NULL,
    [IsUsed] bit  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [OperationKey] in table 'C__RefactorLog'
ALTER TABLE [dbo].[C__RefactorLog]
ADD CONSTRAINT [PK_C__RefactorLog]
    PRIMARY KEY CLUSTERED ([OperationKey] ASC);
GO

-- Creating primary key on [BracketID] in table 'BracketModels'
ALTER TABLE [dbo].[BracketModels]
ADD CONSTRAINT [PK_BracketModels]
    PRIMARY KEY CLUSTERED ([BracketID] ASC);
GO

-- Creating primary key on [BracketTypeID] in table 'BracketTypeModels'
ALTER TABLE [dbo].[BracketTypeModels]
ADD CONSTRAINT [PK_BracketTypeModels]
    PRIMARY KEY CLUSTERED ([BracketTypeID] ASC);
GO

-- Creating primary key on [TournamentUserID] in table 'TournamentUserModels'
ALTER TABLE [dbo].[TournamentUserModels]
ADD CONSTRAINT [PK_TournamentUserModels]
    PRIMARY KEY CLUSTERED ([TournamentUserID] ASC);
GO

-- Creating primary key on [TournamentUserID], [BracketID] in table 'TournamentUsersBracketModels'
ALTER TABLE [dbo].[TournamentUsersBracketModels]
ADD CONSTRAINT [PK_TournamentUsersBracketModels]
    PRIMARY KEY CLUSTERED ([TournamentUserID], [BracketID] ASC);
GO

-- Creating primary key on [GameID] in table 'GameModels'
ALTER TABLE [dbo].[GameModels]
ADD CONSTRAINT [PK_GameModels]
    PRIMARY KEY CLUSTERED ([GameID] ASC);
GO

-- Creating primary key on [PlatformID] in table 'PlatformModels'
ALTER TABLE [dbo].[PlatformModels]
ADD CONSTRAINT [PK_PlatformModels]
    PRIMARY KEY CLUSTERED ([PlatformID] ASC);
GO

-- Creating primary key on [AccountID] in table 'AccountModels'
ALTER TABLE [dbo].[AccountModels]
ADD CONSTRAINT [PK_AccountModels]
    PRIMARY KEY CLUSTERED ([AccountID] ASC);
GO

-- Creating primary key on [MatchID] in table 'MatchModels'
ALTER TABLE [dbo].[MatchModels]
ADD CONSTRAINT [PK_MatchModels]
    PRIMARY KEY CLUSTERED ([MatchID] ASC);
GO

-- Creating primary key on [GameTypeID] in table 'GameTypeModels'
ALTER TABLE [dbo].[GameTypeModels]
ADD CONSTRAINT [PK_GameTypeModels]
    PRIMARY KEY CLUSTERED ([GameTypeID] ASC);
GO

-- Creating primary key on [AccountInviteCode] in table 'AccountInviteModels'
ALTER TABLE [dbo].[AccountInviteModels]
ADD CONSTRAINT [PK_AccountInviteModels]
    PRIMARY KEY CLUSTERED ([AccountInviteCode] ASC);
GO

-- Creating primary key on [TournamentID] in table 'TournamentModels'
ALTER TABLE [dbo].[TournamentModels]
ADD CONSTRAINT [PK_TournamentModels]
    PRIMARY KEY CLUSTERED ([TournamentID] ASC);
GO

-- Creating primary key on [TournamentInviteCode] in table 'TournamentInviteModels'
ALTER TABLE [dbo].[TournamentInviteModels]
ADD CONSTRAINT [PK_TournamentInviteModels]
    PRIMARY KEY CLUSTERED ([TournamentInviteCode] ASC);
GO

-- Creating primary key on [SiteTeamMemberID] in table 'SiteTeamMemberModels'
ALTER TABLE [dbo].[SiteTeamMemberModels]
ADD CONSTRAINT [PK_SiteTeamMemberModels]
    PRIMARY KEY CLUSTERED ([SiteTeamMemberID] ASC);
GO

-- Creating primary key on [SiteTeamID] in table 'SiteTeamModels'
ALTER TABLE [dbo].[SiteTeamModels]
ADD CONSTRAINT [PK_SiteTeamModels]
    PRIMARY KEY CLUSTERED ([SiteTeamID] ASC);
GO

-- Creating primary key on [TournamentTeamID], [BracketID] in table 'TournamentTeamBracketModels'
ALTER TABLE [dbo].[TournamentTeamBracketModels]
ADD CONSTRAINT [PK_TournamentTeamBracketModels]
    PRIMARY KEY CLUSTERED ([TournamentTeamID], [BracketID] ASC);
GO

-- Creating primary key on [TournamentTeamID] in table 'TournamentTeamModels'
ALTER TABLE [dbo].[TournamentTeamModels]
ADD CONSTRAINT [PK_TournamentTeamModels]
    PRIMARY KEY CLUSTERED ([TournamentTeamID] ASC);
GO

-- Creating primary key on [TournamentTeamMemberID] in table 'TournamentTeamMemberModels'
ALTER TABLE [dbo].[TournamentTeamMemberModels]
ADD CONSTRAINT [PK_TournamentTeamMemberModels]
    PRIMARY KEY CLUSTERED ([TournamentTeamMemberID] ASC);
GO

-- Creating primary key on [EmailAddress] in table 'MailingLists'
ALTER TABLE [dbo].[MailingLists]
ADD CONSTRAINT [PK_MailingLists]
    PRIMARY KEY CLUSTERED ([EmailAddress] ASC);
GO

-- Creating primary key on [AccountForgetID] in table 'AccountForgetModels'
ALTER TABLE [dbo].[AccountForgetModels]
ADD CONSTRAINT [PK_AccountForgetModels]
    PRIMARY KEY CLUSTERED ([AccountForgetID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [BracketID] in table 'TournamentUsersBracketModels'
ALTER TABLE [dbo].[TournamentUsersBracketModels]
ADD CONSTRAINT [FK_UsersBrackets_Tournaments]
    FOREIGN KEY ([BracketID])
    REFERENCES [dbo].[BracketModels]
        ([BracketID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UsersBrackets_Tournaments'
CREATE INDEX [IX_FK_UsersBrackets_Tournaments]
ON [dbo].[TournamentUsersBracketModels]
    ([BracketID]);
GO

-- Creating foreign key on [TournamentUserID] in table 'TournamentUsersBracketModels'
ALTER TABLE [dbo].[TournamentUsersBracketModels]
ADD CONSTRAINT [FK_UserBrackets_TournamentUsers]
    FOREIGN KEY ([TournamentUserID])
    REFERENCES [dbo].[TournamentUserModels]
        ([TournamentUserID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [BracketTypeID] in table 'BracketModels'
ALTER TABLE [dbo].[BracketModels]
ADD CONSTRAINT [FK_Brackets_BracketTypes]
    FOREIGN KEY ([BracketTypeID])
    REFERENCES [dbo].[BracketTypeModels]
        ([BracketTypeID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Brackets_BracketTypes'
CREATE INDEX [IX_FK_Brackets_BracketTypes]
ON [dbo].[BracketModels]
    ([BracketTypeID]);
GO

-- Creating foreign key on [BracketID] in table 'MatchModels'
ALTER TABLE [dbo].[MatchModels]
ADD CONSTRAINT [FK_Matches_Brackets]
    FOREIGN KEY ([BracketID])
    REFERENCES [dbo].[BracketModels]
        ([BracketID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Matches_Brackets'
CREATE INDEX [IX_FK_Matches_Brackets]
ON [dbo].[MatchModels]
    ([BracketID]);
GO

-- Creating foreign key on [MatchID] in table 'GameModels'
ALTER TABLE [dbo].[GameModels]
ADD CONSTRAINT [FK_Games_Matches]
    FOREIGN KEY ([MatchID])
    REFERENCES [dbo].[MatchModels]
        ([MatchID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Games_Matches'
CREATE INDEX [IX_FK_Games_Matches]
ON [dbo].[GameModels]
    ([MatchID]);
GO

-- Creating foreign key on [SentByID] in table 'AccountInviteModels'
ALTER TABLE [dbo].[AccountInviteModels]
ADD CONSTRAINT [FK_UserInvites_Accounts_SentBy]
    FOREIGN KEY ([SentByID])
    REFERENCES [dbo].[AccountModels]
        ([AccountID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserInvites_Accounts_SentBy'
CREATE INDEX [IX_FK_UserInvites_Accounts_SentBy]
ON [dbo].[AccountInviteModels]
    ([SentByID]);
GO

-- Creating foreign key on [TournamentID] in table 'BracketModels'
ALTER TABLE [dbo].[BracketModels]
ADD CONSTRAINT [FK_Brackets_Tournaments]
    FOREIGN KEY ([TournamentID])
    REFERENCES [dbo].[TournamentModels]
        ([TournamentID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Brackets_Tournaments'
CREATE INDEX [IX_FK_Brackets_Tournaments]
ON [dbo].[BracketModels]
    ([TournamentID]);
GO

-- Creating foreign key on [GameTypeID] in table 'TournamentModels'
ALTER TABLE [dbo].[TournamentModels]
ADD CONSTRAINT [FK_Tournaments_GameTypes]
    FOREIGN KEY ([GameTypeID])
    REFERENCES [dbo].[GameTypeModels]
        ([GameTypeID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Tournaments_GameTypes'
CREATE INDEX [IX_FK_Tournaments_GameTypes]
ON [dbo].[TournamentModels]
    ([GameTypeID]);
GO

-- Creating foreign key on [PlatformID] in table 'TournamentModels'
ALTER TABLE [dbo].[TournamentModels]
ADD CONSTRAINT [FK_Tournaments_Platforms]
    FOREIGN KEY ([PlatformID])
    REFERENCES [dbo].[PlatformModels]
        ([PlatformID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Tournaments_Platforms'
CREATE INDEX [IX_FK_Tournaments_Platforms]
ON [dbo].[TournamentModels]
    ([PlatformID]);
GO

-- Creating foreign key on [TournamentID] in table 'TournamentUserModels'
ALTER TABLE [dbo].[TournamentUserModels]
ADD CONSTRAINT [FK_TournamentUser_Tournaments]
    FOREIGN KEY ([TournamentID])
    REFERENCES [dbo].[TournamentModels]
        ([TournamentID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentUser_Tournaments'
CREATE INDEX [IX_FK_TournamentUser_Tournaments]
ON [dbo].[TournamentUserModels]
    ([TournamentID]);
GO

-- Creating foreign key on [TournamentID] in table 'TournamentInviteModels'
ALTER TABLE [dbo].[TournamentInviteModels]
ADD CONSTRAINT [FK_TournamentInvites_Tournaments_TournamentID]
    FOREIGN KEY ([TournamentID])
    REFERENCES [dbo].[TournamentModels]
        ([TournamentID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentInvites_Tournaments_TournamentID'
CREATE INDEX [IX_FK_TournamentInvites_Tournaments_TournamentID]
ON [dbo].[TournamentInviteModels]
    ([TournamentID]);
GO

-- Creating foreign key on [AccountID] in table 'SiteTeamMemberModels'
ALTER TABLE [dbo].[SiteTeamMemberModels]
ADD CONSTRAINT [FK_SiteTeamMembers_Accounts_AccountID]
    FOREIGN KEY ([AccountID])
    REFERENCES [dbo].[AccountModels]
        ([AccountID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SiteTeamMembers_Accounts_AccountID'
CREATE INDEX [IX_FK_SiteTeamMembers_Accounts_AccountID]
ON [dbo].[SiteTeamMemberModels]
    ([AccountID]);
GO

-- Creating foreign key on [BracketID] in table 'TournamentTeamBracketModels'
ALTER TABLE [dbo].[TournamentTeamBracketModels]
ADD CONSTRAINT [FK_TournamentTeamsBrackets_Brackets_BracketID]
    FOREIGN KEY ([BracketID])
    REFERENCES [dbo].[BracketModels]
        ([BracketID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentTeamsBrackets_Brackets_BracketID'
CREATE INDEX [IX_FK_TournamentTeamsBrackets_Brackets_BracketID]
ON [dbo].[TournamentTeamBracketModels]
    ([BracketID]);
GO

-- Creating foreign key on [SiteTeamID] in table 'SiteTeamMemberModels'
ALTER TABLE [dbo].[SiteTeamMemberModels]
ADD CONSTRAINT [FK_SiteTeamMembers_SiteTeams_SiteTeamID]
    FOREIGN KEY ([SiteTeamID])
    REFERENCES [dbo].[SiteTeamModels]
        ([SiteTeamID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SiteTeamMembers_SiteTeams_SiteTeamID'
CREATE INDEX [IX_FK_SiteTeamMembers_SiteTeams_SiteTeamID]
ON [dbo].[SiteTeamMemberModels]
    ([SiteTeamID]);
GO

-- Creating foreign key on [SiteTeamID] in table 'TournamentTeamModels'
ALTER TABLE [dbo].[TournamentTeamModels]
ADD CONSTRAINT [FK_TournamentTeams_SiteTeams_SiteTeamID]
    FOREIGN KEY ([SiteTeamID])
    REFERENCES [dbo].[SiteTeamModels]
        ([SiteTeamID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentTeams_SiteTeams_SiteTeamID'
CREATE INDEX [IX_FK_TournamentTeams_SiteTeams_SiteTeamID]
ON [dbo].[TournamentTeamModels]
    ([SiteTeamID]);
GO

-- Creating foreign key on [TournamentID] in table 'TournamentTeamModels'
ALTER TABLE [dbo].[TournamentTeamModels]
ADD CONSTRAINT [FK_SiteTeamMembers_Tournaments_TournamentID]
    FOREIGN KEY ([TournamentID])
    REFERENCES [dbo].[TournamentModels]
        ([TournamentID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SiteTeamMembers_Tournaments_TournamentID'
CREATE INDEX [IX_FK_SiteTeamMembers_Tournaments_TournamentID]
ON [dbo].[TournamentTeamModels]
    ([TournamentID]);
GO

-- Creating foreign key on [TournamentTeamID] in table 'TournamentTeamBracketModels'
ALTER TABLE [dbo].[TournamentTeamBracketModels]
ADD CONSTRAINT [FK_TournamentTeamsBrackets_TournamentTeam_TournamentTeamID]
    FOREIGN KEY ([TournamentTeamID])
    REFERENCES [dbo].[TournamentTeamModels]
        ([TournamentTeamID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [SiteTeamMemberID] in table 'TournamentTeamMemberModels'
ALTER TABLE [dbo].[TournamentTeamMemberModels]
ADD CONSTRAINT [FK_TournamentTeamMember_SiteTeamMembers_SiteTeamMemberID]
    FOREIGN KEY ([SiteTeamMemberID])
    REFERENCES [dbo].[SiteTeamMemberModels]
        ([SiteTeamMemberID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentTeamMember_SiteTeamMembers_SiteTeamMemberID'
CREATE INDEX [IX_FK_TournamentTeamMember_SiteTeamMembers_SiteTeamMemberID]
ON [dbo].[TournamentTeamMemberModels]
    ([SiteTeamMemberID]);
GO

-- Creating foreign key on [TournamentTeamID] in table 'TournamentTeamMemberModels'
ALTER TABLE [dbo].[TournamentTeamMemberModels]
ADD CONSTRAINT [FK_TournamentTeamMember_TournamentTeams_TournamentTeamID]
    FOREIGN KEY ([TournamentTeamID])
    REFERENCES [dbo].[TournamentTeamModels]
        ([TournamentTeamID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentTeamMember_TournamentTeams_TournamentTeamID'
CREATE INDEX [IX_FK_TournamentTeamMember_TournamentTeams_TournamentTeamID]
ON [dbo].[TournamentTeamMemberModels]
    ([TournamentTeamID]);
GO

-- Creating foreign key on [AccountID] in table 'AccountForgetModels'
ALTER TABLE [dbo].[AccountForgetModels]
ADD CONSTRAINT [FK_AccountModelAccountForgetModel]
    FOREIGN KEY ([AccountID])
    REFERENCES [dbo].[AccountModels]
        ([AccountID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountModelAccountForgetModel'
CREATE INDEX [IX_FK_AccountModelAccountForgetModel]
ON [dbo].[AccountForgetModels]
    ([AccountID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------