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
            platforms = new List<PlatformModel>();
            Model = new PlatformModel();
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
            Reload();
            return false;
        }

        public bool Update()
        {
            Reload();
            return false;
        }

        public bool Delete(int platformId)
        {
            return false;
        }

        public List<PlatformModel> Select()
        {
            return platforms;
        }

        private void Reload()
        {
            platforms = db.GetAllPlatforms();
        }
    }
}