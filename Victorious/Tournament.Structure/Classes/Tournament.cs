using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class Tournament : ITournament
	{
		#region Variables & Properties
		//private uint id;
		//public uint Id
		//{
		//	get { return id; }
		//}
		public string Title
		{ get; set; }
		public string Description
		{ get; set; }
		public List<IPlayer> Players
		{ get; set; }
		public List<IBracket> Brackets
		{ get; set; }
		public float PrizePool
		{ get; set; }
		public bool IsPublic
		{ get; set; }
		#endregion

		#region Ctors
		public Tournament()
			: this("", new List<IPlayer>(), new List<IBracket>(), 0, false)
		{ }
		//public Tournament(uint _id)
		//	: this(_id, "", new List<IPlayer>(), new List<IBracket>(), 0, false)
		//{ }
		public Tournament(/*uint _id,*/ string _title, List<IPlayer> _players, List<IBracket> _brackets, float _pool, bool _isPublic)
		{
			//id = _id;
			Title = _title;
			Players = _players;
			Brackets = _brackets;
			PrizePool = _pool;
			IsPublic = _isPublic;
		}
		public Tournament(TournamentModel _t)
		{
			DatabaseInterface db = new DatabaseInterface();

			Title = _t.Title;
			Description = _t.Description;

			Players = new List<IPlayer>();
			List<UserModel> users = db.GetAllUsersInTournament(_t.TournamentID);
			foreach (UserModel model in users)
			{
				Players.Add(new User(model));
			}

			Brackets = new List<IBracket>();
			switch (_t.TournamentRules.Bracket.BracketType)
			{
				case ("single"):
					CreateSingleElimBracket();
					break;
				case ("double"):
					CreateDoubleElimBracket();
					break;
				default:

					break;
			}
			foreach(IBracket bracket in Brackets)
			{
				bracket.FetchMatches(_t.TournamentID);
			}

			PrizePool = (null == _t.TournamentRules.PrizePurse)
				? 0.0f : (float)(_t.TournamentRules.PrizePurse);
			IsPublic = (null == _t.TournamentRules.IsPublic)
				? false : (bool)(_t.TournamentRules.IsPublic);
		}
		#endregion

		#region Public Methods
		public bool AddPlayer(IPlayer _p)
		{
			foreach (IPlayer p in Players)
			{
				if (p == _p)
				{
					return false;
				}
			}
			Players.Add(_p);
			return true;
		}
		public bool AddBracket(IBracket _b)
		{
			foreach (IBracket b in Brackets)
			{
				if (b == _b)
				{
					return false;
				}
			}
			Brackets.Add(_b);
			return true;
		}
		public bool CreateSingleElimBracket()
		{
			Brackets.Clear();

			Brackets.Add(new SingleElimBracket(Players));
			return true;
		}
		public bool CreateDoubleElimBracket()
		{
			Brackets.Clear();

			Brackets.Add(new DoubleElimBracket(Players));
			return true;
		}
		#endregion
	}
}
