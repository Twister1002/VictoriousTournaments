using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	/// <summary>
	/// KnockoutBracket is the parent of all elimination brackets:
	/// for instance, Single/Double-Elim and Stepladder Brackets.
	/// This abstract class holds the logic shared by all its children.
	/// </summary>
	public abstract class KnockoutBracket : Bracket
	{
		#region Public Methods
		/// <summary>
		/// Resets the state of all Matches.
		/// Deletes all Games and sets scores to 0-0.
		/// Removes Players from Matches they had advanced to.
		/// Clears the Rankings list.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// </summary>
		public override void ResetMatches()
		{
			base.ResetMatches();
			Rankings.Clear();
		}

		/// <summary>
		/// Always returns false.
		/// (Does not apply to knockout brackets)
		/// </summary>
		/// <returns>false</returns>
		public override bool CheckForTies()
		{
			return false;
		}

		/// <summary>
		/// Always throws a NotImplementedException.
		/// (Does not apply to knockout brackets)
		/// </summary>
		public override bool GenerateTiebreakers()
		{
			throw new NotImplementedException
				("Not applicable for knockout brackets!");
		}

		/// <summary>
		/// Sets MaxGames for every Match in the given round.
		/// Can also be used to affect the GrandFinal (rounds + 1).
		/// If an even value is input, an exception is thrown.
		/// If the given value is invalid, an exception is thrown.
		/// If the round can't be found, an exception is thrown.
		/// If any Match in the round is finished, an exception is thrown.
		/// </summary>
		/// <param name="_round">Number of round to affect</param>
		/// <param name="_maxGamesPerMatch">Max Games per Match</param>
		public override void SetMaxGamesForWholeRound(int _round, int _maxGamesPerMatch)
		{
			if (0 == _maxGamesPerMatch % 2)
			{
				throw new ScoreException
					("Games per Match must be ODD in an elimination bracket!");
			}
			base.SetMaxGamesForWholeRound(_round, _maxGamesPerMatch);
		}

		/// <summary>
		/// Sets MaxGames for every Match in the given Lower-Bracket round.
		/// If an even value is input, an exception is thrown.
		/// If the given value is invalid, an exception is thrown.
		/// If the round can't be found, an exception is thrown.
		/// If any Match in the round is finished, an exception is thrown.
		/// </summary>
		/// <param name="_round">Number of round to affect</param>
		/// <param name="_maxGamesPerMatch">Max Games per Match</param>
		public override void SetMaxGamesForWholeLowerRound(int _round, int _maxGamesPerMatch)
		{
			if (0 == _maxGamesPerMatch % 2)
			{
				throw new ScoreException
					("Games per Match must be ODD in an elimination bracket!");
			}
			base.SetMaxGamesForWholeLowerRound(_round, _maxGamesPerMatch);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Processes the effects of adding a "game win" to a Match.
		/// If the Match ends, the winner and loser advance,
		/// so long as they have legal next matches to advance to.
		/// </summary>
		/// <param name="_matchNumber">Number of Match initially affected</param>
		/// <param name="_slot">Slot of game winner: Defender or Challenger</param>
		/// <returns>List of Models of Matches that are changed</returns>
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();

			int nextWinnerNumber;
			int nextLoserNumber;
			Match match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.IsFinished)
			{
				if (nextWinnerNumber > 0)
				{
					// Advance the winning player:
					Match nextWinMatch = GetInternalMatch(nextWinnerNumber);
					for (int i = 0; i < nextWinMatch.PreviousMatchNumbers.Length; ++i)
					{
						if (_matchNumber == nextWinMatch.PreviousMatchNumbers[i])
						{
							nextWinMatch.AddPlayer
								(match.Players[(int)(match.WinnerSlot)], (PlayerSlot)i);
							alteredMatches.Add(GetMatchModel(nextWinMatch));
							break;
						}
					}
				}

				if (nextLoserNumber > 0)
				{
					// Advance the losing player:
					Match nextLosMatch = GetInternalMatch(nextLoserNumber);
					for (int i = 0; i < nextLosMatch.PreviousMatchNumbers.Length; ++i)
					{
						if (_matchNumber == nextLosMatch.PreviousMatchNumbers[i])
						{
							PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
								? PlayerSlot.Challenger
								: PlayerSlot.Defender;

							nextLosMatch.AddPlayer(match.Players[(int)loserSlot], (PlayerSlot)i);
							alteredMatches.Add(GetMatchModel(nextLosMatch));
							break;
						}
					}
				}
			}

			return alteredMatches;
		}

		/// <summary>
		/// Processes the effects of removing a game(s) from a Match.
		/// If the Match was previously finished, any players who
		/// had previously advanced are called back, and their results invalidated.
		/// </summary>
		/// <param name="_matchNumber">Number of Match initially affected</param>
		/// <param name="_games">List of Games removed (if any)</param>
		/// <param name="_formerMatchWinnerSlot">Slot of Match winner prior to removal</param>
		/// <returns>List of Models of Matches that are changed</returns>
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();

			int nextWinnerNumber;
			int nextLoserNumber;
			Match match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.WinnerSlot != _formerMatchWinnerSlot)
			{
				this.IsFinished = false;

				PlayerSlot loserSlot = (_formerMatchWinnerSlot == PlayerSlot.Defender)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;

				alteredMatches.AddRange(RemovePlayerFromFutureMatches
					(nextWinnerNumber, match.Players[(int)_formerMatchWinnerSlot].Id));
				List<MatchModel> secondList = RemovePlayerFromFutureMatches
					(nextLoserNumber, match.Players[(int)loserSlot].Id);

				List<int> dupeMatchNums = alteredMatches.Select(m => m.MatchNumber).ToList()
					.Intersect(secondList.Select(m => m.MatchNumber).ToList())
					.ToList();
				alteredMatches.RemoveAll(m => dupeMatchNums.Contains(m.MatchNumber));
				alteredMatches.AddRange(secondList);
			}

			return alteredMatches;
		}

		/// <summary>
		/// Applies effects to Rankings any time a Match is modified.
		/// This is used in conjunction with methods to modify the bracket.
		/// If a win has been applied, this adds the results (match loser, possibly match winner)
		/// to the Rankings.
		/// If a game has been removed/win reversed, the Rankings are cleared and recalculated.
		/// </summary>
		/// <param name="_matchNumber">Number of Match initally affected</param>
		/// <param name="_games">List of Games removed (if any)</param>
		/// <param name="_isAddition">Is the result being added or removed?</param>
		/// <param name="_oldMatch">Model of the Match before the modification</param>
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			if (_isAddition &&
				_oldMatch.WinnerID.GetValueOrDefault(-1) < 0 &&
				_oldMatch.NextLoserMatchNumber.GetValueOrDefault(-1) < 0)
			{
				int nextWinnerNumber;
				int nextLoserNumber;
				Match match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

				if (match.IsFinished)
				{
					// Add losing Player to Rankings:
					int rank = CalculateRank(_matchNumber);
					IPlayer losingPlayer = match.Players[
						(PlayerSlot.Defender == match.WinnerSlot)
						? (int)PlayerSlot.Challenger
						: (int)PlayerSlot.Defender];
					Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));

					if (nextWinnerNumber < 0)
					{
						// Finals match: Add winner to Rankings:
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name,
							1));
						IsFinished = true;
					}

					Rankings.Sort(SortRankingRanks);
				}
			}
			else if (!_isAddition &&
				_oldMatch.WinnerID.GetValueOrDefault(-1) > -1)
			{
				RecalculateRankings();
			}
		}

		/// <summary>
		/// Abstract: overridden in specific brackets to find
		/// an eliminated player's rank.
		/// </summary>
		/// <param name="_matchNumber">Number of finished Match</param>
		/// <returns>Player's rank</returns>
		protected abstract int CalculateRank(int _matchNumber);

		/// <summary>
		/// Clears the Rankings, and recalculates them from the Matches list.
		/// Finds every eliminated player, calculates his rank, and adds him to the Rankings.
		/// Sorts the Rankings.
		/// If no players are eliminated, the Rankings will be an empty list.
		/// </summary>
		protected override void RecalculateRankings()
		{
			if (null == Rankings)
			{
				Rankings = new List<IPlayerScore>();
			}
			Rankings.Clear();

			if (NumberOfMatches > 0)
			{
				for (int n = NumberOfMatches; n > 0; --n)
				{
					Match match = GetInternalMatch(n);
					if (match.NextLoserMatchNumber > 0)
					{
						break;
					}

					if (match.IsFinished)
					{
						// Add losing Player to the Rankings:
						int rank = CalculateRank(match.MatchNumber);
						IPlayer losingPlayer = match.Players[
							(PlayerSlot.Defender == match.WinnerSlot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));
					}
				}

				Match finalMatch = GetInternalMatch(NumberOfMatches);
				if (finalMatch.IsFinished)
				{
					// Add Finals winner to Rankings:
					IPlayer winningPlayer = finalMatch
						.Players[(int)finalMatch.WinnerSlot];
					Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, 1));
					this.IsFinished = true;
				}

				Rankings.Sort(SortRankingRanks);
			}
		}

		/// <summary>
		/// Simply calls RecalculateRankings().
		/// </summary>
		protected override void UpdateRankings()
		{
			RecalculateRankings();
		}

		/// <summary>
		/// Removes the specified Player from all Matches "after" the given Match.
		/// Invalidates any future results he has been part of.
		/// This is called upon reversing a Player's bracket advancement,
		/// such as resetting a Match he had won.
		/// If the given Match is not found, an exception is thrown.
		/// If the Player ID is not found, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match</param>
		/// <param name="_playerId">ID of Player to remove</param>
		/// <returns>List of Models of Matches that changed</returns>
		protected virtual List<MatchModel> RemovePlayerFromFutureMatches(int _matchNumber, int _playerId)
		{
			if (_matchNumber < 1 || _playerId == -1)
			{
				return (new List<MatchModel>());
			}

			List<MatchModel> alteredMatches = new List<MatchModel>();
			int nextWinnerNumber;
			int nextLoserNumber;
			Match match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.Players.Any(p => p?.Id == _playerId))
			{
				if (match.IsFinished)
				{
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;

					// Remove any advanced Players from future Matches:
					alteredMatches.AddRange(RemovePlayerFromFutureMatches
						(nextWinnerNumber, match.Players[(int)(match.WinnerSlot)].Id));
					List<MatchModel> secondList = RemovePlayerFromFutureMatches
						(nextLoserNumber, match.Players[(int)loserSlot].Id);

					List<int> dupeNumbers = alteredMatches.Select(m => m.MatchNumber).ToList()
						.Intersect(secondList.Select(m => m.MatchNumber).ToList())
						.ToList();
					alteredMatches.RemoveAll(m => dupeNumbers.Contains(m.MatchNumber));
					alteredMatches.AddRange(secondList);
				}

				OnGamesDeleted(match.Games);
				match.RemovePlayer(_playerId);
			}

			alteredMatches.RemoveAll(m => m.MatchNumber == _matchNumber);
			alteredMatches.Add(GetMatchModel(match));

			return alteredMatches;
		}

		/// <summary>
		/// Given a match number, gets the Match
		/// and its NextMatchNumber and NextLoserMatchNumber.
		/// If the Match is not found, an exception is thrown.
		/// </summary>
		/// <param name="_matchNumber">Number of Match to find</param>
		/// <param name="_nextMatchNumber">Where this Match's winner advances to</param>
		/// <param name="_nextLoserMatchNumber">Where this Match's loser advances to</param>
		/// <returns>The Match in question</returns>
		protected Match GetMatchData(int _matchNumber, out int _nextMatchNumber, out int _nextLoserMatchNumber)
		{
			Match m = GetInternalMatch(_matchNumber);
			_nextMatchNumber = m.NextMatchNumber;
			_nextLoserMatchNumber = m.NextLoserMatchNumber;

			return m;
		}
		#endregion
	}
}
