using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	/// <summary>
	/// Interface for Matches.
	/// </summary>
	public interface IMatch
	{
		#region Variables & Properties
		/// <summary>
		/// Amount of "games" won to Win the Match.
		/// Default 1
		/// </summary>
		ushort WinsNeeded { get; set; }

		/// <summary>
		/// Two-element array: Indexes of both Players in Match.
		/// (References Players list in Bracket)
		/// Default [-1, -1]
		/// </summary>
		int[] PlayerIndexes { get; set; }

		/// <summary>
		/// Two-element array: Score of both Players.
		/// </summary>
		ushort[] Score { get; set; }

		/// <summary>
		/// Which Round of the Bracket this Match is in.
		/// "Reverse" index (Final match is 0)
		/// Default -1
		/// </summary>
		int RoundIndex { get; set; }

		/// <summary>
		/// Index of the Match, within the Round.
		/// Default -1
		/// </summary>
		int MatchIndex { get; set; }

		/// <summary>
		/// Number of the Match.
		/// First is 1, counting up to the Final.
		/// </summary>
		int MatchNumber { get; set; }

		/// <summary>
		/// List of Match numbers,
		/// where Players enter this Match from.
		/// (shouldn't be larger than 2)
		/// </summary>
		List<int> PrevMatchNumbers { get; set; }

		/// <summary>
		/// Match number where this Match's winner goes.
		/// -1 if not applicable
		/// </summary>
		int NextMatchNumber { get; set; }

		/// <summary>
		/// Match number in Lower Bracket (if present)
		/// where this Match's loser goes.
		/// -1 if not applicable
		/// </summary>
		int NextLoserMatchNumber { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Add _playerIndex to PlayerIndexes[]
		/// </summary>
		/// <param name="_playerIndex">Index of IPlayer object,
		/// references Players list in Bracket.</param>
		/// <param name="_index">Slot to add to, in PlayerIndexes[]</param>
		void AddPlayer(int _playerIndex, int _index);

		/// <summary>
		/// Removes the passed-in PlayerIndex from the Match, if present.
		/// </summary>
		/// <param name="_playerIndex">Index of IPlayer,
		/// references Players list in Bracket.</param>
		void RemovePlayer(int _playerIndex);

		/// <summary>
		/// Removes all Player Indexes from the Match.
		/// </summary>
		void RemovePlayers();

		/// <summary>
		/// Adds a "win" to the Player at _index.
		/// </summary>
		/// <param name="_index">Index of a Player in Match, either 0 or 1.</param>
		void AddWin(int _index);

		/// <summary>
		/// Adds a Match Number,
		/// that a winning Player will come from
		/// to enter this Match.
		/// </summary>
		/// <param name="_n">Previous Match's Number</param>
		void AddPrevMatchNumber(int _n);
		#endregion
	}
}
