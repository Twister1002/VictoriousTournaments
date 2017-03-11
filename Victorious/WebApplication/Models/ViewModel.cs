using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public Exception dbException { get; set; }
        public ViewError error = ViewError.NONE;
        protected DatabaseInterface db = new DatabaseInterface();
        public String message { get; set; }
    }
}