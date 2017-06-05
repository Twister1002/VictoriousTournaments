CREATE TABLE [dbo].[TournamentTeamBrackets]
(
	[TournamentTeamID] INT NOT NULL , 
    [BracketID] INT NOT NULL, 
    [Seed] INT NOT NULL, 
    PRIMARY KEY ([TournamentTeamID], [BracketID]),
    CONSTRAINT FK_TournamentTeamsBrackets_TournamentTeam_TournamentTeamID FOREIGN KEY(TournamentTeamID) REFERENCES TournamentTeams(TournamentTeamID) ON DELETE CASCADE,
    CONSTRAINT FK_TournamentTeamsBrackets_Brackets_BracketID FOREIGN KEY(BracketID) REFERENCES Brackets(BracketID) ON DELETE CASCADE,


)
