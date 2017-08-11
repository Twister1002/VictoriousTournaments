using DatabaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib.Repositories
{
    class AccountForgetRepository : IAccountForgetRepository
    {
        public AccountForgetModel Get(string token)
        {
            throw new NotImplementedException();
        }

        public AccountForgetModel Get(string token, int accountID)
        {
            throw new NotImplementedException();
        }

        public void TokenUsed(string token, int accountID)
        {
            throw new NotImplementedException();
        }
    }
}
