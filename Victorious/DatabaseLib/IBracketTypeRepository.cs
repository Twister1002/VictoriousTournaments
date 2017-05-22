﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    interface IBracketTypeRepository : IDisposable
    {
        IList<BracketTypeModel> GetAllBracketTypes(bool returnOnlyActive);
        DbError UpdateBracketType(BracketTypeModel bracketType);
        void Save();

    }
}
