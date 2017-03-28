﻿using System;
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
		/// Gets the number of Players in the tournament.
		/// </summary>
		/// <returns>Number of Players</returns>
		int NumberOfPlayers();

		/// <summary>
		/// Clears and sets a new list of Players.
		/// </summary>
		/// <param name="_players">New list of Player-type objects</param>
		void SetNewPlayerlist(List<IPlayer> _players);

		/// <summary>
		/// Copy Players from one (finished) bracket to another,
		/// ordered by their ranking.
		/// </summary>
		/// <param name="_initialBracketIndex">Index of finished bracket,
		/// in this Tournament's bracket[] array</param>
		/// <param name="_newBracketIndex">Index of second bracket</param>
		void AdvancePlayersByRanking(int _initialBracketIndex, int _newBracketIndex);

		/// <summary>
		/// Add a Player.
		/// </summary>
		/// <param name="_player">Player-type object to add.</param>
		void AddPlayer(IPlayer _player);

		/// <summary>
		/// Replaces a player/slot in the playerlist.
		/// Also replaces old Player in any brackets.
		/// </summary>
		/// <param name="_player">Player-type object to add.</param>
		/// <param name="_index">Slot in list to replace.</param>
		void ReplacePlayer(IPlayer _player, int _index);

		/// <summary>
		/// Removes a Player from the tournament.
		/// </summary>
		/// <param name="_player">Player-type object to remove.</param>
		void RemovePlayer(IPlayer _player);

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
		/// <param name="_bracket">Bracket-type object to add.</param>
		void AddBracket(IBracket _bracket);

		/// <summary>
		/// Removes a Bracket from the tournament.
		/// </summary>
		/// <param name="_bracket">Bracket-type object to remove.</param>
		void RemoveBracket(IBracket _bracket);

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
