﻿using System;
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

    public struct RoundHeader
    {
        public int roundNum;
        public int bestOf;
        public String title;
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

        public Model(IService service)
        {
            services = service;
        }
    }
}