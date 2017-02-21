namespace DataLib
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

     partial class VictoriousDbContext : DbContext
    {
        public VictoriousDbContext()
            : base("name=CloudStagingDb")
        {
           
        }

        public DbSet<Bracket> Brackets { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<TournamentRule> TournamentRules { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Bracket>()
            //    .HasMany(e => e.TournamentRules)
            //    .WithRequired(e => e.Bracket)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<TournamentRule>()
                .Property(e => e.EntryFee)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TournamentRule>()
                .Property(e => e.PrizePurse)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Tournament>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Tournament>()
                .HasMany(e => e.Matches)
                .WithRequired(e => e.Tournament)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tournament>()
                .HasOptional(e => e.TournamentRules)
                .WithRequired()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tournament>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Tournaments)
                .Map(m => m.ToTable("UsersInTournament").MapLeftKey("TournamentID").MapRightKey("UserID"));

            modelBuilder.Entity<User>()
                .Property(e => e.PhoneNumber)
                .IsFixedLength();

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
