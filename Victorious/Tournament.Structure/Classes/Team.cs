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
		public Team()
		{
			Name = "";
			TeamMembers = new List<User>();
		}
		public Team(Team _team)
		{
			this.Id = _team.Id;
			this.Name = _team.Name;

			this.TeamMembers = new List<User>();
			foreach (User u in _team.TeamMembers)
			{
				this.TeamMembers.Add(new User(u));
			}
		}
		#endregion

		#region Public Methods
		public void AddMember(User _user)
		{
			if (TeamMembers.Contains(_user))
			{
				throw new DuplicateObjectException
					("Team already contains this User!");
			}

			TeamMembers.Add(_user);
		}
		public void RemoveMember(User _user)
		{
			if (!TeamMembers.Remove(_user))
			{
				throw new PlayerNotFoundException
					("User not found in this team!");
			}
		}
		#endregion
	}
}
