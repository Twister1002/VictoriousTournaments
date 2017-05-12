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
            Reload();
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
            DbError result = DbError.NONE;

            if (!String.IsNullOrEmpty(Model.PlatformName))
            {
                result = db.AddPlatform(Model);
            }

            Reload();
            return result == DbError.SUCCESS;
        }

        public bool Update()
        {
            Reload();
            return false;
        }

        public bool Delete(int platformId)
        {
            DbError result = db.DeletePlatform(platformId);

            Reload();
            return result == DbError.SUCCESS;
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