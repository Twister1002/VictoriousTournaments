using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class DoubleElimBracket : SingleElimBracket
	{
		#region Variables & Properties
		public List<List<IMatch>> LowerRounds
		{ get; set; }
		public IMatch GrandFinal
		{ get; set; }
		#endregion

		#region Ctors
		public DoubleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public DoubleElimBracket(List<IPlayer> _players)
			: base(_players)
		{ }
		#endregion

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			base.CreateBracket(_winsPerMatch);
			LowerRounds = new List<List<IMatch>>();

			int totalMatches = CalculateTotalLowerBracketMatches(Players.Count);
			int numMatches = 0;
			int rIndex = 0;
			while (numMatches < totalMatches)
			{
				LowerRounds.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, rIndex / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.RoundNumber = rIndex;
					m.MatchIndex = LowerRounds[rIndex].Count;
					m.WinsNeeded = _winsPerMatch;
					LowerRounds[rIndex].Add(m);
				}
				++rIndex;
			}

			rIndex = 0;
			for (; rIndex + 1 < LowerRounds.Count; ++rIndex)
			{
				bool rIndexIsEven = (0 == rIndex % 2) ? true : false;
				if (rIndexIsEven && LowerRounds[rIndex + 1].Count == LowerRounds[rIndex].Count)
				{
					// Round is "normal," but one team is coming from Upper Bracket
					for (int mIndex = 0; mIndex < LowerRounds[rIndex].Count; ++mIndex)
					{
						// Assign prev/next matchup indexes
						LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex);
						Rounds[rIndex / 2][mIndex].NextLoserMatchIndex = mIndex;
						// *************** THIS ISN'T QUITE RIGHT [mIndex]

						LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2);
						LowerRounds[rIndex + 1][mIndex].NextMatchIndex = mIndex;
					}
				}
				else if (!rIndexIsEven && LowerRounds[rIndex + 1].Count == (LowerRounds[rIndex].Count * 2))
				{
					// Round is "normal"
					for (int mIndex = 0; mIndex < LowerRounds[rIndex].Count; ++mIndex)
					{
						// Assign prev/next matchup indexes
						LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2);
						LowerRounds[rIndex + 1][mIndex * 2].NextMatchIndex = mIndex;

						LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2 + 1);
						LowerRounds[rIndex + 1][mIndex * 2 + 1].NextMatchIndex = mIndex;
					}
				}
				else
				{
					// Round is abnormal. Case is not possible (for now)
				}
			}

			rIndex = LowerRounds.Count - 1;
			if (rIndex >= 0)
			{
				// We have enough teams to created a Lower Bracket.
				// Manually update the first Lower round,
				// and create a Grand Final match

				for (int mIndex = 0; mIndex < LowerRounds[rIndex].Count; ++mIndex)
				{
					// Assign prev/next matchup indexes for FIRST round
					// (both teams come from Upper Bracket)
					LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2);
					Rounds[rIndex / 2 + 1][mIndex * 2].NextLoserMatchIndex = mIndex;

					LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2 + 1);
					Rounds[rIndex / 2 + 1][mIndex * 2 + 1].NextLoserMatchIndex = mIndex;
				}

				GrandFinal = new Match();
				GrandFinal.WinsNeeded = _winsPerMatch;
			}
		}

		public override void AddWin(IMatch _match, int _index)
		{
			if (_match == GrandFinal)
			{
				GrandFinal.AddWin(_index);
				if (GrandFinal.Score[_index] >= GrandFinal.WinsNeeded)
				{
					// Match is over. Do something?
				}
			}

			for (int r = 0; r < Rounds.Count; ++r)
			{
				for (int m = 0; m < Rounds[r].Count; ++m)
				{
					if (Rounds[r][m] == _match)
					{
						base.AddWin(r, m, _index);

						if (Rounds[r][m].Score[_index] >= Rounds[r][m].WinsNeeded)
						{
							// Match over!

							if (0 == r)
							{
								// Final round: move winner to Grand Finals!
								GrandFinal.PlayerIndexes[0] = Rounds[r][m].PlayerIndexes[_index];
							}
							// Move the loser
							int loserIndex = (0 == _index) ? 1 : 0;
							int nMatchIndex = Rounds[r][m].NextLoserMatchIndex;
							int nRoundIndex = (r == Rounds.Count - 1) ? (r * 2 - 1) : (r * 2);
							for (int i = 0; i < LowerRounds[nRoundIndex][nMatchIndex].PrevMatchIndexes.Count; ++i)
							{
								if (m == LowerRounds[nRoundIndex][nMatchIndex].PrevMatchIndexes[i])
								{
									LowerRounds[nRoundIndex][nMatchIndex].PlayerIndexes[i] = Rounds[r][m].PlayerIndexes[loserIndex];
									return;
								}
							}
						}
					}
				}
			}

			// If we reach here, _match is not in the Upper Bracket
			for (int r = 0; r < LowerRounds.Count; ++r)
			{
				for (int m = 0; m < LowerRounds[r].Count; ++m)
				{
					if (_match == LowerRounds[r][m])
					{
						// Found match. Add win:
						LowerRounds[r][m].AddWin(_index);

						if (LowerRounds[r][m].Score[_index] >= LowerRounds[r][m].WinsNeeded)
						{
							// Match is over. Move the winner forward:
							if (0 == r)
							{
								// Final round: Move winner to Grand Finals!
								GrandFinal.PlayerIndexes[1] = LowerRounds[0][0].PlayerIndexes[_index];
								return;
							}

							// Move the winner forward
							int nmIndex = LowerRounds[r][m].NextMatchIndex;
							for (int i = LowerRounds[r - 1][nmIndex].PrevMatchIndexes.Count - 1; i >= 0; --i)
							{
								if (m == LowerRounds[r - 1][nmIndex].PrevMatchIndexes[i])
								{
									LowerRounds[r - 1][nmIndex].PlayerIndexes[i] = LowerRounds[r][m].PlayerIndexes[_index];
									return;
								}
							}
						}
					}
				}
			}
		}

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
		#endregion
	}
}
