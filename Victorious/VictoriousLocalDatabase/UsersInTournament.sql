CREATE TABLE [dbo].[UsersInTournament] (
    [UserID]       INT NOT NULL,
    [TournamentID] INT NOT NULL,
    CONSTRAINT [FK_UsersInTournament_ToTournaments] FOREIGN KEY ([TournamentID]) REFERENCES [dbo].[Tournaments] ([TournamentID]),
    CONSTRAINT [FK_UsersInTournament_ToUsers] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
);

