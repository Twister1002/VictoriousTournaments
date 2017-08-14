using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public class AccountRetrieve : Model
    {
        public AccountRetrieve(IService service) : base(service)
        {
        }
    }
}