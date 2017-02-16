using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class SingleElimBracket : Bracket
	{
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

		// Ctors
		public SingleElimBracket(uint _id)
		{
			id = _id;
			players = new List<IPlayer>();
			rounds = new List<List<IMatch>>();
			//rounds.Add(new List<IMatch>());
		}

		// Methods
		public bool AddPlayer(IPlayer _p)
		{
			foreach(IPlayer p in Players)
			{
				if(p.Id == _p.Id)
				{
					return false;
				}
			}
			players.Add(_p);
			return true;
		}
		public void AddRound()
		{
			rounds.Add(new List<IMatch>());
		}
		public bool AddMatch(int _roundIndex)
		{
			//rounds[_roundIndex].Add(new Match());
			return false;
		}
		public bool AddMatch(int _roundIndex, IMatch _m)
		{
			if(_roundIndex >= rounds.Count)
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
	}
}
