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

	/// <summary>
	/// Interface for Matches.
	/// </summary>
	public interface IMatch
	{
		#region Variables & Properties
		ushort WinsNeeded { get; set; }
		//int[] PlayerIndexes { get; set; }
		ushort[] Score { get; }
		//int RoundIndex { get; set; }
		//int MatchIndex { get; set; }
		int MatchNumber { get; }
		List<int> PrevMatchNumbers { get; }
		int NextMatchNumber { get; }
		int NextLoserMatchNumber { get; }
		#endregion

		#region Methods
		int DefenderIndex();
		int ChallengerIndex();
		void AddPlayer(int _playerIndex, PlayerSlot _slot = PlayerSlot.unspecified);
		void RemovePlayer(int _playerIndex);
		void ResetPlayers();
		void AddWin(PlayerSlot _slot);
		void ResetScore();
		void SetRoundIndex(int _index);
		void SetMatchIndex(int _index);
		void SetMatchNumber(int _number);
		void AddPrevMatchNumber(int _number);
		void SetNextMatchNumber(int _number);
		void SetNextLoserMatchNumber(int _number);
		#endregion
	}
}
