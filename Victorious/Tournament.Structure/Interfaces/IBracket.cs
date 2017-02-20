using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IBracket
	{
		uint Id { get; }
		List<IPlayer> Players { get; set; }
		List<List<IMatch>> Rounds { get; set; }

		bool AddPlayer(IPlayer _p);
		void AddRound();
		bool AddMatch(int _roundIndex);
		bool AddMatch(int _roundIndex, IMatch _m);
	}
}
