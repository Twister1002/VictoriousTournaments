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
            WARNING,
            EXCEPTION,
            CRITICAL
        }

        public String[] errorClassNames = new String[] {"none", "warning", "exception", "critical" };
        public ViewError error = ViewError.NONE;
        protected DatabaseInterface db;
        public String ErrorMessage { get; set; }


    }
}