using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class SingleElimBracket : Bracket
	{
		#region Variables & Properties
		// inherits BracketType BracketType
		// inherits bool IsFinalized
		// inherits bool IsFinished
		// inherits List<IPlayer> Players
		// inherits List<IPlayerScore> Rankings
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		// inherits int NumberOfMatches
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

			BracketType = BracketTypeModel.BracketType.SINGLE;
			ResetBracket();
			CreateBracket(_maxGamesPerMatch);
		}
#if false
		public SingleElimBracket(int _numPlayers)
		{
			Players = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				Players.Add(new User());
			}

			BracketType = BracketTypeModel.BracketType.SINGLE;
			ResetBracket();
			CreateBracket();
		}
#endif
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

			this.BracketType = BracketTypeModel.BracketType.SINGLE;
			this.IsFinalized = _model.Finalized;

			List<UserModel> userModels = _model.UserSeeds
				.OrderBy(ubs => ubs.Seed)
				.Select(ubs => ubs.User)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (UserModel um in userModels)
			{
				Players.Add(new User(um));
			}

			ResetBracket();
			int totalMatches = Players.Count - 1;

			this.Matches = new Dictionary<int, IMatch>();
			foreach (MatchModel mm in _model.Matches)
			{
				IMatch match = new Match(mm);
				if (match.RoundIndex > NumberOfRounds)
				{
					this.NumberOfRounds = match.RoundIndex;
				}
				Matches.Add(match.MatchNumber, match);

				++NumberOfMatches;
				if (NumberOfMatches >= totalMatches)
				{
					break;
				}
			}

			this.Rankings = new List<IPlayerScore>();
			if (BracketTypeModel.BracketType.SINGLE == BracketType)
			{
				UpdateRankings();
				if (Matches[NumberOfMatches].IsFinished)
				{
					IPlayer winningPlayer = Matches[NumberOfMatches]
						.Players[(int)Matches[NumberOfMatches].WinnerSlot];
					Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, -1, 1));
					IPlayer losingPlayer = Matches[NumberOfMatches].Players[
						(PlayerSlot.Defender == Matches[NumberOfMatches].WinnerSlot)
						? (int)PlayerSlot.Challenger
						: (int)PlayerSlot.Defender];
					Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, 2));

					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
					this.IsFinished = true;
				}
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracket();
			if (_gamesPerMatch < 1)
			{
				throw new BracketException
					("Games Per Match must be greater than 0!");
			}
			else if (_gamesPerMatch % 2 == 0)
			{
				throw new BracketException
					("Games Per Match must be odd!");
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

		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			GameModel gameModel = GetMatch(_matchNumber).AddGame(_defenderScore, _challengerScore, _winnerSlot);
			AddWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore)
		{
			GameModel gameModel = GetMatch(_matchNumber).AddGame(_defenderScore, _challengerScore);
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
							GetMatch(nextWinnerNumber).AddPlayer
								(match.Players[(int)(match.WinnerSlot)], (PlayerSlot)i);
							break;
						}
					}
				}
				else
				{
					// Add winner to Rankings:
					Rankings.Add(new PlayerScore
						(match.Players[(int)(match.WinnerSlot)].Id,
						match.Players[(int)(match.WinnerSlot)].Name, -1, 1));
					IsFinished = true;
				}

				if (BracketTypeModel.BracketType.SINGLE == this.BracketType)
				{
					// Add losing player to Rankings:
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					int rank = -1;
					if (null != LowerMatches && LowerMatches.ContainsKey(_matchNumber))
					{
						rank = NumberOfMatches - GetLowerRound(match.RoundIndex)[0].MatchNumber + 2;
					}
					else if (null != Matches && Matches.ContainsKey(_matchNumber))
					{
						rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);
					}
					else if (null != GrandFinal && GrandFinal.MatchNumber == _matchNumber)
					{
						rank = 2;
					}

					Rankings.Add(new PlayerScore
						(match.Players[(int)loserSlot].Id, match.Players[(int)loserSlot].Name, -1, rank));
					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				}
			}

			return gameModel;
		}
		public override GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);
			if (!match.Games.Exists(g => g.GameNumber == _gameNumber))
			{
				throw new GameNotFoundException
					("Game not found; Game Number may be invalid!");
			}

			bool needToUpdateRankings = false;
			if (match.IsFinished &&
				match.WinnerSlot != _winnerSlot)
			{
				needToUpdateRankings = true;
				// Remove advanced players from future matches:
				RemovePlayerFromFutureMatches
					(nextWinnerNumber, ref match.Players[(int)(match.WinnerSlot)]);
			}

			GameModel gameModel = GetMatch(_matchNumber).UpdateGame(_gameNumber, _defenderScore, _challengerScore, _winnerSlot);

			if (needToUpdateRankings)
			{
				IsFinished = false;
				UpdateRankings();
			}

			AddWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
		public override void RemoveLastGame(int _matchNumber)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);
			bool needToUpdateRankings = false;

			if (match.IsFinished)
			{
				needToUpdateRankings = true;
				// Remove advanced players from future matches:
				RemovePlayerFromFutureMatches
					(nextWinnerNumber, ref match.Players[(int)(match.WinnerSlot)]);
			}

			// Remove the game and update rankings:
			GetMatch(_matchNumber).RemoveLastGame();
			if (needToUpdateRankings)
			{
				IsFinished = false;
				UpdateRankings();
			}
		}
		public override void ResetMatchScore(int _matchNumber)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);
			bool needToUpdateRankings = false;

			if (match.IsFinished)
			{
				needToUpdateRankings = true;
				// Remove advanced players from future matches:
				RemovePlayerFromFutureMatches
					(match.NextMatchNumber, ref match.Players[(int)match.WinnerSlot]);
			}

			// Reset score and update rankings:
			GetMatch(_matchNumber).ResetScore();
			if (needToUpdateRankings)
			{
				IsFinished = false;
				UpdateRankings();
			}
		}

		public override void ResetMatches()
		{
			base.ResetMatches();
			Rankings.Clear();
		}
		#endregion

		#region Private Methods
		protected override void AddWinEffects(int _matchNumber, PlayerSlot _slot)
		{
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
							GetMatch(nextWinnerNumber).AddPlayer
								(match.Players[(int)(match.WinnerSlot)], (PlayerSlot)i);
							break;
						}
					}
				}
				else
				{
					// Add winner to Rankings:
					Rankings.Add(new PlayerScore
						(match.Players[(int)(match.WinnerSlot)].Id,
						match.Players[(int)(match.WinnerSlot)].Name, -1, 1));
					IsFinished = true;
				}

				if (BracketTypeModel.BracketType.SINGLE == this.BracketType)
				{
					// Add losing player to Rankings:
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					int rank = -1;
					if (null != LowerMatches && LowerMatches.ContainsKey(_matchNumber))
					{
						rank = NumberOfMatches - GetLowerRound(match.RoundIndex)[0].MatchNumber + 2;
					}
					else if (null != Matches && Matches.ContainsKey(_matchNumber))
					{
						rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);
					}
					else if (null != GrandFinal && GrandFinal.MatchNumber == _matchNumber)
					{
						rank = 2;
					}

					Rankings.Add(new PlayerScore
						(match.Players[(int)loserSlot].Id, match.Players[(int)loserSlot].Name, -1, rank));
					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				}
			}
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

		protected virtual void RemovePlayerFromFutureMatches(int _matchNumber, ref IPlayer _player)
		{
			if (_matchNumber < 1 || null == _player)
			{
				return;
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new MatchNotFoundException
					("Invalid match number called by recursive method!");
			}

			if (Matches[_matchNumber].Players.Contains(_player))
			{
				if (Matches[_matchNumber].IsFinished)
				{
					// Remove any advanced Players from future Matches:
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextMatchNumber,
						ref Matches[_matchNumber].Players[(int)(Matches[_matchNumber].WinnerSlot)]);
				}

				Matches[_matchNumber].RemovePlayer(_player.Id);
			}
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
					Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, rank));
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
