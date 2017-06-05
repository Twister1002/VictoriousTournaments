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
            return unitOfWork.GameTypeRepo.Get(gameTypeId);
        }

        public List<GameTypeModel> GetAllGameTypes()
        {
            return unitOfWork.GameTypeRepo.GetAll().ToList();
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
            return unitOfWork.BracketTypeRepo.Get(bracketTypeId);
        }

        public List<BracketTypeModel> GetAllBracketTypes()
        {
            return unitOfWork.BracketTypeRepo.GetAll().ToList();
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
            return unitOfWork.PlatformRepo.Get(platformId);
        }

        public List<PlatformModel> GetAllPlatforms()
        {
            return unitOfWork.PlatformRepo.GetAll().ToList();
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

    }
}
