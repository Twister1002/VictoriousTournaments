using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class BracketEventArgs :EventArgs
	{
		public bool UpdateBracket
		{ get; private set; }
		public List<MatchModel> UpdatedMatches
		{ get; private set; }
		public List<GameModel> UpdatedGames
		{ get; private set; }

		public BracketEventArgs(bool _update, List<MatchModel> _matches, List<GameModel> _games)
		{
			UpdateBracket = _update;
			UpdatedMatches = _matches;
			UpdatedGames = _games;
		}
		public BracketEventArgs(bool _update)
			: this(true, null, null)
		{ }
		public BracketEventArgs(List<MatchModel> _matches)
			: this(false, _matches, null)
		{ }
		public BracketEventArgs(List<GameModel> _games)
			: this(false, null, _games)
		{ }
	}
}
