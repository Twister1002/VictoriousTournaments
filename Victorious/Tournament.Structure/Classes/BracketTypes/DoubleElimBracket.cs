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
		// inherits int Id
		// inherits BracketType BracketType
		// inherits bool IsFinalized
		// inherits bool IsFinished
		// inherits List<IPlayer> Players
		// inherits List<IPlayerScore> Rankings
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches
		// inherits int NumberOfLowerRounds
		// inherits IMatch GrandFinal
		// inherits int NumberOfMatches
		#endregion

		#region Ctors
		public DoubleElimBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
			: base(_players, _maxGamesPerMatch)
		{
			BracketType = BracketType.DOUBLE;
		}
#if false
		public DoubleElimBracket(int _numPlayers)
			: base(_numPlayers)
		{
			BracketType = BracketTypeModel.BracketType.DOUBLE;
		}
#endif
		public DoubleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public DoubleElimBracket(BracketModel _model)
			: base(_model)
		{
			this.IsFinished = false;
			this.BracketType = BracketType.DOUBLE;

			if (CalculateTotalLowerBracketMatches(Players.Count) > 0)
			{
				int numOfGrandFinal = _model.Matches.Count - 1;
				int i = 0;

				this.LowerMatches = new Dictionary<int, IMatch>();
				foreach (MatchModel mm in _model.Matches)
				{
					if (i >= Matches.Count
						&& i < numOfGrandFinal)
					{
						IMatch match = new Match(mm);
						if (match.RoundIndex > NumberOfLowerRounds)
						{
							this.NumberOfLowerRounds = match.RoundIndex;
						}
						LowerMatches.Add(match.MatchNumber, match);
						++NumberOfMatches;
					}
					else if (i == numOfGrandFinal)
					{
						this.GrandFinal = new Match(mm);
						++NumberOfMatches;
					}

					++i;
				}
			}

			UpdateRankings();
			if (GrandFinal.IsFinished)
			{
				IPlayer winningPlayer = GrandFinal.Players[(int)GrandFinal.WinnerSlot];
				Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, -1, 1));
				IPlayer losingPlayer = GrandFinal.Players[
					(PlayerSlot.Defender == GrandFinal.WinnerSlot)
					? (int)PlayerSlot.Challenger
					: (int)PlayerSlot.Defender];
				Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, 2));

				Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				this.IsFinished = true;
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			base.CreateBracket(_gamesPerMatch);
			if (0 == NumberOfMatches)
			{
				return;
			}

			List<List<IMatch>> roundList = new List<List<IMatch>>();
			int totalMatches = CalculateTotalLowerBracketMatches(Players.Count);
			int numMatches = 0;
			int r = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				roundList.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, r / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.SetMaxGames(_gamesPerMatch);
					roundList[r].Add(m);
				}
				++r;
			}

			// Assign Match Numbers
			int matchNum = 1 + Matches.Count;
			for (r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (IMatch match in roundList[r])
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
				GrandFinal = new Match();
				GrandFinal.SetMatchNumber(matchNum);
				GrandFinal.SetMaxGames(_gamesPerMatch);
				GrandFinal.SetRoundIndex(1);
				GrandFinal.SetMatchIndex(1);
				GrandFinal.AddPreviousMatchNumber(Matches.Count);
				GrandFinal.AddPreviousMatchNumber(roundList[0][0].MatchNumber);
				// Connect Final matches to Grand Final
				roundList[0][0].SetNextMatchNumber(GrandFinal.MatchNumber);
				Matches[Matches.Count].SetNextMatchNumber(GrandFinal.MatchNumber);

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

#if false
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			GameModel gameModel = base.AddGame(_matchNumber, _defenderScore, _challengerScore, _winnerSlot);
			return gameModel;
		}
#endif
#if false
		public override void RemoveLastGame(int _matchNumber)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.IsFinished)
			{
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				RemovePlayerFromFutureMatches
					(nextLoserNumber, ref match.Players[(int)loserSlot]);
			}

			base.RemoveLastGame(_matchNumber);
		}
		public override void ResetMatchScore(int _matchNumber)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.IsFinished)
			{
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				RemovePlayerFromFutureMatches
					(match.NextLoserMatchNumber, ref match.Players[(int)loserSlot]);
			}

			base.ResetMatchScore(_matchNumber);
		}
#endif

		public override void SetMaxGamesForWholeLowerRound(int _round, int _maxGamesPerMatch)
		{
			if (0 == _maxGamesPerMatch)
			{
				throw new ScoreException
					("Games/Match must be odd in an elimination bracket!");
			}
			base.SetMaxGamesForWholeLowerRound(_round, _maxGamesPerMatch);
		}
		#endregion

		#region Private Methods
		protected override void UpdateScore(int _matchNumber, GameModel _game, bool _isAddition, bool _wasFinished)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (_isAddition)
			{
				if (match.IsFinished && nextLoserNumber < 0)
				{
					// Add losing player to Rankings:
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					int rank = 2; // 2 = GrandFinal loser
					if (null != LowerMatches && LowerMatches.ContainsKey(_matchNumber))
					{
						rank = NumberOfMatches - GetLowerRound(match.RoundIndex)[0].MatchNumber + 2;
					}
#if false
					else if (null != Matches && Matches.ContainsKey(_matchNumber))
					{
						rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);
					}
					else if (null != GrandFinal && GrandFinal.MatchNumber == _matchNumber)
					{
						rank = 2;
					}
#endif

					Rankings.Add(new PlayerScore
						(match.Players[(int)loserSlot].Id, match.Players[(int)loserSlot].Name, -1, rank));
					if (null != GrandFinal && GrandFinal.MatchNumber == _matchNumber)
					{
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name, -1, 1));
					}
					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				}
			}
			else if (_wasFinished && !(match.IsFinished))
			{
				UpdateRankings();
			}
		}
		protected override void ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{

			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.IsFinished)
			{
				base.ApplyWinEffects(_matchNumber, _slot);

				if (nextLoserNumber > 0)
				{
					// Advance the losing player:
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					IMatch nextMatch = GetMatch(nextLoserNumber);

					for (int i = 0; i < nextMatch.PreviousMatchNumbers.Length; ++i)
					{
						if (_matchNumber == nextMatch.PreviousMatchNumbers[i])
						{
							GetMatch(nextLoserNumber).AddPlayer(match.Players[(int)loserSlot], (PlayerSlot)i);
							break;
						}
					}
				}
			}
		}
		protected override void ApplyGameRemovalEffects(int _matchNumber, GameModel _game, bool _wasFinished)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.IsFinished != _wasFinished)
			{
				base.ApplyGameRemovalEffects(_matchNumber, _game, _wasFinished);

				PlayerSlot loserSlot = (_game.DefenderID == _game.WinnerID)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				RemovePlayerFromFutureMatches
					(nextLoserNumber, match.Players[(int)loserSlot].Id);
			}
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

		protected override void RemovePlayerFromFutureMatches(int _matchNumber, int _playerId)
		{
			if (_matchNumber < 1 || _playerId == -1)
			{
				return;
			}

			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.Players
				.Where(p => p != null)
				.Any(p => p.Id == _playerId))
			{
				if (match.IsFinished)
				{
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;

					RemovePlayerFromFutureMatches
						(nextWinnerNumber, match.Players[(int)(match.WinnerSlot)].Id);
					RemovePlayerFromFutureMatches
						(nextLoserNumber, match.Players[(int)loserSlot].Id);
				}

				GetMatch(_matchNumber).RemovePlayer(_playerId);
			}
		}

		protected override void UpdateRankings()
		{
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
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, rank));
					}
				}
			}

			// Special case check: DEB has a play-in round
			if (Matches[1].NextLoserMatchNumber < 1)
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
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, rank));
					}
				}
			}

			Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
		}
#endregion
	}
}
