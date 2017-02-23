using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class Team : Player
	{
		#region Variables & Properties
		public List<User> TeamMembers
		{ get; set; }
		#endregion

		#region Ctors
		public Team(string _name) : base(_name)
		{
			TeamMembers = new List<User>();
		}
		#endregion

		#region Public Methods
		public bool AddMember(User _user)
		{
			if (TeamMembers.Contains(_user))
			{
				return false;
			}
			TeamMembers.Add(_user);
			return true;
		}
		public bool RemoveMember(User _user)
		{
			if (TeamMembers.Remove(_user))
			{
				return true;
			}
			return false;
		}
		#endregion
	}
}
