using System;
using WebApplication.Interfaces;

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
        public int BracketTypeID;
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
        public static String[] errorClassNames = new String[] {
            "none",
            "success",
            "warning",
            "error",
        };

        protected IService services;
        public ViewError error = ViewError.NONE;
        public String message;
        public Exception ex;

        public Model(IService service)
        {
            services = service;
        }
    }
}