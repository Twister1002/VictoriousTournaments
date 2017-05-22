using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    interface IPlatformRepository : IDisposable
    {
        DbError AddPlatform(PlatformModel platform);
        PlatformModel GetPlatform(int platformID);
        IList<PlatformModel> GetAllPlatforms();
        DbError UpdatePlatform(PlatformModel platform);
        DbError DeletePlatform(int platformId);
        void Save();


    }
}
