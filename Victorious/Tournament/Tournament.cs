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
			winsNeeded = 0;
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
			int firstRoundIndex = listRounds.Count - 1;
			int numByes = totalTeams - (2 * listRounds[firstRoundIndex].Count);

			// Add teams to first round games
			for (int rIndex = 0, tIndex = 0;
				rIndex < (listRounds[firstRoundIndex].Count / 2);
				++rIndex, ++tIndex)
			{
				(listRounds[firstRoundIndex])[rIndex].AddTeam(listTeams[numByes + tIndex]);
				(listRounds[firstRoundIndex])[rIndex].AddTeam(listTeams[listTeams.Count - 1 - tIndex]);

				++tIndex;
				(listRounds[firstRoundIndex])[listRounds[firstRoundIndex].Count - 1 - rIndex]
					.AddTeam(listTeams[numByes + tIndex]);
				(listRounds[firstRoundIndex])[listRounds[firstRoundIndex].Count - 1 - rIndex]
					.AddTeam(listTeams[listTeams.Count - 1 - tIndex]);
			}

			// Add teams to second round games
			// ....

		}
	}
}
