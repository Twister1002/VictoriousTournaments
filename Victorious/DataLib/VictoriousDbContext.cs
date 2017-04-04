namespace DataLib
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    partial class VictoriousDbContext : DbContext
    {
#if DEBUG
        public VictoriousDbContext()
            : base("name=VictoriousTestDbContext")
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<VictoriousDbContext>());
            //Database.CreateIfNotExists();
            //Database.SetInitializer<VictoriousDbContext>(null);
            this.Configuration.LazyLoadingEnabled = true;

        }
#elif !DEBUG
        public VictoriousDbContext()
           : base(@"Data Source=.\;Initial Catalog=VictoriousLocalDatabase;Integrated Security=True;")
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<VictoriousDbContext>());
            this.Configuration.LazyLoadingEnabled = false;
        }
#endif

        public DbSet<BracketModel> Brackets { get; set; }
        public DbSet<MatchModel> Matches { get; set; }
        public DbSet<TournamentRuleModel> TournamentRules { get; set; }
        public DbSet<TournamentModel> Tournaments { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserBracketSeedModel> UserBracketSeeds { get; set; }
        public DbSet<TeamModel> Teams { get; set; }
        public DbSet<TeamMemberModel> TeamMembers { get; set; }
        public DbSet<BracketTypeModel> BracketTypes { get; set; }
        public DbSet<UserInTournamentModel> UsersInTournaments { get; set; }
        public DbSet<GameModel> Games { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            //modelBuilder.Entity<Bracket>()
            //    .HasMany(e => e.TournamentRules)
            //    .WithRequired(e => e.Bracket)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<BracketModel>()
                .HasMany(e => e.Matches)
                .WithRequired()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BracketModel>()
                .HasRequired(e => e.Tournament)
                .WithMany(e => e.Brackets)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<UsersInTournamentsModel>()
            //     .HasKey(e => e.UserID)
            //     .HasKey(e => e.TournamentID);

            //modelBuilder.Entity<TournamentRuleModel>()
            //    .HasOptional(e => e.Tournament)
            //    .WithOptionalDependent(e => e.TournamentRules);

            //modelBuilder.Entity<TournamentRuleModel>()
            //    .Property(e => e.TournamentRulesID)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //modelBuilder.Entity<TournamentModel>()
            //    .HasOptional(e => e.TournamentRules)
            //    .WithOptionalPrincipal(e => e.Tournament);               

            //modelBuilder.Entity<TournamentModel>()
            //    .HasOptional(e => e.TournamentRules)
            //    .WithRequired()
            //    .Map(k => k.MapKey("TournamentRulesID"));

            modelBuilder.Entity<TournamentRuleModel>()
                .Property(e => e.EntryFee)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TournamentRuleModel>()
                .Property(e => e.PrizePurse)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TournamentModel>()
                .Property(e => e.Description)
                .IsUnicode(false);

            //modelBuilder.Entity<TournamentModel>()
            //    .HasMany(e => e.Matches)
            //    .WithRequired(e => e.Tournament)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<BracketModel>()
                .HasMany(e => e.UserSeeds)
                .WithRequired()
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TournamentModel>()
            //    .HasOptional(e => e.TournamentRules)
            //    .WithRequired()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TournamentModel>()
            //    .HasMany(e => e.Users)
            //    .WithMany(e => e.Tournaments)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("TournamentID");
            //        m.MapRightKey("UserID");
            //        m.ToTable("UsersTournaments");
            //    });

            //modelBuilder.Entity<TournamentModel>()
            //    .HasMany(e => e.Users)
            //    .WithMany(e => e.Tournaments);

            modelBuilder.Entity<TeamModel>()
                .HasMany(e => e.TeamMembers)
                .WithOptional()
                .WillCascadeOnDelete(false);


            //modelBuilder.Entity<TournamentModel>()
            //    .HasMany(e => e.Users)
            //    .WithOptional()
            //    .WillCascadeOnDelete(true);
            //modelBuilder.Entity<TournamentModel>()
            //    .HasMany(e => e.Users)
            //    .WithMany(e => e.Tournaments)
            //    .Map(m => m.ToTable("UsersTournaments").MapLeftKey("TournamentID").MapRightKey("UserID"));

            modelBuilder.Entity<UserModel>()
                .Property(e => e.PhoneNumber)
                .IsFixedLength();

            modelBuilder.Entity<UserModel>()
                .ToTable("Users");

            modelBuilder.Entity<TournamentModel>()
                .ToTable("Tournaments");

            modelBuilder.Entity<TournamentRuleModel>()
                .ToTable("TournamentRules");

            modelBuilder.Entity<MatchModel>()
                .ToTable("Matches");

            modelBuilder.Entity<BracketModel>()
                .ToTable("Brackets");

            modelBuilder.Entity<TeamModel>()
                .ToTable("Teams");

            modelBuilder.Entity<TeamMemberModel>()
                .ToTable("TeamMembers");

            modelBuilder.Entity<BracketTypeModel>()
                .ToTable("BracketTypes");

            modelBuilder.Entity<UserInTournamentModel>()
                .ToTable("UsersInTournaments");

            modelBuilder.Entity<TeamMemberModel>()
                .ToTable("TeamMembers");

            //modelBuilder.Entity<MatchModel>()

            modelBuilder.Entity<MatchModel>()
                .HasOptional(e => e.Challenger)
                .WithMany(e => e.ChallengerMatches);

            modelBuilder.Entity<MatchModel>()
                .HasOptional(e => e.Defender)
                .WithMany(e => e.DefenderMatches);

            //modelBuilder.Entity<MatchModel>()
            //    .HasRequired(e => e.Challenger)
            //    .WithMany(e => e.ChallengerMatches)
            //    .HasForeignKey(e => e.ChallengerID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<MatchModel>()
            //   .HasRequired(e => e.Defender)
            //   .WithMany(e => e.DefenderMatches)
            //   .HasForeignKey(e => e.DefenderID)
            //   .WillCascadeOnDelete(false);

            //modelBuilder.Entity<UserModel>()
            //    .HasMany(e => e.Matches);

            //modelBuilder.Entity<Match>()
            //    .HasRequired(e => e.Winner)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.Matches)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(e => e.ChallengerID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.Matches1)
            //    .WithRequired(e => e.User1)
            //    .HasForeignKey(e => e.DefenderID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.Matches2)
            //    .WithOptional(e => e.User2)
            //    .HasForeignKey(e => e.WinnerID);

        }
    }
}
