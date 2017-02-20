using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassData
{
	public class Matchup
	{
		private int winsNeeded; // default 1
		private Team[] teams = { null, null };
		//private int[] teamIndexes = { -1, -1 };
		private int[] score = { 0, 0 };
		private List<int> prevMatchupIndexes;
		private int nextMatchupIndex;

		public Matchup()
		{
			winsNeeded = 1;
			prevMatchupIndexes = new List<int>();
			nextMatchupIndex = -1;
		}

		// Properties/Accessors
		public Team[] Teams
		{
			get { return teams; }
			set { teams = value; }
		}
		//public int[] TeamIndexes
		//{
		//	get { return teamIndexes; }
		//}
		public List<int> PrevMatchupIndexes
		{
			get { return prevMatchupIndexes; }
		}
		public int NextMatchupIndex
		{
			get { return nextMatchupIndex; }
			set { nextMatchupIndex = value; }
		}

		// Mutators/Modifiers
		public void AddPrevMatchupIndex(int _i)
		{
			prevMatchupIndexes.Add(_i);
		}
		public bool AddTeam(Team _t)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (null == Teams[i])
				{
					Teams[i] = _t;
					return true;
				}
			}
			return false;
		}
		//public bool AddTeam(int _teamIndex)
		//{
		//	if (-1 == teamIndexes[0])
		//	{
		//		teamIndexes[0] = _teamIndex;
		//		return true;
		//	}
		//	else if (-1 == teamIndexes[1])
		//	{
		//		teamIndexes[1] = _teamIndex;
		//		return true;
		//	}
		//	return false;
		//}
		public void RemoveTeams()
		{
			//teamIndexes[0] = teamIndexes[1] = -1;
			Teams[0] = Teams[1] = null;
		}
		//public bool RemoveTeam(int _teamIndex)
		//{
		//	if (teamIndexes[0] == _teamIndex)
		//	{
		//		teamIndexes[0] = -1;
		//		return true;
		//	}
		//	else if (teamIndexes[1] == _teamIndex)
		//	{
		//		teamIndexes[1] = -1;
		//		return true;
		//	}
		//	return false;
		//}
		public bool RemoveTeam(Team _t)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (null != Teams[i]
					&& _t.Id == Teams[i].Id)
				{
					Teams[i] = null;
					return true;
				}
			}
			return false;
		}
	}
}
