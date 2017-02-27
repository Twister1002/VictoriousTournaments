using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public interface IBracket
	{
		#region Variables & Properties
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
		/// Sets Matches with current info from DB.
		/// </summary>
		/// <param name="_matchModels">ICollection of DB Matches.</param>
		void UpdateCurrentMatches(ICollection<MatchModel> _matchModels);

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

		/// <summary>
		/// Get all matches in the requested round.
		/// </summary>
		/// <param name="_index">Round index (0 is the final round).</param>
		/// <returns>List of IMatches.</returns>
		List<IMatch> GetRound(int _index);

		/// <summary>
		/// Get the requested match.
		/// </summary>
		/// <param name="_roundIndex">Round index (0 is the final round).</param>
		/// <param name="_index">Match index inside the round.</param>
		/// <returns>Requested IMatch.</returns>
		IMatch GetMatch(int _roundIndex, int _index);

		/// <summary>
		/// Add a Player to the Players list.
		/// </summary>
		/// <param name="_p">Player to add.</param>
		void AddPlayer(IPlayer _p);

		/// <summary>
		/// Add a new round to the Bracket.
		/// </summary>
		void AddRound();

		/// <summary>
		/// Add an existing Match to specified round.
		/// </summary>
		/// <param name="_roundIndex">Index of the round to add Match to.</param>
		/// <param name="_m">Match to add.</param>
		void AddMatch(int _roundIndex, IMatch _m);
		#endregion
	}
}
