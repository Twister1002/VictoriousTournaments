using DatabaseLib;
using System;
using WebApplication.Utility;

namespace WebApplication.Models.Old
{
    public abstract class ViewModel
    {
        public enum ViewError
        {
            NONE,
            SUCCESS,
            WARNING,
            ERROR
        }

        public String[] errorClassNames = new String[] {
            "none",
            "success",
            "warning",
            "error",
        };
        
        public ViewError error = ViewError.NONE;
        protected Service services;

        protected ViewModel(IUnitOfWork uow)
        {
            services = new Service(uow);
        }
        
        public String message { get; set; }
        public Exception dbException { get; set; }
    }
}