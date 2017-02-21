using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class Match : IMatch
	{
		#region Variables & Properties
		//private uint id;
		//public uint Id
		//{
		//	get { return id; }
		//}
		public ushort WinsNeeded
		{ get; set; }
		//public IPlayer[] Players
		//{ get; set; }
		public int[] PlayerIndexes
		{ get; set; }
		public ushort[] Score
		{ get; set; }
		//public int BracketId
		//{ get; set; }
		public int RoundNumber
		{ get; set; }
		public int MatchIndex
		{ get; set; }
		public List<int> PrevMatchIndexes
		{ get; set; }
		public int NextMatchIndex
		{ get; set; }
		#endregion

		#region Ctors
		public Match() : this(1, new int[2] { -1, -1 }, new ushort[2] { 0, 0 }, -1, -1, new List<int>(), -1) { }
		//public Match(uint _id)
		//{
		//	id = _id;
		//	WinsNeeded = 1;
		//	//Players = new IPlayer[2] { null, null };
		//	PlayerIndexes = new int[2] { -1, -1 };
		//	Score = new ushort[2] { 0, 0 };
		//	BracketId = -1;
		//	RoundNumber = -1;
		//	MatchIndex = -1;
		//	PrevMatchIndexes = new List<int>();
		//	NextMatchIndex = -1;
		//}
		public Match(/*uint _id,*/ ushort _winsNeeded, /*IPlayer[] _players*/ int[] _playerIndexes, ushort[] _score, /*int _bracketId,*/ int _roundNumber, int _matchIndex, List<int> _prevMatchIndexes, int _nextMatchIndex)
		{
			//id = _id;
			WinsNeeded = _winsNeeded;
			//Players = _players;
			PlayerIndexes = _playerIndexes;
			Score = _score;
			//BracketId = _bracketId;
			RoundNumber = _roundNumber;
			MatchIndex = _matchIndex;
			PrevMatchIndexes = _prevMatchIndexes;
			NextMatchIndex = _nextMatchIndex;
		}
		#endregion

		#region Public Methods
		//public bool AddPlayer(IPlayer _p)
		//{
		//	//foreach (IPlayer p in Players)
		//	//{
		//	//	if (p.Id == _p.Id)
		//	//	{
		//	//		return false;
		//	//	}
		//	//}
		//	//for (int i = 0; i < 2; ++i)
		//	//{
		//	//	if (null == players[i])
		//	//	{
		//	//		players[i] = _p;
		//	//		return true;
		//	//	}
		//	//}
		//	return false;
		//}
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
			if(Score[_index] >= WinsNeeded)
			{
				return false;
			}

			Score[_index] += 1;
			return true;
		}
		public bool AddPrevMatchIndex(int _i)
		{
			if (PrevMatchIndexes.Count >= 2)
			{
				return false;
			}
			PrevMatchIndexes.Add(_i);
			return true;
		}
		#endregion
	}
}
