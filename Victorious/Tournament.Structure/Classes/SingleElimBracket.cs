using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class SingleElimBracket : Bracket
	{
		#region Variables & Properties
		// inherits List<IPlayer> Players
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		#endregion

		#region Ctors
		public SingleElimBracket(List<IPlayer> _players)
		{
			if (null == _players)
			{
				throw new NullReferenceException();
			}

			Players = _players;
			ResetBracket();
			CreateBracket();
		}
		public SingleElimBracket()
			: this(new List<IPlayer>())
		{ }
		#endregion

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			ResetBracket();
			if (Players.Count < 2)
			{
				//throw new ArgumentOutOfRangeException();
				return;
			}

			#region Create the Bracket
			int totalMatches = Players.Count - 1;
			int numMatches = 0;
			List<List<IMatch>> roundList = new List<List<IMatch>>();

			// Create the Matches
			for (int r = 0; numMatches < totalMatches; ++r)
			{
				roundList.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, r) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.WinsNeeded = _winsPerMatch;
					roundList[r].Add(m);
				}
			}

			// Assign Match Numbers
			int matchNum = 1;
			for (int r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (IMatch match in roundList[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			// Tie Matches Together
			for (int r = 0; r + 1 < roundList.Count; ++r)
			{
				if (roundList[r + 1].Count == (roundList[r].Count * 2))
				{
					// "Normal" rounds: twice as many matchups
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup numbers
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2].MatchNumber);
						roundList[r + 1][m * 2].SetNextMatchNumber(currNum);

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2 + 1].MatchNumber);
						roundList[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				// Else: round is abnormal. Ignore it for now (we'll handle it later)
			}
			#endregion

			#region Assign the Players
			// Assign top two seeds to final match
			int pIndex = 0;
			roundList[0][0].AddPlayer(pIndex++);
			roundList[0][0].AddPlayer(pIndex++);

			for (int r = 0; r + 1 < roundList.Count; ++r)
			{
				// We're shifting back one player for each match in the prev round
				int prevRoundMatches = roundList[r + 1].Count;

				if ((roundList[r].Count * 2) > prevRoundMatches)
				{
					// Abnormal round ahead: we need to allocate prevMatchIndexes
					// to correctly distribute bye seeds

					int prevMatchNumber = 1;

					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int[] playerIndexes = new int[2]
							{ roundList[r][m].DefenderIndex(), roundList[r][m].ChallengerIndex() };
						foreach (int p in playerIndexes)
						{
							if (p >= pIndex - prevRoundMatches)
							{
								roundList[r][m].AddPreviousMatchNumber(prevMatchNumber);
								roundList[r + 1][prevMatchNumber - 1].SetNextMatchNumber(roundList[r][m].MatchNumber);
								++prevMatchNumber;
							}
						}
					}
				}

				for (int m = 0; m < roundList[r].Count; ++m)
				{
					// For each match, shift/reassign all teams to the prev bracket level
					// If prev level is abnormal, only shift 1 (or 0) teams
					if (1 <= roundList[r][m].PreviousMatchNumbers.Count)
					{
						ReassignPlayers(roundList[r][m], roundList[r + 1]);
						//ReassignPlayers(roundList[r][m], r);
					}
				}

				for (int prePlayers = pIndex - 1; prePlayers >= 0; --prePlayers)
				{
					for (int m = 0; m < prevRoundMatches; ++m)
					{
						if (roundList[r + 1][m].DefenderIndex() == prePlayers ||
							roundList[r + 1][m].ChallengerIndex() == prePlayers)
						{
							// Add prev round's teams (according to seed) from the master list
							roundList[r + 1][m].AddPlayer(pIndex++);
							break;
						}
					}
				}
			}

			// Move bracket data to member variables (Matches dictionary)
			NumberOfRounds = roundList.Count;
			Matches = new Dictionary<int, IMatch>();
			for (int r = 0; r < roundList.Count; ++r)
			{
				for (int m = 0; m < roundList[r].Count; ++m)
				{
					roundList[r][m].SetRoundIndex(roundList.Count - r);
					roundList[r][m].SetMatchIndex(m + 1);
					Matches[roundList[r][m].MatchNumber] = roundList[r][m];
				}
			}
			#endregion
		}

		public override void UpdateCurrentMatches(ICollection<MatchModel> _matchModels)
		{
			// REPLACE THIS
			throw new NotImplementedException();
		}

		public override void AddWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_matchNumber < 1 ||
				(_slot != PlayerSlot.Defender && _slot != PlayerSlot.Challenger))
			{
				throw new IndexOutOfRangeException();
			}
			if (null == Matches[_matchNumber])
			{
				throw new KeyNotFoundException();
			}

			Matches[_matchNumber].AddWin(_slot);

			if (Matches[_matchNumber].Score[(int)_slot] >= Matches[_matchNumber].WinsNeeded
				&& Matches[_matchNumber].RoundIndex < NumberOfRounds)
			{
				// Player won the match. Advance!
				int nmNumber = Matches[_matchNumber].NextMatchNumber;
				for (int i = 0; i < Matches[nmNumber].PreviousMatchNumbers.Count; ++i)
				{
					if (_matchNumber == Matches[nmNumber].PreviousMatchNumbers[i])
					{
						PlayerSlot newSlot = (1 == Matches[nmNumber].PreviousMatchNumbers.Count)
							? PlayerSlot.Challenger : (PlayerSlot)i;
						Matches[nmNumber].AddPlayer((PlayerSlot.Defender == _slot)
							? Matches[_matchNumber].DefenderIndex()
							: Matches[_matchNumber].ChallengerIndex()
							, newSlot);
						break;
					}
				}
			}
		}
#if false
		public override void AddWin(IMatch _match, PlayerSlot _slot)
		{
			AddWin(_match.MatchNumber, _slot);
		}
#endif
		public override void SubtractWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_matchNumber < 1 ||
				(_slot != PlayerSlot.Defender && _slot != PlayerSlot.Challenger))
			{
				throw new IndexOutOfRangeException();
			}
			if (null == Matches[_matchNumber])
			{
				throw new KeyNotFoundException();
			}

			// If this Match is over, remove advanced Players from future Matches
			if (Matches[_matchNumber].Score[(int)_slot] == Matches[_matchNumber].WinsNeeded)
			{
				RemovePlayerFromFutureMatches(Matches[_matchNumber].NextMatchNumber,
					(PlayerSlot.Defender == _slot)
					? Matches[_matchNumber].DefenderIndex()
					: Matches[_matchNumber].ChallengerIndex());
			}

			// Subtract a Win
			Matches[_matchNumber].SubtractWin(_slot);
		}

		public override void ResetMatchScore(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new IndexOutOfRangeException();
			}
			if (null == Matches[_matchNumber])
			{
				throw new KeyNotFoundException();
			}

			// If this Match is over, remove advanced Players from future Matches
			if (Matches[_matchNumber].Score[(int)PlayerSlot.Defender] >= Matches[_matchNumber].WinsNeeded)
			{
				RemovePlayerFromFutureMatches
					(Matches[_matchNumber].NextMatchNumber, Matches[_matchNumber].DefenderIndex());
			}
			if (Matches[_matchNumber].Score[(int)PlayerSlot.Challenger] >= Matches[_matchNumber].WinsNeeded)
			{
				RemovePlayerFromFutureMatches
					(Matches[_matchNumber].NextMatchNumber, Matches[_matchNumber].ChallengerIndex());
			}

			// Reset Score
			Matches[_matchNumber].ResetScore();
		}
#endregion

#region Private Methods
#if false
		private void ReassignPlayer(int _pIndex, IMatch _currMatch, int _currRound, int _newMatchNum)
		{
			if (null == _currMatch || _newMatchNum < 1)
			{
				throw new NullReferenceException();
			}

			if (_currMatch.DefenderIndex() == _pIndex ||
				_currMatch.ChallengerIndex() == _pIndex)
			{
				_currMatch.RemovePlayer(_pIndex);

				foreach (IMatch match in Rounds[_currMatch.RoundIndex + 1])
				{
					if (match.MatchNumber == _newMatchNum)
					{
						match.AddPlayer(_pIndex, 0);
						if (match.PlayerIndexes.Contains(_pIndex))
						{
							return;
						}
					}
				}
			}

			throw new KeyNotFoundException();
		}
#endif
		private void ReassignPlayers(IMatch _currMatch, List<IMatch> _prevRound)
		{
			if (null == _currMatch ||
				null == _prevRound || 0 == _prevRound.Count)
			{
				throw new NullReferenceException();
			}

			int i = 0;
			if (1 <= _currMatch.PreviousMatchNumbers.Count)
			{
				if (2 == _currMatch.PreviousMatchNumbers.Count)
				{
					// Reassign the higher seed (Defender)
					foreach (IMatch match in _prevRound)
					{
						if (match.MatchNumber == _currMatch.PreviousMatchNumbers[i])
						{
							match.AddPlayer(_currMatch.DefenderIndex());
							_currMatch.RemovePlayer(_currMatch.DefenderIndex());
							++i;
							break;
						}
					}
				}

				//Reassign the lower seed (Challenger)
				foreach (IMatch match in _prevRound)
				{
					if (match.MatchNumber == _currMatch.PreviousMatchNumbers[i])
					{
						match.AddPlayer(_currMatch.ChallengerIndex());
						_currMatch.RemovePlayer(_currMatch.ChallengerIndex());
						break;
					}
				}
			}
		}
#if false
		private void ReassignPlayers(IMatch _currMatch, int _currRound)
		{
			if (null == _currMatch || _currRound < 0)
			{
				throw new NullReferenceException();
			}
			if (_currRound + 1 >= Rounds.Count)
			{
				throw new IndexOutOfRangeException();
			}

			int i = 0;
			if (1 <= _currMatch.PreviousMatchNumbers.Count)
			{
				if (2 == _currMatch.PreviousMatchNumbers.Count)
				{
					// Reassign the higher seed (Defender)
					foreach (IMatch match in Rounds[_currRound + 1])
					{
						if (match.MatchNumber == _currMatch.PreviousMatchNumbers[i])
						{
							match.AddPlayer(_currMatch.DefenderIndex());
							_currMatch.RemovePlayer(_currMatch.DefenderIndex());
							++i;
							break;
						}
					}
				}

				// Reassign the lower seed (Challenger)
				foreach (IMatch match in Rounds[_currRound + 1])
				{
					if (match.MatchNumber == _currMatch.PreviousMatchNumbers[i])
					{
						match.AddPlayer(_currMatch.ChallengerIndex());
						_currMatch.RemovePlayer(_currMatch.ChallengerIndex());
						break;
					}
				}
			}
		}
#endif
#if false
		protected void AddWin(int _roundIndex, int _matchIndex, PlayerSlot _slot)
		{
			if (_roundIndex < 0 || _roundIndex >= Rounds.Count
				|| _matchIndex < 0 || _matchIndex >= Rounds[_roundIndex].Count)
			{
				throw new IndexOutOfRangeException();
			}

			Rounds[_roundIndex][_matchIndex].AddWin(_slot);

			if (0 == _roundIndex)
			{
				return;
			}
			if (Rounds[_roundIndex][_matchIndex].Score[(int)_slot] >= Rounds[_roundIndex][_matchIndex].WinsNeeded)
			{
				// Player won the match. Advance!

				// Move the winner:
				int nmNumber = Rounds[_roundIndex][_matchIndex].NextMatchNumber;
				foreach (IMatch match in Rounds[_roundIndex - 1])
				{
					if (nmNumber == match.MatchNumber)
					{
						for (int i = 0; i < match.PreviousMatchNumbers.Count; ++i)
						{
							if (Rounds[_roundIndex][_matchIndex].MatchNumber == match.PreviousMatchNumbers[i])
							{
								PlayerSlot newSlot = (1 == match.PreviousMatchNumbers.Count)
									? PlayerSlot.Challenger : (PlayerSlot)i;
								match.AddPlayer(
									(PlayerSlot.Defender == _slot)
									? Rounds[_roundIndex][_matchIndex].DefenderIndex()
									: Rounds[_roundIndex][_matchIndex].ChallengerIndex()
									, newSlot);
								return;
							}
						}
					}
				}
			}
		}
#endif
		protected virtual void RemovePlayerFromFutureMatches(int _matchNumber, int _playerIndex)
		{
			if (_matchNumber < 1 ||
				_playerIndex < 0 || _playerIndex >= Players.Count)
			{
				return;
			}
			if (null == Matches[_matchNumber])
			{
				throw new KeyNotFoundException();
			}

			if (Matches[_matchNumber].DefenderIndex() == _playerIndex ||
				Matches[_matchNumber].ChallengerIndex() == _playerIndex)
			{
				if (Matches[_matchNumber].Score[(int)PlayerSlot.Defender] >= Matches[_matchNumber].WinsNeeded)
				{
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextMatchNumber, Matches[_matchNumber].DefenderIndex());
				}
				if (Matches[_matchNumber].Score[(int)PlayerSlot.Challenger] >= Matches[_matchNumber].WinsNeeded)
				{
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextMatchNumber, Matches[_matchNumber].ChallengerIndex());
				}

				Matches[_matchNumber].RemovePlayer(_playerIndex);
			}
		}
#endregion
	}
}
