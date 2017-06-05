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

        public void DeleteSiteTeam(int siteTeamId)
        {
            unitOfWork.SiteTeamRepo.Delete(siteTeamId);
        }

        #endregion


        #region SiteTeamMember

        public void AddSiteTeamMemeber(SiteTeamMemberModel siteTeamMember)
        {

        }

        #endregion


    }
}
