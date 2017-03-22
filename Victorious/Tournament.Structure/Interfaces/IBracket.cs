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
		BracketTypeModel.BracketType BracketType { get; }
		bool IsFinished { get; }
		List<IPlayer> Players { get; }
		int[] Rankings { get; }

		/// <summary>
		/// NULL if not applicable.
		/// </summary>
		//Dictionary<int, IMatch> Matches { get; }

		int NumberOfRounds { get; }

		/// <summary>
		/// NULL if not applicable.
		/// </summary>
		//Dictionary<int, IMatch> LowerMatches { get; }

		int NumberOfLowerRounds { get; }

		/// <summary>
		/// NULL if not applicable.
		/// </summary>
		IMatch GrandFinal { get; }

		/// <summary>
		/// TOTAL number of matches.
		/// Includes Lower Bracket & Grand Final.
		/// </summary>
		int NumberOfMatches { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Generates the bracket (rounds & matches).
		/// </summary>
		/// <param name="_winsPerMatch">"Games" needed to win each match.</param>
		void CreateBracket(ushort _winsPerMatch = 1);

		/// <summary>
		/// Record one win for the specified match.
		/// Advances Player if the Match is over.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match.</param>
		/// <param name="_slot">Winning player's slot: Defender or Challenger</param>
		void AddWin(int _matchNumber, PlayerSlot _slot);

		/// <summary>
		/// Remove one win for the specified match.
		/// Resets any affected "future" matches.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match.</param>
		/// <param name="_slot">Player slot: Defender or Challenger.</param>
		void SubtractWin(int _matchNumber, PlayerSlot _slot);

		/// <summary>
		/// Reset score for the specified match.
		/// Resets any affected "future" matches.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match.</param>
		void ResetMatchScore(int _matchNumber);

		/// <summary>
		/// Gets the number of Players in the Bracket.
		/// </summary>
		/// <returns>Number of Players.</returns>
		int NumberOfPlayers();

		/// <summary>
		/// Add a Player.
		/// </summary>
		/// <param name="_player">Player-type object to add.</param>
		void AddPlayer(IPlayer _player);

		/// <summary>
		/// Replaces a player/slot in the playerlist
		/// with the new indicated Player.
		/// </summary>
		/// <param name="_player">Player-type object to add.</param>
		/// <param name="_index">Slot in list to replace.</param>
		void ReplacePlayer(IPlayer _player, int _index);

		/// <summary>
		/// Remove a Player from the bracket.
		/// </summary>
		/// <param name="_player">Player-type object to remove.</param>
		void RemovePlayer(IPlayer _player);

		/// <summary>
		/// Clears the bracket's player list.
		/// </summary>
		void ResetPlayers();

		/// <summary>
		/// Get all Matches in specified round.
		/// (_index=1 returns FIRST round)
		/// </summary>
		/// <param name="_round">Round number to get.</param>
		/// <returns>List of Matches in the round.</returns>
		List<IMatch> GetRound(int _round);

		/// <summary>
		/// Get all Matches in specified lower bracket round.
		/// (_index=1 returns FIRST round)
		/// </summary>
		/// <param name="_round">Round number to get.</param>
		/// <returns>List of Matches in the round.</returns>
		List<IMatch> GetLowerRound(int _round);

		/// <summary>
		/// Get a specific Match object from:
		/// Upper & Lower Brackets and Grand Final.
		/// </summary>
		/// <param name="_matchNumber">Match Number of the desired Match.</param>
		/// <returns>Match object.</returns>
		IMatch GetMatch(int _matchNumber);

		/// <summary>
		/// Clears all Matches and rounds in the bracket.
		/// </summary>
		//void ResetBracket();
#endregion
	}
}
