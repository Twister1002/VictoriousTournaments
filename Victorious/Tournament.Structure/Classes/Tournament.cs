using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class Tournament
	{
		// Variables
		private uint id;
		private string title;
		private string description;
		private List<IPlayer> players;
		private List<IBracket> brackets;
		//createdOn
		//createdById
		//winnerId
		//lastEditedOn
		//lastEditedById
		private float prizePurse;
		private bool isPublic;
		//cutoffDate
		//startDate
		//endDate

		// Properties
		public uint Id
		{
			get { return id; }
		}
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
		public List<IPlayer> Players
		{
			get { return players; }
			set { players = value; }
		}
		public List<IBracket> Brackets
		{
			get { return brackets; }
			set { brackets = value; }
		}
		public float PrizePurse
		{
			get { return prizePurse; }
			set { prizePurse = value; }
		}
		public bool IsPublic
		{
			get { return isPublic; }
			set { isPublic = value; }
		}

		// Ctors
		public Tournament(uint _id)
		{
			id = _id;
			title = "";
			players = new List<IPlayer>();
			brackets = new List<IBracket>();
			prizePurse = 0;
			isPublic = false;
		}

		// Methods
		public bool AddPlayer(IPlayer _p)
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
		public bool AddBracket(IBracket _b)
		{
			foreach (IBracket b in Brackets)
			{
				if (b.Id == _b.Id)
				{
					return false;
				}
			}
			brackets.Add(_b);
			return true;
		}
	}
}
