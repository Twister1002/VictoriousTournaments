using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class DoubleElimBracket : SingleElimBracket
	{
		#region Variables & Properties

		#endregion

		#region Ctors
		public DoubleElimBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
			: base(_players, _maxGamesPerMatch)
		{
			BracketType = BracketType.DOUBLE;
		}
		public DoubleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public DoubleElimBracket(BracketModel _model)
			: base(_model)
		{
			this.NumberOfLowerRounds = 0;
			if (_model.Matches.Count > 0)
			{
				if (CalculateTotalLowerBracketMatches(Players.Count) > 0)
				{
					int numOfGrandFinal = _model.Matches.Count;

					foreach (MatchModel mm in _model.Matches.OrderBy(m => m.MatchNumber))
					{
						if (Matches.ContainsKey(mm.MatchNumber))
						{
							// Case 1: match is upper bracket:
							continue;
						}
						if (mm.MatchNumber == numOfGrandFinal)
						{
							// Case 2: match is grand final:
							this.grandFinal = new Match(mm);
						}
						else
						{
							// Case 3: match is lower bracket:
							Match match = new Match(mm);
							LowerMatches.Add(match.MatchNumber, match);
							this.NumberOfLowerRounds = Math.Max(NumberOfLowerRounds, match.RoundIndex);
						}
					}
				}
				this.NumberOfMatches = Matches.Count + LowerMatches.Count;
				if (null != grandFinal)
				{
					++NumberOfMatches;
				}
			}

			RecalculateRankings();
			if (null != grandFinal && grandFinal.IsFinished)
			{
				this.IsFinished = true;
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			if (Players.Count < 4)
			{
				return;
			}

			base.CreateBracket(_gamesPerMatch);

			List<List<Match>> roundList = new List<List<Match>>();
			int totalMatches = CalculateTotalLowerBracketMatches(Players.Count);
			int numMatches = 0;
			int r = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				roundList.Add(new List<Match>());
				for (int i = 0;
					i < Math.Pow(2, r / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					Match m = new Match();
					m.SetMaxGames(_gamesPerMatch);
					roundList[r].Add(m);
				}
				++r;
			}

			// Assign Match Numbers
			int matchNum = 1 + Matches.Count;
			for (r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (Match match in roundList[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			// Tie Matches Together
			bool flipSeeds = true;
			for (r = roundList.Count - 2; r >= 0; --r)
			{
				bool rIndexIsEven = (0 == r % 2) ? true : false;
				if (rIndexIsEven && roundList[r + 1].Count == roundList[r].Count)
				{
					// Round is "normal," but one team is coming from Upper Bracket
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						List<IMatch> upperRound = GetRound(NumberOfRounds - (r / 2));
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup indexes
						if (flipSeeds)
						{
							roundList[r][m].AddPreviousMatchNumber(upperRound[upperRound.Count - 1 - m].MatchNumber);
							Matches[upperRound[upperRound.Count - 1 - m].MatchNumber].SetNextLoserMatchNumber(currNum);
						}
						else
						{
							roundList[r][m].AddPreviousMatchNumber(upperRound[m].MatchNumber);
							Matches[upperRound[m].MatchNumber].SetNextLoserMatchNumber(currNum);
						}
						// ************* THIS ISN'T QUITE RIGHT (RE-SEED ORDER)

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m].MatchNumber);
						roundList[r + 1][m].SetNextMatchNumber(currNum);
					}

					flipSeeds = !flipSeeds;
				}
				else if (!rIndexIsEven && roundList[r + 1].Count == (roundList[r].Count * 2))
				{
					// Round is "normal"
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup indexes
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2].MatchNumber);
						roundList[r + 1][m * 2].SetNextMatchNumber(currNum);

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2 + 1].MatchNumber);
						roundList[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				else
				{
					// Round is abnormal. Case is not possible
					// (unless we later decide to include it)
				}

				flipSeeds = !flipSeeds;
			}

			r = roundList.Count - 1;
			if (r >= 0)
			{
				// We have enough teams to have created a Lower Bracket.
				// Manually update the first Lower round,
				// and create a Grand Final match

				for (int m = 0; m < roundList[r].Count; ++m)
				{
					List<IMatch> upperRound = GetRound(NumberOfRounds - (r / 2 + 1));
					int currNum = roundList[r][m].MatchNumber;

					// Assign prev/next matchup indexes for FIRST round
					// (both teams come from Upper Bracket)
					roundList[r][m].AddPreviousMatchNumber(upperRound[m * 2].MatchNumber);
					Matches[upperRound[m * 2].MatchNumber].SetNextLoserMatchNumber(currNum);

					roundList[r][m].AddPreviousMatchNumber(upperRound[m * 2 + 1].MatchNumber);
					Matches[upperRound[m * 2 + 1].MatchNumber].SetNextLoserMatchNumber(currNum);
				}

				// Create a Grand Final
				grandFinal = new Match();
				grandFinal.SetMatchNumber(matchNum);
				grandFinal.SetMaxGames(_gamesPerMatch);
				grandFinal.SetRoundIndex(1);
				grandFinal.SetMatchIndex(1);
				grandFinal.AddPreviousMatchNumber(Matches.Count);
				grandFinal.AddPreviousMatchNumber(roundList[0][0].MatchNumber);
				// Connect Final matches to Grand Final
				roundList[0][0].SetNextMatchNumber(grandFinal.MatchNumber);
				Matches[Matches.Count].SetNextMatchNumber(grandFinal.MatchNumber);

				// Move new bracket data to member variables (LowerMatches dictionary)
				NumberOfLowerRounds = roundList.Count;
				for (r = 0; r < roundList.Count; ++r)
				{
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						roundList[r][m].SetRoundIndex(roundList.Count - r);
						roundList[r][m].SetMatchIndex(m + 1);
						LowerMatches.Add(roundList[r][m].MatchNumber, roundList[r][m]);
					}
				}
				NumberOfMatches += (LowerMatches.Count + 1);
			}
		}

		public override void SetMaxGamesForWholeLowerRound(int _round, int _maxGamesPerMatch)
		{
			if (0 == _maxGamesPerMatch % 2)
			{
				throw new ScoreException
					("Games/Match must be odd in an elimination bracket!");
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
				alteredMatches.AddRange(base.ApplyWinEffects(_matchNumber, _slot));

				if (nextLoserNumber > 0)
				{
					// Advance the losing player:
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					Match nextMatch = GetInternalMatch(nextLoserNumber);

					for (int i = 0; i < nextMatch.PreviousMatchNumbers.Length; ++i)
					{
						if (_matchNumber == nextMatch.PreviousMatchNumbers[i])
						{
							nextMatch.AddPlayer(match.Players[(int)loserSlot], (PlayerSlot)i);
							alteredMatches.Add(GetMatchModel(nextMatch));
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
				alteredMatches.AddRange(base.ApplyGameRemovalEffects(_matchNumber, _games, _formerMatchWinnerSlot));

				PlayerSlot loserSlot = (_formerMatchWinnerSlot == PlayerSlot.Defender)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				alteredMatches.AddRange(RemovePlayerFromFutureMatches
					(nextLoserNumber, match.Players[(int)loserSlot].Id));
			}

			return alteredMatches;
		}
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			Match match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (_isAddition)
			{
				if (match.IsFinished && nextLoserNumber < 0)
				{
					// Add losing player to Rankings:
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					int rank = 2; // 2 = grand final loser
					if (null != LowerMatches && LowerMatches.ContainsKey(_matchNumber))
					{
						rank = NumberOfMatches - GetLowerRound(match.RoundIndex)[0].MatchNumber + 2;
					}

					Rankings.Add(new PlayerScore
						(match.Players[(int)loserSlot].Id,
						match.Players[(int)loserSlot].Name,
						rank));
					if (null != grandFinal && grandFinal.MatchNumber == _matchNumber)
					{
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name,
							1));
					}
					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				}
			}
			else if (_oldMatch.WinnerID.HasValue && _oldMatch.WinnerID > -1)
			{
				RecalculateRankings();
			}
		}

		protected override void RecalculateRankings()
		{
			if (null == Rankings)
			{
				Rankings = new List<IPlayerScore>();
			}
			Rankings.Clear();

			for (int r = 1; r <= NumberOfLowerRounds; ++r)
			{
				List<IMatch> round = GetLowerRound(r);
				int rank = NumberOfMatches - round[0].MatchNumber + 2;

				foreach (IMatch match in round)
				{
					if (match.IsFinished)
					{
						// Add losing Player to the Rankings:
						IPlayer losingPlayer = match.Players[
							(PlayerSlot.Defender == match.WinnerSlot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));
					}
				}
			}

			// Special case check: DEB has a play-in round
			if (NumberOfMatches > 0 && Matches[1].NextLoserMatchNumber < 1)
			{
				int rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);

				foreach (IMatch match in GetRound(1))
				{
					if (match.IsFinished)
					{
						IPlayer losingPlayer = match.Players[
							(PlayerSlot.Defender == match.WinnerSlot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));
					}
				}
			}

			if (null != grandFinal && grandFinal.IsFinished)
			{
				// Add grand final results to Rankings:
				IPlayer winningPlayer = grandFinal.Players[(int)grandFinal.WinnerSlot];
				Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, 1));
				IPlayer losingPlayer = grandFinal.Players[
					(PlayerSlot.Defender == grandFinal.WinnerSlot)
					? (int)PlayerSlot.Challenger
					: (int)PlayerSlot.Defender];
				Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, 2));
			}

			Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
		}
		protected override void UpdateRankings()
		{
			RecalculateRankings();
		}

		protected override List<MatchModel> RemovePlayerFromFutureMatches(int _matchNumber, int _playerId)
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();

			if (_matchNumber < 1 || _playerId == -1)
			{
				return alteredMatches;
			}

			int nextWinnerNumber;
			int nextLoserNumber;
			Match match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.Players
				.Where(p => p != null)
				.Any(p => p.Id == _playerId))
			{
				if (match.IsFinished)
				{
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;

					alteredMatches.AddRange(RemovePlayerFromFutureMatches
						(nextWinnerNumber, match.Players[(int)(match.WinnerSlot)].Id));
					List<MatchModel> secondList = RemovePlayerFromFutureMatches
						(nextLoserNumber, match.Players[(int)loserSlot].Id);
					alteredMatches.RemoveAll(firstM => secondList.Select(secM => secM.MatchNumber).Contains(firstM.MatchNumber));
					alteredMatches.AddRange(secondList);
				}

				OnGamesDeleted(match.Games);
				match.RemovePlayer(_playerId);
			}

			alteredMatches.RemoveAll(m => m.MatchNumber == _matchNumber);
			alteredMatches.Add(GetMatchModel(match));

			return alteredMatches;
		}

		private int CalculateTotalLowerBracketMatches(int _numPlayers)
		{
			if (_numPlayers < 4)
			{
				return 0;
			}

			int normalizedPlayers = 2;
			while (true)
			{
				int next = normalizedPlayers * 2;
				if (next <= _numPlayers)
				{
					normalizedPlayers = next;
				}
				else
				{
					break;
				}
			}
			return (normalizedPlayers - 2);
		}
		#endregion
	}
}
