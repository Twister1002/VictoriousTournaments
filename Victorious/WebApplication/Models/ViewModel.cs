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
#if DEBUG
        protected IUnitOfWork work = new UnitOfWork();
        protected DatabaseRepository db = new DatabaseRepository("Debug");
#elif !DEBUG
        protected DatabaseRepository db = new DatabaseRepository("Production");
#endif
        protected abstract void Init();
        public String message { get; set; }
        public Exception dbException { get; set; }
    }
}