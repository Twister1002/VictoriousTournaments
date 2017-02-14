using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassData
{
	public class Team
	{
		// stuff
	}

	public class Tournament
	{
		private int totalTeams;
		private List<Team> listTeams;
		private List<List<Matchup>> listRounds; // each List<Matchup> is a particular round.

		public Tournament(int _teams)
		{
			totalTeams = _teams;
			listTeams = new List<Team>();
			for (int i = 0; i < totalTeams; ++i)
			{
				listTeams.Add(new Team());
			}
			listRounds = new List<List<Matchup>>();

			CreateBracket();
			AssignTeams();
		}

		public void PrintInfo()
		{
			for (int i = 1; i <= listRounds.Count; ++i)
			{
				if (1 == i)
				{
					Console.WriteLine("Byes: " + (totalTeams - listRounds[listRounds.Count - i].Count * 2));
				}
				Console.WriteLine("Round " + i + " matches: " + listRounds[listRounds.Count - i].Count);
			}

			Console.WriteLine("\n\nFirst Round Matches:");
			Console.WriteLine("---------------");
			foreach (Matchup m in listRounds[listRounds.Count - 1])
			{
				foreach (int tIndex in m.TeamIndexes)
				{
					Console.WriteLine("Seed: " + (tIndex + 1));
				}
				Console.WriteLine();
			}
			Console.WriteLine("\nSecond Round Matches:");
			Console.WriteLine("---------------");
			foreach(Matchup m in listRounds[listRounds.Count - 2])
			{
				foreach (int tIndex in m.TeamIndexes)
				{
					string seedStr = (tIndex >= 0) ? (tIndex + 1).ToString() : "winner";
					Console.WriteLine("Seed: " + seedStr);
				}
				Console.WriteLine();
			}
		}

		private void CreateBracket()
		{
			int totalMatches = totalTeams - 1;

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
				// Else: round is abnormal, ignore it for now (we'll handle it later)
			}
		}

		private void AssignTeams()
		{
			// Assign top two seeds to final match
			int tIndex = 0;
			listRounds[0][0].AddTeam(tIndex++);
			listRounds[0][0].AddTeam(tIndex++);

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
						foreach (int t in listRounds[rIndex][mIndex].TeamIndexes)
						{
							if (t >= tIndex - prevRoundMatchups)
							{
								listRounds[rIndex][mIndex].AddPrevMatchupIndex(prevMatchupIndex);
								listRounds[rIndex + 1][prevMatchupIndex].NextMatchupIndex = mIndex;
								++prevMatchupIndex;
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
						int pIndex = 0;

						if (2 == listRounds[rIndex][mIndex].PrevMatchupIndexes.Count)
						{
							ReassignTeam(listRounds[rIndex][mIndex].TeamIndexes[0],
								listRounds[rIndex][mIndex],
								listRounds[rIndex + 1][(listRounds[rIndex][mIndex].PrevMatchupIndexes[pIndex++])]);
						}
						ReassignTeam(listRounds[rIndex][mIndex].TeamIndexes[1],
							listRounds[rIndex][mIndex],
							listRounds[rIndex + 1][(listRounds[rIndex][mIndex].PrevMatchupIndexes[pIndex])]);
					}
				}

				for (int preTeams = tIndex - 1; preTeams >= 0; --preTeams)
				{
					for (int mIndex = 0; mIndex < prevRoundMatchups; ++mIndex)
					{
						if (listRounds[rIndex + 1][mIndex].TeamIndexes.Contains(preTeams))
						{
							// Add previous round's teams (according to seed) from the master list
							listRounds[rIndex + 1][mIndex].AddTeam(tIndex++);
							break;
						}
					}
				}
			}
		}

		private bool ReassignTeam(int _tIndex, Matchup _currMatchup, Matchup _newMatchup)
		{
			if (_currMatchup.TeamIndexes.Contains(_tIndex))
			{
				_currMatchup.RemoveTeam(_tIndex);
				_newMatchup.AddTeam(_tIndex);
				if (_newMatchup.TeamIndexes.Contains(_tIndex))
				{
					return true;
				}
			}
			return false;
		}

	}
}
