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
		public bool IsReady
		{ get; private set; }
		public bool IsFinished
		{ get; private set; }
		public ushort WinsNeeded
		{ get; private set; }
		public IPlayer[] Players
		{ get; private set; }
		public PlayerSlot WinnerSlot
		{ get; private set; }
		public ushort[] Score
		{ get; private set; }
		public int RoundIndex
		{ get; private set; }
		public int MatchIndex
		{ get; private set; }
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
		public Match(bool _isReady, bool _isFinished, ushort _winsNeeded, IPlayer[] _players, PlayerSlot _winnerSlot, ushort[] _score, int _roundIndex, int _matchIndex, int _matchNumber, List<int> _prevMatchNumbers, int _nextMatchNumber, int _nextLoserMatchNumber)
		{
			if (null == _players
				|| null == _score
				|| null == _prevMatchNumbers)
			{
				throw new NullReferenceException();
			}

			//IsReady = _isReady;
			//IsFinished = _isFinished;
			WinsNeeded = _winsNeeded;
			Players = _players;
			IsReady = (null != Players[0] && null != Players[1])
				? true : false;
			WinnerSlot = _winnerSlot;
			IsFinished = (PlayerSlot.Defender == WinnerSlot || PlayerSlot.Challenger == WinnerSlot)
				? true : false;
			Score = _score;
			RoundIndex = _roundIndex;
			MatchIndex = _matchIndex;
			MatchNumber = _matchNumber;
			PreviousMatchNumbers = _prevMatchNumbers;
			NextMatchNumber = _nextMatchNumber;
			NextLoserMatchNumber = _nextLoserMatchNumber;
		}
		public Match()
			: this(false, false, 1, new IPlayer[2] { null, null }, PlayerSlot.unspecified, new ushort[2] { 0, 0 }, -1, -1, -1, new List<int>(), -1, -1)
		{ }

		public Match(MatchModel _m) // , List<IPlayer> _playerList)
		{
			if (null == _m
				//|| null == _playerList
				|| null == _m.ChallengerID
				|| null == _m.DefenderID
				//|| null == _m.TournamentID
				|| null == _m.WinnerID
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

			Players = new IPlayer[2];
			Players[0] = (null == _m.Defender)
				? null : new User(_m.Defender);
			Players[1] = (null == _m.Challenger)
				? null : new User(_m.Challenger);

			IsReady = (null == Players[0] || null == Players[1])
				? false : true;

			Score = new ushort[2] { 0, 0 };
			Score[0] = (ushort)(_m.DefenderScore);
			Score[1] = (ushort)(_m.ChallengerScore);
			if (Score[0] > WinsNeeded || Score[1] > WinsNeeded)
			{
				throw new ArgumentOutOfRangeException();
			}
			WinnerSlot = PlayerSlot.unspecified;
			if (Score[(int)PlayerSlot.Defender] == WinsNeeded)
			{
				WinnerSlot = PlayerSlot.Defender;
			}
			else if (Score[(int)PlayerSlot.Challenger] == WinsNeeded)
			{
				WinnerSlot = PlayerSlot.Challenger;
			}
			IsFinished = (PlayerSlot.unspecified == WinnerSlot)
				? false : true;

			RoundIndex = (int)(_m.RoundIndex);
			MatchIndex = (int)(_m.MatchIndex);
			MatchNumber = _m.MatchNumber;

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
#if false
		public int DefenderIndex()
		{
			return PlayerIndexes[0];
		}
		public int ChallengerIndex()
		{
			return PlayerIndexes[1];
		}
#endif
		public void AddPlayer(IPlayer _player, PlayerSlot _slot = PlayerSlot.unspecified)
		{
			if (_slot != PlayerSlot.unspecified &&
				_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new IndexOutOfRangeException();
			}
			if ((null != Players[0] && Players[0].Id == _player.Id) ||
				(null != Players[1] && Players[1].Id == _player.Id))
			{
				throw new DuplicateObjectException();
			}

			for (int i = 0; i < 2; ++i)
			{
				if ((int)_slot == i || _slot == PlayerSlot.unspecified)
				{
					if (null == Players[i])
					{
						Players[i] = _player;

						if (null != Players[0] && null != Players[1])
						{
							IsReady = true;
						}
						return;
					}
				}
			}

			throw new SlotFullException();
		}
		public void ReplacePlayer(IPlayer _newPlayer, int _oldPlayerId)
		{
			if (null == _newPlayer)
			{
				throw new NullReferenceException();
			}

			if (null != Players[(int)PlayerSlot.Defender] &&
				_oldPlayerId == Players[(int)PlayerSlot.Defender].Id)
			{
				Players[(int)PlayerSlot.Defender] = _newPlayer;
			}
			else if (null != Players[(int)PlayerSlot.Challenger] &&
				_oldPlayerId == Players[(int)PlayerSlot.Challenger].Id)
			{
				Players[(int)PlayerSlot.Challenger] = _newPlayer;
			}
			else
			{
				throw new KeyNotFoundException();
			}
		}
		public void RemovePlayer(int _playerId)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (null != Players[i] && Players[i].Id == _playerId)
				{
					Players[i] = null;

					ResetScore();
					IsReady = false;
					return;
				}
			}

			throw new KeyNotFoundException();
		}
		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new IPlayer[2];
			}
			Players[0] = Players[1] = null;

			ResetScore();
			IsReady = false;
		}

		public void AddWin(PlayerSlot _slot)
		{
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new IndexOutOfRangeException();
			}
			if (IsFinished || !IsReady)
			{
				throw new InactiveMatchException();
			}

			Score[(int)_slot] += 1;
			if (Score[(int)_slot] >= WinsNeeded)
			{
				WinnerSlot = _slot;
				IsFinished = true;
			}
		}
		public void SubtractWin(PlayerSlot _slot)
		{
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new IndexOutOfRangeException();
			}
			if (!IsReady)
			{
				throw new InactiveMatchException();
			}
			if (Score[(int)_slot] <= 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			if (Score[(int)_slot] == WinsNeeded)
			{
				IsFinished = false;
				WinnerSlot = PlayerSlot.unspecified;
			}
			Score[(int)_slot] -= 1;
		}
		public void ResetScore()
		{
			if (null == Score)
			{
				Score = new ushort[2];
			}

			IsFinished = false;
			WinnerSlot = PlayerSlot.unspecified;
			Score[0] = Score[1] = 0;
		}

		public void SetWinsNeeded(ushort _wins)
		{
			if (IsFinished)
			{
				throw new InactiveMatchException();
			}
			if (_wins < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			WinsNeeded = _wins;
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
