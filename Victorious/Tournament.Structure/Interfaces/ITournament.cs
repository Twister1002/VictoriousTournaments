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
		/// <summary>
		/// Gets the number of players in the tournament.
		/// </summary>
		/// <returns>Number of Players</returns>
		int NumberOfPlayers();

		/// <summary>
		/// Add a Player.
		/// </summary>
		/// <param name="_p">Player-type object to add.</param>
		void AddPlayer(IPlayer _p);

		/// <summary>
		/// Removes a Player from the tournament.
		/// </summary>
		/// <param name="_p">Player-type object to remove.</param>
		void RemovePlayer(IPlayer _p);

		/// <summary>
		/// Clears the tournament's player list.
		/// </summary>
		void ResetPlayers();

		/// <summary>
		/// Gets the number of Brackets in the tournament.
		/// </summary>
		/// <returns>Number of Brackets</returns>
		int NumberOfBrackets();

		/// <summary>
		/// Add a Bracket.
		/// </summary>
		/// <param name="_b">Bracket-type object to add.</param>
		void AddBracket(IBracket _b);

		/// <summary>
		/// Removes a Bracket from the tournament.
		/// </summary>
		/// <param name="_b">Bracket-type object to remove.</param>
		void RemoveBracket(IBracket _b);

		/// <summary>
		/// Clears the tournament's bracket list.
		/// </summary>
		void ResetBrackets();

		#region Bracket Creation Methods
		/// <summary>
		/// Adds a new SingleElimBracket to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the Bracket.</param>
		void AddSingleElimBracket(List<IPlayer> _playerList);

		/// <summary>
		/// Adds a new SingleElimBracket to the tournament,
		/// with a pre-sized but empty player list.
		/// </summary>
		/// <param name="_numPlayers">Number of empty player slots.</param>
		void AddSingleElimBracket(int _numPlayers);

		/// <summary>
		/// Adds a new DoubleElimBracket to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the Bracket.</param>
		void AddDoubleElimBracket(List<IPlayer> _playerList);

		/// <summary>
		/// Adds a new DoubleElimBracket to the tournament,
		/// with a pre-sized but empty player list.
		/// </summary>
		/// <param name="_numPlayers">Number of empty player slots.</param>
		void AddDoubleElimBracket(int _numPlayers);

		/// <summary>
		/// Adds a new Round Robin stage to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the stage.</param>
		/// <param name="_numRounds">Number of rounds for the stage. (matches per player)</param>
		void AddRoundRobinBracket(List<IPlayer> _playerList, int _numRounds = 0);

		/// <summary>
		/// Adds a new Round Robin stage to the tournament,
		/// with a pre-sized but empty player list.
		/// </summary>
		/// <param name="_numPlayers">Number of empty player slots.</param>
		/// <param name="_numRounds">Number of rounds for the stage.</param>
		void AddRoundRobinBracket(int _numPlayers, int _numRounds = 0);

		/// <summary>
		/// Adds a new Group Stage to the tournament.
		/// </summary>
		/// <param name="_playerList">List of Players for the stage.</param>
		/// <param name="_numGroups">Number of groups to divide players into.</param>
		void AddGroupStageBracket(List<IPlayer> _playerList, int _numGroups = 2);

		/// <summary>
		/// Adds a new Group Stage to the tournament,
		/// with a pre-sized but empty player list.
		/// </summary>
		/// <param name="_numPlayers">Number of empty player slots.</param>
		/// <param name="_numGroups">Number of groups to divide players into.</param>
		void AddGroupStageBracket(int _numPlayers, int _numGroups = 2);
		#endregion
		#endregion
	}
}
