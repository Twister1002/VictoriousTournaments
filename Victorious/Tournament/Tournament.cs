using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament
{
	public class Team
	{
		// stuff
	}

	public class Matchup
	{
		private int winsNeeded; // default 1
		private Team[] teams = { null, null };
		private int[] score = { 0, 0 };
		private List<int> prevMatchupIndexes;
		private int nextMatchupIndex;

		public Matchup()
		{
			winsNeeded = 1;

			nextMatchupIndex = -1;
		}
		public bool AddTeam(Team _t)
		{
			if (null == teams[0])
			{
				teams[0] = _t;
				return true;
			}
			else if (null == teams[1])
			{
				teams[1] = _t;
				return true;
			}
			return false;
		}
		public void RemoveTeams()
		{
			teams[0] = null;
			teams[1] = null;
		}
		public void RemoveTeam(Team t)
		{
			if (teams[0] == t)
			{
				teams[0] = null;
			}
			else if (teams[1] == t)
			{
				teams[1] = null;
			}
		}
		public Team[] Teams
		{
			get { return teams; }
		}
		public List<int> PrevMatchupIndexes
		{
			get { return prevMatchupIndexes; }
		}
		public void AddPrevMatchupIndex(int _i)
		{
			prevMatchupIndexes.Add(_i);
		}
		public int NextMatchupIndex
		{
			get { return nextMatchupIndex; }
			set { nextMatchupIndex = value; }
		}
	}

	public class Tournament
	{
		private int totalTeams;
		private List<Team> listTeams;
		private List<List<Matchup>> listRounds; // each List<Matchup> is a particular round.

		public Tournament(int _teams)
		{
			totalTeams = _teams;
			CreateBracket();
		}

		private bool RemoveTeamFromRound(Team t, int roundIndex)
		{
			foreach (Matchup m in listRounds[roundIndex])
			{
				if (m.Teams.Contains(t))
				{
					m.RemoveTeam(t);
					return true;
				}
			}
			return false;
		}
		private bool ReassignTeam(Team _t, Matchup _currMatchup, Matchup _newMatchup)
		{
			if (_currMatchup.Teams.Contains(_t))
			{
				_currMatchup.RemoveTeam(_t);
				_newMatchup.AddTeam(_t);
				if (_newMatchup.Teams.Contains(_t))
				{
					return true;
				}
			}
			return false;
		}

		public void PrintInfo()
		{
			for (int i = 1; i <= listRounds.Count; ++i)
			{
				if (1 == i)
				{
					Console.WriteLine("Byes: " + (totalTeams - listRounds[listRounds.Count - i].Count * 2));
				}
				Console.WriteLine("Round " + i + "matches: " + listRounds[listRounds.Count - i].Count);
			}
		}

		private void CreateBracket()
		{
			int totalMatches = totalTeams - 1;
			listRounds = new List<List<Matchup>>();

			int numMatches = 0;
			int roundIndex = 0;
			while (numMatches < totalMatches)
			{
				listRounds.Add(new List<Matchup>());
				for (int i = 0;
					i < Math.Pow(2, roundIndex) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round, depending on total number of teams/matches
					listRounds[roundIndex].Add(new Matchup());
				}
				++roundIndex;
			}

			for (int rIndex = 0; rIndex + 1 < listRounds.Count; ++rIndex)
			{
				if (listRounds[rIndex + 1].Count == (listRounds[rIndex].Count * 2))
				{
					// "Normal" rounds: twice as many matchups
					for (int mIndex = 0; mIndex < listRounds[rIndex].Count; ++mIndex)
					{
						// Assign prev/next matchup indexes
						listRounds[rIndex][mIndex].AddPrevMatchupIndex(mIndex * 2);
						listRounds[rIndex + 1][mIndex * 2].NextMatchupIndex = mIndex;

						listRounds[rIndex][mIndex].AddPrevMatchupIndex(mIndex * 2 + 1);
						listRounds[rIndex + 1][mIndex * 2 + 1].NextMatchupIndex = mIndex;
					}
				}
				// If round is abnormal, ignore it for now (we'll handle it later)
			}
		}

		private void AssignTeams()
		{
			/////////////////////////////////////////
			// CURRENT STATE OF ALGORITHM:
			// broke as fuck
			/////////////////////////////////////////

			// Assign top two seeds to final match
			int tIndex = 0;
			listRounds[0][0].AddTeam(listTeams[tIndex++]);
			listRounds[0][0].AddTeam(listTeams[tIndex++]);

			for (int rIndex = 0; rIndex + 1 < listRounds.Count; ++rIndex)
			{
				// We're moving back one team for each prevRoundMatchup:
				int prevRoundMatchups = listRounds[rIndex + 1].Count;

				if ((listRounds[rIndex].Count * 2) > prevRoundMatchups)
				{
					// Abnormal round ahead: we need to allocate prevMatchupIndexes...
					// to correctly distribute bye teams/seeds

					int prevMatchupIndex = 0;

					for (int mIndex = 0; mIndex < listRounds[rIndex].Count; ++mIndex)
					{
						foreach (Team t in listRounds[rIndex][mIndex].Teams)
						{
							for (int i = tIndex - 1 - prevRoundMatchups; i + 1 < tIndex; ++i)
							{
								if (listTeams[i] == t)
								{
									// "This" team will be pushed back (low seed), so we assign
									// a new index/pointer to help us
									listRounds[rIndex][mIndex].AddPrevMatchupIndex(prevMatchupIndex);
									listRounds[rIndex + 1][prevMatchupIndex].NextMatchupIndex = mIndex;
									++prevMatchupIndex;
									break;
								}
							}
						}
					}
				}

				for (int mIndex = 0; mIndex < listRounds[rIndex].Count; ++mIndex)
				{
					// For each matchup, shift/reassign all teams to the prev bracket level
					// If prev level is abnormally sized, only shift 1 (or 0) teams
					if (1 <= listRounds[rIndex][mIndex].PrevMatchupIndexes.Count)
					{
						if (2 == listRounds[rIndex][mIndex].PrevMatchupIndexes.Count)
						{
							ReassignTeam(listRounds[rIndex][mIndex].Teams[0],
								listRounds[rIndex][mIndex],
								listRounds[rIndex + 1][(listRounds[rIndex][mIndex].PrevMatchupIndexes[0])]);
						}
						ReassignTeam(listRounds[rIndex][mIndex].Teams[1],
							listRounds[rIndex][mIndex],
							listRounds[rIndex + 1][(listRounds[rIndex][mIndex].PrevMatchupIndexes[1])]);
					}
				}

				for (int preTeams = tIndex - 1; preTeams >= 0; --preTeams)
				{
					for (int mIndex = 0; mIndex < prevRoundMatchups; ++mIndex)
					{
						if (listRounds[rIndex + 1][mIndex].Teams.Contains(listTeams[preTeams]))
						{
							// Add previous round's teams (according to seed) from the master list
							listRounds[rIndex + 1][mIndex].AddTeam(listTeams[tIndex++]);
							break;
						}
					}
				}
			}

#if false
			for (int rIndex = 1; rIndex < listRounds.Count; ++rIndex)
			{
				if (listRounds[rIndex].Count == (listRounds[rIndex - 1].Count * 2))
				{
					// New round is "normal" (twice the games)
					for (int mIndex = 0; mIndex < listRounds[rIndex - 1].Count; ++mIndex)
					{
						// Move/add teams down from future round
						listRounds[rIndex][mIndex * 2].AddTeam(listRounds[rIndex - 1][mIndex].Teams[0]);
						listRounds[rIndex][mIndex * 2 + 1].AddTeam(listRounds[rIndex - 1][mIndex].Teams[1]);
						// REMOVE teams from future round
						listRounds[rIndex - 1][mIndex].RemoveTeams();
					}
				}
				else
				{
					// Abnormal round: Figure out the byes!
					int currTeamIndex = tIndex - 1;
					for (int i = 0; i < listRounds[rIndex].Count; ++i)
					{

					}
				}

			}
#endif
#if false
			//int firstRoundIndex = listRounds.Count - 1;
			//int numByes = totalTeams - (2 * listRounds[firstRoundIndex].Count);

			//// Add teams to first round games
			//for (int rIndex = 0, tIndex = 0;
			//	rIndex < (listRounds[firstRoundIndex].Count / 2);
			//	++rIndex, ++tIndex)
			//{
			//	(listRounds[firstRoundIndex])[rIndex].AddTeam(listTeams[numByes + tIndex]);
			//	(listRounds[firstRoundIndex])[rIndex].AddTeam(listTeams[listTeams.Count - 1 - tIndex]);

			//	++tIndex;
			//	(listRounds[firstRoundIndex])[listRounds[firstRoundIndex].Count - 1 - rIndex]
			//		.AddTeam(listTeams[numByes + tIndex]);
			//	(listRounds[firstRoundIndex])[listRounds[firstRoundIndex].Count - 1 - rIndex]
			//		.AddTeam(listTeams[listTeams.Count - 1 - tIndex]);
			//}

			// Add teams to second round games
			// ....
#endif
		}
	}
}
