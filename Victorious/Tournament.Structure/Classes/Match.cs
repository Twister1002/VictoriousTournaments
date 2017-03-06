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
		private int[] PlayerIndexes
		{ get; set; }
		public ushort[] Score
		{ get; private set; }
		private int RoundIndex
		{ get; set; }
		private int MatchIndex
		{ get; set; }
		public int MatchNumber
		{ get; private set; }
		public List<int> PreviousMatchNumbers
		{ get; private set; }
		public int NextMatchNumber
		{ get; private set; }
		public int NextLoserMatchNumber
		{ get; private set; }
		#endregion

		#region Ctors
		public Match(ushort _winsNeeded, int[] _playerIndexes, ushort[] _score, int _roundIndex, int _matchIndex, int _matchNumber, List<int> _prevMatchNumbers, int _nextMatchNumber, int _nextLoserMatchNumber)
		{
			if (null == _playerIndexes
				|| null == _score
				|| null == _prevMatchNumbers)
			{
				throw new NullReferenceException();
			}

			WinsNeeded = _winsNeeded;
			PlayerIndexes = _playerIndexes;
			Score = _score;
			RoundIndex = _roundIndex;
			MatchIndex = _matchIndex;
			MatchNumber = _matchNumber;
			PreviousMatchNumbers = _prevMatchNumbers;
			NextMatchNumber = _nextMatchNumber;
			NextLoserMatchNumber = _nextLoserMatchNumber;
		}
		public Match()
			: this(1, new int[2] { -1, -1 }, new ushort[2] { 0, 0 }, -1, -1, -1, new List<int>(), -1, -1)
		{ }

		public Match(MatchModel _m, List<IPlayer> _playerList)
		{
			if (null == _m
				|| null == _playerList
				|| null == _m.ChallengerID
				|| null == _m.DefenderID
				//|| null == _m.TournamentID
				//|| null == _m.WinnerID
				|| null == _m.ChallengerScore
				|| null == _m.DefenderScore
				|| null == _m.RoundIndex
				|| null == _m.Challenger
				|| null == _m.Defender
				//|| null == _m.Tournament
				|| null == _m.WinsNeeded
				|| null == _m.MatchIndex
				//|| null == _m.MatchNumber
				|| null == _m.NextMatchNumber
				|| null == _m.NextLoserMatchNumber)
			{
				throw new NullReferenceException();
			}

			WinsNeeded = (ushort)(_m.WinsNeeded);

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

			RoundIndex = (int)(_m.RoundIndex);
			MatchIndex = (int)(_m.MatchIndex);
			MatchNumber = (int)(_m.MatchNumber);

			PreviousMatchNumbers = new List<int>();
			if (null != _m.PrevDefenderMatchNumber)
			{
				PreviousMatchNumbers.Add((int)(_m.PrevDefenderMatchNumber));
			}
			if (null != _m.PrevChallengerMatchNumber)
			{
				PreviousMatchNumbers.Add((int)(_m.PrevChallengerMatchNumber));
			}

			NextMatchNumber = (int)(_m.NextMatchNumber);
			NextLoserMatchNumber = (int)(_m.NextLoserMatchNumber);
		}
		#endregion

		#region Public Methods
		public int DefenderIndex()
		{
			return PlayerIndexes[0];
		}
		public int ChallengerIndex()
		{
			return PlayerIndexes[1];
		}
		public void AddPlayer(int _playerIndex, PlayerSlot _slot = PlayerSlot.unspecified)
		{
			if (_slot != PlayerSlot.unspecified &&
				_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new IndexOutOfRangeException();
			}
			if (PlayerIndexes[0] == _playerIndex ||
				PlayerIndexes[1] == _playerIndex)
			{
				throw new DuplicateObjectException();
			}

			for (int i = 0; i < 2; ++i)
			{
				if ((int)_slot == i ||
					_slot == PlayerSlot.unspecified)
				{
					if (PlayerIndexes[i] < 0)
					{
						PlayerIndexes[i] = _playerIndex;
						return;
					}
				}
			}

			throw new SlotFullException();
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
		public void ResetPlayers()
		{
			if (null == PlayerIndexes)
			{
				PlayerIndexes = new int[2];
			}
			PlayerIndexes[0] = PlayerIndexes[1] = -1;
			ResetScore();
		}

		public void AddWin(PlayerSlot _slot)
		{
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = 0; i < 2; ++i)
			{
				if (Score[i] >= WinsNeeded || PlayerIndexes[i] == -1)
				{
					throw new InactiveMatchException();
				}
			}

			Score[(int)_slot] += 1;
		}
		public void SubtractWin(PlayerSlot _slot)
		{
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new IndexOutOfRangeException();
			}
			if (Score[(int)_slot] <= 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			
			Score[(int)_slot] -= 1;
		}
		public void ResetScore()
		{
			if (null == Score)
			{
				Score = new ushort[2];
			}
			Score[0] = Score[1] = 0;
		}

		public void SetRoundIndex(int _index)
		{
			if (RoundIndex > -1)
			{
				throw new AlreadyAssignedException();
			}
			RoundIndex = _index;
		}
		public void SetMatchIndex(int _index)
		{
			if (MatchIndex > -1)
			{
				throw new AlreadyAssignedException();
			}
			MatchIndex = _index;
		}
		public void SetMatchNumber(int _number)
		{
			if (MatchNumber > -1)
			{
				throw new AlreadyAssignedException();
			}
			MatchNumber = _number;
		}
		public void AddPreviousMatchNumber(int _number)
		{
			if (PreviousMatchNumbers.Count >= 2)
			{
				throw new AlreadyAssignedException();
			}
			PreviousMatchNumbers.Add(_number);
		}
		public void SetNextMatchNumber(int _number)
		{
			if (NextMatchNumber > -1)
			{
				throw new AlreadyAssignedException();
			}
			NextMatchNumber = _number;
		}
		public void SetNextLoserMatchNumber(int _number)
		{
			if (NextLoserMatchNumber > -1)
			{
				throw new AlreadyAssignedException();
			}
			NextLoserMatchNumber = _number;
		}
		#endregion

		#region Private Methods
		#endregion
	}
}
