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

			Rounds.Clear();

			#region Create the Bracket
			int totalMatches = Players.Count - 1;
			int numMatches = 0;
			int roundIndex = 0;
			while (numMatches < totalMatches)
			{
				Rounds.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, roundIndex) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.RoundNumber = roundIndex;
					m.MatchIndex = Rounds[roundIndex].Count;
					m.WinsNeeded = _winsPerMatch;
					AddMatch(roundIndex, m);
				}
				++roundIndex;
			}

			for (int rIndex = 0; rIndex + 1 < Rounds.Count; ++rIndex)
			{
				if (Rounds[rIndex + 1].Count == (Rounds[rIndex].Count * 2))
				{
					// "Normal" rounds: twice as many matchups
					for (int mIndex = 0; mIndex < Rounds[rIndex].Count; ++mIndex)
					{
						// Assign prev/next matchup indexes
						Rounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2);
						Rounds[rIndex + 1][mIndex * 2].NextMatchIndex = mIndex;

						Rounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2 + 1);
						Rounds[rIndex + 1][mIndex * 2 + 1].NextMatchIndex = mIndex;
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

			for (int rIndex = 0; rIndex + 1 < Rounds.Count; ++rIndex)
			{
				// We're shifting back one player for each match in the prev round
				int prevRoundMatches = Rounds[rIndex + 1].Count;

				if ((Rounds[rIndex].Count * 2) > prevRoundMatches)
				{
					// Abnormal round ahead: we need to allocate prevMatchIndexes
					// to correctly distribute bye seeds

					int prevMatchIndex = 0;

					for (int mIndex = 0; mIndex < Rounds[rIndex].Count; ++mIndex)
					{
						foreach (int p in Rounds[rIndex][mIndex].PlayerIndexes)
						{
							if (p >= pIndex - prevRoundMatches)
							{
								Rounds[rIndex][mIndex].AddPrevMatchIndex(prevMatchIndex);
								Rounds[rIndex + 1][prevMatchIndex].NextMatchIndex = mIndex;
								++prevMatchIndex;
							}
						}
					}
				}

				for (int mIndex = 0; mIndex < Rounds[rIndex].Count; ++mIndex)
				{
					// For each match, shift/reassign all teams to the prev bracket level
					// If prev level is abnormal, only shift 1 (or 0) teams
					if (1 <= Rounds[rIndex][mIndex].PrevMatchIndexes.Count)
					{
						int prevIndex = 0;

						if (2 == Rounds[rIndex][mIndex].PrevMatchIndexes.Count)
						{
							ReassignPlayer(
								Rounds[rIndex][mIndex].PlayerIndexes[0],
								Rounds[rIndex][mIndex],
								Rounds[rIndex + 1][(Rounds[rIndex][mIndex].PrevMatchIndexes[prevIndex++])]);
						}
						ReassignPlayer(
							Rounds[rIndex][mIndex].PlayerIndexes[1],
							Rounds[rIndex][mIndex],
							Rounds[rIndex + 1][(Rounds[rIndex][mIndex].PrevMatchIndexes[prevIndex])]);
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
#if false
			for (int rIndex = 0; rIndex < Rounds.Count; ++rIndex)
			{
				for (int mIndex = 0; mIndex < Rounds[rIndex].Count; ++mIndex)
				{
					foreach (MatchModel model in _matchModels)
					{
						if (rIndex == model.RoundNumber && mIndex == model.MatchIndex)
						{
							Rounds[rIndex][mIndex] = new Match(model, Players);
							break;
						}
					}
				}
			}
#endif
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
				int nmIndex = Rounds[_roundIndex][_matchIndex].NextMatchIndex;
				for (int i = 0; i < Rounds[_roundIndex - 1][nmIndex].PrevMatchIndexes.Count; ++i)
				{
					if (_matchIndex == Rounds[_roundIndex - 1][nmIndex].PrevMatchIndexes[i])
					{
						Rounds[_roundIndex - 1][nmIndex].PlayerIndexes[i] = Rounds[_roundIndex][_matchIndex].PlayerIndexes[_index];
						return;
					}
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
		private void ReassignPlayer(int _pIndex, IMatch _currMatch, IMatch _newMatch)
		{
			if (null == _currMatch || null == _newMatch)
			{
				throw new NullReferenceException();
			}

			if (_currMatch.PlayerIndexes.Contains(_pIndex))
			{
				_currMatch.RemovePlayer(_pIndex);
				_newMatch.AddPlayer(_pIndex, 0);
				if (_newMatch.PlayerIndexes.Contains(_pIndex))
				{
					return;
				}
			}

			throw new KeyNotFoundException();
		}
#endregion
	}
}
