using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class DoubleElimBracket : SingleElimBracket
	{
		public List<List<IMatch>> LowerRounds
		{ get; set; }

		public DoubleElimBracket() : base()
		{
			LowerRounds = new List<List<IMatch>>();
		}

		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			base.CreateBracket(_winsPerMatch);
			LowerRounds.Clear();

			#region Create the Bracket
			int totalMatches = Players.Count - 2;
			int numMatches = 0;
			int roundIndex = 0;
			while (numMatches < totalMatches)
			{
				Rounds.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, roundIndex / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.RoundNumber = roundIndex;
					m.MatchIndex = Rounds[roundIndex].Count;
					m.WinsNeeded = _winsPerMatch;
					Rounds[roundIndex].Add(m);
				}
				++roundIndex;
			}

			for (int rIndex = 0; rIndex + 1 < Rounds.Count; ++rIndex)
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
						LowerRounds[rIndex + 1][mIndex * 2].NextMatchIndex = mIndex;
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
				// Else: round is abnormal. FUCK

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
			#endregion
		}
	}
}
