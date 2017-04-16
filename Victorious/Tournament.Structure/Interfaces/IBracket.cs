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

		/// <summary>
		/// Is Bracket creation finalized; ready to play?
		/// </summary>
		bool IsFinalized { get; }

		/// <summary>
		/// Is Bracket play finished & winner determined?
		/// </summary>
		bool IsFinished { get; }

		/// <summary>
		/// Players list, ordered by seed.
		/// </summary>
		List<IPlayer> Players { get; }

		/// <summary>
		/// Ordered Rankings list, containing:
		/// Player ID, name, score, and rank.
		/// </summary>
		List<IPlayerScore> Rankings { get; }

		/// <summary>
		/// Limit on the number of rounds;
		/// for RoundRobin-type brackets.
		/// </summary>
		int MaxRounds { get; set; }

		/// <summary>
		/// Number of rounds in the upper bracket.
		/// (if "upper" is N/A, this is total rounds)
		/// </summary>
		int NumberOfRounds { get; }

		/// <summary>
		/// Returns 0 if there is no lower bracket.
		/// </summary>
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
		/// <param name="_gamesPerMatch">Max games played each match</param>
		void CreateBracket(int _gamesPerMatch = 1);

		void RestoreMatch(int _matchNumber, MatchModel _model);

		/// <summary>
		/// Add/record a finished Game.
		/// </summary>
		/// <param name="_matchNumber">Match to contain this Game</param>
		/// <param name="_defenderScore">Score for Defender-slot Player</param>
		/// <param name="_challengerScore">Score for Challenger-slot Player</param>
		/// <param name="_winnerSlot">Slot of winner (Defender/Challenger)</param>
		/// <returns>Model of the new Game</returns>
		GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot);
		[System.Obsolete("use AddGame(int, int, int, PlayerSlot) isntead", false)]
		GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore);

		/// <summary>
		/// Delete/un-record a Match's most recent Game.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to modify</param>
		void RemoveLastGame(int _matchNumber);

		/// <summary>
		/// Reset score for the specified match.
		/// Resets any affected "future" matches.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match</param>
		void ResetMatchScore(int _matchNumber);

		/// <summary>
		/// Gets the number of Players in the Bracket.
		/// </summary>
		/// <returns>Count of Players</returns>
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
		/// Swaps two Players' seeds/positions.
		/// (Deletes all Matches)
		/// </summary>
		/// <param name="_index1">P1's index (0-indexed)</param>
		/// <param name="_index2">P2's index (0-indexed)</param>
		void SwapPlayers(int _index1, int _index2);

		/// <summary>
		/// Moves a Player's seed/position in the playerlist,
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
		/// <param name="_playerId">ID of Player to remove</param>
		void RemovePlayer(int _playerId);

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

		/// <summary>
		/// Set the max number of Games PER MATCH for one round.
		/// </summary>
		/// <param name="_roundIndex">Round of Matches to modify</param>
		/// <param name="_maxGamesPerMatch">How many Games each Match may last</param>
		void SetMaxGamesForWholeRound(int _round, int _maxGamesPerMatch);

		/// <summary>
		/// Set the max number of Games PER MATCH for one lower round.
		/// </summary>
		/// <param name="_roundIndex">Round (lower bracket) of Matches to modify</param>
		/// <param name="_maxGamesPerMatch">How many Games each Match may last</param>
		void SetMaxGamesForWholeLowerRound(int _round, int _maxGamesPerMatch);

		/// <summary>
		/// Resets EVERY Match to a pre-play state (no games played).
		/// </summary>
		void ResetMatches();
#endregion
	}
}
