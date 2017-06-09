using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;
using WebApplication.Models.ViewModels;
using WebApplication.Utility;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public class Platform : Model
    {
        private PlatformModel Model;
        public PlatformViewModel viewModel { get; private set; }

        public List<PlatformModel> Platforms { get; private set; }

        public Platform(IService service) : base(service)
        {
            Init();
        }

        private void Init()
        {
            Retrieve();
        }

        #region CRUD
        public bool Create(PlatformViewModel viewModel)
        {
            this.viewModel = viewModel;
            ApplyChanges();

            services.Type.AddPlatform(Model);
            return services.Save();
        }

        public void Retrieve()
        {
            Platforms = services.Type.GetAllPlatforms();
        }

        public bool Update()
        {
            throw new NotSupportedException("Not able to update a Platform");
        }

        public bool Delete(PlatformViewModel viewModel)
        {
            services.Type.DeletePlatform(viewModel.PlatformID);
            return services.Save();
        }
        #endregion

        #region ViewModel
        public void ApplyChanges()
        {
            if (Model == null)
            {
                Model = new PlatformModel();
            }

            Model.PlatformName = viewModel.Platform;
        }

        public void SetFields()
        {
            viewModel.Platform = Model.PlatformName;
        }
        #endregion
    }
}