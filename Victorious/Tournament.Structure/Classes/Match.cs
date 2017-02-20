using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class Match : IMatch
	{
		// Variables
		private uint id;
		private ushort winsNeeded;
		//private IPlayer[] players;
		private int[] playerIndexes;
		private ushort[] score;
		private int bracketId;
		private int roundNumber;
		private int matchIndex;
		private List<int> prevMatchIndexes;
		private int nextMatchIndex;

		// Properties
		public uint Id
		{
			get { return id; }
		}
		public ushort WinsNeeded
		{
			get { return winsNeeded; }
			set { winsNeeded = value; }
		}
		//public IPlayer[] Players
		//{
		//	get { return players; }
		//	set { players = value; }
		//}
		public int[] PlayerIndexes
		{
			get { return playerIndexes; }
			set { playerIndexes = value; }
		}
		public ushort[] Score
		{
			get { return score; }
			set { score = value; }
		}
		public int BracketId
		{
			get { return bracketId; }
			set { bracketId = value; }
		}
		public int RoundNumber
		{
			get { return roundNumber; }
			set { roundNumber = value; }
		}
		public int MatchIndex
		{
			get { return matchIndex; }
			set { matchIndex = value; }
		}
		public List<int> PrevMatchIndexes
		{
			get { return prevMatchIndexes; }
			set { prevMatchIndexes = value; }
		}
		public int NextMatchIndex
		{
			get { return nextMatchIndex; }
			set { nextMatchIndex = value; }
		}

		// Ctors
		public Match() : this(0) { }
		public Match(uint _id)
		{
			id = _id;
			winsNeeded = 1;
			//players = new IPlayer[2] { null, null };
			playerIndexes = new int[2] { -1, -1 };
			score = new ushort[2] { 0, 0 };
			bracketId = -1;
			roundNumber = -1;
			matchIndex = -1;
			prevMatchIndexes = new List<int>();
			nextMatchIndex = -1;
		}
		public Match(uint _id, ushort _winsNeeded, /*IPlayer[] _players*/ int[] _playerIndexes, ushort[] _score, int _bracketId, int _roundNumber, int _matchIndex, List<int> _prevMatchIndexes, int _nextMatchIndex)
		{
			id = _id;
			winsNeeded = _winsNeeded;
			//players = _players;
			playerIndexes = _playerIndexes;
			score = _score;
			bracketId = _bracketId;
			roundNumber = _roundNumber;
			matchIndex = _matchIndex;
			prevMatchIndexes = _prevMatchIndexes;
			nextMatchIndex = _nextMatchIndex;
		}

		// Methods
		public bool AddPlayer(IPlayer _p)
		{
			//foreach (IPlayer p in Players)
			//{
			//	if (p.Id == _p.Id)
			//	{
			//		return false;
			//	}
			//}
			//for (int i = 0; i < 2; ++i)
			//{
			//	if (null == players[i])
			//	{
			//		players[i] = _p;
			//		return true;
			//	}
			//}
			return false;
		}
		public bool AddPlayer(int _playerIndex)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (-1 == PlayerIndexes[i])
				{
					PlayerIndexes[i] = _playerIndex;
					return true;
				}
			}
			return false;
		}
		public bool RemovePlayer(int _playerIndex)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (PlayerIndexes[i] == _playerIndex)
				{
					PlayerIndexes[i] = -1;
					return true;
				}
			}
			return false;
		}
		public void RemovePlayers()
		{
			PlayerIndexes[0] = PlayerIndexes[1] = -1;
		}
		public bool AddWin(int _index)
		{
			if(score[_index] >= WinsNeeded)
			{
				return false;
			}

			score[_index] += 1;
			if (score[_index] == WinsNeeded)
			{
				// TODO : Trigger victory condition/routine
			}
			return true;
		}
		public bool AddPrevMatchIndex(int _i)
		{
			if (prevMatchIndexes.Count >= 2)
			{
				return false;
			}
			prevMatchIndexes.Add(_i);
			return true;
		}

	}
}
