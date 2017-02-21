﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class SingleElimBracket : Bracket
	{
		#region Variables & Properties
		private uint id;
		public override uint Id
		{
			get { return id; }
		}
		public override List<IPlayer> Players
		{ get; set; }
		public override List<List<IMatch>> Rounds
		{ get; set; }
		#endregion

		#region Ctors
		public SingleElimBracket(uint _id)
		{
			id = _id;
			Players = new List<IPlayer>();
			Rounds = new List<List<IMatch>>();
			//rounds.Add(new List<IMatch>());
		}
		public SingleElimBracket(List<IPlayer> _players)
		{
			id = 0;
			Players = _players;
			Rounds = new List<List<IMatch>>();

			//CreateBracket();
		}
		#endregion
		#region Public Methods
		public override void CreateBracket()
		{
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
					int mIndex = Rounds[roundIndex].Count;
					Rounds[roundIndex].Add(
						new Match(0,                // id
							1,                      // winsNeeded
							new int[2] { -1, -1 },  // playerIndexes
							new ushort[2] { 0, 0 }, // score
							(int)Id,                // bracketId
							roundIndex,             // roundNumber
							mIndex,                 // matchIndex
							new List<int>(),        // prevMatchesIndex
							-1));                   // nextMatchIndex
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
			Rounds[0][0].AddPlayer(pIndex++);
			Rounds[0][0].AddPlayer(pIndex++);

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
								Rounds[rIndex + 1][(Rounds[rIndex][mIndex].PrevMatchIndexes[prevIndex])]);
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
							Rounds[rIndex + 1][mIndex].AddPlayer(pIndex++);
							break;
						}
					}
				}
			}
			#endregion
		}
		public override bool AddPlayer(IPlayer _p)
		{
			foreach (IPlayer p in Players)
			{
				if (p.Id == _p.Id)
				{
					return false;
				}
			}
			Players.Add(_p);
			return true;
		}
		public override void AddRound()
		{
			Rounds.Add(new List<IMatch>());
		}
		public override bool AddMatch(int _roundIndex)
		{
			//rounds[_roundIndex].Add(new Match());
			return false;
		}
		public override bool AddMatch(int _roundIndex, IMatch _m)
		{
			if (_roundIndex >= Rounds.Count)
			{
				return false;
			}
			foreach (List<IMatch> r in Rounds)
			{
				foreach (IMatch m in r)
				{
					if (m.Id == _m.Id)
					{
						return false;
					}
				}
			}
			Rounds[_roundIndex].Add(_m);
			return true;
		}
		public override void AddWin(int _roundIndex, int _matchIndex, int _index)
		{
			Rounds[_roundIndex][_matchIndex].AddWin(_index);

			if (Rounds[_roundIndex][_matchIndex].Score[_index] >= Rounds[_roundIndex][_matchIndex].WinsNeeded)
			{
				// Player won the match. Advance!
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
					if (Rounds[r][m].Id == _match.Id)
					{
						AddWin(r, m, _index);
					}
				}
			}
		}

		#endregion
		#region Private Methods
		private bool ReassignPlayer(int _pIndex, IMatch _currMatch, IMatch _newMatch)
		{
			if (_currMatch.PlayerIndexes.Contains(_pIndex))
			{
				_currMatch.RemovePlayer(_pIndex);
				_newMatch.AddPlayer(_pIndex);
				if (_newMatch.PlayerIndexes.Contains(_pIndex))
				{
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
