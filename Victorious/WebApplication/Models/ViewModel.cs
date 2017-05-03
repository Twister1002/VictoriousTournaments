using System;
using DatabaseLib;

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
        protected DbInterface db = new DbInterface();
        public String message { get; set; }
        public Exception dbException { get; set; }
    }
}