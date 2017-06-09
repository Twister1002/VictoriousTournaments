using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Interfaces;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public enum TournamentStatus
    {
        ADMIN,
        ACTIVE,
        UPCOMING,
        PAST
    }

    public enum BracketSection
    {
        UPPER,
        LOWER,
        FINAL
    }

    public struct RoundHeader
    {
        public int roundNum;
        public int bestOf;
        public String title;
    }

    public struct BracketInfo
    {
        public int BracketType;
        public int NumberOfRounds;
    }

    public enum ViewError
    {
        NONE,
        SUCCESS,
        WARNING,
        ERROR
    }

    public abstract class Model
    {
        protected IService services;

        public String[] errorClassNames = new String[] {
            "none",
            "success",
            "warning",
            "error",
        };

        public ViewError error = ViewError.NONE;
        public String message;
        public Exception ex;

        public Model(IService service)
        {
            services = service;
        }
    }
}