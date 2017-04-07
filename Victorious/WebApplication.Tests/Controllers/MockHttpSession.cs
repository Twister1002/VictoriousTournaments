using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication.Tests.Controllers
{
    class MockHttpSession : HttpSessionStateBase
    {
        Dictionary<string, object> sessionData = new Dictionary<string, object>();

        public override object this[string name]
        {
            get { return sessionData[name]; }
            set { sessionData[name] = value; }
        }
    }
}
