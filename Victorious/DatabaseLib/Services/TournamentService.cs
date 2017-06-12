using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Sql;


namespace DatabaseLib.Services
{
    public class TournamentService
    {
        IUnitOfWork unitOfWork;

        public TournamentService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

        }

        #region Tournaments

        public void AddTournament(TournamentModel tournament)
        {
            //TournamentInviteModel invite = new TournamentInviteModel()
            //{
            //    TournamentID = tournamentToAdd.TournamentID,
            //    TournamentInviteCode = tournamentToAdd.InviteCode,
            //    DateCreated = DateTime.Today,
            //    IsExpired = false,
            //    DateExpires = tournamentToAdd.RegistrationEndDate
            //};
            //AddTournamentInvite(invite, true);
            unitOfWork.TournamentRepo.Add(tournament);
            //invite.TournamentID = tournamentToAdd.TournamentID;
            //UpdateTournamentInvite(invite);

        }

        public TournamentModel GetTournament(int tournamentId)
        {
            try
            {
                return unitOfWork.TournamentRepo.Get(tournamentId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TournamentModel> GetAllTournaments()
        {
            try
            {
                return unitOfWork.TournamentRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return new List<TournamentModel>();
            }
        }

        public void UpdateTournament(TournamentModel tournament)
        {
            unitOfWork.TournamentRepo.Update(tournament);
        }

        public void DeleteTournament(int tournamentId)
        {
            unitOfWork.TournamentRepo.Delete(tournamentId);
        }

        public List<TournamentModel> FindTournaments(Dictionary<string, string> searchParams, int returnCount = 25)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();

            using (VictoriousEntities context = new VictoriousEntities())
            {
                context.TournamentModels.Include(x => x.Brackets)
                     .Include(x => x.GameType)
                     .Include(x => x.Platform)
                     .Include(x => x.TournamentUsers)
                     .Include(x => x.TournamentInvites)
                     .Load();
                try
                {
                    List<SqlParameter> sqlparams = new List<SqlParameter>();
                    string query = string.Empty;
                    query = "SELECT TOP(" + returnCount + ")* FROM Tournaments WHERE PublicViewing = 1 ";
                    foreach (KeyValuePair<String, String> data in searchParams)
                    {
                        if (query != String.Empty) query += " AND ";
                        string val = data.Value;
                      
                        if (data.Key == "TournamentStartDate" || data.Key == "RegistrationStartDate")
                        {
                            val = DateTime.Parse(val).ToShortDateString();
                            query += data.Key + " >= @" + data.Key;
                            sqlparams.Add(new SqlParameter("@" + data.Key, val));
                        }
                        else if (data.Key == "TournamentEndDate" || data.Key == "RegistrationEndDate")
                        {
                            val = DateTime.Parse(val).ToShortDateString();
                            query += data.Key +  " <= @" + data.Key;
                            sqlparams.Add(new SqlParameter("@" + data.Key, val));
                        }
                        else if (data.Key == "CreatedOn")
                        {
                            val = DateTime.Parse(val).ToShortDateString();
                            query += "datediff(day," + data.Key + ", " + "@" + data.Key + ") = 0 ";

                            sqlparams.Add(new SqlParameter("@" + data.Key, val));
                        }
                        //if (data.Key == "TournamentStartDate" || data.Key == "TournamentEndDate" || data.Key == "RegistrationStartDate" ||
                        //    data.Key == "RegistrationEndDate" || data.Key == "CreatedOn")
                        //{
                        //    val = DateTime.Parse(val).ToShortDateString();
                        //    query += "datediff(day," + data.Key + ", " + "@" + data.Key + ") = 0 ";

                        //    sqlparams.Add(new SqlParameter("@" + data.Key, val));

                        //}
                        else if (data.Key == "Title")
                        {
                            query += data.Key + " LIKE @" + data.Key;
                            sqlparams.Add(new SqlParameter("@" + data.Key, "%" + val + "%"));
                        }
                        else
                        {
                            query += data.Key + " = @" + data.Key;
                            sqlparams.Add(new SqlParameter("@" + data.Key, val));

                        }
                    }
                    query += " ORDER BY TournamentStartDate ASC";

                    tournaments = context.TournamentModels.SqlQuery(query, sqlparams.ToArray()).ToList();
                    query = string.Empty;

                    //if (tournaments.Count == 0)
                    //{
                    //    query = "SELECT TOP(25)* FROM Tournaments WHERE IsPublic = 1 ORDER BY TournamentStartDate ASC";
                    //    tournaments = context.TournamentModels.SqlQuery(query).ToList();
                    //}


                }
                catch (Exception)
                {
                    //interfaceException = ex;
                    //WriteException(ex);
                    tournaments.Clear();
                }
            }
            return tournaments;

        }


        #endregion


        #region TournamentUsers

        public void AddTournamentUser(TournamentUserModel tournamentUser)
        {
            unitOfWork.TournamentUserRepo.Add(tournamentUser);
        }

        public TournamentUserModel GetTournamentUser(int tournamentUserId)
        {
            try
            {
                return unitOfWork.TournamentUserRepo.Get(tournamentUserId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TournamentUserModel> GetAllTournamentUsers()
        {
            try
            {
                return unitOfWork.TournamentUserRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return new List<TournamentUserModel>();
            }
        }

        public List<TournamentUserModel> GetAllUsersInTournament(int tournamentId)
        {
            try
            {
                return unitOfWork.TournamentRepo.Get(tournamentId).TournamentUsers.ToList();
            }
            catch (Exception ex)
            {
                return new List<TournamentUserModel>();
            }
        }

        public void UpdateTournamentUser(TournamentUserModel tournamentUser)
        {
            unitOfWork.TournamentUserRepo.Update(tournamentUser);
        }

        public void DeleteTournamentUser(int tournamentUserId)
        {
            unitOfWork.TournamentUserRepo.Delete(tournamentUserId);
        }

        public void CheckUserIn(int tournamentUserId)
        {
            TournamentUserModel tournamentUser = unitOfWork.TournamentUserRepo.Get(tournamentUserId);
            tournamentUser.CheckInTime = DateTime.Now;
            tournamentUser.IsCheckedIn = true;

        }

        #endregion


        #region TournamentInvites

        [Obsolete("Use AddTournamentInvite(TournamentInviteModel tournamentInviteToAdd)")]
        public void AddTournamentInvite(TournamentInviteModel tournamentInviteToAdd, bool save = false)
        {
            unitOfWork.TournamentInviteRepo.Add(tournamentInviteToAdd);
            if (save)
                unitOfWork.Save();
        }

        public void AddTournamentInvite(TournamentInviteModel tournamentInvite)
        {
            unitOfWork.TournamentInviteRepo.Add(tournamentInvite);
        }

        public TournamentInviteModel GetTournamentInvite(string tournamentInviteCode)
        {
            try
            {
                return unitOfWork.TournamentInviteRepo.GetAll().Single(x => x.TournamentInviteCode == tournamentInviteCode);
            }
            catch (Exception ex)
            {
                return new TournamentInviteModel();
            }
        }

        public List<TournamentInviteModel> GetAllTournamentInvites()
        {
            try
            {
                return unitOfWork.TournamentInviteRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return new List<TournamentInviteModel>();
            }
        }

        public void UpdateTournamentInvite(TournamentInviteModel tournamentInvite)
        {
            unitOfWork.TournamentInviteRepo.Update(tournamentInvite);
        }

        public void DeleteTournamentInvite(string tournamentInviteCode)
        {
            TournamentInviteModel tournamentInviteToDelete = GetTournamentInvite(tournamentInviteCode);
            unitOfWork.TournamentInviteRepo.DeleteEntity(tournamentInviteToDelete);
        }

        #endregion


        #region Brackets

        public void AddBracket(BracketModel bracket)
        {
            unitOfWork.BracketRepo.Add(bracket);
        }

        public BracketModel GetBracket(int bracketId)
        {
            try
            {
                return unitOfWork.BracketRepo.Get(bracketId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<BracketModel> GetAllBrackets()
        {
            try
            {
                return unitOfWork.BracketRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return new List<BracketModel>();
            }
        }

        public List<BracketModel> GetAllBracketsInTournament(int tournamnetId)
        {
            try
            {
                TournamentModel tournament = unitOfWork.TournamentRepo.Get(tournamnetId);
                return tournament.Brackets.ToList();
            }
            catch (Exception ex)
            {
                return new List<BracketModel>();
            }
        }

        public void UpdateBracket(BracketModel bracket)
        {
            unitOfWork.BracketRepo.Update(bracket);
        }

        public void DeleteBracket(int bracketId)
        {
            unitOfWork.BracketRepo.Delete(bracketId);
        }

        #endregion


        #region Matches

        public void AddMatch(MatchModel match)
        {
            unitOfWork.MatchRepo.Add(match);
        }

        public MatchModel GetMatch(int matchId)
        {
            try
            {
                MatchModel match = unitOfWork.MatchRepo.Get(matchId);
                match.Challenger = unitOfWork.TournamentUserRepo.Get(match.ChallengerID);
                match.Defender = unitOfWork.TournamentUserRepo.Get(match.DefenderID);
                return match;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<MatchModel> GetAllMatches()
        {
            try
            {
                var matches = unitOfWork.MatchRepo.GetAll().ToList();
                foreach (var match in matches)
                {
                    match.Challenger = unitOfWork.TournamentUserRepo.Get(match.ChallengerID);
                    match.Defender = unitOfWork.TournamentUserRepo.Get(match.DefenderID);
                }
                return unitOfWork.MatchRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return new List<MatchModel>();
            }
        }

        public void UpdateMatch(MatchModel match)
        {
            unitOfWork.MatchRepo.UpdateDetachCheck(match);
        }

        public void DeleteMatch(int matchId)
        {
            unitOfWork.MatchRepo.Delete(matchId);
        }

        public List<MatchModel> GetAllMatchesInBracket(int bracketId)
        {
            try
            {
                return unitOfWork.MatchRepo.GetAll().Where(x => x.BracketID == bracketId).ToList();
            }
            catch (Exception ex)
            {
                return new List<MatchModel>();
            }
        }

        #endregion


        #region Games

        public void AddGame(GameModel game)
        {
            unitOfWork.GameRepo.Add(game);
        }

        public GameModel GetGame(int gameId)
        {
            try
            {
                return unitOfWork.GameRepo.Get(gameId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<GameModel> GetAllGames()
        {
            try
            {
                return unitOfWork.GameRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return new List<GameModel>();
            }
        }

        public void UpdateGame(GameModel game)
        {
            unitOfWork.GameRepo.Update(game);
        }

        public void DeleteGame(int gameId)
        {
            unitOfWork.GameRepo.Delete(gameId);
        }

        #endregion


        #region TournamentUsersBrackets

        public void AddTournamentUsersBracket(TournamentUsersBracketModel tournamentUserBracket)
        {
            unitOfWork.TournamentUsersBracketRepo.Add(tournamentUserBracket);
        }

        public TournamentUsersBracketModel GetTournamentUsersBracket(int tournamentUserId, int bracketId)
        {
            try
            {
                return unitOfWork.TournamentUsersBracketRepo.GetSingle(x => x.TournamentUserID == tournamentUserId && x.BracketID == bracketId);
                //return unitOfWork.TournamentUsersBracketRepo.Get(tournamentUsersBracketId);
            }
            catch (Exception ex)
            {
                return new TournamentUsersBracketModel();
            }
        }

        public void UpdateTournamentUsersBracket(TournamentUsersBracketModel tournamentUserBracket)
        {

            unitOfWork.TournamentUsersBracketRepo.Update(tournamentUserBracket);

        }

        public void DeleteTournamentUsersBracket(int tournamentUserId, int bracketId)
        {
            TournamentUsersBracketModel t = GetTournamentUsersBracket(tournamentUserId, bracketId);
            unitOfWork.TournamentUsersBracketRepo.DeleteEntity(t);
        }


        #endregion

    }
}
