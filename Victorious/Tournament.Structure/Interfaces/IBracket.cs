using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IBracket
	{
		#region Variables & Properties
		//uint Id { get; }

		/// <summary>
		/// List of Teams/Users in the Bracket.
		/// </summary>
		List<IPlayer> Players { get; set; }

		/// <summary>
		/// List of "rounds": List of Matches.
		/// Match = "Rounds[round][match]"
		/// </summary>
		List<List<IMatch>> Rounds { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Generates a bracket (Rounds list),
		/// based on current class data.
		/// </summary>
		/// <param name="_winsPerMatch">Default number of wins
		/// needed to "win" each Match.</param>
		void CreateBracket(ushort _winsPerMatch = 1);

		/// <summary>
		/// Fill the bracket with current Match data from the DB.
		/// </summary>
		/// <param name="_tournamentId">ID of the bracket's Tournament.</param>
		/// <returns>True if successful.</returns>
		bool FetchMatches(int _tournamentId);

		/// <summary>
		/// Add a Player to the Players list.
		/// </summary>
		/// <param name="_p">Player to add.</param>
		/// <returns>False if _p is already present,
		/// True if successful.</returns>
		bool AddPlayer(IPlayer _p);

		/// <summary>
		/// Add a new round to the Bracket.
		/// </summary>
		void AddRound();

		/// <summary>
		/// Add a new Match to the specified round.
		/// </summary>
		/// <param name="_roundIndex">Index of the round to add Match to.</param>
		/// <returns>False if index is invalid, True if successful.</returns>
		bool AddMatch(int _roundIndex);

		/// <summary>
		/// Add an existing Match to specified round.
		/// </summary>
		/// <param name="_roundIndex">Index of the round to add Match to.</param>
		/// <param name="_m">Match to add.</param>
		/// <returns>False if index is invalid or Match is a duplicate,
		/// True if successful.</returns>
		bool AddMatch(int _roundIndex, IMatch _m);

		/// <summary>
		/// Record a "game" win.
		/// If applicable, will advance the winner (and loser).
		/// </summary>
		/// <param name="_roundIndex">Round of the Match.</param>
		/// <param name="_matchIndex">Index with round of the Match.</param>
		/// <param name="_index">Index of winning Player (0 or 1).</param>
		void AddWin(int _roundIndex, int _matchIndex, int _index);

		/// <summary>
		/// Record a "game" win.
		/// If applicable, will advance the winner (and loser).
		/// </summary>
		/// <param name="_match">Match object.</param>
		/// <param name="_index">Index of winning Player (0 or 1).</param>
		void AddWin(IMatch _match, int _index);
		#endregion
	}
}
