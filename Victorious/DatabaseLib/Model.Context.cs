﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatabaseLib
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VictoriousEntities : DbContext
    {
        public VictoriousEntities()
            : base("name=VictoriousEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__RefactorLog> C__RefactorLog { get; set; }
        public virtual DbSet<AccountModel> AccountModels { get; set; }
        public virtual DbSet<BracketModel> BracketModels { get; set; }
        public virtual DbSet<BracketTypeModel> BracketTypeModels { get; set; }
        public virtual DbSet<MatchModel> MatchModels { get; set; }
        public virtual DbSet<GameTypeModel> GameTypeModels { get; set; }
        public virtual DbSet<TournamentUserModel> TournamentUserModels { get; set; }
        public virtual DbSet<TournamentUsersBracketModel> TournamentUsersBracketModels { get; set; }
        public virtual DbSet<TournamentModel> TournamentModels { get; set; }
        public virtual DbSet<GameModel> GameModels { get; set; }
        public virtual DbSet<TournamentInvite> TournamentInvites { get; set; }
        public virtual DbSet<PlatformModel> PlatformModels { get; set; }
    }
}
