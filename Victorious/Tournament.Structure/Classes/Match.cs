using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class Match : IMatch
	{
		#region Variables & Properties
		public ushort WinsNeeded
		{ get; set; }
		public int[] PlayerIndexes
		{ get; set; }
		public ushort[] Score
		{ get; set; }
		public int RoundNumber
		{ get; set; }
		public int MatchIndex
		{ get; set; }
		public List<int> PrevMatchIndexes
		{ get; set; }
		public int NextMatchIndex
		{ get; set; }
		public int NextLoserMatchIndex
		{ get; set; }
		#endregion

		#region Ctors
		public Match()
			: this(1, new int[2] { -1, -1 }, new ushort[2] { 0, 0 }, -1, -1, new List<int>(), -1, -1)
		{ }
		public Match(ushort _winsNeeded, int[] _playerIndexes, ushort[] _score, int _roundNumber, int _matchIndex, List<int> _prevMatchIndexes, int _nextMatchIndex, int _nextLoserMatchIndex)
		{
			WinsNeeded = _winsNeeded;
			PlayerIndexes = _playerIndexes;
			Score = _score;
			RoundNumber = _roundNumber;
			MatchIndex = _matchIndex;
			PrevMatchIndexes = _prevMatchIndexes;
			NextMatchIndex = _nextMatchIndex;
			NextLoserMatchIndex = _nextLoserMatchIndex;
		}
		public Match(MatchModel _m, List<IPlayer> _playerList)
		{
			WinsNeeded = _m.WinsNeeded;

			PlayerIndexes = new int[2] { -1, -1 };
			for (int i = 0; i < _playerList.Count; ++i)
			{
				if (_m.Defender.UserID == _playerList[i].Id)
				{
					PlayerIndexes[0] = i;
				}
				else if (_m.Challenger.UserID == _playerList[i].Id)
				{
					PlayerIndexes[1] = i;
				}
			}

			Score = new ushort[2] { 0, 0 };
			Score[0] = (null == _m.DefenderScore)
				? (ushort)0 : (ushort)(_m.DefenderScore);
			Score[1] = (null == _m.ChallengerScore)
				? (ushort)0 : (ushort)(_m.ChallengerScore);

			RoundNumber = (null == _m.RoundNumber)
				? -1 : (int)(_m.RoundNumber);
			MatchIndex = _m.MatchIndex;

			PrevMatchIndexes = new List<int>();

			NextMatchIndex = (null == _m.NextMatchIndex)
				? -1 : (int)(_m.NextMatchIndex);
			NextLoserMatchIndex = (null == _m.NextLoserMatchIndex)
				? -1 : (int)(_m.NextLoserMatchIndex);
		}
		#endregion

		#region Public Methods
		public bool AddPlayer(int _playerIndex, int _index)
		{
			if (PlayerIndexes[_index] > -1)
			{
				return false;
			}
			PlayerIndexes[_index] = _playerIndex;
			return true;
		}
		public bool RemovePlayer(int _playerIndex)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (PlayerIndexes[i] == _playerIndex)
				{
					PlayerIndexes[i] = -1;
					return true;
				}
			}
			return false;
		}
		public void RemovePlayers()
		{
			PlayerIndexes[0] = PlayerIndexes[1] = -1;
		}
		public void AddWin(int _index)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (Score[i] >= WinsNeeded || PlayerIndexes[i] == -1)
				{
					throw new InactiveMatchException();
				}
			}

			Score[_index] += 1;
		}
		public bool AddPrevMatchIndex(int _i)
		{
			if (PrevMatchIndexes.Count >= 2)
			{
				return false;
			}
			PrevMatchIndexes.Add(_i);
			return true;
		}
		#endregion
	}
}
