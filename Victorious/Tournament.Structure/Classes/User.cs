using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class User : Player
	{
		#region Variables & Properties
		// inherits int Id
		// inherits string Name
		public string Firstname
		{ get; set; }
		public string Lastname
		{ get; set; }
		public string Email
		{ get; set; }
		#endregion

		#region Ctors
		public User(int _id, string _name, string _first, string _last, string _email)
		{
			if (null == _name
				|| null == _first
				|| null == _last
				|| null == _email)
			{
				throw new NullReferenceException
					("You've got NULL problems in your User construction...");
			}

			Id = _id;
			Name = _name;
			Firstname = _first;
			Lastname = _last;
			Email = _email;
		}
		public User()
			: this(-1, "", "", "", "")
		{ }
		public User(User _user)
		{
			if (null == _user)
			{
				throw new ArgumentNullException("_user");
			}

			this.Id = _user.Id;
			this.Name = _user.Name;
			this.Firstname = _user.Firstname;
			this.Lastname = _user.Lastname;
			this.Email = _user.Email;
		}
		public User(AccountModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.AccountID;
			this.Name = _model.Username;
			this.Firstname = _model.FirstName;
			this.Lastname = _model.LastName;
			this.Email = _model.Email;
		}
		public User(TournamentUserModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.TournamentUserID;
			this.Name = _model.Username;
		}
		#endregion

		#region Public Methods
		public override AccountModel GetAccountModel()
		{
			AccountModel model = new AccountModel();
			model.AccountID = this.Id;
			model.Username = this.Name;
			model.FirstName = this.Firstname;
			model.LastName = this.Lastname;
			model.Email = this.Email;
			return model;
		}
		public override TournamentUserModel GetTournamentUserModel()
		{
			TournamentUserModel model = new TournamentUserModel();
			model.TournamentUserID = this.Id;
			model.AccountID = this.Id;
			model.Username = this.Name;
			return model;
		}
		public override TournamentUsersBracketModel GetTournamentUsersBracketModel(int _bracketId, int _seed)
		{
			TournamentUsersBracketModel model = new TournamentUsersBracketModel();
			model.BracketID = _bracketId;
			model.Seed = _seed;
			model.TournamentUser = this.GetTournamentUserModel();
			return model;
		}
		#endregion
	}
}
