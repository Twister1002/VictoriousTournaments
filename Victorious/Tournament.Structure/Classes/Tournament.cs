using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class Tournament
	{
		#region Variables
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
		#endregion
		#region Properties
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
		#endregion

		#region Ctors
		public Tournament()
			: this(0, "", new List<IPlayer>(), new List<IBracket>(), 0, false)
		{ }
		public Tournament(uint _id)
			: this(_id, "", new List<IPlayer>(), new List<IBracket>(), 0, false)
		{ }
		public Tournament(uint _id, string _title, List<IPlayer> _players, List<IBracket> _brackets, float _purse, bool _isPublic)
		{
			id = _id;
			title = _title;
			players = _players;
			brackets = _brackets;
			prizePurse = _purse;
			isPublic = _isPublic;
		}
		#endregion
		#region Public Methods
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
		public bool CreateSingleElimBracket()
		{
			Brackets.Clear();

			Brackets.Add(new SingleElimBracket(Players));
			return true;
		}
		#endregion
	}
}
