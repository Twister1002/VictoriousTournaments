using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;

namespace WebApplication.Models.Administrator
{
    public class BracketTypeViewModel : ViewModel
    {
        public List<BracketTypeModel> Brackets { get; private set; }

        public BracketTypeViewModel()
        {
            Select();
        }
        protected override void Init()
        {
        }

        private void Select()
        {
            Brackets = db.GetAllBracketTypes(false);
        }

        public bool Update(int bracketTypeId)
        {
            BracketTypeModel model = Brackets.First(y => y.BracketTypeID == bracketTypeId);
            model.IsActive = model.IsActive ? false : true;

            bool updated = db.UpdateBracketType(model) == DbError.SUCCESS;

            Select();
            return updated; 
        }
    }
}