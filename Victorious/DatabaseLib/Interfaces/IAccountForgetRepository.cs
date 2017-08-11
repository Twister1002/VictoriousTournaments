using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib.Interfaces
{
    interface IAccountForgetRepository
    {
        AccountForgetModel Get(String token);

        AccountForgetModel Get(String token, int accountID);

        void TokenUsed(String token, int accountID);
    }
}
