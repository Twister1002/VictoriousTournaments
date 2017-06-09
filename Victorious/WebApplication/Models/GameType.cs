using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;
using WebApplication.Models.ViewModels;

namespace WebApplication.Models
{
    public class GameType : Model
    {
        private GameTypeViewModel viewModel;
        private GameTypeModel Model;

        public List<GameTypeModel> Games { get; private set; }

        public GameType(IUnitOfWork work) : base(work)
        {
            Init();
        }

        private void Init()
        {
            Retrieve();
        }

        #region CRUD
        public bool Create(GameTypeViewModel viewModel)
        {
            this.viewModel = viewModel;
            ApplyChanges();

            services.Type.AddGameType(Model);
            return services.Save();
        }

        public void Retrieve()
        {
            Games = services.Type.GetAllGameTypes();
        }

        public bool Update()
        {
            throw new NotSupportedException("Not able to update a Game");
        }

        public bool Delete(GameTypeViewModel viewModel)
        {
            services.Type.DeleteGameType(viewModel.GameID);
            return services.Save();
        }
        #endregion

        #region ViewModel
        public void ApplyChanges()
        {
            if (Model == null)
            {
                Model = new GameTypeModel();
            }

            Model.Title = viewModel.Title;
        }

        public void SetFields()
        {
            viewModel.Title = Model.Title;
        }
        #endregion
    }
}