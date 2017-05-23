using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public interface IUnitOfWork : IDisposable
    {
        ITournamentRepository TournamentRepository { get; }

        void Dispose();
    }
}