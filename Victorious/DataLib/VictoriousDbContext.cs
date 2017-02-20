namespace DataLib
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class VictoriousDbContext : DbContext
    {
        public VictoriousDbContext()
            : base("name=VictoriousDbContext")
        {
        }

        public virtual DbSet<Bracket> Brackets { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<TournamentRule> TournamentRules { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }
        public virtual DbSet<User> Users { get; set; }

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

            //modelBuilder.Entity<Tournament>()
            //    .HasOptional(e => e.TournamentRules)
            //    .WithRequired(e => e.Tournament)
            //    .WillCascadeOnDelete(false);

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