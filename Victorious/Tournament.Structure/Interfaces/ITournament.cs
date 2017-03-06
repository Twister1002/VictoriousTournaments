using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface ITournament
	{
		#region Variables & Properties
		string Title { get; set; }
		string Description { get; set; }
		List<IPlayer> Players { get; }
		List<IBracket> Brackets { get; }
		float PrizePool { get; set; }
		bool IsPublic { get; set; }
		#endregion

		#region Methods
		int NumberOfPlayers();
		void AddPlayer(IPlayer _p);
		void RemovePlayer(IPlayer _p);
		void ResetPlayers();
		int NumberOfBrackets();
		void AddBracket(IBracket _b);
		void RemoveBracket(IBracket _b);
		void ResetBrackets();
		void AddSingleElimBracket(List<IPlayer> _playerList);
		void AddSingleElimBracket(int _numPlayers);
		void AddDoubleElimBracket(List<IPlayer> _playerList);
		void AddDoubleElimBracket(int _numPlayers);
		#endregion
	}
}
