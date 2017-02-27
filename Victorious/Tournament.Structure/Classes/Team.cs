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
		public Team(string _name)
		{
			Name = _name;
			TeamMembers = new List<User>();
		}
		#endregion

		#region Public Methods
		public void AddMember(User _user)
		{
			if (TeamMembers.Contains(_user))
			{
				throw new DuplicateObjectException();
			}

			TeamMembers.Add(_user);
		}
		public void RemoveMember(User _user)
		{
			if (!TeamMembers.Remove(_user))
			{
				throw new KeyNotFoundException();
			}
		}
		#endregion
	}
}
