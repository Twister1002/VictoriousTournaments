using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Utility
{
    public static class ErrorCodes
    {
        public static Dictionary<int, String> Errors = new Dictionary<int, string>()
        {
            // Site codes from 0-100
            // Tournament codes from 101 - 200
            // Bracket codes from 201 - 300
            // Match Codes from 301 - 400
            // Game codes from 401 - 500
            // Ajax codes from 501 - 600

            { 101, "Unable to create tournament" },
            { 102, "Unable to finalize tournament" }

        };
    }
}