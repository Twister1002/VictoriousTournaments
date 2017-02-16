using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public abstract class Bracket : IBracket
	{
		public abstract uint Id { get; }
		public abstract List<IPlayer> Players { get; set; }
		public abstract List<List<IMatch>> Rounds { get; set; }
	}
}
