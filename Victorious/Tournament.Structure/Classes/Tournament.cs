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
		{ get; private set; }
		public List<IBracket> Brackets
		{ get; private set; }
		public float PrizePool
		{ get; set; }
		public bool IsPublic
		{ get; set; }
		#endregion

		#region Ctors
		public Tournament(string _title, string _description, List<IPlayer> _players, List<IBracket> _brackets, float _pool, bool _isPublic)
		{
			Title = _title;
			Description = _description;
			Players = _players;
			Brackets = _brackets;
			PrizePool = _pool;
			IsPublic = _isPublic;
		}
		public Tournament()
			: this("", "", new List<IPlayer>(), new List<IBracket>(), 0.0f, false)
		{ }

		public Tournament(TournamentModel _t)
		{
			Title = _t.Title;
			Description = _t.Description;

			Players = new List<IPlayer>();
			foreach (UserModel model in _t.Users)
			{
				Players.Add(new User(model));
			}

			Brackets = new List<IBracket>();
			foreach(BracketModel bModel in _t.Brackets)
			{
				// THIS WILL NEED TO BE UPDATED
				switch (bModel.BracketType)
				{
					case ("single"):
						AddSingleElimBracket(Players);
						break;
					case ("double"):
						AddDoubleElimBracket(Players);
						break;

					default:
						break;
				}

				Brackets[Brackets.Count - 1].UpdateCurrentMatches(bModel.Matches);
			}

			PrizePool = (null == _t.TournamentRules.PrizePurse)
				? 0.0f : (float)(_t.TournamentRules.PrizePurse);
			IsPublic = (null == _t.TournamentRules.IsPublic)
				? false : (bool)(_t.TournamentRules.IsPublic);
		}
		#endregion

		#region Public Methods
		public int NumberOfPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			return Players.Count;
		}
		public void AddPlayer(IPlayer _p)
		{
			if (null == _p || null == Players)
			{
				throw new NullReferenceException();
			}
			if (Players.Contains(_p))
			{
				throw new DuplicateObjectException();
			}

			Players.Add(_p);
		}
		public void RemovePlayer(IPlayer _p)
		{
			if (null == _p || null == Players)
			{
				throw new NullReferenceException();
			}
			if (!Players.Remove(_p))
			{
				throw new KeyNotFoundException();
			}
		}
		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			Players.Clear();
		}

		public int NumberOfBrackets()
		{
			if (null == Brackets)
			{
				Brackets = new List<IBracket>();
			}
			return Brackets.Count;
		}
		public void AddBracket(IBracket _b)
		{
			if (null == _b || null == Brackets)
			{
				throw new NullReferenceException();
			}
			if (Brackets.Contains(_b))
			{
				throw new DuplicateObjectException();
			}

			Brackets.Add(_b);
		}
		public void RemoveBracket(IBracket _b)
		{
			if (null == _b || null == Brackets)
			{
				throw new NullReferenceException();
			}
			if (!Brackets.Remove(_b))
			{
				throw new KeyNotFoundException();
			}
		}
		public void ResetBrackets()
		{
			if (null == Brackets)
			{
				Brackets = new List<IBracket>();
			}
			Brackets.Clear();
		}

		public void AddSingleElimBracket(List<IPlayer> _playerList)
		{
			Brackets.Add(new SingleElimBracket(_playerList));
		}
		public void AddSingleElimBracket(int _numPlayers)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddSingleElimBracket(pList);
		}
		public void AddDoubleElimBracket(List<IPlayer> _playerList)
		{
			Brackets.Add(new DoubleElimBracket(_playerList));
		}
		public void AddDoubleElimBracket(int _numPlayers)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddDoubleElimBracket(pList);
		}
		#endregion
	}
}
