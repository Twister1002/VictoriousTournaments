using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public enum PlayerSlot
	{
		unspecified = -1,
		Defender = 0,
		Challenger = 1
	};
	
	public interface IMatch
	{
		#region Variables & Properties
		int Id { get; }

		[System.Obsolete("use .GetModel()", false)]
		MatchModel Model { get; }

		/// <summary>
		/// Is the Match set/ready to play?
		/// </summary>
		bool IsReady { get; }

		/// <summary>
		/// Is the Match finished/won?
		/// </summary>
		bool IsFinished { get; }

		int MaxGames { get; }
		IPlayer[] Players { get; }

		/// <summary>
		/// Slot of winning player:
		/// Defender, Challenger, or unspecified.
		/// </summary>
		PlayerSlot WinnerSlot { get; }

		/// <summary>
		/// List of completed/recorded Games.
		/// </summary>
		List<IGame> Games { get; }

		/// <summary>
		/// 2-sized Array, indexes correspond to Players array.
		/// </summary>
		int[] Score { get; }

		/// <summary>
		/// 1-indexed
		/// </summary>
		int RoundIndex { get; }

		/// <summary>
		/// 1-indexed
		/// </summary>
		int MatchIndex { get; }

		/// <summary>
		/// First Match = 1
		/// </summary>
		int MatchNumber { get; }

		/// <summary>
		/// Match Numbers of Matches sending winners to this Match.
		/// 2-sized Array, default values are -1.
		/// Indexes correspond to Players array.
		/// </summary>
		int[] PreviousMatchNumbers { get; }

		/// <summary>
		/// Number of Match this winner is sent to.
		/// </summary>
		int NextMatchNumber { get; }

		/// <summary>
		/// Number of Match this loser is sent to.
		/// (-1 if not applicable)
		/// </summary>
		int NextLoserMatchNumber { get; }
		#endregion

		#region Methods
		[System.Obsolete("use .GetModel()", false)]
		MatchModel GetModel(int _matchId);

		/// <summary>
		/// Create a Model for this Match.
		/// </summary>
		/// <returns>MatchModel-type object</returns>
		MatchModel GetModel();

		/// <summary>
		/// Assigns a Player to this Match.
		/// If slot is unspecified, player will be
		/// assigned to first open slot.
		/// </summary>
		/// <param name="_player">Player-type object to add</param>
		/// <param name="_slot">Slot to assign player to</param>
		void AddPlayer(IPlayer _player, PlayerSlot _slot = PlayerSlot.unspecified);

		/// <summary>
		/// Replace a Player in this Match.
		/// </summary>
		/// <param name="_newPlayer">New Player object to add</param>
		/// <param name="_oldPlayerId">ID of Player to replace</param>
		void ReplacePlayer(IPlayer _newPlayer, int _oldPlayerId);

		/// <summary>
		/// Remove the specified Player from the Match.
		/// (Resets Match's score)
		/// </summary>
		/// <param name="_playerId">ID of Player to remove</param>
		void RemovePlayer(int _playerId);

		/// <summary>
		/// Clears both Players from the Match.
		/// (Resets Match's score)
		/// </summary>
		void ResetPlayers();

		/// <summary>
		/// Add/record a finished Game.
		/// </summary>
		/// <param name="_game">Game-type object to add</param>
		void AddGame(IGame _game);

		/// <summary>
		/// Delete/un-record the most recent Game.
		/// </summary>
		/// <returns>IGame that was removed</returns>
		IGame RemoveLastGame();

		[System.Obsolete("use AddGame(IGame) instead", false)]
		void AddWin(PlayerSlot _slot);
		[System.Obsolete("use RemoveLastGame() instead", false)]
		void SubtractWin(PlayerSlot _slot);

		/// <summary>
		/// Resets Match score to 0-0.
		/// </summary>
		void ResetScore();

		/// <summary>
		/// Set the max number of Games to play this Match.
		/// </summary>
		/// <param name="_numberOfGames">How many games this match may last</param>
		void SetMaxGames(int _numberOfGames);

		/// <summary>
		/// Sets a NEW round index for Match.
		/// (Will not modify an existing value)
		/// </summary>
		/// <param name="_index">Round index (1-indexed)</param>
		void SetRoundIndex(int _index);

		/// <summary>
		/// Sets a NEW match index inside the round.
		/// (Will not modify an existing value)
		/// </summary>
		/// <param name="_index">Match index (1-indexed)</param>
		void SetMatchIndex(int _index);

		/// <summary>
		/// Sets a NEW identification number for the Match.
		/// (Will not modify an existing value)
		/// </summary>
		/// <param name="_number">Match Number (minimum 1)</param>
		void SetMatchNumber(int _number);

		/// <summary>
		/// Add a match to PreviousMatchNumbers.
		/// (Will not add more than 2)
		/// </summary>
		/// <param name="_number">Number of Match to add</param>
		void AddPreviousMatchNumber(int _number, PlayerSlot _slot = PlayerSlot.unspecified);

		/// <summary>
		/// Sets match that winner will advance to.
		/// (Will not modify an existing value)
		/// </summary>
		/// <param name="_number">Number of match to set</param>
		void SetNextMatchNumber(int _number);

		/// <summary>
		/// Sets lower bracket match for loser to advance to.
		/// (Will not modify an existing value)
		/// </summary>
		/// <param name="_number">Number of match to set</param>
		void SetNextLoserMatchNumber(int _number);
#endregion
	}
}
