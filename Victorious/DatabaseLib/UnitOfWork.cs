using System;

namespace DatabaseLib
{
    public class UnitOfWork : IUnitOfWork
    {
        private VictoriousEntities context = new VictoriousEntities();
        IRepository<AccountInviteModel> accountInviteRepo;
        IRepository<AccountModel> accountRepo;
        IRepository<BracketModel> bracketRepo;
        IRepository<BracketTypeModel> bracketTypeRepo;
        IRepository<GameModel> gameRepo;
        IRepository<GameTypeModel> gameTypeRepo;
        IRepository<MatchModel> matchRepo;
        IRepository<PlatformModel> platformRepo;
        IRepository<TournamentModel> tournamentRepo;
        IRepository<TournamentUserModel> tournamentUserRepo;
        IRepository<TournamentInviteModel> tournamentInviteRepo;
        IRepository<TournamentUsersBracketModel> tournamentUsersBracketRepo;
        IRepository<TournamentTeamModel> tournamentTeamRepo;
        IRepository<TournamentTeamMemberModel> tournamentTeamMemberRepo;
        IRepository<TournamentTeamBracketModel> tournamentTeamBracketRepo;
        IRepository<SiteTeamModel> siteTeamRepo;
        IRepository<SiteTeamMemberModel> siteTeamMemberRepo;
        IRepository<MailingListModel> mailingListRepo;

        Exception exception;

        public UnitOfWork(string name = null, VictoriousEntities context = null)
        {
            if (name != null)
            {
                context = new VictoriousEntities(name);
                context.Configuration.AutoDetectChangesEnabled = true;
            }
            if (context != null)
            {
                this.context = context; 
            }
            //this.context.Configuration.AutoDetectChangesEnabled = false;
        }

        public bool Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                this.SetException(ex);
                return false;
            }
            return true;
        }

        public void Refresh()
        {
            context.Dispose();
            context = new VictoriousEntities();
        }

        public void SetException(Exception _exception)
        {
            exception = _exception;
        }

        public Exception GetException()
        {
            return exception;
        }
      
        public IRepository<AccountModel> AccountRepo
        {
            get
            {
                if (this.accountRepo == null)
                {
                    this.accountRepo = new Repository<AccountModel>(context);
                }
                return accountRepo;
            }
        }

        public IRepository<AccountInviteModel> AccountInviteRepo
        {
            get
            {
                if (this.accountInviteRepo == null)
                {
                    this.accountInviteRepo = new Repository<AccountInviteModel>(context);
                }
                return accountInviteRepo;
            }
        }

        public IRepository<BracketModel> BracketRepo
        {
            get
            {
                if (this.bracketRepo == null)
                {
                    this.bracketRepo = new Repository<BracketModel>(context);
                }
                return bracketRepo;
            }
        }

        public IRepository<BracketTypeModel> BracketTypeRepo
        {
            get
            {
                if (this.bracketTypeRepo == null)
                {
                    this.bracketTypeRepo = new Repository<BracketTypeModel>(context);
                }
                return bracketTypeRepo;
            }
        }

        public IRepository<GameModel> GameRepo
        {
            get
            {
                if (this.gameRepo == null)
                {
                    this.gameRepo = new Repository<GameModel>(context);
                }
                return gameRepo;
            }
        }

        public IRepository<GameTypeModel> GameTypeRepo
        {
            get
            {
                if (this.gameTypeRepo == null)
                {
                    this.gameTypeRepo = new Repository<GameTypeModel>(context);
                }
                return gameTypeRepo;
            }
        }

        public IRepository<MatchModel> MatchRepo
        {
            get
            {
                if (this.matchRepo == null)
                {
                    this.matchRepo = new Repository<MatchModel>(context);
                }
                return matchRepo;
            }
        }

        public IRepository<PlatformModel> PlatformRepo
        {
            get
            {
                if (this.platformRepo == null)
                {
                    this.platformRepo = new Repository<PlatformModel>(context);
                }
                return platformRepo;
            }
        }

        public IRepository<TournamentUserModel> TournamentUserRepo
        {
            get
            {
                if (this.tournamentUserRepo == null)
                {
                    this.tournamentUserRepo = new Repository<TournamentUserModel>(context);
                }
                return tournamentUserRepo;
            }
        }

        public IRepository<TournamentInviteModel> TournamentInviteRepo
        {
            get
            {
                if (this.tournamentInviteRepo == null)
                {
                    this.tournamentInviteRepo = new Repository<TournamentInviteModel>(context);
                }
                return tournamentInviteRepo;
            }
        }

        public IRepository<TournamentUsersBracketModel> TournamentUsersBracketRepo
        {
            get
            {
                if (this.tournamentInviteRepo == null)
                {
                    this.tournamentUsersBracketRepo = new Repository<TournamentUsersBracketModel>(context);
                }
                return tournamentUsersBracketRepo;
            }
        }

        public IRepository<TournamentModel> TournamentRepo
        {
            get
            {
                if (this.tournamentRepo == null)
                {
                    this.tournamentRepo = new Repository<TournamentModel>(context);
                }
                return tournamentRepo;
            }
        }

        public IRepository<TournamentTeamModel> TournamentTeamRepo
        {
            get
            {
                if (this.tournamentTeamRepo == null)
                {
                    this.tournamentTeamRepo = new Repository<TournamentTeamModel>(context);
                }
                return tournamentTeamRepo;
            }
        }

        public IRepository<TournamentTeamMemberModel> TournamentTeamMemberRepo
        {
            get
            {
                if (this.tournamentTeamMemberRepo == null)
                {
                    this.tournamentTeamMemberRepo = new Repository<TournamentTeamMemberModel>(context);
                }
                return tournamentTeamMemberRepo;
            }
        }

        public IRepository<TournamentTeamBracketModel> TournamentTeamBracketRepo
        {
            get
            {
                if (this.tournamentTeamBracketRepo == null)
                {
                    this.tournamentTeamBracketRepo = new Repository<TournamentTeamBracketModel>(context);
                }
                return tournamentTeamBracketRepo;
            }
        }

        public IRepository<SiteTeamModel> SiteTeamRepo
        {
            get
            {
                if (this.siteTeamRepo == null)
                {
                    this.siteTeamRepo = new Repository<SiteTeamModel>(context);
                }
                return siteTeamRepo;
            }
        }

        public IRepository<SiteTeamMemberModel> SiteTeamMemberRepo
        {
            get
            {
                if (this.siteTeamMemberRepo == null)
                {
                    this.siteTeamMemberRepo = new Repository<SiteTeamMemberModel>(context);
                }
                return siteTeamMemberRepo;
            }
        }

        public IRepository<MailingListModel> MailingListRepo
        {
            get
            {
                if (this.mailingListRepo == null)
                {
                    this.mailingListRepo = new Repository<MailingListModel>(context);
                }
                return mailingListRepo;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                    context.Dispose();
                    //context = new VictoriousEntities();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
