using DatabaseLib;
using System.Collections.Generic;
using WebApplication.Interfaces;

namespace WebApplication.Models.Administrator.Old
{
    public class PlatformTypeViewModel : PlatformTypeFields
    {
        public List<PlatformModel> platforms { get; protected set; }
        public PlatformModel Model { get; protected set; }

        public PlatformTypeViewModel(IUnitOfWork work) : base(work)
        {
            Model = new PlatformModel();
            Init();
        }

        public void Init()
        {
            Select();
        }

        public void ApplyChanges()
        {
            Model.PlatformName = this.Platform;
        }

        public void SetFields()
        {
            this.Platform = Model.PlatformName;
        }

        public bool Create()
        {
            ApplyChanges();
            typeService.AddPlatform(Model);

            if (Save())
            {
                Select();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Update(int id)
        {
            return false;
        }

        public bool Delete(int platformId)
        {
            typeService.DeletePlatform(platformId);
            if (Save())
            {
                Select();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Select()
        {
            platforms = typeService.GetAllPlatforms();
        }
    }
}