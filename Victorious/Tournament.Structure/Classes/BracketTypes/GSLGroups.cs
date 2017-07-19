using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class GSLGroups : DoubleElimBracket, IGroupStage
	{
		private class GSLBracket : DoubleElimBracket
		{
			#region Variables & Properties
			//public int Id
			//public BracketType BracketType
			//public bool IsFinalized
			//public bool IsFinished
			//public List<IPlayer> Players
			//public List<IPlayerScore> Rankings
			//public int MaxRounds = 0
			//protected Dictionary<int, Match> Matches
			//public int NumberOfRounds
			//protected Dictionary<int, Match> LowerMatches
			//public int NumberOfLowerRounds
			//protected Match grandFinal = null
			//public IMatch GrandFinal = null
			//public int NumberOfMatches
			//protected int MatchWinValue = 0
			//protected int MatchTieValue = 0
			//protected List<IBracket> Groups = empty
			//public int NumberOfGroups = 0
			#endregion

			#region Ctors
			public GSLBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
				: base(_players, _maxGamesPerMatch)
			{
				//BracketType = BracketType.GSL;
			}
			public GSLBracket()
				: this(new List<IPlayer>())
			{ }
			public GSLBracket(BracketModel _model)
			{
				throw new NotImplementedException();
			}
			#endregion

			#region Public Methods
			public override void CreateBracket(int _gamesPerMatch = 1)
			{
				base.CreateBracket(_gamesPerMatch);
				grandFinal = null;
				--NumberOfMatches;
			}
			public override bool Validate()
			{
				if ((Players?.Count ?? 0) != 4 &&
					Players.Count != 8)
				{
					return false;
				}

				return true;
			}
			#endregion

			#region Private Methods
			protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
			{
				List<MatchModel> alteredMatches = new List<MatchModel>();

				int nextWinnerNumber;
				int nextLoserNumber;
				IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

				if (match.NextMatchNumber <= NumberOfMatches)
				{
					// Case 1: Not a final/endpoint match. Treat like a DEB:
					alteredMatches.AddRange(base.ApplyWinEffects(_matchNumber, _slot));
				}
				else if (match.IsFinished)
				{
					if (nextLoserNumber > 0)
					{
						// Case 2: UB Finals.
						// Advance loser to lower bracket:
						PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
							? PlayerSlot.Challenger
							: PlayerSlot.Defender;
						GetInternalMatch(nextLoserNumber).AddPlayer
							(match.Players[(int)loserSlot], PlayerSlot.Defender);
						alteredMatches.Add(GetMatchModel(nextLoserNumber));
						// Check lower bracket completion:
						if (GetLowerRound(NumberOfLowerRounds)[0].IsFinished)
						{
							this.IsFinished = true;
						}
					}
					else
					{
						// Case 3: LB Finals.
						// Check upper bracket completion:
						if (GetRound(NumberOfRounds)[0].IsFinished)
						{
							this.IsFinished = true;
						}
					}
				}

				return alteredMatches;
			}
			// void ApplyGameRemovalEffects() just uses DEB's version.
			protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
			{
				// The base method will add all match losers to the Rankings:
				base.UpdateScore(_matchNumber, _games, _isAddition, _oldMatch);

				// Now, check for special cases (winners to add):
				if (_isAddition &&
					_oldMatch.WinnerID.GetValueOrDefault(-1) < 0)
				{
					int nextWinnerNumber;
					int nextLoserNumber;
					IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

					if (match.IsFinished &&
						nextWinnerNumber > NumberOfMatches)
					{
						if (nextLoserNumber < 0)
						{
							// Case 1: Lower bracket finals.
							// Add winner (rank 2):
							Rankings.Add(new PlayerScore
								(match.Players[(int)(match.WinnerSlot)].Id,
								match.Players[(int)(match.WinnerSlot)].Name,
								2));
						}
						else
						{
							// Case 2: Upper bracket finals.
							// Add winner (rank 1):
							Rankings.Add(new PlayerScore
								(match.Players[(int)(match.WinnerSlot)].Id,
								match.Players[(int)(match.WinnerSlot)].Name,
								1));
						}

						Rankings.Sort(SortRankingRanks);
					}
				}
			}

			protected override void RecalculateRankings()
			{
				// The base method will add all losers to rankings.
				// It will also add the LB winner as Rank 1...
				base.RecalculateRankings();

				if (Rankings.Count > 0)
				{
					if (1 == Rankings[0].Rank)
					{
						// The LB winner was erroneously added as Rank 1.
						// Fix it (change to 2):
						Rankings[0].Rank = 2;
					}

					IMatch upperFinal = GetRound(NumberOfRounds)[0];
					if (upperFinal.IsFinished)
					{
						// Add the UB winner as Rank 1:
						Rankings.Add(new PlayerScore
							(upperFinal.Players[(int)(upperFinal.WinnerSlot)].Id,
							upperFinal.Players[(int)(upperFinal.WinnerSlot)].Name,
							1));

						Rankings.Sort(SortRankingRanks);
					}
				}
			}

			protected override List<MatchModel> RemovePlayerFromFutureMatches(int _matchNumber, int _playerId)
			{
				if (_matchNumber > NumberOfMatches)
				{
					return new List<MatchModel>();
				}
				return base.RemovePlayerFromFutureMatches(_matchNumber, _playerId);
			}
			#endregion
		}

		#region Variables & Properties
		//public int Id
		//public BracketType BracketType
		//public bool IsFinalized
		//public bool IsFinished
		//public List<IPlayer> Players
		//public List<IPlayerScore> Rankings
		//public int MaxRounds
		//protected Dictionary<int, Match> Matches = empty
		//public int NumberOfRounds
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue
		//protected int MatchTieValue
		//protected List<IBracket> Groups

		public int NumberOfGroups
		{ get; set; }

		protected List<List<IPlayerScore>> GroupRankings
		{ get; set; }
		#endregion

		#region Ctors
		public GSLGroups(List<IPlayer> _players, int _numberOfGroups, int _maxGamesPerMatch = 1)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.GSLGROUP;
			NumberOfGroups = _numberOfGroups;

			CreateBracket(_maxGamesPerMatch);
		}
		public GSLGroups()
			: this(new List<IPlayer>(), 2)
		{ }
		public GSLGroups(BracketModel _model)
		{
			SetDataFromModel(_model);
		}
		#endregion

		#region Public Methods
		public override bool Validate()
		{
			if (false == base.Validate())
			{
				return false;
			}

			if (NumberOfGroups < 2 ||
				(Players.Count != NumberOfGroups * 4/* &&
				 Players.Count != NumberOfGroups * 8*/))
			{
				return false;
			}

			return true;
		}

		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			// First, clear any existing Matches and results:
			ResetBracketData();
			if (NumberOfGroups < 2 ||
				(Players.Count != NumberOfGroups * 4/* &&
				 Players.Count != NumberOfGroups * 8*/))
			{
				return;
			}

			// DividePlayersIntoGroups() uses our chosen method to separate the playerlist:
			List<List<IPlayer>> playerGroups = DividePlayersIntoGroups(NumberOfGroups);
			GroupRankings.Capacity = NumberOfGroups;

			List<IBracket> groups = new List<IBracket>();
			for (int g = 0; g < playerGroups.Count; ++g)
			{
				// For each group, generate a full GSL bracket:
				groups.Add(new GSLBracket(playerGroups[g], _gamesPerMatch));
			}

			/*
			 * So now we've generated all of our Matches.
			 * We want to copy them into our main Bracket, but there's a problem:
			 * each internal bracket numbers their Matches from 1.
			 * We need unique MatchNumbers, so we go through and create
			 * new "copy" Matches, but with those unique MatchNumbers we need.
			*/

			// For every group...
			for (int g = 0; g < groups.Count; ++g)
			{
				// For every Match within that group...
				for (int m = 1; m <= groups[g].NumberOfMatches; ++m)
				{
					++NumberOfMatches;
					IMatch currMatch = groups[g].GetMatch(m);

					// Copy/create a new Match:
					Match match = new Match();
					// Set the basic match data:
					match.SetMaxGames(currMatch.MaxGames);
					match.SetRoundIndex(currMatch.RoundIndex);
					match.SetMatchIndex(currMatch.MatchIndex);
					match.SetMatchNumber(NumberOfMatches);
					match.SetGroupNumber(g + 1);
					match.AddPlayer(currMatch.Players[(int)PlayerSlot.Defender]);
					match.AddPlayer(currMatch.Players[(int)PlayerSlot.Challenger]);

					// Set the match progression data.
					// First, apply the offset to NextLoserMatchNumber (only if applicable):
					match.SetNextLoserMatchNumber((-1 == currMatch.NextLoserMatchNumber) ? -1
						: (currMatch.NextLoserMatchNumber - currMatch.MatchNumber + match.MatchNumber));
					// Due to a GSL quirk, there is no grand final, but upper&lower finals still point there.
					// We can use this opportunity to remove that false pointer.
					// (for other matches, apply the same MatchNumber offset):
					match.SetNextMatchNumber((currMatch.NextMatchNumber > groups[g].NumberOfMatches) ? -1
						: (currMatch.NextMatchNumber - currMatch.MatchNumber + match.MatchNumber));

					if (match.NextLoserMatchNumber > 0)
					{
						// Any match that progresses the loser goes to upper bracket:
						Matches.Add(match.MatchNumber, match);
					}
					else
					{
						// Otherwise, add it to the lower bracket:
						LowerMatches.Add(match.MatchNumber, match);
					}
				}
			}

			NumberOfRounds = Matches.Values
				.Max(m => m.RoundIndex);
			NumberOfLowerRounds = LowerMatches.Values
				.Max(m => m.RoundIndex);
		}

		public override void ResetMatches()
		{
			base.ResetMatches();

			// Also clear each Rankings list:
			foreach (List<IPlayerScore> group in GroupRankings)
			{
				group.Clear();
			}
			Rankings.Clear();
		}

		public override bool CheckForTies()
		{
			return false;
		}

		public override bool GenerateTiebreakers()
		{
			throw new NotImplementedException
				("Not applicable for knockout brackets!");
		}

		public override void ReplacePlayer(IPlayer _player, int _index)
		{
			int? oldPlayerId = Players[_index]?.Id;

			base.ReplacePlayer(_player, _index);

			// After replacing the old player,
			// we also need to find & replace him in the group-specific Rankings:
			foreach (List<IPlayerScore> groupRanks in GroupRankings)
			{
				int i = groupRanks.FindIndex(r => r.Id == oldPlayerId.Value);
				if (i > -1)
				{
					// The player will always only be in one group.
					// After we find it, replace him and break out.
					groupRanks[i].ReplacePlayerData(_player.Id, _player.Name);
					break;
				}
			}
		}

		#region Accessors
		public int NumberOfRoundsInGroup(int _groupNumber)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			List<int> roundNums = Matches.Values
				.Where(m => m.GroupNumber == _groupNumber)
				.Select(m => m.RoundIndex).ToList();

			if (roundNums.Any())
			{
				return roundNums.Max();
			}
			// Else: (_groupNumber is too high)
			return 0;
		}

		public int NumberOfLowerRoundsInGroup(int _groupNumber)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			List<int> roundNums = Matches.Values
				.Where(m => m.GroupNumber == _groupNumber)
				.Select(m => m.RoundIndex).ToList();

			if (roundNums.Any())
			{
				return roundNums.Max();
			}
			// Else: (_groupNumber is too high)
			return 0;
		}

		public override BracketModel GetModel(int _tournamentID)
		{
			BracketModel model = base.GetModel(_tournamentID);
			model.NumberOfGroups = this.NumberOfGroups;

			return model;
		}

		public List<IPlayerScore> GetGroupRanking(int _groupNumber)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}
			if (_groupNumber > NumberOfGroups)
			{
				return new List<IPlayerScore>();
			}

			return GroupRankings[_groupNumber - 1];
		}

		public List<IMatch> GetRound(int _groupNumber, int _round)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			return GetRound(_round)
				.Where(m => m.GroupNumber == _groupNumber)
				.ToList();
		}

		public List<IMatch> GetLowerRound(int _groupNumber, int _round)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			return GetLowerRound(_round)
				.Where(m => m.GroupNumber == _groupNumber)
				.ToList();
		}
		#endregion
		#region Mutators
		public void SetMaxGamesForWholeRound(int _groupNumber, int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}
			if (0 == _maxGamesPerMatch % 2)
			{
				throw new ScoreException
					("Games per Match must be ODD in an elimination bracket!");
			}

			List<IMatch> round = GetRound(_groupNumber, _round);
			if (round.Any(m => m.IsFinished))
			{
				throw new InactiveMatchException
					("One or more matches in this round is already finished!");
			}

			foreach (Match match in round)
			{
				match.SetMaxGames(_maxGamesPerMatch);
			}
		}

		public void SetMaxGamesForWholeLowerRound(int _groupNumber, int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}
			if (0 == _maxGamesPerMatch % 2)
			{
				throw new ScoreException
					("Games per Match must be ODD in an elimination bracket!");
			}

			List<IMatch> round = GetLowerRound(_groupNumber, _round);
			if (round.Any(m => m.IsFinished))
			{
				throw new InactiveMatchException
					("One or more matches in this round is already finished!");
			}

			foreach (Match match in round)
			{
				match.SetMaxGames(_maxGamesPerMatch);
			}
		}
		#endregion
		#endregion

		#region Private Methods
		protected override void SetDataFromModel(BracketModel _model)
		{
			// Call the base (Bracket) method to set common data and playerlist:
			base.SetDataFromModel(_model);
			this.NumberOfGroups = _model.NumberOfGroups;

			if (_model.Matches.Count > 0)
			{
				foreach (MatchModel matchModel in _model.Matches)
				{
					// Convert each MatchModel to a Match:
					Match m = new Match(matchModel);

					// Add the Match to the appropriate dictionary:
					if (m.NextLoserMatchNumber > 0)
					{
						Matches.Add(m.MatchNumber, m);
					}
					else
					{
						LowerMatches.Add(m.MatchNumber, m);
					}
				}

				// Set the Bracket properties:
				this.NumberOfMatches = Matches.Count + LowerMatches.Count;
				this.NumberOfRounds = Matches.Values
					.Max(m => m.RoundIndex);
				this.NumberOfLowerRounds = LowerMatches.Values
					.Max(m => m.RoundIndex);
				this.IsFinished = LowerMatches.Values
					.All(m => m.IsFinished);
			}

			// Go through the Matches to recalculate the current Rankings:
			RecalculateRankings();

			if (this.IsFinalized && false == Validate())
			{
				throw new BracketValidationException
					("Bracket is Finalized but not Valid!");
			}
		}

		protected override void RecalculateRankings()
		{
			if (null == Rankings)
			{
				Rankings = new List<IPlayerScore>();
			}
			Rankings.Clear();
			if (null == GroupRankings)
			{
				GroupRankings = new List<List<IPlayerScore>>();
				GroupRankings.Capacity = NumberOfGroups;
			}
			GroupRankings.Clear();

			for (int n = 1; n <= NumberOfMatches; ++n)
			{
				Match match = GetInternalMatch(n);

				if (match.IsFinished)
				{
					if (match.NextLoserMatchNumber < 0)
					{
						// Add losing Player to the Rankings:
						int rank = CalculateRank(match.MatchNumber);
						IPlayer losingPlayer = match.Players[
							(PlayerSlot.Defender == match.WinnerSlot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						IPlayerScore ps = new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank);

						Rankings.Add(ps);
						GroupRankings[match.GroupNumber - 1].Add(ps);
					}

					if (match.NextMatchNumber < 0)
					{
						// Add winning Player to the Rankings:
						// (rank=1 from upper bracket, rank=2 from lower bracket)
						int rank = (Matches.ContainsKey(match.MatchNumber)) ? 1 : 2;
						IPlayer winningPlayer = match.Players[(int)match.WinnerSlot];
						IPlayerScore ps = new PlayerScore(winningPlayer.Id, winningPlayer.Name, rank);

						Rankings.Add(ps);
						GroupRankings[match.GroupNumber - 1].Add(ps);
					}
				}
			}

			// Sort the Rankings by player rank:
			// (tied ranks are ordered by seed)
			Rankings.Sort(SortRankingRanks);
			foreach (List<IPlayerScore> group in GroupRankings)
			{
				group.Sort(SortRankingRanks);
			}

			if (LowerMatches.Values.All(m => m.IsFinished))
			{
				this.IsFinished = true;
			}
		}
		#endregion
	}
}
