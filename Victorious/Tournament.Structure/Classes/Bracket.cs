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
		public Dictionary<int, IMatch> Matches
		{ get; protected set; }
		public int NumberOfRounds
		{ get; protected set; }
		public Dictionary<int, IMatch> LowerMatches
		{ get; protected set; }
		public int NumberOfLowerRounds
		{ get; protected set; }
		public IMatch GrandFinal
		{ get; protected set; }
		public int NumberOfMatches
		{ get; protected set; }
		#endregion

		#region Abstract Methods
		public abstract void CreateBracket(ushort _winsPerMatch = 1);
		public abstract void AddWin(int _matchNumber, PlayerSlot _slot);
		public abstract void SubtractWin(int _matchNumber, PlayerSlot _slot);
		public abstract void ResetMatchScore(int _matchNumber);
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

		public List<IMatch> GetRound(int _round)
		{
			if (null == Matches)
			{
				throw new NullReferenceException();
			}
			if (_round < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			List<IMatch> ret = Matches.Values
				.Where(m => m.RoundIndex == _round)
				.OrderBy(m => m.MatchIndex)
				.ToList();
			return ret;
		}
		public List<IMatch> GetLowerRound(int _round)
		{
			if (null == LowerMatches)
			{
				throw new NullReferenceException();
			}
			if (_round < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			List<IMatch> ret = LowerMatches.Values
				.Where(m => m.RoundIndex == _round)
				.OrderBy(m => m.MatchIndex)
				.ToList();
			return ret;
		}
		public IMatch GetMatch(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new IndexOutOfRangeException();
			}

			if (null != GrandFinal &&
				GrandFinal.MatchNumber == _matchNumber)
			{
				return GrandFinal;
			}
			if (null != Matches &&
				Matches.ContainsKey(_matchNumber))
			{
				return Matches[_matchNumber];
			}
			if (null != LowerMatches &&
				LowerMatches.ContainsKey(_matchNumber))
			{
				return LowerMatches[_matchNumber];
			}

			throw new KeyNotFoundException();
		}
		public void ResetBracket()
		{
			Matches = null;
			LowerMatches = null;
			GrandFinal = null;
			NumberOfRounds = NumberOfLowerRounds = 0;
			NumberOfMatches = 0;
		}
		#endregion

		#region Private Methods

		#endregion
	}
}
