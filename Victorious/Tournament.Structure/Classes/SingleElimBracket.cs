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
		// inherits int NumberOfMatches
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
		public SingleElimBracket(int _numPlayers)
		{
			Players = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				Players.Add(new User());
			}

			ResetBracket();
			CreateBracket();
		}
		public SingleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public SingleElimBracket(BracketModel _model)
		{
			if (null == _model)
			{
				throw new NullReferenceException();
			}

			List<UserModel> userModels = _model.UserSeeds
				.OrderBy(ubs => ubs.Seed)
				.Select(ubs => ubs.User)
				.ToList();
			Players = new List<IPlayer>();
			foreach (UserModel um in userModels)
			{
				Players.Add(new User(um));
			}

			ResetBracket();
			int totalMatches = Players.Count - 1;

			Matches = new Dictionary<int, IMatch>();
			foreach (MatchModel mm in _model.Matches)
			{
				IMatch match = new Match(mm);
				if (match.RoundIndex > NumberOfRounds)
				{
					NumberOfRounds = match.RoundIndex;
				}
				Matches.Add(match.MatchNumber, match);

				++NumberOfMatches;
				if (NumberOfMatches >= totalMatches)
				{
					break;
				}
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			if (_winsPerMatch < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			ResetBracket();
			if (Players.Count < 2)
			{
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
					m.SetWinsNeeded(_winsPerMatch);
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
			roundList[0][0].AddPlayer(Players[pIndex++]);
			roundList[0][0].AddPlayer(Players[pIndex++]);

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
						int[] playerIndexes = new int[2] { -1, -1 };
						for (int i = 0; i < Players.Count; ++i)
						{
							if (Players[i].Equals(roundList[r][m].Players[0]))
							{
								playerIndexes[0] = i;
							}
							else if (Players[i].Equals(roundList[r][m].Players[1]))
							{
								playerIndexes[1] = i;
							}
						}

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
					}
				}

				for (int prePlayers = pIndex - 1; prePlayers >= 0; --prePlayers)
				{
					for (int m = 0; m < prevRoundMatches; ++m)
					{
						if (roundList[r + 1][m].Players.Contains(Players[prePlayers]))
						{
							// Add prev round's teams (according to seed) from the master list
							roundList[r + 1][m].AddPlayer(Players[pIndex++]);
							break;
						}
					}
				}
			}
			#endregion

			#region Set Bracket Member Variables
			// Move bracket data to member variables (Matches dictionary)
			NumberOfRounds = roundList.Count;
			Matches = new Dictionary<int, IMatch>();
			for (int r = 0; r < roundList.Count; ++r)
			{
				for (int m = 0; m < roundList[r].Count; ++m)
				{
					roundList[r][m].SetRoundIndex(roundList.Count - r);
					roundList[r][m].SetMatchIndex(m + 1);
					Matches.Add(roundList[r][m].MatchNumber, roundList[r][m]);
				}
			}
			NumberOfMatches = Matches.Count;
			#endregion
		}

		public override void AddWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_matchNumber < 1 ||
				(_slot != PlayerSlot.Defender && _slot != PlayerSlot.Challenger))
			{
				throw new IndexOutOfRangeException();
			}
			if (!Matches.ContainsKey(_matchNumber))
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
						Matches[nmNumber].AddPlayer
							(Matches[_matchNumber].Players[(int)_slot] , newSlot);
						break;
					}
				}
			}
		}

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
			if (_slot == Matches[_matchNumber].WinnerSlot)
			{
				RemovePlayerFromFutureMatches
					(Matches[_matchNumber].NextMatchNumber,
					ref Matches[_matchNumber].Players[(int)_slot]);
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
			if (Matches[_matchNumber].IsFinished)
			{
				RemovePlayerFromFutureMatches
					(Matches[_matchNumber].NextMatchNumber,
					ref Matches[_matchNumber].Players[(int)(Matches[_matchNumber].WinnerSlot)]);
			}

			// Reset Score
			Matches[_matchNumber].ResetScore();
		}
		#endregion

		#region Private Methods
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
							match.AddPlayer(_currMatch.Players[0]);
							_currMatch.RemovePlayer(_currMatch.Players[0].Id);
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
						match.AddPlayer(_currMatch.Players[1]);
						_currMatch.RemovePlayer(_currMatch.Players[1].Id);
						break;
					}
				}
			}
		}

		protected virtual void RemovePlayerFromFutureMatches(int _matchNumber, ref IPlayer _player)
		{
			if (_matchNumber < 1 || null == _player)
			{
				return;
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new KeyNotFoundException();
			}

			if (Matches[_matchNumber].Players.Contains(_player))
			{
				if (Matches[_matchNumber].IsFinished)
				{
					// Remove any advanced Players from future Matches:
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextMatchNumber,
						ref Matches[_matchNumber].Players[(int)(Matches[_matchNumber].WinnerSlot)]);
				}

				Matches[_matchNumber].RemovePlayer(_player.Id);
			}
		}
		#endregion
	}
}
