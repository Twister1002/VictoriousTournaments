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

        /// <summary>
        /// Adds a single Tournament to the database.
        /// </summary>
        /// <param name="tournament"> The Tournament to be added. </param>
        public void AddTournament(TournamentModel tournament)
        {
            unitOfWork.TournamentRepo.Add(tournament);
        }

        /// <summary>
        /// Retreives a single TournamentModel from the database.
        /// </summary>
        /// <param name="tournamentId"> The Id of the Tournament to be retreived. </param>
        /// <returns> Returns a single TournamentModel or null if an exception is thrown. </returns>
        /// <remarks> If a matching Id is not found, a ObjectNotFoundException will be thrown. </remarks>
        public TournamentModel GetTournament(int tournamentId)
        {
            try
            {
                return unitOfWork.TournamentRepo.Get(tournamentId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives all Tournaments from the database.
        /// </summary>
        /// <returns> Returns a List of TournamentsModels. </returns>
        public List<TournamentModel> GetAllTournaments()
        {
            try
            {
                return unitOfWork.TournamentRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<TournamentModel>();
            }
        }

        /// <summary>
        /// Updates a single Tournament.
        /// </summary>
        /// <param name="tournament"> The Tournament that is being updated. </param>
        public void UpdateTournament(TournamentModel tournament)
        {
            unitOfWork.TournamentRepo.Update(tournament);
        }

        /// <summary>
        /// Deletes a single Tournament from the database. 
        /// </summary>
        /// <param name="tournamentId"> The Id of the Tournament that is to be deleted. </param>
        public void DeleteTournament(int tournamentId)
        {
            unitOfWork.TournamentRepo.Delete(tournamentId);
        }

        /// <summary>
        /// Search function for tournaments. 
        /// </summary>
        /// <param name="searchParams"> Dictionary used to specify search criteria. </param>
        /// <param name="returnCount"> Number of items to return. </param>
        /// <returns> Returns a List of Tournaments. </returns>
        /// <remarks>
        /// For the <paramref name="searchParams"/>, the key is the column name and the value is the data being used to search against that column.
        /// </remarks>
        public List<TournamentModel> FindTournaments(Dictionary<string, string> searchParams, int returnCount = 25)
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
                    else
                    {
                        int intVal = 0;
                        if (int.TryParse(val, out intVal))
                        {
                            query += data.Key + " = @" + data.Key;
                            sqlparams.Add(new SqlParameter("@" + data.Key, intVal));
                        }
                        else
                        {
                            query += data.Key + " LIKE @" + data.Key;
                            sqlparams.Add(new SqlParameter("@" + data.Key, "%" + val + "%"));
                        }
                    }
                }
                query += " ORDER BY TournamentStartDate ASC";

                tournaments = unitOfWork.TournamentRepo.Get(query, sqlparams);
            }
            catch (Exception ex)
            {
                //interfaceException = ex;
                //WriteException(ex);
                unitOfWork.SetException(ex);
                tournaments.Clear();
            }

            return tournaments;
        }


        #endregion


        #region TournamentUsers

        /// <summary>
        /// Adds a single TournamentUser to the databse.
        /// </summary>
        /// <param name="tournamentUser"> The TournamentUser to be added. </param>
        public void AddTournamentUser(TournamentUserModel tournamentUser)
        {
            unitOfWork.TournamentUserRepo.Add(tournamentUser);
        }

        /// <summary>
        /// Retreives a single TournamentUser from the database.
        /// </summary>
        /// <param name="tournamentUserId"> Id of the TournamentUser to be retreived. </param>
        /// <returns> Returns a single TournamentUserModel or null if an exception is thrown. </returns>
        /// <remarks> If a matching Id is not found, a ObjectNotFoundException will be thrown. </remarks>
        public TournamentUserModel GetTournamentUser(int tournamentUserId)
        {
            try
            {
                return unitOfWork.TournamentUserRepo.Get(tournamentUserId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retreieve all TournamentUsers from the database.
        /// </summary>
        /// <returns> Returns a List of TournamentUserModels. </returns>
        public List<TournamentUserModel> GetAllTournamentUsers()
        {
            try
            {
                return unitOfWork.TournamentUserRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<TournamentUserModel>();
            }
        }

        /// <summary>
        /// Retreives all TournamentUser in a specific tournament.
        /// </summary>
        /// <param name="tournamentId"> Id of the tournament to retreive TournamentUserModels from. </param>
        /// <returns> List of Tournaments </returns>
        public List<TournamentUserModel> GetAllUsersInTournament(int tournamentId)
        {
            try
            {
                return unitOfWork.TournamentRepo.Get(tournamentId).TournamentUsers.ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<TournamentUserModel>();
            }
        }

        /// <summary>
        /// Updates a single TournamentUser.
        /// </summary>
        /// <param name="tournamentUser"> The TournamentUser to be updated. <param>
        public void UpdateTournamentUser(TournamentUserModel tournamentUser)
        {
            unitOfWork.TournamentUserRepo.Update(tournamentUser);
        }

        /// <summary>
        /// Deletes a single TournamentUser from the database.
        /// </summary>
        /// <param name="tournamentUserId"> The Id of the TournamentUser to delete. </param>
        public void DeleteTournamentUser(int tournamentUserId)
        {
            unitOfWork.TournamentUserRepo.Delete(tournamentUserId);
        }

        /// <summary>
        /// Marks a TournamentUser as checked-in for a tournament.
        /// </summary>
        /// <param name="tournamentUserId"> Id of the TournamentUser that is being checked-in. </param>
        public void CheckUserIn(int tournamentUserId)
        {
            TournamentUserModel tournamentUser = unitOfWork.TournamentUserRepo.Get(tournamentUserId);
            tournamentUser.CheckInTime = DateTime.Now;
            tournamentUser.IsCheckedIn = true;

        }

        #endregion


        #region TournamentInvites

        /// <summary>
        /// Adds a single TournamentInvite to the database. 
        /// </summary>
        /// <param name="tournamentInvite"> The model of the TournamentInvite that is being added. </param>
        public void AddTournamentInvite(TournamentInviteModel tournamentInvite)
        {
            unitOfWork.TournamentInviteRepo.Add(tournamentInvite);
        }

        /// <summary>
        /// Retreives a single TournamentInvite from the database.
        /// </summary>
        /// <param name="tournamentInviteCode"> The invite code of the TournamentInvite being retreived. </param>
        /// <returns> Returns a single TournamentInviteModel or null if an exception is thrown. </returns>
        /// <remarks> If a matching Id is not found, and ObjectNotFoundException will be thrown. </remarks>
        public TournamentInviteModel GetTournamentInvite(string tournamentInviteCode)
        {
            try
            {
                return unitOfWork.TournamentInviteRepo.GetAll().Single(x => x.TournamentInviteCode == tournamentInviteCode);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new TournamentInviteModel();
            }
        }

        /// <summary>
        /// Retreives all TournamentInvites from the database.
        /// </summary>
        /// <returns> Returns a List of TournamentInvitesModels. </returns>
        public List<TournamentInviteModel> GetAllTournamentInvites()
        {
            try
            {
                return unitOfWork.TournamentInviteRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<TournamentInviteModel>();
            }
        }

        /// <summary>
        /// Updates a single TournamentInvite.
        /// </summary>
        /// <param name="tournamentInvite"> The model of the TournamentInvite to be updated. </param>
        public void UpdateTournamentInvite(TournamentInviteModel tournamentInvite)
        {
            unitOfWork.TournamentInviteRepo.Update(tournamentInvite);
        }

        /// <summary>
        /// Deletes a single TournamentInvite from the database.
        /// </summary>
        /// <param name="tournamentInviteCode"> The invite code of the TournamentInvite to delete. </param>
        public void DeleteTournamentInvite(string tournamentInviteCode)
        {
            TournamentInviteModel tournamentInviteToDelete = GetTournamentInvite(tournamentInviteCode);
            unitOfWork.TournamentInviteRepo.DeleteEntity(tournamentInviteToDelete);
        }

        #endregion


        #region Brackets

        /// <summary>
        /// Adds a single Bracket to the database.
        /// </summary>
        /// <param name="bracket"> The model of the bracket to be added to the database. </param>
        public void AddBracket(BracketModel bracket)
        {
            unitOfWork.BracketRepo.Add(bracket);
        }

        /// <summary>
        /// Retreives a single Bracket from the database.
        /// </summary>
        /// <param name="bracketId"> The Id of the bracket to be retreived. </param>
        /// <returns> Returns a BracketModel, or null if an exception is thrown. </returns>
        /// <remarks> If a matching Id could not be found, a ObjectNotFoundException will be thrown. </remarks>
        public BracketModel GetBracket(int bracketId)
        {
            try
            {
                return unitOfWork.BracketRepo.Get(bracketId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives all Brackets from the database.
        /// </summary>
        /// <returns> Returns a List of BracketModels. </returns>
        public List<BracketModel> GetAllBrackets()
        {
            try
            {
                return unitOfWork.BracketRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<BracketModel>();
            }
        }

        /// <summary>
        /// Retreives all Brackets in a specified Tournament.
        /// </summary>
        /// <param name="tournamnetId"> The Id of the Tournament to retrieve the brackets from. </param>
        /// <returns> Returns a List of Brackets. </returns>
        public List<BracketModel> GetAllBracketsInTournament(int tournamnetId)
        {
            try
            {
                TournamentModel tournament = unitOfWork.TournamentRepo.Get(tournamnetId);
                return tournament.Brackets.ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<BracketModel>();
            }
        }

        /// <summary>
        /// Updates a single Bracket int the database.
        /// </summary>
        /// <param name="bracket"> The model of the Bracket to be updated. </param>
        public void UpdateBracket(BracketModel bracket)
        {
            unitOfWork.BracketRepo.Update(bracket);
        }

        /// <summary>
        /// Deletes a single Bracket from the database.
        /// </summary>
        /// <param name="bracketId"> The Id of the Bracket to delete. </param>
        public void DeleteBracket(int bracketId)
        {
            unitOfWork.BracketRepo.Delete(bracketId);
        }

        #endregion


        #region Matches

        /// <summary>
        /// Adds a slingle Match to the database.
        /// </summary>
        /// <param name="match"> The model of the Match to be added. </param>
        public void AddMatch(MatchModel match)
        {
            unitOfWork.MatchRepo.Add(match);
        }

        /// <summary>
        /// Retreives a single Match from the database.
        /// </summary>
        /// <param name="matchId"> The Id of the Match to be retreived. </param>
        /// <returns> Returns a MatchModel, or null if an exception is thrown. </returns>
        /// <remarks> If a matching Id could not be found, an ObjectNotFoundException will be thrown. </remarks>
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
                unitOfWork.SetException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives all Matches in the database.
        /// </summary>
        /// <returns> Returns a List of Matches. </returns>
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
                unitOfWork.SetException(ex);
                return new List<MatchModel>();
            }
        }

        /// <summary>
        /// Updates a single Match in the database. 
        /// </summary>
        /// <param name="match"> The model of the Match to be updated. </param>
        public void UpdateMatch(MatchModel match)
        {
            unitOfWork.MatchRepo.UpdateDetachCheck(match);
        }

        /// <summary>
        /// Deletes a single Match from the database.
        /// </summary>
        /// <param name="matchId"> The Id of the Match to delete.</param>
        public void DeleteMatch(int matchId)
        {
            unitOfWork.MatchRepo.Delete(matchId);
        }

        /// <summary>
        /// Retreives all Matches in a specified Bracket.
        /// </summary>
        /// <param name="bracketId"> The Id of the Bracket from which to retreive the Matches. </param>
        /// <returns> Returns a List of Matches. </returns>
        public List<MatchModel> GetAllMatchesInBracket(int bracketId)
        {
            try
            {
                return unitOfWork.MatchRepo.GetAll().Where(x => x.BracketID == bracketId).ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<MatchModel>();
            }
        }

        #endregion


        #region Games

        /// <summary>
        /// Adds a single Game to the database. 
        /// </summary>
        /// <param name="game"> The model of the Game to be added. </param>
        public void AddGame(GameModel game)
        {
            unitOfWork.GameRepo.Add(game);
        }

        /// <summary>
        /// Retreives a single Game from the database.
        /// </summary>
        /// <param name="gameId"> The Id of the Game to be retrieved. </param>
        /// <returns> Returns a single Game, or null if an error is thrown. </returns>
        /// <remarks> If a Game with a matching Id could not be found, an ObjectNotFoundException will be thrown. </remarks>
        public GameModel GetGame(int gameId)
        {
            try
            {
                return unitOfWork.GameRepo.Get(gameId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retreives all games from the database.
        /// </summary>
        /// <returns> Returns a List of Games </returns>
        public List<GameModel> GetAllGames()
        {
            try
            {
                return unitOfWork.GameRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<GameModel>();
            }
        }

        /// <summary>
        /// Updates a single Game in the database.
        /// </summary>
        /// <param name="game"> The model of the Game to be updated. </param>
        public void UpdateGame(GameModel game)
        {
            unitOfWork.GameRepo.Update(game);
        }

        /// <summary>
        /// Deletes a single game from the database. 
        /// </summary>
        /// <param name="gameId"></param>
        public void DeleteGame(int gameId)
        {
            unitOfWork.GameRepo.Delete(gameId);
        }

        #endregion


        #region TournamentUsersBrackets

        /// <summary>
        /// Adds a single TournamentUsersBracket to the database. 
        /// </summary>
        /// <param name="tournamentUserBracket"> The model of the TournamentUsersBracket to be added. </param>
        public void AddTournamentUsersBracket(TournamentUsersBracketModel tournamentUserBracket)
        {
            unitOfWork.TournamentUsersBracketRepo.Add(tournamentUserBracket);
        }

        /// <summary>
        /// Retreives a single TournamentUsersBracket from the database.
        /// </summary>
        /// <param name="tournamentUserId"> The Id if the TournamentUser </param>
        /// <param name="bracketId"> The Id of the Bracket. </param>
        /// <returns> Returns a TournamentUsersBracketModel, or null if an exception is thrown. </returns>
        /// <remarks> If a TournamentUsersBracket with the matching Ids could not be found, a ObjectNotFoundException will be thrown. </remarks>
        public TournamentUsersBracketModel GetTournamentUsersBracket(int tournamentUserId, int bracketId)
        {
            try
            {
                return unitOfWork.TournamentUsersBracketRepo.GetSingle(x => x.TournamentUserID == tournamentUserId && x.BracketID == bracketId);
                //return unitOfWork.TournamentUsersBracketRepo.Get(tournamentUsersBracketId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new TournamentUsersBracketModel();
            }
        }

        /// <summary>
        /// Updates a single TournamentUserBracket to the database.
        /// </summary>
        /// <param name="tournamentUserBracket"> The model of the TournamentUsersBracket to update. </param>
        public void UpdateTournamentUsersBracket(TournamentUsersBracketModel tournamentUserBracket)
        {

            unitOfWork.TournamentUsersBracketRepo.Update(tournamentUserBracket);

        }

        /// <summary>
        /// Deletes a single TournamentUsersBracket from the database.
        /// </summary>
        /// <param name="tournamentUserId"> The Id of the TournamentUser. </param>
        /// <param name="bracketId"> The Id of the Bracket that the TournamentUser is in. </param>
        public void DeleteTournamentUsersBracket(int tournamentUserId, int bracketId)
        {
            TournamentUsersBracketModel t = GetTournamentUsersBracket(tournamentUserId, bracketId);
            unitOfWork.TournamentUsersBracketRepo.DeleteEntity(t);
        }


        #endregion

    }
}
