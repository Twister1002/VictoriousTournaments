using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Interfaces
{
    public interface IViewModel<T> where T : class
    {
        void SetupViewModel();
        void ApplyChanges(T viewModel);
        void SetFields();
    }
}