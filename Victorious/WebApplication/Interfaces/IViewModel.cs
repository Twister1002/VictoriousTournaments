using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Interfaces
{
    public interface IViewModel
    {
        void Init();
        void ApplyChanges();
        void SetFields();
    }
}