using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class SingleElimBracket : Bracket
	{
		#region Variables & Properties
		// Variables
		private uint id;
		private List<IPlayer> players;
		private List<List<IMatch>> rounds;
		// Properties
		public override uint Id
		{
			get { return id; }
		}
		public override List<IPlayer> Players
		{
			get { return players; }
			set { players = value; }
		}
		public override List<List<IMatch>> Rounds
		{
			get { return rounds; }
			set { rounds = value; }
		}
		#endregion

		#region Ctors
		public SingleElimBracket(uint _id)
		{
			id = _id;
			players = new List<IPlayer>();
			rounds = new List<List<IMatch>>();
			//rounds.Add(new List<IMatch>());
		}
		public SingleElimBracket(List<IPlayer> _players)
		{
			id = 0;
			players = _players;
			rounds = new List<List<IMatch>>();

			CreateBracket();
		}
		#endregion
		#region Public Methods
		public override bool AddPlayer(IPlayer _p)
		{
			foreach (IPlayer p in Players)
			{
				if (p.Id == _p.Id)
				{
					return false;
				}
			}
			players.Add(_p);
			return true;
		}
		public override void AddRound()
		{
			rounds.Add(new List<IMatch>());
		}
		public override bool AddMatch(int _roundIndex)
		{
			//rounds[_roundIndex].Add(new Match());
			return false;
		}
		public override bool AddMatch(int _roundIndex, IMatch _m)
		{
			if (_roundIndex >= rounds.Count)
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
			rounds[_roundIndex].Add(_m);
			return true;
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
		private void CreateBracket()
		{
			#region Create the Bracket
			int totalMatches = players.Count - 1;
			int numMatches = 0;
			int roundIndex = 0;
			while (numMatches < totalMatches)
			{
				rounds.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, roundIndex) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					int mIndex = rounds[roundIndex].Count;
					rounds[roundIndex].Add(
						new Match(0,				// id
							1,						// winsNeeded
							new int[2] { -1, -1 },	// playerIndexes
							new ushort[2] { 0, 0 }, // score
							(int)Id,				// bracketId
							roundIndex,				// roundNumber
							mIndex,					// matchIndex
							new List<int>(),		// prevMatchesIndex
							-1));					// nextMatchIndex
				}
				++roundIndex;
			}

			for (int rIndex = 0; rIndex + 1 < rounds.Count; ++rIndex)
			{
				if (rounds[rIndex + 1].Count == (rounds[rIndex].Count * 2))
				{
					// "Normal" rounds: twice as many matchups
					for (int mIndex = 0; mIndex < rounds[rIndex].Count; ++mIndex)
					{
						// Assign prev/next matchup indexes
						rounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2);
						rounds[rIndex + 1][mIndex * 2].NextMatchIndex = mIndex;

						rounds[rIndex][mIndex].AddPrevMatchIndex(mIndex * 2 + 1);
						rounds[rIndex + 1][mIndex * 2 + 1].NextMatchIndex = mIndex;
					}
				}
				// Else: round is abnormal. Ignore it for now (we'll handle it later)
			}
			#endregion

			#region Assign the Players
			// Assign top two seeds to final match
			int pIndex = 0;
			rounds[0][0].AddPlayer(pIndex++);
			rounds[0][0].AddPlayer(pIndex++);

			for (int rIndex = 0; rIndex + 1 < rounds.Count; ++rIndex)
			{
				// We're shifting back one player for each match in the prev round
				int prevRoundMatches = rounds[rIndex + 1].Count;

				if ((rounds[rIndex].Count * 2) > prevRoundMatches)
				{
					// Abnormal round ahead: we need to allocate prevMatchIndexes
					// to correctly distribute bye seeds

					int prevMatchIndex = 0;

					for (int mIndex = 0; mIndex < rounds[rIndex].Count; ++mIndex)
					{
						foreach (int p in rounds[rIndex][mIndex].PlayerIndexes)
						{
							if (p >= pIndex - prevRoundMatches)
							{
								rounds[rIndex][mIndex].AddPrevMatchIndex(prevMatchIndex);
								rounds[rIndex + 1][prevMatchIndex].NextMatchIndex = mIndex;
								++prevMatchIndex;
							}
						}
					}
				}

				for (int mIndex = 0; mIndex < rounds[rIndex].Count; ++mIndex)
				{
					// For each match, shift/reassign all teams to the prev bracket level
					// If prev level is abnormal, only shift 1 (or 0) teams
					if (1 <= rounds[rIndex][mIndex].PrevMatchIndexes.Count)
					{
						int prevIndex = 0;

						if (2 == rounds[rIndex][mIndex].PrevMatchIndexes.Count)
						{
							ReassignPlayer(rounds[rIndex][mIndex].PlayerIndexes[0],
								rounds[rIndex][mIndex],
								rounds[rIndex + 1][(rounds[rIndex][mIndex].PrevMatchIndexes[prevIndex])]);
						}
						ReassignPlayer(rounds[rIndex][mIndex].PlayerIndexes[1],
							rounds[rIndex][mIndex],
							rounds[rIndex + 1][(rounds[rIndex][mIndex].PrevMatchIndexes[prevIndex])]);
					}
				}

				for (int prePlayers = pIndex - 1; prePlayers >= 0; --prePlayers)
				{
					for (int mIndex = 0; mIndex < prevRoundMatches; ++mIndex)
					{
						if (rounds[rIndex + 1][mIndex].PlayerIndexes.Contains(prePlayers))
						{
							// Add prev round's teams (according to seed) from the master list
							rounds[rIndex + 1][mIndex].AddPlayer(pIndex++);
							break;
						}
					}
				}
			}
			#endregion
		}
		#endregion
	}
}
