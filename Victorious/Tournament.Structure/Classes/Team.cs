using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class Team : Player
	{
		#region Variables & Properties
		// inherits int Id
		// inherits string Name
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
		public override UserModel GetModel()
		{
			throw new NotImplementedException();
		}
#if false
		public TeamModel GetModel()
		{
			TeamModel model = new TeamModel();
			model.TeamID = this.Id;
			model.TeamName = this.Name;

			List<TeamMemberModel> tMemberModels = new List<TeamMemberModel>();
			foreach (User user in TeamMembers)
			{
				TeamMemberModel tmModel = new TeamMemberModel();
				tmModel.UserID = user.Id;
				tmModel.TeamID = this.Id;
				tmModel.Team = model;

				tmModel.User = new UserModel();
				tmModel.User.UserID = user.Id;
				tmModel.User.Username = user.Name;
				tmModel.User.FirstName = user.Firstname;
				tmModel.User.LastName = user.Lastname;
				tmModel.User.Email = user.Email;

				tMemberModels.Add(tmModel);
			}

			return model;
		}
#endif

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
