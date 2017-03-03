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
			int rIndex = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				Rounds.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, rIndex) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.RoundIndex = rIndex;
					m.MatchIndex = Rounds[rIndex].Count;
					m.WinsNeeded = _winsPerMatch;
					AddMatch(rIndex, m);
				}
				++rIndex;
			}

			// Assign Match Numbers
			int matchNum = 1;
			for (rIndex = Rounds.Count - 1; rIndex >= 0; --rIndex)
			{
				foreach (IMatch match in Rounds[rIndex])
				{
					match.MatchNumber = matchNum++;
				}
			}

			// Tie Matches Together
			for (rIndex = 0; rIndex + 1 < Rounds.Count; ++rIndex)
			{
				if (Rounds[rIndex + 1].Count == (Rounds[rIndex].Count * 2))
				{
					// "Normal" rounds: twice as many matchups
					for (int mIndex = 0; mIndex < Rounds[rIndex].Count; ++mIndex)
					{
						int currNum = Rounds[rIndex][mIndex].MatchNumber;

						// Assign prev/next matchup numbers
						Rounds[rIndex][mIndex].AddPrevMatchNumber
							(Rounds[rIndex + 1][mIndex * 2].MatchNumber);
						Rounds[rIndex + 1][mIndex * 2].NextMatchNumber = currNum;

						Rounds[rIndex][mIndex].AddPrevMatchNumber
							(Rounds[rIndex + 1][mIndex * 2 + 1].MatchNumber);
						Rounds[rIndex + 1][mIndex * 2 + 1].NextMatchNumber = currNum;
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

			for (rIndex = 0; rIndex + 1 < Rounds.Count; ++rIndex)
			{
				// We're shifting back one player for each match in the prev round
				int prevRoundMatches = Rounds[rIndex + 1].Count;

				if ((Rounds[rIndex].Count * 2) > prevRoundMatches)
				{
					// Abnormal round ahead: we need to allocate prevMatchIndexes
					// to correctly distribute bye seeds

					int prevMatchNumber = 1;

					for (int mIndex = 0; mIndex < Rounds[rIndex].Count; ++mIndex)
					{
						foreach (int p in Rounds[rIndex][mIndex].PlayerIndexes)
						{
							if (p >= pIndex - prevRoundMatches)
							{
								Rounds[rIndex][mIndex].AddPrevMatchNumber(prevMatchNumber);
								Rounds[rIndex + 1][prevMatchNumber - 1].NextMatchNumber =
									Rounds[rIndex][mIndex].MatchNumber;
								++prevMatchNumber;
							}
						}
					}
				}

				for (int mIndex = 0; mIndex < Rounds[rIndex].Count; ++mIndex)
				{
					// For each match, shift/reassign all teams to the prev bracket level
					// If prev level is abnormal, only shift 1 (or 0) teams
					if (1 <= Rounds[rIndex][mIndex].PrevMatchNumbers.Count)
					{
						int prevIndex = 0;

						if (2 == Rounds[rIndex][mIndex].PrevMatchNumbers.Count)
						{
							ReassignPlayer(
								Rounds[rIndex][mIndex].PlayerIndexes[0],
								Rounds[rIndex][mIndex],
								Rounds[rIndex][mIndex].PrevMatchNumbers[prevIndex++]);
						}
						ReassignPlayer(
							Rounds[rIndex][mIndex].PlayerIndexes[1],
							Rounds[rIndex][mIndex],
							Rounds[rIndex][mIndex].PrevMatchNumbers[prevIndex]);
					}
				}

				for (int prePlayers = pIndex - 1; prePlayers >= 0; --prePlayers)
				{
					for (int mIndex = 0; mIndex < prevRoundMatches; ++mIndex)
					{
						if (Rounds[rIndex + 1][mIndex].PlayerIndexes.Contains(prePlayers))
						{
							// Add prev round's teams (according to seed) from the master list
							Rounds[rIndex + 1][mIndex].AddPlayer(pIndex++, 1);
							break;
						}
					}
				}
			}
			#endregion
		}

		public override void UpdateCurrentMatches(ICollection<MatchModel> _matchModels)
		{
			for (int rIndex = Rounds.Count - 1; rIndex >= 0; --rIndex)
			{
				for (int mIndex = 0; mIndex < Rounds[rIndex].Count; ++mIndex)
				{
					foreach (MatchModel model in _matchModels)
					{
						if (model.MatchNumber == Rounds[rIndex][mIndex].MatchNumber)
						{
							Rounds[rIndex][mIndex] = new Match(model, Players);
							break;
						}
					}
				}
			}
		}

		public override void AddWin(int _roundIndex, int _matchIndex, int _index)
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
								match.PlayerIndexes[i] = Rounds[_roundIndex][_matchIndex].PlayerIndexes[_index];
								return;
							}
						}
					}
					break;
				}
			}
		}
		public override void AddWin(IMatch _match, int _index)
		{
			// Just find the appropriate indexes of _match, and call the overloaded AddWin()
			for (int r = 0; r < Rounds.Count; ++r)
			{
				for (int m = 0; m < Rounds[r].Count; ++m)
				{
					if (Rounds[r][m] == _match)
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
#endregion
	}
}
