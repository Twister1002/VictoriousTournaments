using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public abstract class Bracket : IBracket
	{
		#region Variables & Properties
		public abstract List<IPlayer> Players { get; set; }
		public abstract List<List<IMatch>> Rounds { get; set; }
		#endregion

		#region Abstract Methods
		public abstract void CreateBracket(ushort _winsPerMatch = 1);
		public abstract void UpdateCurrentMatches(ICollection<MatchModel> _matchModels);
		public abstract void AddWin(int _roundIndex, int _matchIndex, int _index);
		public abstract void AddWin(IMatch _match, int _index);
		#endregion

		#region Public Methods
		public List<IMatch> GetRound(int _index)
		{
			if (_index < 0 || _index >= Rounds.Count)
			{
				throw new IndexOutOfRangeException();
			}

			return Rounds[_index];
		}
		public IMatch GetMatch(int _roundIndex, int _index)
		{
			List<IMatch> matches = GetRound(_roundIndex);

			if (_index < 0 || _index >= matches.Count)
			{
				throw new IndexOutOfRangeException();
			}

			return matches[_index];
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
		protected void AddRound()
		{
			if (null == Rounds)
			{
				throw new NullReferenceException();
			}

			Rounds.Add(new List<IMatch>());
		}
		protected void AddMatch(int _roundIndex, IMatch _m)
		{
			if (null == _m || null == Rounds)
			{
				throw new NullReferenceException();
			}
			if (_roundIndex >= Rounds.Count || _roundIndex < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			//foreach (List<IMatch> r in Rounds)
			//{
			//	if (r.Contains(_m))
			//	{
			//		throw new DuplicateObjectException();
			//	}
			//}

			Rounds[_roundIndex].Add(_m);
		}
		#endregion
	}
}
