using System;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public abstract class ViewModel
    {
        public enum ViewError
        {
            NONE,
            SUCCESS,
            WARNING,
            EXCEPTION,
            CRITICAL
        }

        public String[] errorClassNames = new String[] {
            "none",
            "success",
            "warning",
            "exception",
            "critical"
        };
        
        public ViewError error = ViewError.NONE;
        protected Service services;

        protected ViewModel(Service service)
        {
            services = service;
        }
        
        public String message { get; set; }
        public Exception dbException { get; set; }
    }
}