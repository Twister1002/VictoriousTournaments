using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Interfaces;

namespace WebApplication.Models.Old
{
    public class HomeViewModel : ViewModel
    {
        public HomeViewModel(IUnitOfWork work) : base(work){

        }

        public void ApplyChanges()
        {
            throw new NotImplementedException();
        }

        public void SetFields()
        {
            throw new NotImplementedException();
        }
        public void Init() { 
            throw new NotImplementedException();
        }
    }
}