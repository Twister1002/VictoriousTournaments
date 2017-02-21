using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public abstract class Bracket : IBracket
	{
		//public abstract uint Id { get; }
		public abstract List<IPlayer> Players { get; set; }
		public abstract List<List<IMatch>> Rounds { get; set; }

		public abstract void CreateBracket();
		public abstract bool AddPlayer(IPlayer _p);
		public abstract void AddRound();
		public abstract bool AddMatch(int _roundIndex);
		public abstract bool AddMatch(int _roundIndex, IMatch _m);
		public abstract void AddWin(int _roundIndex, int _matchIndex, int _index);
		public abstract void AddWin(IMatch _match, int _index);
	}
}
