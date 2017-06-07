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
                return null;
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
                return null;
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
                return null;
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
                return null;
            }
        }

        #endregion


        #region TournamentTeamBrackets
        
        public void AddTournamentTeamBracket(TournamentTeamBracketModel tournamentTeamBracket)
        {
            unitOfWork.TournamentTeamBracketRepo.Add(tournamentTeamBracket);
        }

        public TournamentTeamBracketModel GetTournamentTeamBracket(int tournamentTeamBracketId)
        {
            try
            {
                return unitOfWork.TournamentTeamBracketRepo.Get(tournamentTeamBracketId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TournamentTeamBracketModel> GetAllTOurnamentBrackets()
        {
            try
            {
                return unitOfWork.TournamentTeamBracketRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void UpdateTournamentTeamBracket(TournamentTeamBracketModel tournamentTeamBracket)
        {
            unitOfWork.TournamentTeamBracketRepo.Update(tournamentTeamBracket);
        }

        public void DeleteTournamentTeamBracket(int tournamentTeamBracketId)
        {
            unitOfWork.TournamentTeamBracketRepo.Delete(tournamentTeamBracketId);
        }

        #endregion

    }
}
