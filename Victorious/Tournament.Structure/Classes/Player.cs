using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class Player : IPlayer
	{
		#region Variables & Properties
		public int Id
		{ get; protected set; }
		public string Name
		{ get; set; }
		public string Email
		{ get; set; }
		#endregion

		#region Ctors
		public Player(int _id, string _name, string _email = "")
		{
			this.Id = _id;
			this.Name = _name;
			this.Email = _email;
		}
		public Player()
			: this(
				  -1, // ID
				  "" // Name
				  )
		{ }
		public Player(IPlayer _other)
		{
			if (null == _other)
			{
				throw new ArgumentNullException("_other");
			}

			this.Id = _other.Id;
			this.Name = _other.Name;
			this.Email = _other.Email;
		}
		public Player(AccountModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.AccountID;
			this.Name = _model.Username;
			this.Email = _model.Email;
		}
		public Player(TournamentUserModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.AccountID ?? _model.TournamentUserID;
			//this.Id = _model.TournamentUserID;
			this.Name = _model.Name;
			this.Email = "";
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates an AccountModel of this Player.
		/// </summary>
		public AccountModel GetAccountModel()
		{
			AccountModel model = new AccountModel();
			model.AccountID = this.Id;
			model.Username = this.Name;
			model.Email = this.Email;
			return model;
		}

		/// <summary>
		/// Creates a TournamentUserModel of this Player.
		/// </summary>
		public TournamentUserModel GetTournamentUserModel()
		{
			TournamentUserModel model = new TournamentUserModel();
			model.TournamentUserID = this.Id;
			model.AccountID = this.Id;
			model.Name = this.Name;
			return model;
		}

		/// <summary>
		/// Creates a TournamentUsersBracketModel of this Player.
		/// This Model will also contain the Player's bracketID and seed.
		/// </summary>
		/// <param name="_bracketId">ID of containing Bracket</param>
		/// <param name="_seed">Player's seed-value within the Bracket</param>
		public TournamentUsersBracketModel GetTournamentUsersBracketModel(int _bracketId, int _seed, int _tournamentId = 0)
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
