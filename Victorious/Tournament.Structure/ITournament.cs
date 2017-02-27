using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface ITournament
	{
		#region Variables & Properties
		/// <summary>
		/// Tournament's name.
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Tournament's description.
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// List of users/teams participating in tournament.
		/// </summary>
		List<IPlayer> Players { get; set; }

		/// <summary>
		/// List of brackets for the tournament.
		/// (temporarily limited to one)
		/// </summary>
		List<IBracket> Brackets { get; set; }

		/// <summary>
		/// Tournament's payout.
		/// </summary>
		float PrizePool { get; set; }

		/// <summary>
		/// Is tournament publicly viewable?
		/// </summary>
		bool IsPublic { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Add a Player object to tournament's list.
		/// </summary>
		/// <param name="_p">Player object to add.</param>
		/// <returns>False if _p is a duplicate, True if successful.</returns>
		bool AddPlayer(IPlayer _p);

		/// <summary>
		/// Add an existing Bracket object to tournament's list.
		/// </summary>
		/// <param name="_b">Bracket object to add.</param>
		/// <returns>False if _b is a duplicate, True if successful.</returns>
		bool AddBracket(IBracket _b);

		/// <summary>
		/// Create a new Single Elim Bracket, and add to the tournament.
		/// (temp: replaces any current bracket)
		/// </summary>
		/// <returns>False if error, True if successful.</returns>
		bool CreateSingleElimBracket();

		/// <summary>
		/// Create a new Single Elim Bracket, and add to the tournament.
		/// (temp: replaces any current bracket)
		/// </summary>
		/// <returns>False if error, True if successful.</returns>
		bool CreateDoubleElimBracket();
		#endregion
	}
}
