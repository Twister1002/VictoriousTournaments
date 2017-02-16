﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IMatch
	{
		// Properties
		uint Id { get; }
		ushort WinsNeeded { get; set; }
		IPlayer[] Players { get; set; }
		ushort[] Score { get; set; }
		int BracketId { get; set; }
		int RoundNumber { get; set; }
		int MatchIndex { get; set; }
		List<int> PrevMatchIndexes { get; set; }
		int NextMatchIndex { get; set; }

		// Methods
		bool AddPlayer(IPlayer _p);
		bool AddWin(int _playerIndex);
		bool AddPrevMatchIndex(int _i);
	}
}
