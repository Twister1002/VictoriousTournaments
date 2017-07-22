using System;
using System.Collections.Generic;
using WebApplication.Interfaces;
using WebApplication.Models.ViewModels;

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

    public enum ViewError
    {
        NONE,
        SUCCESS,
        WARNING,
        ERROR
    }

    public struct RoundHeader
    {
        public int roundNum;
        public int bestOf;
        public String title;
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
        protected String message;
        protected ViewError error;

        public Model(IService service)
        {
            services = service;
            message = String.Empty;
            error = ViewError.NONE;
        }

        public void SetMessage(String message, ViewError error)
        {
            this.message = message;
            this.error = error;
        }

        public String GetMessage()
        {
            return message;
        }

        public ViewError GetErrorType()
        {
            return error;
        }
    }
}