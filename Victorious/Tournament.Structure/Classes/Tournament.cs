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
		public Tournament(string _title, List<IPlayer> _players, List<IBracket> _brackets, float _pool, bool _isPublic)
		{
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
			foreach (UserModel model in _t.Users)
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
		public void AddPlayer(IPlayer _p)
		{
			if (Players.Contains(_p))
			{
				throw new DuplicateObjectException();
			}
			
			Players.Add(_p);
		}
		public void AddBracket(IBracket _b)
		{
			if (Brackets.Contains(_b))
			{
				throw new DuplicateObjectException();
			}
			
			Brackets.Add(_b);
		}
		public void CreateSingleElimBracket()
		{
			Brackets.Clear();
			Brackets.Add(new SingleElimBracket(Players));
		}
		public void CreateDoubleElimBracket()
		{
			Brackets.Clear();
			Brackets.Add(new DoubleElimBracket(Players));
		}
		#endregion
	}
}
