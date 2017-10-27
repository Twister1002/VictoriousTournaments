using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class TournamentRepository : ITournamentRepository, IDisposable
    {
        private VictoriousEntities context;
        public Exception exception;

        public TournamentRepository()
        {
            //context = new VictoriousEntities();
        }

        public TournamentRepository(VictoriousEntities context)
        {
            this.context = context;
        }

        public DbError AddTournament(TournamentModel tournament)
        {
            //TournamentModel _tournament = new TournamentModel();
            try
            {
                TournamentInviteModel invite = new TournamentInviteModel()
                {
                    DateCreated = DateTime.Now,
                    IsExpired = false,
                    NumberOfUses = 0,
                    TournamentID = tournament.TournamentID,
                    TournamentInviteCode = tournament.InviteCode
                };
                
                
                //_tournament = tournament;
                //_tournament.CreatedOn = DateTime.Now;
                //_tournament.LastEditedOn = DateTime.Now;
                ////context.SaveChanges();
                //context.TournamentModels.Add(_tournament);
                context.TournamentModels.Add(tournament);
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                return DbError.FAILED_TO_ADD;
            }
            return DbError.SUCCESS;
        }

        public DbError DeleteTournament(int tournamentId)
        {
            try
            {
                TournamentModel _tournament = context.TournamentModels.Find(tournamentId);

                //foreach (var bracket in _tournament.Brackets.ToList())
                //{
                //    DeleteBracket(bracket.BracketID);
                //}
                //foreach (var user in _tournament.TournamentUsers.ToList())
                //{
                //    DeleteTournamentUser(user.TournamentUserID);
                //}
                //context.TournamentModels.Remove(_tournament);
                //context.SaveChanges();
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                return DbError.FAILED_TO_DELETE;
            }
            return DbError.SUCCESS;
        }

        public IList<TournamentModel> GetAllTournaments()
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            try
            {
                tournaments = context.TournamentModels.ToList();
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                tournaments.Clear();
            }
            return tournaments;
        }

        /// <summary>
        /// Gathers all the games and counts how many tournaments are available for that game
        /// </summary>
        /// <returns>A list of all games and how many tournaments are available</returns>
        public List<KeyValuePair<GameTypeModel, int>> GetAllTournamentsByGame()
        {
            List<KeyValuePair<GameTypeModel, int>> available = new List<KeyValuePair<GameTypeModel, int>>();

            // Get all games
            List<GameTypeModel> games = context.GameTypeModels.ToList();

            foreach (GameTypeModel game in games)
            {
                int gameTournaments = context.TournamentModels.GroupBy(x => x.GameTypeID == game.GameTypeID).Count();
                available.Add(new KeyValuePair<GameTypeModel, int>(game, gameTournaments));
            }

            return available;

        }

        public TournamentModel GetTournament(int tournamentId)
        {
            TournamentModel tournament = new TournamentModel();
            try
            {
                tournament = context.TournamentModels.Find(tournamentId);
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                tournament = null;
            }
            return tournament;
        }

       
        public DbError UpdateTournament(TournamentModel tournament)
        {
            using (var db = new VictoriousEntities())
            {
                try
                {
                    TournamentModel _tournament = db.TournamentModels.Find(tournament.TournamentID);
                    db.Entry(_tournament).CurrentValues.SetValues(tournament);

                    //foreach (var bracket in _tournament.Brackets)
                    //{
                    //    foreach (var match in bracket.Matches)
                    //    {
                    //        match.Challenger = db.TournamentUserModels.Find(match.ChallengerID);
                    //        match.Defender = db.TournamentUserModels.Find(match.DefenderID);
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    exception = ex;
                    WriteException(ex);
                    return DbError.FAILED_TO_UPDATE;
                }
                return DbError.SUCCESS;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TournamentRepository() {
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

        private void WriteException(Exception ex, [CallerMemberName] string funcName = null)
        {
            Console.WriteLine("Exception " + ex + " in " + funcName);
        }

        public IList<TournamentUserModel> GetAllUsersInTournament(int tournamentId)
        {
            List<TournamentUserModel> list = new List<TournamentUserModel>();
            try
            {
                list = context.TournamentModels.Find(tournamentId).TournamentUsers.ToList();
            }
            catch (Exception ex)
            {
                exception = ex;
                WriteException(ex);
                list.Clear();
            }
            return list;
        }
    }
}
