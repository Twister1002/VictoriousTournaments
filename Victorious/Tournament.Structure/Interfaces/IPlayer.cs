using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	/// <summary>
	/// Interface used to fill slots for Users or Teams.
	/// </summary>
	public interface IPlayer
	{
		#region Variables & Properties
		int Id { get; }
		string Name { get; set; }
		string Email { get; set; }
		#endregion

		#region Public Methods
		AccountModel GetAccountModel();
		TournamentUserModel GetTournamentUserModel();
		TournamentUsersBracketModel GetTournamentUsersBracketModel(int _bracketId, int _seed);
		#endregion
	}
}
