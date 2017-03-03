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

			// Create the Matches
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

			// Assign Match Numbers
			int matchNum = 1 + Rounds[0][0].MatchNumber;
			for (rIndex = LowerRounds.Count - 1; rIndex >= 0; --rIndex)
			{
				foreach (IMatch match in Rounds[rIndex])
				{
					match.MatchNumber = matchNum++;
				}
			}

			// Tie Matches Together
			for (rIndex = 0; rIndex + 1 < LowerRounds.Count; ++rIndex)
			{
				bool rIndexIsEven = (0 == rIndex % 2) ? true : false;
				if (rIndexIsEven && LowerRounds[rIndex + 1].Count == LowerRounds[rIndex].Count)
				{
					// Round is "normal," but one team is coming from Upper Bracket
					for (int mIndex = 0; mIndex < LowerRounds[rIndex].Count; ++mIndex)
					{
						int currNum = LowerRounds[rIndex][mIndex].MatchNumber;

						// Assign prev/next matchup indexes
						LowerRounds[rIndex][mIndex].AddPrevMatchNumber
							(Rounds[rIndex / 2][mIndex].MatchNumber);
						Rounds[rIndex / 2][mIndex].NextLoserMatchNumber = currNum;
						// *************** THIS ISN'T QUITE RIGHT [mIndex]

						LowerRounds[rIndex][mIndex].AddPrevMatchNumber
							(LowerRounds[rIndex + 1][mIndex].MatchNumber);
						LowerRounds[rIndex + 1][mIndex].NextMatchNumber = currNum;
					}
				}
				else if (!rIndexIsEven && LowerRounds[rIndex + 1].Count == (LowerRounds[rIndex].Count * 2))
				{
					// Round is "normal"
					for (int mIndex = 0; mIndex < LowerRounds[rIndex].Count; ++mIndex)
					{
						int currNum = LowerRounds[rIndex][mIndex].MatchNumber;

						// Assign prev/next matchup indexes
						LowerRounds[rIndex][mIndex].AddPrevMatchNumber
							(LowerRounds[rIndex + 1][mIndex * 2].MatchNumber);
						LowerRounds[rIndex + 1][mIndex * 2].NextMatchNumber = currNum;

						LowerRounds[rIndex][mIndex].AddPrevMatchNumber
							(LowerRounds[rIndex + 1][mIndex * 2 + 1].MatchNumber);
						LowerRounds[rIndex + 1][mIndex * 2 + 1].NextMatchNumber = currNum;
					}
				}
				else
				{
					// Round is abnormal. Case is not possible (for now?)
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
					int currNum = LowerRounds[rIndex][mIndex].MatchNumber;

					// Assign prev/next matchup indexes for FIRST round
					// (both teams come from Upper Bracket)
					LowerRounds[rIndex][mIndex].AddPrevMatchNumber
						(Rounds[rIndex / 2 + 1][mIndex * 2].MatchNumber);
					Rounds[rIndex / 2 + 1][mIndex * 2].NextLoserMatchNumber = currNum;

					LowerRounds[rIndex][mIndex].AddPrevMatchNumber
						(Rounds[rIndex / 2 + 1][mIndex * 2 + 1].MatchNumber);
					Rounds[rIndex / 2 + 1][mIndex * 2 + 1].NextLoserMatchNumber = currNum;
				}

				GrandFinal = new Match();
				GrandFinal.MatchNumber = matchNum;
				GrandFinal.WinsNeeded = _winsPerMatch;
				GrandFinal.RoundIndex = GrandFinal.MatchIndex = 0;
				GrandFinal.AddPrevMatchNumber(Rounds[0][0].MatchNumber);
				GrandFinal.AddPrevMatchNumber(LowerRounds[0][0].MatchNumber);
			}
			else
			{
				GrandFinal = null;
			}
		}

		public override void UpdateCurrentMatches(ICollection<MatchModel> _matchModels)
		{
			base.UpdateCurrentMatches(_matchModels);

			// Update Lower Bracket
			for (int rIndex = LowerRounds.Count - 1; rIndex >= 0; --rIndex)
			{
				for (int mIndex = 0; mIndex < LowerRounds[rIndex].Count; ++mIndex)
				{
					foreach (MatchModel model in _matchModels)
					{
						if (model.MatchNumber == LowerRounds[rIndex][mIndex].MatchNumber)
						{
							LowerRounds[rIndex][mIndex] = new Match(model, Players);
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

		public override void AddWin(IMatch _match, int _index)
		{
			// Is _match the GrandFinal?
			if (_match == GrandFinal)
			{
				GrandFinal.AddWin(_index);
				if (GrandFinal.Score[_index] >= GrandFinal.WinsNeeded)
				{
					// Match is over. Do something?
				}
			}

			// Is _match in upper bracket?
			for (int r = 0; r < Rounds.Count; ++r)
			{
				for (int m = 0; m < Rounds[r].Count; ++m)
				{
					if (Rounds[r][m] == _match)
					{
						// Found _match:
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
							int nMatchNumber = Rounds[r][m].NextLoserMatchNumber;
							int nRIndex = (r == Rounds.Count - 1) ? (r * 2 - 1) : (r * 2);
							foreach (IMatch match in LowerRounds[nRIndex])
							{
								if (match.MatchNumber == nMatchNumber &&
									match.PrevMatchNumbers.Contains(Rounds[r][m].MatchNumber))
								{
									match.AddPlayer(loserIndex, 0);
									return;
								}
							}
						}
					}
				}
			}

			// Is _match in lower bracket?
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
										}
									}
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
