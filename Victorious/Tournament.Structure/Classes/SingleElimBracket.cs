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
		public override List<IPlayer> Players
		{ get; set; }
		public override List<List<IMatch>> Rounds
		{ get; set; }
		#endregion

		#region Ctors
		public SingleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public SingleElimBracket(List<IPlayer> _players)
		{
			if (null == _players)
			{
				throw new NullReferenceException();
			}

			Players = _players;
			Rounds = new List<List<IMatch>>();
			CreateBracket();
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			if (Players.Count < 2)
			{
				throw new ArgumentOutOfRangeException();
			}

			#region Create the Bracket
			Rounds.Clear();
			int totalMatches = Players.Count - 1;
			int numMatches = 0;
			int r = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				Rounds.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, r) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.RoundIndex = r;
					m.MatchIndex = Rounds[r].Count;
					m.WinsNeeded = _winsPerMatch;
					AddMatch(r, m);
				}
				++r;
			}

			// Assign Match Numbers
			int matchNum = 1;
			for (r = Rounds.Count - 1; r >= 0; --r)
			{
				foreach (IMatch match in Rounds[r])
				{
					match.MatchNumber = matchNum++;
				}
			}

			// Tie Matches Together
			for (r = 0; r + 1 < Rounds.Count; ++r)
			{
				if (Rounds[r + 1].Count == (Rounds[r].Count * 2))
				{
					// "Normal" rounds: twice as many matchups
					for (int m = 0; m < Rounds[r].Count; ++m)
					{
						int currNum = Rounds[r][m].MatchNumber;

						// Assign prev/next matchup numbers
						Rounds[r][m].AddPrevMatchNumber(Rounds[r + 1][m * 2].MatchNumber);
						Rounds[r + 1][m * 2].NextMatchNumber = currNum;

						Rounds[r][m].AddPrevMatchNumber(Rounds[r + 1][m * 2 + 1].MatchNumber);
						Rounds[r + 1][m * 2 + 1].NextMatchNumber = currNum;
					}
				}
				// Else: round is abnormal. Ignore it for now (we'll handle it later)
			}
			#endregion

			#region Assign the Players
			// Assign top two seeds to final match
			int pIndex = 0;
			Rounds[0][0].AddPlayer(pIndex++, 0);
			Rounds[0][0].AddPlayer(pIndex++, 1);

			for (r = 0; r + 1 < Rounds.Count; ++r)
			{
				// We're shifting back one player for each match in the prev round
				int prevRoundMatches = Rounds[r + 1].Count;

				if ((Rounds[r].Count * 2) > prevRoundMatches)
				{
					// Abnormal round ahead: we need to allocate prevMatchIndexes
					// to correctly distribute bye seeds

					int prevMatchNumber = 1;

					for (int m = 0; m < Rounds[r].Count; ++m)
					{
						foreach (int p in Rounds[r][m].PlayerIndexes)
						{
							if (p >= pIndex - prevRoundMatches)
							{
								Rounds[r][m].AddPrevMatchNumber(prevMatchNumber);
								Rounds[r + 1][prevMatchNumber - 1].NextMatchNumber =
									Rounds[r][m].MatchNumber;
								++prevMatchNumber;
							}
						}
					}
				}

				for (int m = 0; m < Rounds[r].Count; ++m)
				{
					// For each match, shift/reassign all teams to the prev bracket level
					// If prev level is abnormal, only shift 1 (or 0) teams
					if (1 <= Rounds[r][m].PrevMatchNumbers.Count)
					{
						int prevIndex = 0;

						if (2 == Rounds[r][m].PrevMatchNumbers.Count)
						{
							ReassignPlayer(
								Rounds[r][m].PlayerIndexes[0],
								Rounds[r][m],
								Rounds[r][m].PrevMatchNumbers[prevIndex++]);
						}
						ReassignPlayer(
							Rounds[r][m].PlayerIndexes[1],
							Rounds[r][m],
							Rounds[r][m].PrevMatchNumbers[prevIndex]);
					}
				}

				for (int prePlayers = pIndex - 1; prePlayers >= 0; --prePlayers)
				{
					for (int m = 0; m < prevRoundMatches; ++m)
					{
						if (Rounds[r + 1][m].PlayerIndexes.Contains(prePlayers))
						{
							// Add prev round's teams (according to seed) from the master list
							Rounds[r + 1][m].AddPlayer(pIndex++, 1);
							break;
						}
					}
				}
			}
			#endregion
		}

		public override void UpdateCurrentMatches(ICollection<MatchModel> _matchModels)
		{
			for (int r = Rounds.Count - 1; r >= 0; --r)
			{
				for (int m = 0; m < Rounds[r].Count; ++m)
				{
					foreach (MatchModel model in _matchModels)
					{
						if (model.MatchNumber == Rounds[r][m].MatchNumber)
						{
							Rounds[r][m] = new Match(model, Players);
							break;
						}
					}
				}
			}
		}

		public override void AddWin(IMatch _match, int _index)
		{
			AddWin(_match.MatchNumber, _index);
		}
		public override void AddWin(int _matchNumber, int _index)
		{
			if (_index < 0 || _index > 1)
			{
				throw new IndexOutOfRangeException();
			}

			// Find the appropriate indexes of _match, and call the private AddWin()
			for (int r = 0; r < Rounds.Count; ++r)
			{
				for (int m = 0; m < Rounds[r].Count; ++m)
				{
					if (Rounds[r][m].MatchNumber == _matchNumber)
					{
						AddWin(r, m, _index);
						return;
					}
				}
			}

			throw new KeyNotFoundException();
		}
		#endregion

		#region Private Methods
		private void ReassignPlayer(int _pIndex, IMatch _currMatch, int _newMatchNum)
		{
			if (null == _currMatch || _newMatchNum < 1)
			{
				throw new NullReferenceException();
			}

			if (_currMatch.PlayerIndexes.Contains(_pIndex))
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

		protected void AddWin(int _roundIndex, int _matchIndex, int _index)
		{
			if (_roundIndex < 0 || _roundIndex >= Rounds.Count
				|| _matchIndex < 0 || _matchIndex >= Rounds[_roundIndex].Count)
			{
				throw new IndexOutOfRangeException();
			}

			Rounds[_roundIndex][_matchIndex].AddWin(_index);

			if (0 == _roundIndex)
			{
				return;
			}
			if (Rounds[_roundIndex][_matchIndex].Score[_index] >= Rounds[_roundIndex][_matchIndex].WinsNeeded)
			{
				// Player won the match. Advance!

				// Move the winner:
				int nmNumber = Rounds[_roundIndex][_matchIndex].NextMatchNumber;
				foreach (IMatch match in Rounds[_roundIndex - 1])
				{
					if (nmNumber == match.MatchNumber)
					{
						for (int i = 0; i < match.PrevMatchNumbers.Count; ++i)
						{
							if (Rounds[_roundIndex][_matchIndex].MatchNumber == match.PrevMatchNumbers[i])
							{
								int newIndex = (1 == match.PrevMatchNumbers.Count) ? 1 : i;
								match.AddPlayer(Rounds[_roundIndex][_matchIndex].PlayerIndexes[_index], newIndex);
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
