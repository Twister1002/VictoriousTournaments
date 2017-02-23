namespace DataLib
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    partial class VictoriousDbContext : DbContext
    {
        public VictoriousDbContext()
            : base("name=VictoriousTestDbContext")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<VictoriousDbContext>());
        }

        public DbSet<BracketModel> Brackets { get; set; }
        public DbSet<MatchModel> Matches { get; set; }
        public DbSet<TournamentRuleModel> TournamentRules { get; set; }
        public DbSet<TournamentModel> Tournaments { get; set; }
        public DbSet<UserModel> Users { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Bracket>()
            //    .HasMany(e => e.TournamentRules)
            //    .WithRequired(e => e.Bracket)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TournamentRuleModel>()
                .Property(e => e.TournamnetRulesID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TournamentRuleModel>()
                .Property(e => e.EntryFee)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TournamentRuleModel>()
                .Property(e => e.PrizePurse)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TournamentModel>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<TournamentModel>()
                .HasMany(e => e.Matches)
                .WithRequired(e => e.Tournament)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TournamentModel>()
                .HasOptional(e => e.TournamentRules)
                .WithRequired()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TournamentModel>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Tournaments)
                .Map(m => m.ToTable("UsersInTournament").MapLeftKey("TournamentID").MapRightKey("UserID"));

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

            //modelBuilder.Entity<Match>()
            //    .HasRequired(e => e.Challenger)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);


            //modelBuilder.Entity<Match>()
            //    .HasRequired(e => e.Defender)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);


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
