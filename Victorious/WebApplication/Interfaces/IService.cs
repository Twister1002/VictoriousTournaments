using DatabaseLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Interfaces
{
    public interface IService
    {
        AccountService Account { get; }
        TournamentService Tournament { get; }
        TypeService Type { get; }
        Exception GetException();
        bool Save();
    }
}
