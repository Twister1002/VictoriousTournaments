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
		List<IPlayerScore> Rankings { get; }
		int NumberOfRounds { get; }
		int NumberOfLowerRounds { get; }

		/// <summary>
		/// NULL if not applicable.
		/// </summary>
		IMatch GrandFinal { get; }

		/// <summary>
		/// TOTAL number of matches.
		/// Includes Lower Bracket & Grand Final, when available.
		/// </summary>
		int NumberOfMatches { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Generates the bracket (rounds & matches).
		/// </summary>
		/// <param name="_winsPerMatch">"Games" needed to win each match</param>
		void CreateBracket(ushort _winsPerMatch = 1);

		/// <summary>
		/// Record one win for the specified match.
		/// Advances Player if the Match is over.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match</param>
		/// <param name="_slot">Winning player's slot: Defender or Challenger</param>
		void AddWin(int _matchNumber, PlayerSlot _slot);

		/// <summary>
		/// Remove one win for the specified match.
		/// Resets any affected "future" matches.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match</param>
		/// <param name="_slot">Player slot: Defender or Challenger</param>
		void SubtractWin(int _matchNumber, PlayerSlot _slot);

		/// <summary>
		/// Reset score for the specified match.
		/// Resets any affected "future" matches.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match</param>
		void ResetMatchScore(int _matchNumber);

		/// <summary>
		/// Gets the number of Players in the Bracket.
		/// </summary>
		/// <returns>Number of Players</returns>
		int NumberOfPlayers();

		/// <summary>
		/// Gets a Player's seed in this Bracket.
		/// </summary>
		/// <param name="_playerId">ID of specified Player</param>
		/// <returns>Seed of specified Player</returns>
		int GetPlayerSeed(int _playerId);

		/// <summary>
		/// Randomizes seed values of Player's in this Bracket.
		/// (Deletes all Matches)
		/// </summary>
		void RandomizeSeeds();

		/// <summary>
		/// Replace this bracket's Players (if any)
		/// with the passed-in list.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_players">List of Player-type objects to store</param>
		void SetNewPlayerlist(List<IPlayer> _players);

		/// <summary>
		/// Add a Player to the Bracket.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_player">Player-type object to add</param>
		void AddPlayer(IPlayer _player);

		/// <summary>
		/// Replaces a player/slot in the playerlist
		/// with the new indicated Player.
		/// Also replaces Player in all Matches.
		/// </summary>
		/// <param name="_player">Player-type object to add</param>
		/// <param name="_index">Slot in list to replace (0-indexed)</param>
		void ReplacePlayer(IPlayer _player, int _index);

		/// <summary>
		/// Swaps two Players' positions.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_index1">P1's index (0-indexed)</param>
		/// <param name="_index2">P2's index (0-indexed)</param>
		void SwapPlayers(int _index1, int _index2);

		/// <summary>
		/// Moves a Player's position in the playerlist,
		/// adjusting all other Players.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_oldIndex">Player's current index (0-indexed)</param>
		/// <param name="_newIndex">New index (0-indexed)</param>
		void ReinsertPlayer(int _oldIndex, int _newIndex);

		/// <summary>
		/// Remove a Player from the bracket.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_player">Player-type object to remove</param>
		void RemovePlayer(IPlayer _player);

		/// <summary>
		/// Clears the bracket's player list.
		/// (Deletes all Matches)
		/// </summary>
		void ResetPlayers();

		/// <summary>
		/// Get all Matches in specified round.
		/// (_index=1 returns FIRST round)
		/// </summary>
		/// <param name="_round">Round number to get (1-indexed)</param>
		/// <returns>List of Matches in the round</returns>
		List<IMatch> GetRound(int _round);

		/// <summary>
		/// Get all Matches in specified lower bracket round.
		/// (_index=1 returns FIRST round)
		/// </summary>
		/// <param name="_round">Round number to get (1-indexed)</param>
		/// <returns>List of Matches in the round</returns>
		List<IMatch> GetLowerRound(int _round);

		/// <summary>
		/// Get a specific Match object from:
		/// Upper & Lower Brackets and Grand Final.
		/// </summary>
		/// <param name="_matchNumber">Match Number of the desired Match</param>
		/// <returns>Specified Match object</returns>
		IMatch GetMatch(int _matchNumber);
#endregion
	}
}
