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

		public Matchup()
		{
			winsNeeded = 1;
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
		public Team[] Teams
		{
			get { return teams; }
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
					listRounds[roundIndex].Add(new Matchup());
				}
				++roundIndex;
			}
		}

		private void AssignTeams()
		{
			/////////////////////////////////////////
			// CURRENT STATE OF ALGORITHM:
			// should work for (power of 2) teams: NEEDS TESTING
			// does not work for bad numbers of teams
			/////////////////////////////////////////

			int tIndex = 0;
			listRounds[0][0].AddTeam(listTeams[tIndex++]);
			listRounds[0][0].AddTeam(listTeams[tIndex++]);

			for (int rIndex = 1; rIndex < listRounds.Count; ++rIndex)
			{
				for (int mIndex = 0; mIndex < listRounds[rIndex - 1].Count; ++mIndex)
				{
					// Move/add teams from future round
					listRounds[rIndex][mIndex * 2].AddTeam(listRounds[rIndex - 1][mIndex].Teams[0]);
					listRounds[rIndex][mIndex * 2 + 1].AddTeam(listRounds[rIndex - 1][mIndex].Teams[1]);
					// REMOVE teams from future round
					listRounds[rIndex - 1][mIndex].RemoveTeams();
				}

				for (int preTeams = tIndex - 1; preTeams >= 0; --preTeams)
				{
					for (int mIndex = 0; mIndex < listRounds[rIndex].Count; ++mIndex)
					{
						if (listRounds[rIndex][mIndex].Teams[0] == listTeams[preTeams])
						{
							// Add new round's teams (according to seed)
							listRounds[rIndex][mIndex].AddTeam(listTeams[tIndex++]);
							break;
						}
					}
				}
			}
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
