using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void AddTournament(TournamentModel tournamentToAdd)
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
            unitOfWork.TournamentRepo.Add(tournamentToAdd);
            //invite.TournamentID = tournamentToAdd.TournamentID;
            //UpdateTournamentInvite(invite);

        }

        public TournamentModel GetTournament(int tournamentId)
        {
            return unitOfWork.TournamentRepo.Get(tournamentId);
        }

        public List<TournamentModel> GetAllTournaments()
        {
            return unitOfWork.TournamentRepo.GetAll().ToList();
        }

        public void UpdateTournament(TournamentModel tournamentToUpdate)
        {
            unitOfWork.TournamentRepo.Update(tournamentToUpdate);
        }

        public void DeleteTournament(int tournamentId)
        {
            unitOfWork.TournamentRepo.Delete(tournamentId);
        }

        public List<TournamentModel> FindTournaments(Dictionary<string, string> searchParams, int returnCount = 25)
        {


            using (VictoriousEntities context = new VictoriousEntities())
            {
                List<TournamentModel> tournaments = new List<TournamentModel>();
                try
                {
                    List<SqlParameter> sqlparams = new List<SqlParameter>();
                    string query = string.Empty;
                    query = "SELECT TOP(" + returnCount + ")* FROM Tournaments WHERE PublicViewing = 1 ";
                    foreach (KeyValuePair<String, String> data in searchParams)
                    {
                        if (query != String.Empty) query += " AND ";
                        string val = data.Value;
                        if (data.Key == "TournamentStartDate" || data.Key == "TournamentEndDate" || data.Key == "RegistrationStartDate" ||
                            data.Key == "RegistrationEndDate" || data.Key == "CreatedOn")
                        {
                            val = DateTime.Parse(val).ToShortDateString();
                            query += "datediff(day," + data.Key + ", " + "@" + data.Key + ") = 0 ";

                            sqlparams.Add(new SqlParameter("@" + data.Key, val));

                        }
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
                catch (Exception ex)
                {
                    //interfaceException = ex;
                    //WriteException(ex);
                    tournaments.Clear();
                }
                return tournaments;
            }

        }


        #endregion


        #region TournamentUsers

        public void AddTournamentUser(TournamentUserModel tournamentUser)
        {
            unitOfWork.TournamentUserRepo.Add(tournamentUser);
        }

        public TournamentUserModel GetTournamentUser(int tournamentUserId)
        {
            return unitOfWork.TournamentUserRepo.Get(tournamentUserId);
        }

        public List<TournamentUserModel> GetAllTournamentUsers()
        {
            return unitOfWork.TournamentUserRepo.GetAll().ToList();
        }

        public List<TournamentUserModel> GetAllUsersInTournament(int tournamentId)
        {
            return unitOfWork.TournamentRepo.Get(tournamentId).TournamentUsers.ToList();
        }

        public void UpdateTournamentUser(TournamentUserModel tournamentUserToUpdate)
        {
            unitOfWork.TournamentUserRepo.Update(tournamentUserToUpdate);
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

        public void AddTournamentInvite(TournamentInviteModel tournamentInviteToAdd, bool save = false)
        {
            unitOfWork.TournamentInviteRepo.Add(tournamentInviteToAdd);
            if (save)
                unitOfWork.Save();
        }

        public TournamentInviteModel GetTournamentInvite(string tournamentInviteCode)
        {
            return unitOfWork.TournamentInviteRepo.GetAll().Single(x => x.TournamentInviteCode == tournamentInviteCode);
        }

        public List<TournamentInviteModel> GetAllTournamentInvites()
        {
            return unitOfWork.TournamentInviteRepo.GetAll().ToList();
        }

        public void UpdateTournamentInvite(TournamentInviteModel tournamentInviteToUpdate)
        {
            unitOfWork.TournamentInviteRepo.Update(tournamentInviteToUpdate);
        }

        public void DeleteTournamentInvite(string tournamentInviteCode)
        {
            TournamentInviteModel tournamentInviteToDelete = GetTournamentInvite(tournamentInviteCode);
            unitOfWork.TournamentInviteRepo.DeleteEntity(tournamentInviteToDelete);
        }

        #endregion


        #region Brackets

        public void AddBracket(BracketModel bracketToAdd)
        {
            unitOfWork.BracketRepo.Add(bracketToAdd);
        }

        public BracketModel GetBracket(int bracketId)
        {
            return unitOfWork.BracketRepo.Get(bracketId);
        }

        public List<BracketModel> GetAllBrackets()
        {
            return unitOfWork.BracketRepo.GetAll().ToList();
        }

        public List<BracketModel> GetAllBracketsInTournament(int tournamnetId)
        {
            TournamentModel tournament = unitOfWork.TournamentRepo.Get(tournamnetId);
            return tournament.Brackets.ToList();
        }

        public void UpdateBracket(BracketModel bracketToUpdate)
        {
            unitOfWork.BracketRepo.Update(bracketToUpdate);
        }

        public void DeleteBracket(int bracketId)
        {
            unitOfWork.BracketRepo.Delete(bracketId);
        }

        #endregion


        #region Matches

        public void AddMatch(MatchModel matchToAdd)
        {
            unitOfWork.MatchRepo.Add(matchToAdd);
        }

        public MatchModel GetMatch(int matchId)
        {
            return unitOfWork.MatchRepo.Get(matchId);
        }

        public List<MatchModel> GetAllMatches()
        {
            return unitOfWork.MatchRepo.GetAll().ToList();
        }

        public void UpdateMatch(MatchModel matchToUpdate)
        {
            unitOfWork.MatchRepo.Update(matchToUpdate);
        }

        public void DeleteMatch(int matchId)
        {
            unitOfWork.MatchRepo.Delete(matchId);
        }

        public List<MatchModel> GetAllMatchesInBracket(int bracketId)
        {
            return unitOfWork.MatchRepo.GetAll().Where(x => x.BracketID == bracketId).ToList();
        }

        #endregion


        #region Games

        public void AddGame(GameModel gameToAdd)
        {
            unitOfWork.GameRepo.Add(gameToAdd);
        }

        public GameModel GetGame(int gameId)
        {
            return unitOfWork.GameRepo.Get(gameId);
        }

        public List<GameModel> GetAllGames()
        {
            return unitOfWork.GameRepo.GetAll().ToList();
        }

        public void UpdateGame(GameModel gameToUpdate)
        {
            unitOfWork.GameRepo.Update(gameToUpdate);
        }

        public void DeleteGame(int gameId)
        {
            unitOfWork.GameRepo.Delete(gameId);
        }

        #endregion


        #region TournamentUsersBrackets

        public void AddTournamentUsersBracket(TournamentUsersBracketModel tournamentUserBracketToAdd)
        {
            unitOfWork.TournamentUsersBracketRepo.Add(tournamentUserBracketToAdd);
        }

        public TournamentUsersBracketModel GetTournamentUsersBracket(int tournamentUserId, int bracketId)
        {
            return unitOfWork.TournamentUsersBracketRepo.GetSingle(x => x.TournamentUserID == tournamentUserId && x.BracketID == bracketId);
            //return unitOfWork.TournamentUsersBracketRepo.Get(tournamentUsersBracketId);
        }

        public void UpdateTournamentUsersBracket(TournamentUsersBracketModel tournamentUserBracketToUpdate)
        {
            unitOfWork.TournamentUsersBracketRepo.Update(tournamentUserBracketToUpdate);
        }

        public void DeleteTournamentUsersBracket(int tournamentUserId, int bracketId)
        {
            TournamentUsersBracketModel t = GetTournamentUsersBracket(tournamentUserId, bracketId);
            unitOfWork.TournamentUsersBracketRepo.DeleteEntity(t);
        }


        #endregion

    }
}
