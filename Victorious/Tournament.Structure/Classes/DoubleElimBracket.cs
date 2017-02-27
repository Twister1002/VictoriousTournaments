using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class DoubleElimBracket : SingleElimBracket
	{
		// Properties
		public List<List<IMatch>> LowerRounds
		{ get; set; }

		// Ctors
		public DoubleElimBracket() : this(new List<IPlayer>())
		{ }
		public DoubleElimBracket(List<IPlayer> _players) : base(_players)
		{
			LowerRounds = new List<List<IMatch>>();
		}

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			base.CreateBracket(_winsPerMatch);
			LowerRounds.Clear();

			int totalMatches = Players.Count - 2;
			int numMatches = 0;
			int roundIndex = 0;
			while (numMatches < totalMatches)
			{
				LowerRounds.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, roundIndex / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.RoundNumber = roundIndex;
					m.MatchIndex = LowerRounds[roundIndex].Count;
					m.WinsNeeded = _winsPerMatch;
					LowerRounds[roundIndex].Add(m);
				}
				++roundIndex;
			}

			int rIndex = 0;
			for (; rIndex + 1 < LowerRounds.Count; ++rIndex)
			{
				bool rIndexIsEven = (0 == rIndex % 2) ? true : false;
				if (rIndexIsEven && LowerRounds[rIndex + 1].Count == LowerRounds[rIndex].Count)
				{
					// Round is "normal," but one team is coming from Upper Bracket
					for (int mIndex = 0; mIndex < LowerRounds[rIndex].Count; ++mIndex)
					{
						// Assign prev/next matchup indexes
						LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * -1);
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
					// Round is abnormal. FUCK. Do something???
				}
			}

			rIndex = LowerRounds.Count - 1;
			for (int mIndex = 0; mIndex < LowerRounds[rIndex].Count; ++mIndex)
			{
				// Assign prev/next matchup indexes for FIRST round
				// (both teams come from Upper Bracket)
				LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * -2);
				Rounds[Rounds.Count - 1][mIndex * 2].NextLoserMatchIndex = mIndex;

				LowerRounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * -2 - 1);
				Rounds[Rounds.Count - 1][mIndex * 2 + 1].NextLoserMatchIndex = mIndex;
			}
		}

		public override void AddWin(IMatch _match, int _index)
		{
			for (int r = 0; r < Rounds.Count; ++r)
			{
				for (int m = 0; m < Rounds[r].Count; ++m)
				{
					if (Rounds[r][m] == _match)
					{
						base.AddWin(r, m, _index);

						if (Rounds[r][m].Score[_index] >= Rounds[r][m].WinsNeeded)
						{
							// Match over: move the loser
							int loserIndex = (0 == _index) ? 1 : 0;
							int nMatchIndex = Rounds[r][m].NextLoserMatchIndex;
							int nRoundIndex = (r == Rounds.Count - 1) ? (r * 2 - 1) : (r * 2);
							for (int i = 0; i < LowerRounds[nRoundIndex][nMatchIndex].PrevMatchIndexes.Count; ++i)
							{
								if (m == (-1 * LowerRounds[nRoundIndex][nMatchIndex].PrevMatchIndexes[i]))
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
								// TODO : Move winner to Grand Finals
								return;
							}

							// Move the winner forward
							int nmIndex = LowerRounds[r][m].NextMatchIndex;
							for (int i = 0; i < LowerRounds[r - 1][nmIndex].PrevMatchIndexes.Count; ++i)
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
		#endregion
	}
}
