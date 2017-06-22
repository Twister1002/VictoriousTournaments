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

        /// <summary>
        /// Adds a single SiteTeam.
        /// </summary>
        /// <param name="siteTeam"> The model of the SiteTeam to be added. </param>
        public void AddSiteTeam(SiteTeamModel siteTeam)
        {
            unitOfWork.SiteTeamRepo.Add(siteTeam);
        }

        /// <summary>
        /// Retreives a single SiteTeam.
        /// </summary>
        /// <param name="siteTeamId"> The Id of the SiteTeam to reteive. </param>
        /// <returns> Returns a SiteTeamModel, or null if an exception is thrown. </returns>
        /// <remarks> If a matching Id is not found, an ObjectNotFoundException will be thrown. </remarks>
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

        /// <summary>
        /// Retreives all SiteTeams.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Updates a single SiteTeam.
        /// </summary>
        /// <param name="siteTeam"> The model of the SiteTeam to update. </param>
        public void UpdateSiteTeam(SiteTeamModel siteTeam)
        {
            unitOfWork.SiteTeamRepo.Update(siteTeam);
        }

        /// <summary>
        /// Deletes a single SiteTeam.
        /// </summary>
        /// <param name="siteTeamId"> The Id of the SiteTeam to delete. </param>
        public void DeleteSiteTeam(int siteTeamId)
        {
            unitOfWork.SiteTeamRepo.Delete(siteTeamId);
        }

        /// <summary>
        /// Checks to see if a SiteTeam name already exits. 
        /// </summary>
        /// <param name="teamName"> The name of the team to check. </param>
        /// <returns> Returns true if the name exists, else returns false. </returns>
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

        /// <summary>
        /// Adds a single SiteTeamMember.
        /// </summary>
        /// <param name="siteTeamMember"> The model of the SiteTeamMember to add. </param>
        public void AddSiteTeamMemeber(SiteTeamMemberModel siteTeamMember)
        {
            unitOfWork.SiteTeamMemberRepo.Add(siteTeamMember);
        }

        /// <summary>
        /// Retreives a single SiteTeamMember.
        /// </summary>
        /// <param name="siteTeamMemberId"> The Id of the SiteTeamMember to retreive. </param>
        /// <returns> Returns a SiteTeamMemberModel, or null if there is an exception thrown. </returns>
        /// <remarks> If a matching Id is not found, an ObjectNotFoundException will be thrown. </remarks>
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

        /// <summary>
        /// Retreives all SiteTeamMembers.
        /// </summary>
        /// <returns> Returns a List of SiteTeamMemberModels. </returns>
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

        /// <summary>
        /// Updates a single SiteTeamMember.
        /// </summary>
        /// <param name="siteTeamMember"> The model of the SiteTeamMember to updade. </param>
        public void UpdateSiteTeamMember(SiteTeamMemberModel siteTeamMember)
        {
            unitOfWork.SiteTeamMemberRepo.Update(siteTeamMember);
        }

        /// <summary>
        /// Deletes a single SiteTeamMember.
        /// </summary>
        /// <param name="siteTeamMembetId"> The Id of the SiteTeamMember to delete. </param>
        public void DeleteSiteTeamMember(int siteTeamMembetId)
        {
            unitOfWork.SiteTeamMemberRepo.Delete(siteTeamMembetId);
        }

        #endregion


        #region TournamentTeams

        /// <summary>
        /// Adds a single TournamentTeam.
        /// </summary>
        /// <param name="tournamentTeam"> The model of TournamentTeam to add. </param>
        public void AddTournamentTeam(TournamentTeamModel tournamentTeam)
        {
            unitOfWork.TournamentTeamRepo.Add(tournamentTeam);
        }

        /// <summary>
        /// Retreives a single TournamentTeam.
        /// </summary>
        /// <param name="tournamentTeamId"> The Id of the TournamentTeam to retreive. </param>
        /// <returns> Returns a TournamentTeamModel, or null if an exception is thrown. </returns>
        /// <remarks> If a matching Id is not found, an ObjectNotFoundException is thrown. </remarks>
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

        /// <summary>
        /// Retreives all TournamentTeams.
        /// </summary>
        /// <returns> Retruns a List of TournamentTeamModels. </returns>
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

        /// <summary>
        /// Updates a single TournamentTeam.
        /// </summary>
        /// <param name="tournamentTeam"> The model of TournamentTeam to update. </param>
        public void UpdateTournamentTeam(TournamentTeamModel tournamentTeam)
        {
            unitOfWork.TournamentTeamRepo.Update(tournamentTeam);
        }

        /// <summary>
        /// Deletes a single TournamentTeam.
        /// </summary>
        /// <param name="tournamentTeamId"> The Id of the tournamentTeam to delte. </param>
        public void DeleteTournamentTeam(int tournamentTeamId)
        {
            unitOfWork.TournamentTeamRepo.Delete(tournamentTeamId);
        }

        /// <summary>
        /// Checks to see if a TournamentTeam's name is available in a specified Tournament.
        /// </summary>
        /// <param name="teamName"> The team name to check.</param>
        /// <returns> Returns true if the name already exists, else returns false. </returns>
        public bool TournamentTeamNameExists(string teamName, int tournamentID)
        {
            try
            {
                TournamentTeamModel team = unitOfWork.TournamentTeamRepo.GetSingle(x => x.TeamName == teamName && x.TournamentID == tournamentID);
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

        /// <summary>
        /// Adds a single TournamentTeamMember.
        /// </summary>
        /// <param name="tournamentTeamMember"> The model of the TournamentTeamMember to add.</param>
        public void AddTournamentTeamMember(TournamentTeamMemberModel tournamentTeamMember)
        {
            unitOfWork.TournamentTeamMemberRepo.Add(tournamentTeamMember);
        }

        /// <summary>
        /// Retreives a single TournamentTeamMember.
        /// </summary>
        /// <param name="tournamentTeamMemberId"> The Id of TournamentTeamMember to retreive. </param>
        /// <returns> Returns a TournamentTeamMemberModel, or null if an exception is thrown. </returns>
        /// <remarks> If no matchingId is found, an ObjectNotFoundException will be thrown. </remarks>
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

        /// <summary>
        /// Retreives all TournamentTeamMembers.
        /// </summary>
        /// <returns> Returns a List of TournamentTeamMemberModels. </returns>
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

        /// <summary>
        /// Updates a single TournamentTeamMember.
        /// </summary>
        /// <param name="tournamentTeamMember"> The model of the TournamentTeamMember to update. </param>
        public void UpdateTournamentTeamMember(TournamentTeamMemberModel tournamentTeamMember)
        {
            unitOfWork.TournamentTeamMemberRepo.Update(tournamentTeamMember);
        }
        
        /// <summary>
        /// Deletes a single TournamentTeamMember.
        /// </summary>
        /// <param name="tournamentTeamMemberId"> The Id of the TournamentTeamMember to delete.</param>
        public void DeleteTournamentTeamMember(int tournamentTeamMemberId)
        {
            unitOfWork.TournamentTeamMemberRepo.Delete(tournamentTeamMemberId);
        }

        #endregion


        #region TournamentTeamBrackets
        
        /// <summary>
        /// Adds a single TournamentTeamBracket.
        /// </summary>
        /// <param name="tournamentTeamBracket"> The model of the TournamentTeamBracket to add. </param>
        public void AddTournamentTeamBracket(TournamentTeamBracketModel tournamentTeamBracket)
        {
            unitOfWork.TournamentTeamBracketRepo.Add(tournamentTeamBracket);
        }

        /// <summary>
        /// Retreives a single TournamentTeamBracket.
        /// </summary>
        /// <param name="tournamentTeamId"> The Id of the TournamentTeam. </param>
        /// <param name="bracketId"> The Bracket that the TournamentTeam is in. </param>
        /// <returns> Returns a TournamentTeamBracketModel, or null if an exception is thrown. </returns>
        /// <remarks> If no TournamentTeamBracket with matching Ids is found, an ObjectNotFoundException is thrown. </remarks>
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

        /// <summary>
        /// Retreives all TournamentTeamBrackets.
        /// </summary>
        /// <returns> Returns a List of TournamentTeamBrackets. </returns>
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

        /// <summary>
        /// Updates a single TournamentTeamBracket.
        /// </summary>
        /// <param name="tournamentTeamBracket"> The model of the TournamentTeamBracket to update. </param>
        public void UpdateTournamentTeamBracket(TournamentTeamBracketModel tournamentTeamBracket)
        {
            unitOfWork.TournamentTeamBracketRepo.Update(tournamentTeamBracket);
        }

        /// <summary>
        /// Deletes a single TournamentTeamBracket.
        /// </summary>
        /// <param name="tournamentTeamBracket"> The Id of the TournamentTeamBracket to delte. </param>
        public void DeleteTournamentTeamBracket(TournamentTeamBracketModel tournamentTeamBracket)
        {
            unitOfWork.TournamentTeamBracketRepo.DeleteEntity(tournamentTeamBracket);
        }

        #endregion

    }
}
