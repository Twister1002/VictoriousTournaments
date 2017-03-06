using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public interface IBracket
	{
		#region Variables & Properties
		List<IPlayer> Players { get; }
		//List<List<IMatch>> Rounds { get; set; }
		#endregion

		#region Methods
		void CreateBracket(ushort _winsPerMatch = 1);
		void UpdateCurrentMatches(ICollection<MatchModel> _matchModels);
		//void AddWin(IMatch _match, PlayerSlot _slot);
		void AddWin(int _matchNumber, PlayerSlot _slot);

		int NumberOfPlayers();
		void AddPlayer(IPlayer _p);
		void RemovePlayer(IPlayer _p);
		void ResetPlayers();
		int NumberOfRounds();
		List<IMatch> GetRound(int _index);
		//IMatch GetMatch(int _roundIndex, int _index);
		IMatch GetMatch(int _matchNumber);
		void ResetBracket();
		#endregion
	}
}
