using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class SingleElimBracket : Bracket
	{
		#region Variables & Properties
		// int Id
		// BracketType BracketType
		// bool IsFinalized
		// bool IsFinished
		// List<IPlayer> Players
		// List<IPlayerScore> Rankings
		// int MaxRounds -- unused
		// Dictionary<int, IMatch> Matches
		// int NumberOfRounds
		// Dictionary<int, IMatch> LowerMatches -- unused
		// int NumberOfLowerRounds -- unused
		// IMatch GrandFinal -- unused
		// int NumberOfMatches
		// int MatchWinValue -- unused
		// int MatchTieValue -- unused
		#endregion

		#region Ctors
		public SingleElimBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = new List<IPlayer>();
			if (_players.Count > 0)
			{
				if (_players[0] is User)
				{
					foreach (IPlayer p in _players)
					{
						Players.Add(new User(p as User));
					}
				}
				else if (_players[0] is Team)
				{
					foreach (IPlayer p in _players)
					{
						Players.Add(new Team(p as Team));
					}
				}
				else
				{
					Players = _players;
				}
			}

			Id = 0;
			BracketType = BracketType.SINGLE;
			CreateBracket(_maxGamesPerMatch);
		}
		public SingleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public SingleElimBracket(BracketModel _model)
		{
			if (null == _model)
			{
				throw new NullReferenceException
					("Bracket Model cannot be null!");
			}
			
			this.Id = _model.BracketID;
			this.BracketType = _model.BracketType.Type;
			this.IsFinalized = _model.Finalized;

			List<TournamentUserModel> userModels = _model.TournamentUsersBrackets
				.OrderBy(tubm => tubm.Seed, new SeedComparer())
				.Select(tubm => tubm.TournamentUser)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (TournamentUserModel model in userModels)
			{
				Players.Add(new User(model));
			}

			ResetBracketData();
			int totalUBMatches = Players.Count - 1;

			foreach (MatchModel mm in _model.Matches.OrderBy(m => m.MatchNumber))
			{
				if (mm.MatchNumber <= totalUBMatches)
				{
					IMatch match = new Match(mm);
					Matches.Add(match.MatchNumber, match);
					this.NumberOfRounds = Math.Max(NumberOfRounds, match.RoundIndex);
				}
				else
				{
					// Match doesn't belong in upper bracket, so break out:
					break;
				}
			}
			this.NumberOfMatches = Matches.Count;

			if (BracketType.SINGLE == BracketType)
			{
				UpdateRankings();
				if (Matches[NumberOfMatches].IsFinished)
				{
					// Add Finals winner to Rankings:
					IPlayer winningPlayer = Matches[NumberOfMatches]
						.Players[(int)Matches[NumberOfMatches].WinnerSlot];
					Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, 1));
					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
					this.IsFinished = true;
				}
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();
			if (_gamesPerMatch < 1)
			{
				throw new BracketException
					("Games Per Match must be greater than 0!");
			}
			else if (_gamesPerMatch % 2 == 0)
			{
				throw new BracketException
					("Games/Match must be odd in an elimination bracket!");
			}
			if (Players.Count < 2)
			{
				return;
			}

			#region Create the Bracket
			int totalMatches = Players.Count - 1;
			int numMatches = 0;
			List<List<IMatch>> roundList = new List<List<IMatch>>();

			// Create the Matches
			for (int r = 0; numMatches < totalMatches; ++r)
			{
				roundList.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, r) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.SetMaxGames(_gamesPerMatch);
					roundList[r].Add(m);
				}
			}

			// Assign Match Numbers
			int matchNum = 1;
			for (int r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (IMatch match in roundList[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			// Tie Matches Together
			for (int r = 0; r + 1 < roundList.Count; ++r)
			{
				if (roundList[r + 1].Count == (roundList[r].Count * 2))
				{
					// "Normal" rounds: twice as many matchups
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup numbers
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2].MatchNumber);
						roundList[r + 1][m * 2].SetNextMatchNumber(currNum);

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2 + 1].MatchNumber);
						roundList[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				// Else: round is abnormal. Ignore it for now (we'll handle it later)
			}
			#endregion

			#region Assign the Players
			// Assign top two seeds to final match
			int pIndex = 0;
			roundList[0][0].AddPlayer(Players[pIndex++]);
			roundList[0][0].AddPlayer(Players[pIndex++]);

			for (int r = 0; r + 1 < roundList.Count; ++r)
			{
				// We're shifting back one player for each match in the prev round
				int prevRoundMatches = roundList[r + 1].Count;

				if ((roundList[r].Count * 2) > prevRoundMatches)
				{
					// Abnormal round ahead: we need to allocate prevMatchIndexes
					// to correctly distribute bye seeds

					int prevMatchNumber = 1;

					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int[] playerIndexes = new int[2] { -1, -1 };
						for (int i = 0; i < Players.Count; ++i)
						{
							if (Players[i].Equals(roundList[r][m].Players[0]))
							{
								playerIndexes[0] = i;
							}
							else if (Players[i].Equals(roundList[r][m].Players[1]))
							{
								playerIndexes[1] = i;
							}
						}

						int playersToMove = 0;
						foreach (int p in playerIndexes)
						{
							if (p >= pIndex - prevRoundMatches)
							{
								++playersToMove;
							}
						}
						for (int i = 0; i < playerIndexes.Length; ++i)
						{
							if (playerIndexes[i] >= pIndex - prevRoundMatches)
							{
								roundList[r][m].AddPreviousMatchNumber(prevMatchNumber,
									(2 == playersToMove)
									? (PlayerSlot)i : PlayerSlot.Challenger);
								roundList[r + 1][prevMatchNumber - 1].SetNextMatchNumber
									(roundList[r][m].MatchNumber);
								++prevMatchNumber;
							}
						}
					}
				}

				for (int m = 0; m < roundList[r].Count; ++m)
				{
					// For each match, shift/reassign all teams to the prev bracket level
					// If prev level is abnormal, only shift 1 (or 0) teams
					foreach (int n in roundList[r][m].PreviousMatchNumbers)
					{
						if (n > 0)
						{
							ReassignPlayers(roundList[r][m], roundList[r + 1]);
							break;
						}
					}
				}

				for (int prePlayers = pIndex - 1; prePlayers >= 0; --prePlayers)
				{
					for (int m = 0; m < prevRoundMatches; ++m)
					{
						if (roundList[r + 1][m].Players.Contains(Players[prePlayers]))
						{
							// Add prev round's teams (according to seed) from the master list
							roundList[r + 1][m].AddPlayer(Players[pIndex++]);
							break;
						}
					}
				}
			}
			#endregion

			#region Set Bracket Member Variables
			// Move bracket data to member variables (Matches dictionary)
			NumberOfRounds = roundList.Count;
			for (int r = 0; r < roundList.Count; ++r)
			{
				for (int m = 0; m < roundList[r].Count; ++m)
				{
					roundList[r][m].SetRoundIndex(roundList.Count - r);
					roundList[r][m].SetMatchIndex(m + 1);
					Matches.Add(roundList[r][m].MatchNumber, roundList[r][m]);
				}
			}
			NumberOfMatches = Matches.Count;
			#endregion
		}

		public override void SetMaxGamesForWholeRound(int _round, int _maxGamesPerMatch)
		{
			if (0 == _maxGamesPerMatch % 2)
			{
				throw new ScoreException
					("Games/Match must be ODD in an elimination bracket!");
			}
			base.SetMaxGamesForWholeRound(_round, _maxGamesPerMatch);
		}

		public override void ResetMatches()
		{
			base.ResetMatches();
			Rankings.Clear();
		}
		#endregion

		#region Private Methods
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, PlayerSlot _formerMatchWinnerSlot, bool _resetManualWin = false)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (_isAddition)
			{
				if (match.IsFinished)
				{
					// Add losing player to Rankings:
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					int rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);

					Rankings.Add(new PlayerScore
						(match.Players[(int)loserSlot].Id,
						match.Players[(int)loserSlot].Name,
						rank));
					if (nextWinnerNumber < 0)
					{
						// Finals match: Add winner to Rankings:
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name,
							1));
						IsFinished = true;
					}
					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				}
			}
			else if (match.WinnerSlot != _formerMatchWinnerSlot)
			{
				UpdateRankings();
			}
		}
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();

			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.IsFinished)
			{
				if (nextWinnerNumber > 0)
				{
					// Advance the winning player:
					IMatch nextMatch = GetMatch(nextWinnerNumber);
					for (int i = 0; i < nextMatch.PreviousMatchNumbers.Length; ++i)
					{
						if (_matchNumber == nextMatch.PreviousMatchNumbers[i])
						{
							nextMatch.AddPlayer
								(match.Players[(int)(match.WinnerSlot)], (PlayerSlot)i);
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
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.WinnerSlot != _formerMatchWinnerSlot)
			{
				this.IsFinished = (IsFinished && match.IsFinished);
				// Remove advanced players from future matches:
				alteredMatches.AddRange(RemovePlayerFromFutureMatches
					(nextWinnerNumber, match.Players[(int)_formerMatchWinnerSlot].Id));
			}

			return alteredMatches;
		}

		private void ReassignPlayers(IMatch _currMatch, List<IMatch> _prevRound)
		{
			if (null == _currMatch ||
				null == _prevRound || 0 == _prevRound.Count)
			{
				throw new NullReferenceException
					("NULL error in calling ReassignPlayers()...");
			}

			int playersToMove = 2;
			foreach (int n in _currMatch.PreviousMatchNumbers)
			{
				if (n < 0)
				{
					--playersToMove;
				}
			}
			foreach (IMatch match in _prevRound)
			{
				for (int i = 0; i < _currMatch.PreviousMatchNumbers.Length; ++i)
				{
					if (match.MatchNumber == _currMatch.PreviousMatchNumbers[i])
					{
						match.AddPlayer(_currMatch.Players[i]);
						_currMatch.RemovePlayer(_currMatch.Players[i].Id);
						--playersToMove;
					}
				}
				if (0 == playersToMove)
				{
					break;
				}
			}
		}

		protected virtual List<MatchModel> RemovePlayerFromFutureMatches(int _matchNumber, int _playerId)
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();

			if (_matchNumber < 1 || _playerId == -1)
			{
				return alteredMatches;
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new MatchNotFoundException
					("Invalid match number called by recursive method!");
			}

			IMatch match = GetMatch(_matchNumber);
			if (match.Players
				.Where(p => p != null)
				.Any(p => p.Id == _playerId))
			{
				if (match.IsFinished)
				{
					// Remove any advanced Players from future Matches:
					alteredMatches.AddRange(RemovePlayerFromFutureMatches
						(match.NextMatchNumber, match.Players[(int)(match.WinnerSlot)].Id));
				}

				OnGamesDeleted(match.Games);
				match.RemovePlayer(_playerId);
			}

			alteredMatches.Add(GetMatchModel(match));
			return alteredMatches;
			//return (alteredMatches.OrderBy(m => m.MatchNumber).ToList());
		}

		protected override void UpdateRankings()
		{
			Rankings.Clear();

			foreach (IMatch match in Matches.Values)
			{
				if (match.IsFinished)
				{
					// Add losing Player to the Rankings:
					int rank = (int)(Math.Pow(2, (NumberOfRounds - match.RoundIndex)) + 1);
					IPlayer losingPlayer = match.Players[
						(PlayerSlot.Defender == match.WinnerSlot)
						? (int)PlayerSlot.Challenger
						: (int)PlayerSlot.Defender];
					Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));
				}
			}

			Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
		}

		protected IMatch GetMatchData(int _matchNumber, out int _nextMatchNumber, out int _nextLoserMatchNumber)
		{
			IMatch m = GetMatch(_matchNumber);
			_nextMatchNumber = m.NextMatchNumber;
			_nextLoserMatchNumber = m.NextLoserMatchNumber;

			return m;
		}
		#endregion
	}
}
