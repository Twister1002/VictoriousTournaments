using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib.Services
{
    public class TeamService
    {
        IUnitOfWork unitOfWork;

        public TeamService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        #region SiteTeams

        public void AddSiteTeam(SiteTeamModel siteTeam)
        {
            unitOfWork.SiteTeamRepo.Add(siteTeam);
        }

        public SiteTeamModel GetSiteTeam(int siteTeamId)
        {
            try
            {
                return unitOfWork.SiteTeamRepo.Get(siteTeamId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        public List<SiteTeamModel> GetAllSiteTeams()
        {
            try
            {
                return unitOfWork.SiteTeamRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<SiteTeamModel>();
            }
        }

        public void UpdateSiteTeam(SiteTeamModel siteTeam)
        {
            unitOfWork.SiteTeamRepo.Update(siteTeam);
        }

        public void DeleteSiteTeam(int siteTeamId)
        {
            unitOfWork.SiteTeamRepo.Delete(siteTeamId);
        }

        public bool SiteTeamNameExists(string teamName)
        {
            
            try
            {
                unitOfWork.SiteTeamRepo.GetSingle(x => x.TeamName == teamName);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return false;
            }
            return true;
        }

        #endregion


        #region SiteTeamMember

        public void AddSiteTeamMemeber(SiteTeamMemberModel siteTeamMember)
        {
            unitOfWork.SiteTeamMemberRepo.Add(siteTeamMember);
        }

        public SiteTeamMemberModel GetSiteTeamMemeber(int siteTeamMemberId)
        {
            try
            {
                return unitOfWork.SiteTeamMemberRepo.Get(siteTeamMemberId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        public List<SiteTeamMemberModel> GetAllSiteTeamMembers()
        {
            try
            {
                return unitOfWork.SiteTeamMemberRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<SiteTeamMemberModel>();
            }
        }

        public void UpdateSiteTeamMember(SiteTeamMemberModel siteTeamMember)
        {
            unitOfWork.SiteTeamMemberRepo.Update(siteTeamMember);
        }

        public void DeleteSiteTeamMember(int siteTeamMembetId)
        {
            unitOfWork.SiteTeamMemberRepo.Delete(siteTeamMembetId);
        }

        #endregion


        #region TournamentTeams

        public void AddTournamentTeam(TournamentTeamModel tournamentTeam)
        {
            unitOfWork.TournamentTeamRepo.Add(tournamentTeam);
        }

        public TournamentTeamModel GetTournamentTeam(int tournamentTeamId)
        {
            try
            {
                return unitOfWork.TournamentTeamRepo.Get(tournamentTeamId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        public List<TournamentTeamModel> GetAllTournamentTeams()
        {
            try
            {
                return unitOfWork.TournamentTeamRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<TournamentTeamModel>();
            }
        }

        public void UpdateTournamentTeam(TournamentTeamModel tournamentTeam)
        {
            unitOfWork.TournamentTeamRepo.Update(tournamentTeam);
        }

        public void DeleteTournamentTeam(int tournamentTeamId)
        {
            unitOfWork.TournamentTeamRepo.Delete(tournamentTeamId);
        }

        public bool TournamentTeamNameExists(string teamName)
        {
            try
            {
                TournamentTeamModel team = unitOfWork.TournamentTeamRepo.GetSingle(x => x.TeamName == teamName);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return false;
            }
            return true;
        }

        #endregion


        #region TournamentTeamMembers

        public void AddTournamentTeamMember(TournamentTeamMemberModel tournamentTeamMember)
        {
            unitOfWork.TournamentTeamMemberRepo.Add(tournamentTeamMember);
        }

        public TournamentTeamMemberModel GetTournamentTeamMember(int tournamentTeamMemberId)
        {
            try
            {
                return unitOfWork.TournamentTeamMemberRepo.Get(tournamentTeamMemberId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        public List<TournamentTeamMemberModel> GetAllTournamentTeamMembers()
        {
            try
            {
                return unitOfWork.TournamentTeamMemberRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<TournamentTeamMemberModel>();
            }
        }

        public void UpdateTournamentTeamMember(TournamentTeamMemberModel tournamentTeamMember)
        {
            unitOfWork.TournamentTeamMemberRepo.Update(tournamentTeamMember);
        }

        public void DeleteTournamentTeamMember(int tournamentTeamMemberId)
        {
            unitOfWork.TournamentTeamMemberRepo.Delete(tournamentTeamMemberId);
        }

        #endregion


        #region TournamentTeamBrackets
        
        public void AddTournamentTeamBracket(TournamentTeamBracketModel tournamentTeamBracket)
        {
            unitOfWork.TournamentTeamBracketRepo.Add(tournamentTeamBracket);
        }

        public TournamentTeamBracketModel GetTournamentTeamBracket(int tournamentTeamId, int bracketId)
        {
            try
            {
                return unitOfWork.TournamentTeamBracketRepo.GetSingle(x => x.TournamentTeamID == tournamentTeamId && x.BracketID == bracketId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        public List<TournamentTeamBracketModel> GetAllTournamentTeamBrackets()
        {
            try
            {
                return unitOfWork.TournamentTeamBracketRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<TournamentTeamBracketModel>();
            }
        }

        public void UpdateTournamentTeamBracket(TournamentTeamBracketModel tournamentTeamBracket)
        {
            unitOfWork.TournamentTeamBracketRepo.Update(tournamentTeamBracket);
        }

        public void DeleteTournamentTeamBracket(TournamentTeamBracketModel tournamentTeamBracket)
        {
            unitOfWork.TournamentTeamBracketRepo.DeleteEntity(tournamentTeamBracket);
        }

        #endregion

    }
}
