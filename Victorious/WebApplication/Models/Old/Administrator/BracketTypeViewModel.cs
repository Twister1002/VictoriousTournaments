using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseLib;
using WebApplication.Interfaces;

namespace WebApplication.Models.Administrator.Old
{
    public class BracketTypeViewModel : ViewModel, IViewModel
    {
        public List<BracketTypeModel> Brackets { get; private set; }

        public BracketTypeViewModel(IUnitOfWork work) : base(work)
        {
            Init();
        }

        public void Init()
        {
            Select();
        }

        private void Select()
        {
            Brackets = services.TypeService.GetAllBracketTypes();
        }

        public bool Update(int bracketTypeId)
        {
            BracketTypeModel model = Brackets.First(y => y.BracketTypeID == bracketTypeId);
            model.IsActive = model.IsActive ? false : true;

            services.TypeService.UpdateBracketType(model);
            if (services.Save())
            {
                Select();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ApplyChanges()
        {
            throw new NotImplementedException();
        }

        public void SetFields()
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public bool Create()
        {
            throw new NotImplementedException();
        }
    }
}