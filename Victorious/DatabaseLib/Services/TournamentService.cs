using System;
using System.Collections.Generic;
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
            TournamentInviteModel invite = new TournamentInviteModel()
            {
                TournamentID = tournamentToAdd.TournamentID,
                TournamentInviteCode = tournamentToAdd.InviteCode,
                DateCreated = DateTime.Today,
                IsExpired = false,
                DateExpires = tournamentToAdd.RegistrationEndDate
            };
            AddTournamentInvite(invite);
            unitOfWork.TournamentRepo.Add(tournamentToAdd);
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

        public void UpdateTournamentUser(TournamentUserModel tournamentUserToUpdate)
        {
            unitOfWork.TournamentUserRepo.Update(tournamentUserToUpdate);
        }

        public void DeleteTournamentUser(int tournamentUserId)
        {
            unitOfWork.TournamentUserRepo.Delete(tournamentUserId);
        }

        #endregion


        #region TournamentInvites

        public void AddTournamentInvite(TournamentInviteModel tournamentInviteToAdd)
        {
            unitOfWork.TournamentInviteRepo.Add(tournamentInviteToAdd);
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
            unitOfWork.TournamentInviteRepo.Delete(tournamentInviteToDelete.TournamentInviteID);
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

        public void DeleteGame(int gameId)
        {
            unitOfWork.GameRepo.Delete(gameId);
        }

        #endregion

    }
}
