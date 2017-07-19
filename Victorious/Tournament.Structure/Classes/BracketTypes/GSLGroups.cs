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
			//protected int MatchWinValue
			//protected int MatchTieValue
			//protected List<IBracket> Groups = empty
			#endregion

			#region Ctors
			public GSLBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
				: base(_players, _maxGamesPerMatch)
			{ }
			public GSLBracket()
				: this(new List<IPlayer>())
			{ }
			public GSLBracket(BracketModel _model)
			{
				throw new NotImplementedException();
			}
			#endregion

			#region Public Methods
			/// <summary>
			/// Uses the playerlist to generate the bracket structure & matchups.
			/// This creates & populates all the Match objects.
			/// </summary>
			/// <remarks>
			/// This particular method override deletes the Grand Finals match,
			/// which was created by the parent class.
			/// GSL does not use a Grand Final.
			/// </remarks>
			/// <param name="_gamesPerMatch">Max games for every Match</param>
			public override void CreateBracket(int _gamesPerMatch = 1)
			{
				base.CreateBracket(_gamesPerMatch);
				grandFinal = null;
				--NumberOfMatches;
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

		/// <summary>
		/// A list of PlayerScores for each group.
		/// Each group is sorted.
		/// Each PlayerScore is a copied reference from the main Rankings.
		/// These can be referenced with GetGroupRanking().
		/// </summary>
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
		/// <summary>
		/// Verifies this bracket's status is legal.
		/// This is called before allowing play to begin.
		/// </summary>
		/// <returns>true if okay, false if errors</returns>
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

		/// <summary>
		/// Uses the playerlist to generate the bracket structure & matchups.
		/// This creates & populates all the Match objects.
		/// If any Matches already exist, they will be deleted first.
		/// If there are <2 players, nothing will be made.
		/// If there are not 4 players per group, nothing will be made.
		/// </summary>
		/// <param name="_gamesPerMatch">Max games for every Match</param>
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

		/// <summary>
		/// Resets the state of all Matches and bracket progression.
		/// Deletes all Games and sets scores to 0-0.
		/// Removes Players from Matches they had advanced to.
		/// Clears Rankings lists.
		/// May fire MatchesModified and GamesDeleted events, if updates occur.
		/// </summary>
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

		#region Player Methods
		/// <summary>
		/// Removes a Player from the playerlist,
		/// and replaces him with a given Player.
		/// The new Player inherits the old's seed value.
		/// The removed Player is replaced in all groups, Matches, Games, & Rankings
		/// by the new Player.
		/// May fire MatchesModified event, if updates happen.
		/// If the Player-to-replace's index is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_player">Player to add</param>
		/// <param name="_index">Index (in playerlist) of Player to remove</param>
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
		#endregion

		#region Accessors
		/// <summary>
		/// Returns the number of UPPER bracket rounds
		/// in the specified group.
		/// If group number is negative, an exception is thrown.
		/// If group number is too high, returns 0.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <returns>Number of rounds</returns>
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

		/// <summary>
		/// Returns the number of LOWER bracket rounds
		/// in the specified group.
		/// If group number is negative, an exception is thrown.
		/// If group number is too high, returns 0.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <returns>Number of rounds</returns>
		public int NumberOfLowerRoundsInGroup(int _groupNumber)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			List<int> roundNums = LowerMatches.Values
				.Where(m => m.GroupNumber == _groupNumber)
				.Select(m => m.RoundIndex).ToList();

			if (roundNums.Any())
			{
				return roundNums.Max();
			}
			// Else: (_groupNumber is too high)
			return 0;
		}

		/// <summary>
		/// Creates a Model of this Bracket's current state.
		/// Any contained objects (Players, Matches) are also converted into Models.
		/// </summary>
		/// <param name="_tournamentID">ID of containing Tournament</param>
		/// <returns>Matching BracketModel</returns>
		public override BracketModel GetModel(int _tournamentID)
		{
			BracketModel model = base.GetModel(_tournamentID);
			model.NumberOfGroups = this.NumberOfGroups;

			return model;
		}

		/// <summary>
		/// Gets the ordered Rankings list for the given group.
		/// If the given group number is <1, an exception is thrown.
		/// If the group number is out of range, an empty list is returned.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <returns>Sorted list of IPlayerScore rankings objects</returns>
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

		/// <summary>
		/// Gets all Matches in a specified UPPER round, from the given group.
		/// If the group number is <1, an exception is thrown.
		/// If the round number is <1, an exception is thrown.
		/// If either is otherwise out-of-range, an empty list is returned.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <returns>Sorted list of IMatches in the given group's round</returns>
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

		/// <summary>
		/// Gets all Matches in a specified LOWER round, from the given group.
		/// If the group number is <1, an exception is thrown.
		/// If the round number is <1, an exception is thrown.
		/// If either is otherwise out-of-range, an empty list is returned.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <returns>Sorted list of IMatches in the given group's round</returns>
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
		/// <summary>
		/// Sets the max number of games per match for one UPPER round,
		/// in the specified group.
		/// If Max Games is invalid, an exception is thrown.
		/// If the group number is <1, an exception is thrown.
		/// If the round number is <1, an exception is thrown.
		/// If any matches are already finished, an exception is thrown.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <param name="_maxGamesPerMatch">How many Games each Match may last</param>
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

		/// <summary>
		/// Sets the max number of games per match for one LOWER round,
		/// in the specified group.
		/// If Max Games is invalid, an exception is thrown.
		/// If the group number is <1, an exception is thrown.
		/// If the round number is <1, an exception is thrown.
		/// If any matches are already finished, an exception is thrown.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <param name="_maxGamesPerMatch">How many Games each Match may last</param>
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
		/// <summary>
		/// Sets this Bracket's main data from a related BracketModel.
		/// Data affected includes most fields, as well as the playerlist.
		/// </summary>
		/// <param name="_model">Related BracketModel</param>
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

		/// <summary>
		/// Resets the Bracket.
		/// Affects Matches, Rankings, and bracket status.
		/// Also clears group-specific Rankings lists.
		/// </summary>
		protected override void ResetBracketData()
		{
			base.ResetBracketData();

			if (null == GroupRankings)
			{
				GroupRankings = new List<List<IPlayerScore>>();
				GroupRankings.Capacity = NumberOfGroups;
			}
			GroupRankings.Clear();
		}

		/// <summary>
		/// Processes the effects of adding a "game win" to a Match.
		/// If the Match ends, the winner and loser advance,
		/// so long as they have legal next matches to advance to.
		/// </summary>
		/// <remarks>
		/// This particular override method checks and sets
		/// the Bracket's status (finished or not).
		/// </remarks>
		/// <param name="_matchNumber">Number of Match initially affected</param>
		/// <param name="_slot">Slot of game winner: Defender or Challenger</param>
		/// <returns>List of Models of Matches that are changed</returns>
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			// All the progression logic is handled by KnockoutBracket's parent method:
			List<MatchModel> alteredMatches = base.ApplyWinEffects(_matchNumber, _slot);

			// The base method can erroneously set the whole bracket as finished.
			// Check for that, and fix if necessary:
			if (this.IsFinished &&
				LowerMatches.Values.Any(m => !m.IsFinished))
			{
				this.IsFinished = false;
			}

			return alteredMatches;
		}

		/// <summary>
		/// Clears the Rankings, and recalculates them from the Matches list.
		/// Finds every eliminated player, calculates his rank, and adds him to the Rankings.
		/// Sorts the Rankings.
		/// If no players have been eliminated, the Rankings will be an empty list.
		/// Does the same with each group's internal Rankings.
		/// </summary>
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
