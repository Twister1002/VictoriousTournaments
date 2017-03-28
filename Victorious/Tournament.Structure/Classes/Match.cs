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
#if false
		public Match(bool _isReady, bool _isFinished, ushort _winsNeeded, IPlayer[] _players, PlayerSlot _winnerSlot, ushort[] _score, int _roundIndex, int _matchIndex, int _matchNumber, List<int> _prevMatchNumbers, int _nextMatchNumber, int _nextLoserMatchNumber)
		{
			if (null == _players
				|| null == _score
				|| null == _prevMatchNumbers)
			{
				throw new NullReferenceException
					("There's a NULL problem with your Match constructor...");
			}

			Players = new IPlayer[2];
			if (_players[0] is User)
			{
				Players[0] = new User(_players[0] as User);
				Players[1] = new User(_players[1] as User);
			}
			else if (_players[0] is Team)
			{
				Players[0] = new Team(_players[0] as Team);
				Players[1] = new Team(_players[1] as Team);
			}

			//IsReady = _isReady;
			//IsFinished = _isFinished;
			WinsNeeded = _winsNeeded;
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
#endif
		public Match()
		{
			IsReady = false;
			IsFinished = false;
			WinsNeeded = 1;
			Players = new IPlayer[2] { null, null };
			WinnerSlot = PlayerSlot.unspecified;
			Score = new ushort[2] { 0, 0 };
			RoundIndex = -1;
			MatchIndex = -1;
			MatchNumber = -1;
			PreviousMatchNumbers = new List<int>();
			NextMatchNumber = -1;
			NextLoserMatchNumber = -1;
		}
		public Match(Match _match)
		{
			if (null == _match)
			{
				throw new ArgumentNullException("_match");
			}

			this.IsReady = _match.IsReady;
			this.IsFinished = _match.IsFinished;
			this.WinsNeeded = _match.WinsNeeded;
			this.WinnerSlot = _match.WinnerSlot;
			this.RoundIndex = _match.RoundIndex;
			this.MatchIndex = _match.MatchIndex;
			this.MatchNumber = _match.MatchNumber;
			this.NextMatchNumber = _match.NextMatchNumber;
			this.NextLoserMatchNumber = _match.NextLoserMatchNumber;

			this.Players = new IPlayer[2];
			this.Score = new ushort[2];
			for (int i = 0; i < 2; ++i)
			{
				if (_match.Players[i] is User)
				{
					this.Players[i] = new User(_match.Players[i] as User);
				}
				else if (_match.Players[i] is Team)
				{
					throw new NotImplementedException();
				}

				this.Score[i] = _match.Score[i];
			}

			this.PreviousMatchNumbers = new List<int>();
			foreach (int num in _match.PreviousMatchNumbers)
			{
				this.PreviousMatchNumbers.Add(num);
			}
		}

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
				throw new NullReferenceException
					("There's a NULL problem with the Match Model....");
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
				throw new ScoreException
					("Score cannot be higher than Wins Needed!");
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
		public MatchModel GetModel(int _matchId)
		{
			MatchModel model = new MatchModel();
			model.ChallengerID = Players[(int)PlayerSlot.Challenger].Id;
			model.DefenderID = Players[(int)PlayerSlot.Defender].Id;
			model.WinnerID = (PlayerSlot.unspecified == WinnerSlot)
				? (int)WinnerSlot
				: Players[(int)WinnerSlot].Id;
			model.ChallengerScore = Score[(int)PlayerSlot.Challenger];
			model.DefenderScore = Score[(int)PlayerSlot.Defender];

			model.MatchID = _matchId;
			model.RoundIndex = RoundIndex;
			model.MatchNumber = MatchNumber;
			model.WinsNeeded = WinsNeeded;
			model.MatchIndex = MatchIndex;
			model.NextMatchNumber = NextMatchNumber;
			model.NextLoserMatchNumber = NextLoserMatchNumber;

			switch (PreviousMatchNumbers.Count)
			{
				case (2):
					model.PrevDefenderMatchNumber = PreviousMatchNumbers[0];
					model.PrevChallengerMatchNumber = PreviousMatchNumbers[1];
					break;
				case (1):
					model.PrevDefenderMatchNumber = -1;
					model.PrevChallengerMatchNumber = PreviousMatchNumbers[0];
					break;
				default:
					model.PrevDefenderMatchNumber = -1;
					model.PrevChallengerMatchNumber = -1;
					break;
			}

			return model;
		}

		public void AddPlayer(IPlayer _player, PlayerSlot _slot = PlayerSlot.unspecified)
		{
			if (_slot != PlayerSlot.unspecified &&
				_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be -1, 0, or 1!");
			}
			if ((null != Players[0] && Players[0].Id == _player.Id) ||
				(null != Players[1] && Players[1].Id == _player.Id))
			{
				throw new DuplicateObjectException
					("Match already contains this Player!");
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

			throw new SlotFullException
				("Match cannot add Player; there is already a Player in this Slot!");
		}
		public void ReplacePlayer(IPlayer _newPlayer, int _oldPlayerId)
		{
			if (null == _newPlayer)
			{
				throw new NullReferenceException
					("New Player cannot be null!");
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
				throw new PlayerNotFoundException
					("Player not found in this Match!");
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

			throw new PlayerNotFoundException
				("Player not found in this Match!");
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
				throw new InvalidSlotException
					("PlayerSlot must be 0 or 1!");
			}
			if (IsFinished)
			{
				throw new InactiveMatchException
					("Match is finished; can't add more wins!");
			}
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Match is not begun; can't add a win!");
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
				throw new InvalidSlotException
					("PlayerSlot must be 0 or 1!");
			}
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Match is not begun; can't subtract wins!");
			}
			if (Score[(int)_slot] <= 0)
			{
				throw new ScoreException
					("Score is already 0; can't subtract wins!");
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
				throw new InactiveMatchException
					("Match is finished; cannot change victory conditions.");
			}
			if (_wins < 1)
			{
				throw new ScoreException
					("Wins Needed cannot be less than 1!");
			}

			WinsNeeded = _wins;
		}
		public void SetRoundIndex(int _index)
		{
			if (RoundIndex > -1)
			{
				throw new AlreadyAssignedException
					("Round Index is already set!");
			}
			RoundIndex = _index;
		}
		public void SetMatchIndex(int _index)
		{
			if (MatchIndex > -1)
			{
				throw new AlreadyAssignedException
					("Match Index is already set!");
			}
			MatchIndex = _index;
		}
		public void SetMatchNumber(int _number)
		{
			if (MatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Match Number is already set!");
			}
			MatchNumber = _number;
		}
		public void AddPreviousMatchNumber(int _number)
		{
			if (PreviousMatchNumbers.Count >= 2)
			{
				throw new AlreadyAssignedException
					("Previous Match Numbers are already set!");
			}
			PreviousMatchNumbers.Add(_number);
		}
		public void SetNextMatchNumber(int _number)
		{
			if (NextMatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Next Match Number is already set!");
			}
			NextMatchNumber = _number;
		}
		public void SetNextLoserMatchNumber(int _number)
		{
			if (NextLoserMatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Next Loser Match Number is already set!");
			}
			NextLoserMatchNumber = _number;
		}
#endregion

#region Private Methods

#endregion
	}
}
