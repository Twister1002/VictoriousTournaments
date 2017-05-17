using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class BracketEventArgs : EventArgs
	{
		public List<MatchModel> UpdatedMatches
		{ get; private set; }
		public List<int> DeletedGameIDs
		{ get; private set; }

		public BracketEventArgs(List<MatchModel> _matches, List<int> _gameIDs)
		{
			UpdatedMatches = _matches;
			DeletedGameIDs = _gameIDs;
		}
		public BracketEventArgs(List<MatchModel> _matches)
			: this(_matches, new List<int>())
		{ }
		public BracketEventArgs(List<int> _gameIDs)
			: this(new List<MatchModel>(), _gameIDs)
		{ }
	}
}
