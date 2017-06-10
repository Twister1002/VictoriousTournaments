using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public abstract class KnockoutBracket : Bracket
	{
		#region Public Methods
		public override void ResetMatches()
		{
			base.ResetMatches();
			Rankings.Clear();
		}

		public override void SetMaxGamesForWholeRound(int _round, int _maxGamesPerMatch)
		{
			if (0 == _maxGamesPerMatch % 2)
			{
				throw new ScoreException
					("Games per Match must be ODD in an elimination bracket!");
			}
			base.SetMaxGamesForWholeRound(_round, _maxGamesPerMatch);
		}
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

		protected override void UpdateRankings()
		{
			RecalculateRankings();
		}

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
