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
			if (null == _playerIndexes
				|| null == _score
				|| null == _prevMatchIndexes)
			{
				throw new NullReferenceException();
			}

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
			if (null == _m
				|| null == _playerList
				|| null == _m.ChallengerID
				|| null == _m.DefenderID
				|| null == _m.TournamentID
				//|| null == _m.WinnerID
				|| null == _m.ChallengerScore
				|| null == _m.DefenderScore
				|| null == _m.RoundNumber
				|| null == _m.Challenger
				|| null == _m.Defender
				|| null == _m.Tournament
				|| null == _m.WinsNeeded
				|| null == _m.MatchIndex
				|| null == _m.NextMatchIndex
				|| null == _m.NextLoserMatchIndex)
			{
				throw new NullReferenceException();
			}

			WinsNeeded = _m.WinsNeeded;
			if (WinsNeeded < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			PlayerIndexes = new int[2] { -1, -1 };
			int p1id = (int)(_m.DefenderID);
			int p2id = (int)(_m.ChallengerID);
			if (p1id > -1 || p2id > -1)
			{
				// Find and set player indexes (from the PlayerList)
				for (int i = 0; i < _playerList.Count; ++i)
				{
					if (p1id == _playerList[i].Id)
					{
						PlayerIndexes[0] = i;
					}
					else if (p2id == _playerList[i].Id)
					{
						PlayerIndexes[1] = i;
					}
				}
			}

			Score = new ushort[2] { 0, 0 };
			Score[0] = (ushort)(_m.DefenderScore);
			Score[1] = (ushort)(_m.ChallengerScore);
			if (Score[0] > WinsNeeded || Score[1] > WinsNeeded)
			{
				throw new ArgumentOutOfRangeException();
			}

			RoundNumber = (int)(_m.RoundNumber);
			MatchIndex = _m.MatchIndex;

			PrevMatchIndexes = new List<int>();
			// still need to fetch this...

			NextMatchIndex = (int)(_m.NextMatchIndex);
			NextLoserMatchIndex = (int)(_m.NextLoserMatchIndex);
		}
		#endregion

		#region Public Methods
		public void AddPlayer(int _playerIndex, int _index)
		{
			if (_index < 0 || _index > 1)
			{
				throw new IndexOutOfRangeException();
			}
			if (PlayerIndexes[_index] > -1)
			{
				throw new Exception();
			}
			if (PlayerIndexes[0] == _playerIndex || PlayerIndexes[1] == _playerIndex)
			{
				throw new DuplicateObjectException();
			}

			PlayerIndexes[_index] = _playerIndex;
		}
		public void RemovePlayer(int _playerIndex)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (PlayerIndexes[i] == _playerIndex)
				{
					PlayerIndexes[i] = -1;
					return;
				}
			}

			throw new KeyNotFoundException();
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
		public void AddPrevMatchIndex(int _i)
		{
			if (PrevMatchIndexes.Count >= 2)
			{
				throw new ArgumentOutOfRangeException();
			}

			PrevMatchIndexes.Add(_i);
		}
		#endregion
	}
}
