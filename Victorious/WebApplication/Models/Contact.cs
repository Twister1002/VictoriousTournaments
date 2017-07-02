using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Interfaces;
using WebApplication.Models.ViewModels;

namespace WebApplication.Models
{
    public class Contact : Home, IViewModel<ContactViewModel>
    {
        public ContactViewModel viewModel;

        public Contact(IService service) : base(service)
        {
            Init();
        }

        public void Init()
        {
            SetupViewModel();
        }

        public void SendEmail()
        {

        }
        
        public void SetupViewModel()
        {
            viewModel = new ContactViewModel();
        }

        public void SetFields()
        {
            
        }

        public void ApplyChanges(ContactViewModel viewModel)
        {
            
        }
    }
}