using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public enum Permission
    {
        // 0 = None
        NONE = 0,
        // 1 = Site Permissions
        SITE_ADMINISTRATOR = 1, SITE_STANDARD,
        // 100 = Tournament Permissions
        TOURNAMENT_ADMINISTRATOR = 100, TOURNAMENT_STANDARD,
        // 200 = Team Permissions
        TEAM_CAPTAIN = 200, TEAM_STANDARD
    }

    public partial class PermissionModel
    {

        public Permission Permission { get; set; }

    }

}

