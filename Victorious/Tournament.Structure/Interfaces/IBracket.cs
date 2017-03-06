﻿using System;
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
		// protected List<List<IMatch>> Rounds { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Generates the bracket (rounds & matches).
		/// </summary>
		/// <param name="_winsPerMatch">"Games" needed to win each match.</param>
		void CreateBracket(ushort _winsPerMatch = 1);

		/// <summary>
		/// Temporarily public, DON'T USE THIS METHOD.
		/// </summary>
		/// <param name="_matchModels">WHY ARE YOU STILL HERE</param>
		void UpdateCurrentMatches(ICollection<MatchModel> _matchModels);

		/// <summary>
		/// Record one win for the specified match.
		/// Advances Player if the Match is over.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match.</param>
		/// <param name="_slot">Winning player's slot: Defender or Challenger</param>
		void AddWin(int _matchNumber, PlayerSlot _slot);
		//void AddWin(IMatch _match, PlayerSlot _slot);

		/// <summary>
		/// Remove one win for the specified match.
		/// Resets any affected "future" matches.
		/// </summary>
		/// <param name="_matchNumber">Number of specified match.</param>
		/// <param name="_slot">Player slot: Defender or Challenger.</param>
		void SubtractWin(int _matchNumber, PlayerSlot _slot);

		/// <summary>
		/// Gets the number of Players in the Bracket.
		/// </summary>
		/// <returns>Number of Players.</returns>
		int NumberOfPlayers();

		/// <summary>
		/// Add a Player.
		/// </summary>
		/// <param name="_p">Player-type object to add.</param>
		void AddPlayer(IPlayer _p);

		/// <summary>
		/// Remove a Player from the bracket.
		/// </summary>
		/// <param name="_p">Player-type object to remove.</param>
		void RemovePlayer(IPlayer _p);

		/// <summary>
		/// Clears the bracket's player list.
		/// </summary>
		void ResetPlayers();

		/// <summary>
		/// Gets the number of rounds in the bracket.
		/// </summary>
		/// <returns>Number of Rounds.</returns>
		int NumberOfRounds();

		/// <summary>
		/// Get all Matches in specified round.
		/// (Round 1 is the bracket's FIRST round)
		/// </summary>
		/// <param name="_index">Round number to get.</param>
		/// <returns>List of Matches in the round.</returns>
		List<IMatch> GetRound(int _index);

		//IMatch GetMatch(int _roundIndex, int _index);

		/// <summary>
		/// Get a specific Match object.
		/// </summary>
		/// <param name="_matchNumber">Match Number of the desired Match.</param>
		/// <returns>Match object.</returns>
		IMatch GetMatch(int _matchNumber);

		/// <summary>
		/// Clears all Matches and rounds in the bracket.
		/// </summary>
		void ResetBracket();
		#endregion
	}
}
