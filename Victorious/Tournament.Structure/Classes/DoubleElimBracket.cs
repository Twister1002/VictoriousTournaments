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
		// inherits List<List<IMatch>> Rounds
		// inherits List<LIst<IMatch>> LowerRounds
		// inherits IMatch GrandFinal
		#endregion

		#region Ctors
		public DoubleElimBracket(List<IPlayer> _players)
			: base(_players)
		{ }
		public DoubleElimBracket()
			: this(new List<IPlayer>())
		{ }
		#endregion

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			base.CreateBracket(_winsPerMatch);
			LowerRounds = new List<List<IMatch>>();
			int totalMatches = CalculateTotalLowerBracketMatches(Players.Count);
			int numMatches = 0;
			int r = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				LowerRounds.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, r / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.SetRoundIndex(r);
					m.SetMatchIndex(LowerRounds[r].Count);
					m.WinsNeeded = _winsPerMatch;
					LowerRounds[r].Add(m);
				}
				++r;
			}

			// Assign Match Numbers
			int matchNum = 1 + Rounds[0][0].MatchNumber;
			for (r = LowerRounds.Count - 1; r >= 0; --r)
			{
				foreach (IMatch match in LowerRounds[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			// Tie Matches Together
			for (r = 0; r + 1 < LowerRounds.Count; ++r)
			{
				bool rIndexIsEven = (0 == r % 2) ? true : false;
				if (rIndexIsEven && LowerRounds[r + 1].Count == LowerRounds[r].Count)
				{
					// Round is "normal," but one team is coming from Upper Bracket
					for (int m = 0; m < LowerRounds[r].Count; ++m)
					{
						int currNum = LowerRounds[r][m].MatchNumber;

						// Assign prev/next matchup indexes
						LowerRounds[r][m].AddPreviousMatchNumber(Rounds[r / 2][m].MatchNumber);
						Rounds[r / 2][m].SetNextLoserMatchNumber(currNum);
						// *************** THIS ISN'T QUITE RIGHT [mIndex]

						LowerRounds[r][m].AddPreviousMatchNumber(LowerRounds[r + 1][m].MatchNumber);
						LowerRounds[r + 1][m].SetNextMatchNumber(currNum);
					}
				}
				else if (!rIndexIsEven && LowerRounds[r + 1].Count == (LowerRounds[r].Count * 2))
				{
					// Round is "normal"
					for (int m = 0; m < LowerRounds[r].Count; ++m)
					{
						int currNum = LowerRounds[r][m].MatchNumber;

						// Assign prev/next matchup indexes
						LowerRounds[r][m].AddPreviousMatchNumber(LowerRounds[r + 1][m * 2].MatchNumber);
						LowerRounds[r + 1][m * 2].SetNextMatchNumber(currNum);

						LowerRounds[r][m].AddPreviousMatchNumber(LowerRounds[r + 1][m * 2 + 1].MatchNumber);
						LowerRounds[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				else
				{
					// Round is abnormal. Case is not possible (for now?)
				}
			}

			r = LowerRounds.Count - 1;
			if (r >= 0)
			{
				// We have enough teams to created a Lower Bracket.
				// Manually update the first Lower round,
				// and create a Grand Final match

				for (int m = 0; m < LowerRounds[r].Count; ++m)
				{
					int currNum = LowerRounds[r][m].MatchNumber;

					// Assign prev/next matchup indexes for FIRST round
					// (both teams come from Upper Bracket)
					LowerRounds[r][m].AddPreviousMatchNumber(Rounds[r / 2 + 1][m * 2].MatchNumber);
					Rounds[r / 2 + 1][m * 2].SetNextLoserMatchNumber(currNum);

					LowerRounds[r][m].AddPreviousMatchNumber(Rounds[r / 2 + 1][m * 2 + 1].MatchNumber);
					Rounds[r / 2 + 1][m * 2 + 1].SetNextLoserMatchNumber(currNum);
				}

				GrandFinal = new Match();
				GrandFinal.SetMatchNumber(matchNum);
				GrandFinal.WinsNeeded = _winsPerMatch;
				GrandFinal.SetRoundIndex(0);
				GrandFinal.SetMatchIndex(0);
				GrandFinal.AddPreviousMatchNumber(Rounds[0][0].MatchNumber);
				GrandFinal.AddPreviousMatchNumber(LowerRounds[0][0].MatchNumber);
			}
			else
			{
				LowerRounds = null;
				GrandFinal = null;
			}
		}

		public override void UpdateCurrentMatches(ICollection<MatchModel> _matchModels)
		{
			base.UpdateCurrentMatches(_matchModels);

			// Update Lower Bracket
			for (int r = LowerRounds.Count - 1; r >= 0; --r)
			{
				for (int m = 0; m < LowerRounds[r].Count; ++m)
				{
					foreach (MatchModel model in _matchModels)
					{
						if (model.MatchNumber == LowerRounds[r][m].MatchNumber)
						{
							LowerRounds[r][m] = new Match(model, Players);
							break;
						}
					}
				}
			}

			// Update Grand Final
			foreach (MatchModel model in _matchModels)
			{
				if (model.MatchNumber == GrandFinal.MatchNumber)
				{
					GrandFinal = new Match(model, Players);
					break;
				}
			}
		}

		public override void AddWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new IndexOutOfRangeException();
			}

			// Is _match the GrandFinal?
			if (_matchNumber == GrandFinal.MatchNumber)
			{
				GrandFinal.AddWin(_slot);
				if (GrandFinal.Score[(int)_slot] >= GrandFinal.WinsNeeded)
				{
					// Match is over. Do something?
				}
				return;
			}

			// Is _match in upper bracket?
			for (int r = 0; r < Rounds.Count; ++r)
			{
				for (int m = 0; m < Rounds[r].Count; ++m)
				{
					if (Rounds[r][m].MatchNumber == _matchNumber)
					{
						// Found _match:
						base.AddWin(r, m, _slot);

						if (Rounds[r][m].Score[(int)_slot] >= Rounds[r][m].WinsNeeded)
						{
							// Match over!

							if (0 == r)
							{
								// Final round: move winner to Grand Finals!
								GrandFinal.AddPlayer(
									(PlayerSlot.Defender == _slot)
									? Rounds[0][0].DefenderIndex()
									: Rounds[0][0].ChallengerIndex());
							}
							// Move the loser
							PlayerSlot loserSlot = (PlayerSlot.Defender == _slot)
								? PlayerSlot.Challenger : PlayerSlot.Defender;
							int nMatchNumber = Rounds[r][m].NextLoserMatchNumber;
							int nRIndex = (r == Rounds.Count - 1)
								? (r * 2 - 1) : (r * 2);
							foreach (IMatch match in LowerRounds[nRIndex])
							{
								if (match.MatchNumber == nMatchNumber)
								{
									for (int i = 0; i < match.PreviousMatchNumbers.Count; ++i)
									{
										if (match.PreviousMatchNumbers[i] == Rounds[r][m].MatchNumber)
										{
											match.AddPlayer(
												(PlayerSlot.Defender == _slot)
												? Rounds[r][m].ChallengerIndex()
												: Rounds[r][m].DefenderIndex()
												, (PlayerSlot)i);
										}
									}
									break;
								}
							}
						}

						return;
					}
				}
			}

			// Is _match in lower bracket?
			for (int r = 0; r < LowerRounds.Count; ++r)
			{
				for (int m = 0; m < LowerRounds[r].Count; ++m)
				{
					if (_matchNumber == LowerRounds[r][m].MatchNumber)
					{
						// Found match. Add win:
						AddLowerBracketWin(r, m, _slot);
						return;
#if false
						LowerRounds[r][m].AddWin(_index);

						if (LowerRounds[r][m].Score[_index] >= LowerRounds[r][m].WinsNeeded)
						{
							// Match is over. Move the winner forward:
							if (0 == r)
							{
								// Final round: Move winner to Grand Finals!
								GrandFinal.AddPlayer(LowerRounds[0][0].PlayerIndexes[_index], 1);
								return;
							}

							// Move the winner forward
							int nMatchNumber = LowerRounds[r][m].NextMatchNumber;
							foreach (IMatch match in LowerRounds[r - 1])
							{
								if (match.MatchNumber == nMatchNumber)
								{
									for (int i = 0; i < match.PrevMatchNumbers.Count; ++i)
									{
										if (match.PrevMatchNumbers[i] == LowerRounds[r][m].MatchNumber)
										{
											match.AddPlayer(_index, i);
											return;
										}
									}
								}
							}
						}
#endif
					}
				}
			}

			throw new KeyNotFoundException();
		}
#if false
		public override void AddWin(IMatch _match, PlayerSlot _slot)
		{
			AddWin(_match.MatchNumber, _slot);
		}
#endif
		public override void SubtractWin(int _matchNumber, PlayerSlot _slot)
		{
			base.SubtractWin(_matchNumber, _slot);
		}

#if false
		public List<IMatch> GetLowerRound(int _index)
		{
			if (_index < 0 || _index >= LowerRounds.Count)
			{
				throw new IndexOutOfRangeException();
			}

			return LowerRounds[_index];
		}
		public IMatch GetLowerMatch(int _roundIndex, int _index)
		{
			List<IMatch> matches = GetLowerRound(_roundIndex);

			if (_index < 0 || _index >= matches.Count)
			{
				throw new IndexOutOfRangeException();
			}

			return matches[_index];
		}
#endif
		#endregion

		#region Private Methods
		protected int CalculateTotalLowerBracketMatches(int _numPlayers)
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

		protected void AddLowerBracketWin(int _round, int _match, PlayerSlot _slot)
		{
			if (_round < 0 || _round >= LowerRounds.Count
				|| _match < 0 || _match >= LowerRounds[_round].Count)
			{
				throw new IndexOutOfRangeException();
			}

			LowerRounds[_round][_match].AddWin(_slot);

			if (LowerRounds[_round][_match].Score[(int)_slot] >= LowerRounds[_round][_match].WinsNeeded)
			{
				// Match is over. Move the winner forward:
				if (0 == _round)
				{
					// Final round: Move winner to Grand Finals!
					GrandFinal.AddPlayer(
						(PlayerSlot.Defender == _slot)
						? LowerRounds[0][0].DefenderIndex()
						: LowerRounds[0][0].ChallengerIndex()
						, PlayerSlot.Challenger);
					return;
				}

				// Move the winner forward
				int nMatchNumber = LowerRounds[_round][_match].NextMatchNumber;
				foreach (IMatch match in LowerRounds[_round - 1])
				{
					if (match.MatchNumber == nMatchNumber)
					{
						for (int i = 0; i < match.PreviousMatchNumbers.Count; ++i)
						{
							if (match.PreviousMatchNumbers[i] == LowerRounds[_round][_match].MatchNumber)
							{
								match.AddPlayer(
									(PlayerSlot.Defender == _slot)
									? LowerRounds[_round][_match].DefenderIndex()
									: LowerRounds[_round][_match].ChallengerIndex()
									, (PlayerSlot)i);
								return;
							}
						}
					}
				}
			}
		}
		#endregion
	}
}
