using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		bool IsReady { get; }
		bool IsFinished { get; }
		ushort WinsNeeded { get; }
		IPlayer[] Players { get; }
		PlayerSlot WinnerSlot { get; }
		ushort[] Score { get; }
		int RoundIndex { get; }
		int MatchIndex { get; }
		int MatchNumber { get; }

		/// <summary>
		/// Match Numbers of Matches sending winners to this Match.
		/// (list's size must be 0-2)
		/// </summary>
		List<int> PreviousMatchNumbers { get; }

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
#if false
		/// <summary>
		/// Index of first (top) Player:
		/// refers to Bracket's player list.
		/// </summary>
		/// <returns>-1 if no Player is yet assigned to this slot.</returns>
		int DefenderIndex();

		/// <summary>
		/// Index of second (bottom) Player:
		/// refers to Bracket's player list.
		/// </summary>
		/// <returns>-1 if no Player is yet assigned to this slot.</returns>
		int ChallengerIndex();
#endif
		/// <summary>
		/// Assigns a Player to this Match.
		/// If _slot is unspecified, player will be
		/// assigned to first open slot.
		/// </summary>
		/// <param name="_player">Player-type object to add.</param>
		/// <param name="_slot">Slot to assign player to:
		/// Defender or Challenger.</param>
		void AddPlayer(IPlayer _player, PlayerSlot _slot = PlayerSlot.unspecified);

		/// <summary>
		/// Remove the specified Player from the Match,
		/// (also resets Match's score)
		/// </summary>
		/// <param name="_playerId">ID of Player to remove.</param>
		void RemovePlayer(int _playerId);

		/// <summary>
		/// Clears both Players from the Match.
		/// (also resets Match's score)
		/// </summary>
		void ResetPlayers();

		/// <summary>
		/// Record one win for the specified player slot.
		/// </summary>
		/// <param name="_slot">Winner's slot: Defender or Challenger.</param>
		void AddWin(PlayerSlot _slot);

		/// <summary>
		/// Subtract one win from specified player slot.
		/// </summary>
		/// <param name="_slot">Player slot: Defender or Challenger.</param>
		void SubtractWin(PlayerSlot _slot);

		/// <summary>
		/// Resets Match score to 0-0.
		/// </summary>
		void ResetScore();

		/// <summary>
		/// Sets amount of wins needed to advance.
		/// </summary>
		/// <param name="_wins">Wins needed.</param>
		void SetWinsNeeded(ushort _wins);

		/// <summary>
		/// Sets round index for Match.
		/// Will not modify an existing value.
		/// </summary>
		/// <param name="_index">Round index (0=final round).</param>
		void SetRoundIndex(int _index);

		/// <summary>
		/// Sets match index inside the round.
		/// Will not modify an existing value.
		/// </summary>
		/// <param name="_index">Match index (0=first match).</param>
		void SetMatchIndex(int _index);

		/// <summary>
		/// Sets an identification number for the Match.
		/// Will not modify an existing value.
		/// </summary>
		/// <param name="_number">Match Number.</param>
		void SetMatchNumber(int _number);

		/// <summary>
		/// Add a match to PreviousMatchNumbers.
		/// Will not expand list beyond Count=2.
		/// </summary>
		/// <param name="_number">Number of match to add.</param>
		void AddPreviousMatchNumber(int _number);

		/// <summary>
		/// Sets match that winner will advance to.
		/// Will not modify an existing value.
		/// </summary>
		/// <param name="_number">Number of match to set.</param>
		void SetNextMatchNumber(int _number);

		/// <summary>
		/// Sets lower bracket match for loser to advance to.
		/// Will not modify an existing value.
		/// (only applicable for Double Elimination)
		/// </summary>
		/// <param name="_number">Number of match to set.</param>
		void SetNextLoserMatchNumber(int _number);
#endregion
	}
}
