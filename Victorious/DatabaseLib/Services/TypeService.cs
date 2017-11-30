using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib.Services
{
    public class TypeService
    {
        IUnitOfWork unitOfWork;

        public TypeService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region GameTypes

        public void AddGameType(GameTypeModel gameTypeToAdd)
        {
            unitOfWork.GameTypeRepo.Add(gameTypeToAdd);
        }

        public GameTypeModel GetGameType(int gameTypeId)
        {
            try
            {
                return unitOfWork.GameTypeRepo.Get(gameTypeId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        public List<GameTypeModel> GetAllGameTypes()
        {
            try
            {
                return unitOfWork.GameTypeRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<GameTypeModel>();
            }
        }

        public void UpdateGameType(GameTypeModel gameTypeToUpdate)
        {
            unitOfWork.GameTypeRepo.Update(gameTypeToUpdate);
        }

        public void DeleteGameType(int GameTypeId)
        {
            unitOfWork.GameTypeRepo.Delete(GameTypeId);
        }

        #endregion


        #region BracketTypes

        public void AddBracketType(BracketTypeModel bracketTypeToAdd)
        {
            unitOfWork.BracketTypeRepo.Add(bracketTypeToAdd);
        }

        public BracketTypeModel GetBracketType(int bracketTypeId)
        {
            try
            {
                return unitOfWork.BracketTypeRepo.Get(bracketTypeId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        public List<BracketTypeModel> GetAllBracketTypes()
        {
            try
            {
                return unitOfWork.BracketTypeRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<BracketTypeModel>();
            }
        }

        public void UpdateBracketType(BracketTypeModel bracketTypeToUpdate)
        {
            unitOfWork.BracketTypeRepo.Update(bracketTypeToUpdate);
        }

        public void DeleteBracketType(int bracketTypeId)
        {
            unitOfWork.BracketTypeRepo.Delete(bracketTypeId);
        }


        #endregion


        #region Platforms

        public void AddPlatform(PlatformModel platformToAdd)
        {
            unitOfWork.PlatformRepo.Add(platformToAdd);
        }

        public PlatformModel GetPlatform(int platformId)
        {
            try
            {
                return unitOfWork.PlatformRepo.Get(platformId);
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return null;
            }
        }

        public List<PlatformModel> GetAllPlatforms()
        {
            try
            {
                return unitOfWork.PlatformRepo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.SetException(ex);
                return new List<PlatformModel>();
            }
        }

        public void UpdatePlatform(PlatformModel platformToUpdate)
        {
            unitOfWork.PlatformRepo.Update(platformToUpdate);
        }

        public void DeletePlatform(int platformId)
        {
            unitOfWork.PlatformRepo.Delete(platformId);
        }

        #endregion

        #region AccountProvider 
        public List<SocialProviderModel> SocialProviders()
        {
            try
            {
                return unitOfWork.SocialProviderRepo.GetAll().ToList();
            }
            catch (Exception e)
            {
                unitOfWork.SetException(e);
                return new List<SocialProviderModel>();
            }
        }
        #endregion

        #region BroadcastServices
        public List<BroadcastServiceModel> BroadcastServices()
        {
            try
            {
                return unitOfWork.BroadcastServiceRepo.GetAll().ToList();
            }
            catch (Exception e)
            {
                unitOfWork.SetException(e);
                return new List<BroadcastServiceModel>();
            }
        }
        #endregion
    }
}
