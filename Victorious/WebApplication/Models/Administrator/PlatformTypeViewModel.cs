using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Administrator
{
    public class PlatformTypeViewModel : PlatformTypeFields
    {
        public List<String> platforms { get; protected set; }
        public String Model { get; protected set; }

        public PlatformTypeViewModel()
        {
            
        }

        public void ApplyFields()
        {

        }

        public void SetFields()
        {

        }

        public bool Create()
        {
            return false;
        }

        public bool Update()
        {
            return false;
        }

        public bool Delete(int platformId)
        {
            return false;
        }

        public bool Select()
        {
            return false;
        }
    }
}