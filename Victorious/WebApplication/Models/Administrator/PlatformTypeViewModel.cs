using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Administrator
{
    public class PlatformTypeViewModel : PlatformTypeFields
    {
        public List<PlatformModel> platforms { get; protected set; }
        public PlatformModel Model { get; protected set; }

        public PlatformTypeViewModel()
        {
            Model = new PlatformModel();
        }

        protected override void Init()
        {
            Select();
        }

        public void ApplyFields()
        {
            Model.PlatformName = this.Platform;
        }

        public void SetFields()
        {
            this.Platform = Model.PlatformName;
        }

        public bool Create()
        {
            ApplyFields();
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

        public bool Update()
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