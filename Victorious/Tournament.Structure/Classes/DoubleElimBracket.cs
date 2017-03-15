using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class DoubleElimBracket : SingleElimBracket
	{
		#region Variables & Properties
		// inherits List<IPlayer> Players
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches
		// inherits int NumberOfLowerRounds
		// inherits IMatch GrandFinal
		// inherits int NumberOfMatches
		#endregion

		#region Ctors
		public DoubleElimBracket(List<IPlayer> _players)
			: base(_players)
		{ }
		public DoubleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public DoubleElimBracket(BracketModel _model)
			: base(_model)
		{
			if (CalculateTotalLowerBracketMatches(Players.Count) > 0)
			{
				int numOfGrandFinal = _model.Matches.Count - 1;
				int i = 0;

				LowerMatches = new Dictionary<int, IMatch>();
				foreach (MatchModel mm in _model.Matches)
				{
					if (i >= Matches.Count
						&& i < numOfGrandFinal)
					{
						IMatch match = new Match(mm, Players);
						if (match.RoundIndex > NumberOfLowerRounds)
						{
							NumberOfLowerRounds = match.RoundIndex;
						}
						LowerMatches.Add(match.MatchNumber, match);
						++NumberOfMatches;
					}
					else if (i == numOfGrandFinal)
					{
						GrandFinal = new Match(mm, Players);
						++NumberOfMatches;
					}

					++i;
				}
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			base.CreateBracket(_winsPerMatch);
			if (0 == NumberOfMatches)
			{
				return;
			}

			List<List<IMatch>> roundList = new List<List<IMatch>>();
			int totalMatches = CalculateTotalLowerBracketMatches(Players.Count);
			int numMatches = 0;
			int r = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				roundList.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, r / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.WinsNeeded = _winsPerMatch;
					roundList[r].Add(m);
				}
				++r;
			}

			// Assign Match Numbers
			int matchNum = 1 + Matches.Count;
			for (r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (IMatch match in roundList[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			// Tie Matches Together
			for (r = 0; r + 1 < roundList.Count; ++r)
			{
				bool rIndexIsEven = (0 == r % 2) ? true : false;
				if (rIndexIsEven && roundList[r + 1].Count == roundList[r].Count)
				{
					// Round is "normal," but one team is coming from Upper Bracket
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						List<IMatch> upperRound = GetRound(NumberOfRounds - (r / 2));
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup indexes
						roundList[r][m].AddPreviousMatchNumber(upperRound[m].MatchNumber);
						Matches[upperRound[m].MatchNumber].SetNextLoserMatchNumber(currNum);
						// ************* THIS ISN'T QUITE RIGHT (RE-SEED ORDER)

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m].MatchNumber);
						roundList[r + 1][m].SetNextMatchNumber(currNum);
					}
				}
				else if (!rIndexIsEven && roundList[r + 1].Count == (roundList[r].Count * 2))
				{
					// Round is "normal"
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup indexes
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2].MatchNumber);
						roundList[r + 1][m * 2].SetNextMatchNumber(currNum);

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2 + 1].MatchNumber);
						roundList[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				else
				{
					// Round is abnormal. Case is not possible
					// (unless we later decide to include it)
				}
			}

			r = roundList.Count - 1;
			if (r >= 0)
			{
				// We have enough teams to created a Lower Bracket.
				// Manually update the first Lower round,
				// and create a Grand Final match

				for (int m = 0; m < roundList[r].Count; ++m)
				{
					List<IMatch> upperRound = GetRound(NumberOfRounds - (r / 2 + 1));
					int currNum = roundList[r][m].MatchNumber;

					// Assign prev/next matchup indexes for FIRST round
					// (both teams come from Upper Bracket)
					roundList[r][m].AddPreviousMatchNumber(upperRound[m * 2].MatchNumber);
					Matches[upperRound[m * 2].MatchNumber].SetNextLoserMatchNumber(currNum);

					roundList[r][m].AddPreviousMatchNumber(upperRound[m * 2 + 1].MatchNumber);
					Matches[upperRound[m * 2 + 1].MatchNumber].SetNextLoserMatchNumber(currNum);
				}

				// Create a Grand Final
				GrandFinal = new Match();
				GrandFinal.SetMatchNumber(matchNum);
				GrandFinal.WinsNeeded = _winsPerMatch;
				GrandFinal.SetRoundIndex(0);
				GrandFinal.SetMatchIndex(0);
				GrandFinal.AddPreviousMatchNumber(Matches.Count);
				GrandFinal.AddPreviousMatchNumber(roundList[0][0].MatchNumber);
				// Connect Final matches to Grand Final
				roundList[0][0].SetNextMatchNumber(GrandFinal.MatchNumber);
				Matches[Matches.Count].SetNextMatchNumber(GrandFinal.MatchNumber);

				// Move new bracket data to member variables (LowerMatches dictionary)
				NumberOfLowerRounds = roundList.Count;
				LowerMatches = new Dictionary<int, IMatch>();
				for (r = 0; r < roundList.Count; ++r)
				{
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						roundList[r][m].SetRoundIndex(roundList.Count - r);
						roundList[r][m].SetMatchIndex(m + 1);
						LowerMatches.Add(roundList[r][m].MatchNumber, roundList[r][m]);
					}
				}
				NumberOfMatches += (LowerMatches.Count + 1);
			}
			else
			{
				LowerMatches = null;
				GrandFinal = null;
			}
		}

		public override void AddWin(int _matchNumber, PlayerSlot _slot)
		{
			try
			{
				// Check the Upper Bracket:
				base.AddWin(_matchNumber, _slot);

				if (Matches[_matchNumber].Score[(int)_slot] >= Matches[_matchNumber].WinsNeeded)
				{
					// If this is the Upper Final, move winner to the Grand Final
					if (Matches[_matchNumber].RoundIndex == NumberOfRounds)
					{
						GrandFinal.AddPlayer((PlayerSlot.Defender == _slot)
							? Matches[_matchNumber].DefenderIndex()
							: Matches[_matchNumber].ChallengerIndex()
							, PlayerSlot.Defender);
					}

					// "Advance" the loser
					PlayerSlot loserSlot = (PlayerSlot.Defender == _slot)
						? PlayerSlot.Challenger : PlayerSlot.Defender;
					int nmNumber = Matches[_matchNumber].NextLoserMatchNumber;
					for (int i = 0; i < LowerMatches[nmNumber].PreviousMatchNumbers.Count; ++i)
					{
						if (LowerMatches[nmNumber].PreviousMatchNumbers[i] == _matchNumber)
						{
							LowerMatches[nmNumber].AddPlayer((PlayerSlot.Defender == _slot)
								? Matches[_matchNumber].ChallengerIndex()
								: Matches[_matchNumber].DefenderIndex()
								, (PlayerSlot)i);
						}
					}
				}

				return;
			}
			catch (KeyNotFoundException)
			{
				// Check the Grand Final:
				if (_matchNumber == GrandFinal.MatchNumber)
				{
					GrandFinal.AddWin(_slot);
					if (GrandFinal.Score[(int)_slot] >= GrandFinal.WinsNeeded)
					{
						// Match is over; do something???
					}
					return;
				}

				// Check the Lower Bracket:
				if (LowerMatches.ContainsKey(_matchNumber))
				{
					LowerMatches[_matchNumber].AddWin(_slot);

					// If Match is over:
					if (LowerMatches[_matchNumber].Score[(int)_slot] >= LowerMatches[_matchNumber].WinsNeeded)
					{
						int nmNumber = LowerMatches[_matchNumber].NextMatchNumber;
						if (nmNumber == GrandFinal.MatchNumber)
						{
							// Winner to Grand Final:
							GrandFinal.AddPlayer((PlayerSlot.Defender == _slot)
								? LowerMatches[_matchNumber].DefenderIndex()
								: LowerMatches[_matchNumber].ChallengerIndex()
								, PlayerSlot.Challenger);
						}
						else
						{
							// Winner to next Match:
							for (int i = 0; i < LowerMatches[nmNumber].PreviousMatchNumbers.Count; ++i)
							{
								if (_matchNumber == LowerMatches[nmNumber].PreviousMatchNumbers[i])
								{
									PlayerSlot newSlot = (1 == LowerMatches[nmNumber].PreviousMatchNumbers.Count)
										? PlayerSlot.Challenger : (PlayerSlot)i;
									LowerMatches[nmNumber].AddPlayer((PlayerSlot.Defender == _slot)
										? LowerMatches[_matchNumber].DefenderIndex()
										: LowerMatches[_matchNumber].ChallengerIndex()
										, newSlot);
									break;
								}
							}
						}
					}

					return;
				}
			}

			throw new KeyNotFoundException();
		}

		public override void SubtractWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_matchNumber < 1 ||
				(_slot != PlayerSlot.Defender && _slot != PlayerSlot.Challenger))
			{
				throw new ArgumentOutOfRangeException();
			}

			// Check the Grand Final:
			if (GrandFinal.MatchNumber == _matchNumber)
			{
				GrandFinal.SubtractWin(_slot);
				// in future, this may affect other Brackets...
			}
			// Check the Upper Bracket:
			else if (Matches.ContainsKey(_matchNumber))
			{
				if (Matches[_matchNumber].Score[(int)_slot] == Matches[_matchNumber].WinsNeeded)
				{
					// Remove any advanced Players from future matches:
					RemovePlayerFromFutureMatches(Matches[_matchNumber].NextMatchNumber,
						(PlayerSlot.Defender == _slot)
						? Matches[_matchNumber].DefenderIndex()
						: Matches[_matchNumber].ChallengerIndex());
					RemovePlayerFromFutureMatches(Matches[_matchNumber].NextLoserMatchNumber,
						(PlayerSlot.Defender == _slot)
						? Matches[_matchNumber].ChallengerIndex()
						: Matches[_matchNumber].DefenderIndex());
				}

				Matches[_matchNumber].SubtractWin(_slot);
			}
			// Check the Lower Bracket:
			else if (LowerMatches.ContainsKey(_matchNumber))
			{
				if (LowerMatches[_matchNumber].Score[(int)_slot] == LowerMatches[_matchNumber].WinsNeeded)
				{
					// Remove any advanced Players from future matches:
					RemovePlayerFromFutureMatches(LowerMatches[_matchNumber].NextMatchNumber,
						(PlayerSlot.Defender == _slot)
						? LowerMatches[_matchNumber].DefenderIndex()
						: LowerMatches[_matchNumber].ChallengerIndex());
				}

				LowerMatches[_matchNumber].SubtractWin(_slot);
			}
			else
			{
				throw new KeyNotFoundException();
			}
		}

		public override void ResetMatchScore(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			// Check the Grand Final:
			if (GrandFinal.MatchNumber == _matchNumber)
			{
				GrandFinal.ResetScore();
				// if we start storing a "bracket winner", do more here...
			}
			// Check the Upper Bracket:
			else if (Matches.ContainsKey(_matchNumber))
			{
				if (Matches[_matchNumber].Score[(int)PlayerSlot.Defender] >= Matches[_matchNumber].WinsNeeded)
				{
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextMatchNumber, Matches[_matchNumber].DefenderIndex());
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextLoserMatchNumber, Matches[_matchNumber].ChallengerIndex());
				}
				if (Matches[_matchNumber].Score[(int)PlayerSlot.Challenger] >= Matches[_matchNumber].WinsNeeded)
				{
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextMatchNumber, Matches[_matchNumber].ChallengerIndex());
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextLoserMatchNumber, Matches[_matchNumber].DefenderIndex());
				}

				Matches[_matchNumber].ResetScore();
			}
			// Check the Lower Bracket:
			else if (LowerMatches.ContainsKey(_matchNumber))
			{
				if (LowerMatches[_matchNumber].Score[(int)PlayerSlot.Defender] >= LowerMatches[_matchNumber].WinsNeeded)
				{
					RemovePlayerFromFutureMatches
						(LowerMatches[_matchNumber].NextMatchNumber, LowerMatches[_matchNumber].DefenderIndex());
				}
				if (LowerMatches[_matchNumber].Score[(int)PlayerSlot.Challenger] >= LowerMatches[_matchNumber].WinsNeeded)
				{
					RemovePlayerFromFutureMatches
						(LowerMatches[_matchNumber].NextMatchNumber, LowerMatches[_matchNumber].ChallengerIndex());
				}

				LowerMatches[_matchNumber].ResetScore();
			}
			else
			{
				throw new KeyNotFoundException();
			}
		}
#endregion

#region Private Methods
		private int CalculateTotalLowerBracketMatches(int _numPlayers)
		{
			if (_numPlayers < 4)
			{
				return 0;
			}

			int normalizedPlayers = 2;
			while (true)
			{
				int next = normalizedPlayers * 2;
				if (next <= _numPlayers)
				{
					normalizedPlayers = next;
				}
				else
				{
					break;
				}
			}
			return (normalizedPlayers - 2);
		}

		protected override void RemovePlayerFromFutureMatches(int _matchNumber, int _playerIndex)
		{
			if (_matchNumber < 1 ||
				_playerIndex < 0 || _playerIndex >= Players.Count)
			{
				return;
			}

			// Check the Grand Final:
			if (GrandFinal.MatchNumber == _matchNumber)
			{
				try
				{
					GrandFinal.RemovePlayer(_playerIndex);
				}
				catch (KeyNotFoundException)
				{ }
				return;
			}
			// Check the Upper Bracket:
			else if (Matches.ContainsKey(_matchNumber))
			{
				if (Matches[_matchNumber].DefenderIndex() == _playerIndex ||
					Matches[_matchNumber].ChallengerIndex() == _playerIndex)
				{
					// Remove any advanced Players from future Matches:
					if (Matches[_matchNumber].Score[(int)PlayerSlot.Defender] >= Matches[_matchNumber].WinsNeeded)
					{
						RemovePlayerFromFutureMatches
							(Matches[_matchNumber].NextMatchNumber, Matches[_matchNumber].DefenderIndex());
						RemovePlayerFromFutureMatches
							(Matches[_matchNumber].NextLoserMatchNumber, Matches[_matchNumber].ChallengerIndex());
					}
					if (Matches[_matchNumber].Score[(int)PlayerSlot.Challenger] >= Matches[_matchNumber].WinsNeeded)
					{
						RemovePlayerFromFutureMatches
							(Matches[_matchNumber].NextMatchNumber, Matches[_matchNumber].ChallengerIndex());
						RemovePlayerFromFutureMatches
							(Matches[_matchNumber].NextLoserMatchNumber, Matches[_matchNumber].DefenderIndex());
					}

					Matches[_matchNumber].RemovePlayer(_playerIndex);
				}
			}
			// Check the Lower Bracket:
			else if (LowerMatches.ContainsKey(_matchNumber))
			{
				if (LowerMatches[_matchNumber].DefenderIndex() == _playerIndex ||
					LowerMatches[_matchNumber].ChallengerIndex() == _playerIndex)
				{
					// Remove any advanced Players from future Matches:
					if (LowerMatches[_matchNumber].Score[(int)PlayerSlot.Defender] >= LowerMatches[_matchNumber].WinsNeeded)
					{
						RemovePlayerFromFutureMatches
							(LowerMatches[_matchNumber].NextMatchNumber, LowerMatches[_matchNumber].DefenderIndex());
					}
					if (LowerMatches[_matchNumber].Score[(int)PlayerSlot.Challenger] >= LowerMatches[_matchNumber].WinsNeeded)
					{
						RemovePlayerFromFutureMatches
							(LowerMatches[_matchNumber].NextMatchNumber, LowerMatches[_matchNumber].ChallengerIndex());
					}

					LowerMatches[_matchNumber].RemovePlayer(_playerIndex);
				}
			}
			else
			{
				throw new KeyNotFoundException();
			}
		}
#endregion
	}
}
