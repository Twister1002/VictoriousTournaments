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
            Brackets = db.GetAllBracketTypes();
        }

        public bool Update(int bracketTypeId)
        {
            BracketTypeModel model = Brackets.First(y => y.BracketTypeID == bracketTypeId);
            model.IsActive = model.IsActive ? false : true;

            return db.UpdateBracketType(model) == DbError.SUCCESS;
        }
    }
}