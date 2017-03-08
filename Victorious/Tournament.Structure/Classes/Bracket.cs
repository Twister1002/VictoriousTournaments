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
		public List<IPlayer> Players
		{ get; protected set; }
		protected List<List<IMatch>> Rounds
		{ get; set; }
		protected List<List<IMatch>> LowerRounds
		{ get; set; }
		protected IMatch GrandFinal
		{ get; set; }
		#endregion

		#region Abstract Methods
		public abstract void CreateBracket(ushort _winsPerMatch = 1);
		public abstract void UpdateCurrentMatches(ICollection<MatchModel> _matchModels);
		public abstract void AddWin(int _matchNumber, PlayerSlot _slot);
		//public abstract void AddWin(IMatch _match, PlayerSlot _slot);
		public abstract void SubtractWin(int _matchNumber, PlayerSlot _slot);
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
			ResetBracket();
		}
		public void ReplacePlayer(IPlayer _p, int _index)
		{
			if (null == _p)
			{
				throw new NullReferenceException();
			}
			if (_index < 0 || _index >= Players.Count)
			{
				throw new IndexOutOfRangeException();
			}

			Players[_index] = _p;
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

			ResetBracket();
		}
		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}

			Players.Clear();
			ResetBracket();
		}

		public int NumberOfRounds()
		{
			if (null == Rounds)
			{
				Rounds = new List<List<IMatch>>();
			}
			return Rounds.Count;
		}
		public List<IMatch> GetRound(int _round)
		{
			if (null == Rounds)
			{
				throw new NullReferenceException();
			}
			if (_round < 1 || _round > Rounds.Count)
			{
				throw new IndexOutOfRangeException();
			}

			return Rounds[Rounds.Count - _round];
		}
		public int NumberOfLowerRounds()
		{
			if (null == LowerRounds || 0 == LowerRounds.Count)
			{
				throw new NullReferenceException();
			}
			return LowerRounds.Count;
		}
		public List<IMatch> GetLowerRound(int _round)
		{
			if (null == LowerRounds || 0 == LowerRounds.Count)
			{
				throw new NullReferenceException();
			}
			if (_round < 1 || _round > LowerRounds.Count)
			{
				throw new IndexOutOfRangeException();
			}

			return LowerRounds[LowerRounds.Count - _round];
		}
		//public IMatch GetMatch(int _roundIndex, int _index)
		//{
		//	List<IMatch> matches = GetRound(_roundIndex);

		//	if (_index < 0 || _index >= matches.Count)
		//	{
		//		throw new IndexOutOfRangeException();
		//	}
		//	return matches[_index];
		//}
		public IMatch GetMatch(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new IndexOutOfRangeException();
			}

			foreach (List<IMatch> round in Rounds)
			{
				foreach (IMatch match in round)
				{
					if (match.MatchNumber == _matchNumber)
					{
						return match;
					}
				}
			}

			throw new KeyNotFoundException();
		}
		public void ResetBracket()
		{
			if (null == Rounds)
			{
				Rounds = new List<List<IMatch>>();
			}
			Rounds.Clear();
			LowerRounds = null;
			GrandFinal = null;
		}
		#endregion

#region Private Methods
#if false
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
			foreach (List<IMatch> r in Rounds)
			{
				if (r.Contains(_m))
				{
					throw new DuplicateObjectException();
				}
			}

			Rounds[_roundIndex].Add(_m);
		}
#endif
#endregion
	}
}
