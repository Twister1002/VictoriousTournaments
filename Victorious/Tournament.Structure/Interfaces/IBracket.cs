using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IBracket
	{
		//uint Id { get; }
		List<IPlayer> Players { get; set; }
		List<List<IMatch>> Rounds { get; set; }

		void CreateBracket();
		bool AddPlayer(IPlayer _p);
		void AddRound();
		bool AddMatch(int _roundIndex);
		bool AddMatch(int _roundIndex, IMatch _m);
		void AddWin(int _roundIndex, int _matchIndex, int _index);
		void AddWin(IMatch _match, int _index);
	}
}
