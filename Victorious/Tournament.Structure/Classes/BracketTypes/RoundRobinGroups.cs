using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class RoundRobinGroups : GroupStage
	{
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
		//public int NumberOfGroups
		#endregion

		#region Ctors
		public RoundRobinGroups(List<IPlayer> _players, int _numberOfGroups, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.RRGROUP;
			NumberOfGroups = _numberOfGroups;
			MaxRounds = _numberOfRounds;

			CreateBracket(_maxGamesPerMatch);
		}
		public RoundRobinGroups()
			: this(new List<IPlayer>(), 0, 0)
		{ }
		public RoundRobinGroups(BracketModel _model)
		{
			SetDataFromModel(_model);
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();
			List<IBracket> groups = new List<IBracket>();

			List<List<IPlayer>> playerGroups = DividePlayersIntoGroups();
			GroupRankings.Capacity = NumberOfGroups;
#if false
			for (int g = 0; g < playerGroups.Count; ++g)
			{
				groups.Add(new RoundRobinBracket(playerGroups[g], _gamesPerMatch, MaxRounds));

				GroupRankings.Add(new List<IPlayerScore>());
				GroupRankings[g].Capacity = playerGroups[g].Count;
				foreach (IPlayer player in playerGroups[g])
				{
					GroupRankings[g].Add(new PlayerScore(player.Id, player.Name));
					Rankings.Add(GroupRankings[g][GroupRankings[g].Count - 1]);
				}
			}
#endif
			for (int g = 0; g < groups.Count; ++g)
			{
				for (int m = 1; m <= groups[g].NumberOfMatches; ++m)
				{
					++NumberOfMatches;
					IMatch currMatch = groups[g].GetMatch(m);

					if (0 == g)
					{
						Matches.Add(currMatch.MatchNumber, (currMatch as Match));
					}
					else
					{
						Match match = new Match();
						match.SetMaxGames(currMatch.MaxGames);
						match.SetRoundIndex(currMatch.RoundIndex);
						match.SetMatchIndex(currMatch.MatchIndex);
						match.SetMatchNumber(NumberOfMatches);
						match.AddPlayer(currMatch.Players[(int)PlayerSlot.Defender]);
						match.AddPlayer(currMatch.Players[(int)PlayerSlot.Challenger]);

						Matches.Add(match.MatchNumber, match);
					}
					Matches[NumberOfMatches].SetGroupNumber(g + 1);
				}

				Rankings.AddRange(groups[g].Rankings);
				GroupRankings.Add(groups[g].Rankings);
			}

			NumberOfRounds = Matches.Values
				.Select(m => m.RoundIndex)
				.Max();
			Rankings.Sort(SortRankingRanks);
		}

		public override bool CheckForTies()
		{
			for (int g = 1; g <= NumberOfGroups; ++g)
			{
				IBracket 
			}


			if (!(Groups.Any(g => g.IsFinished)))
			{
				throw new BracketException
					("No groups are finished yet!");
			}

			foreach (IBracket group in Groups.Where(g => g.IsFinished))
			{
				if (group.CheckForTies())
				{
					return true;
				}
			}
			return false;
		}
		public override bool GenerateTiebreakers()
		{
			if (!(Groups.Any(g => g.IsFinished)))
			{
				throw new BracketException
					("No groups are finished yet!");
			}

			bool addedMatches = false;
			foreach (IBracket group in Groups.Where(g => g.IsFinished))
			{
				addedMatches |= group.GenerateTiebreakers();
			}

			return addedMatches;
		}
		#endregion

		#region Private Methods
		protected override void SetDataFromModel(BracketModel _model)
		{
			base.SetDataFromModel(_model);
			this.NumberOfGroups = _model.NumberOfGroups;

			foreach (MatchModel matchModel in _model.Matches)
			{
				Matches.Add(matchModel.MatchNumber, new Match(matchModel));
			}

			this.IsFinished = Matches.Values
				.All(m => m.IsFinished);
			RecalculateRankings();

			if (this.IsFinalized && false == Validate())
			{
				throw new BracketValidationException
					("Bracket is Finalized but not Valid!");
			}
		}

		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			this.IsFinished = Matches.Values
				.All(m => m.IsFinished);
			return (new List<MatchModel>());
		}
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			this.IsFinished = (IsFinished && GetMatch(_matchNumber).IsFinished);
			return (new List<MatchModel>());
		}
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			
		}

		protected override void RecalculateRankings()
		{
			if (0 == (Players?.Count ?? 0))
			{
				return;
			}

			foreach (IPlayerScore playerScore in Rankings)
			{
				playerScore.Rank = 1;
				playerScore.ResetScore();
			}

			foreach (IMatch match in Matches.Values.Where(m => !(m.Players.Contains(null))))
			{
				int defIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Defender].Id);
				int chalIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Challenger].Id);

				if (match.IsFinished)
				{
					// Apply the match outcome:
					Outcome defOutcome = Outcome.Tie;
					Outcome chalOutcome = Outcome.Tie;
					switch (match.WinnerSlot)
					{
						case PlayerSlot.Defender:
							defOutcome = Outcome.Win;
							chalOutcome = Outcome.Loss;
							break;
						case PlayerSlot.Challenger:
							defOutcome = Outcome.Loss;
							chalOutcome = Outcome.Win;
							break;
					}

					Rankings[defIndex].AddMatchOutcome(defOutcome, true);
					Rankings[chalIndex].AddMatchOutcome(chalOutcome, true);
				}
				if (match.Games.Count > 0 && !(match.IsManualWin))
				{
					// Update the player scores:
					int defScore = 0, chalScore = 0;
					foreach (IGame game in match.Games)
					{
						defScore += game.Score[(int)PlayerSlot.Defender];
						chalScore += game.Score[(int)PlayerSlot.Challenger];
					}

					Rankings[defIndex].UpdateScores
						(match.Score[(int)PlayerSlot.Defender], defScore, true);
					Rankings[chalIndex].UpdateScores
						(match.Score[(int)PlayerSlot.Challenger], chalScore, true);
				}
			}

			UpdateRankings();
		}
		protected override void UpdateRankings()
		{
			// Calculate MatchScore values for each player:
			int[] playerScores = new int[Rankings.Count];
			for (int p = 0; p < Rankings.Count; ++p)
			{
				Rankings[p].ResetOpponentsScore();
				playerScores[p] = Rankings[p].CalculateScore(MatchWinValue, MatchTieValue, 0);
			}
			// Calculate & Assign OpponentsPoints value for each player:
			foreach (IMatch match in Matches.Values.Where(m => m.IsFinished).ToList())
			{
				Rankings
					.Find(p => p.Id == match.Players[(int)PlayerSlot.Challenger].Id)
					.AddToOpponentsScore(playerScores[Rankings
						.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Defender].Id)]);
				Rankings
					.Find(p => p.Id == match.Players[(int)PlayerSlot.Defender].Id)
					.AddToOpponentsScore(playerScores[Rankings
						.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Challenger].Id)]);
			}

			foreach (List<IPlayerScore> group in GroupRankings)
			{
				group.Clear();
			}
			List<List<IPlayer>> playerGroups = DividePlayersIntoGroups();
			for (int g = 0; g < playerGroups.Count; ++g)
			{
				foreach (IPlayer player in playerGroups[g])
				{
					GroupRankings[g].Add(Rankings.Find(r => r.Id == player.Id));
				}

				GroupRankings[g].Sort(SortRankingScores);
				for (int i = 0; i < GroupRankings[g].Count; ++i)
				{
					GroupRankings[g][i].Rank = i + 1;
					Rankings.Find(r => r.Id == GroupRankings[g][i].Id)
						.Rank = i + 1;
				}
			}

			Rankings.Sort(SortRankingRanks);
		}

		protected override void AddRounds(object _sender, BracketEventArgs _args)
		{
			// Base method relays the RoundsAdded event:
			base.AddRounds(_sender, _args);

			// Update Matches and Rounds:
			NumberOfMatches += _args.UpdatedMatches.Count;
			NumberOfRounds += _args.UpdatedMatches
				.Select(m => m.RoundIndex).Distinct()
				.Count();
			// Get the highest MatchNumber from the new matches:
			int highestNewMatchNum = _args.UpdatedMatches
				.Select(m => m.MatchNumber)
				.Max();

			// Make a list of all the matches with "updated" match numbers:
			List<MatchModel> matchesToUpdate = new List<MatchModel>();
			for (int n = highestNewMatchNum + 1; n <= NumberOfMatches; ++n)
			{
				matchesToUpdate.Add(GetMatchModel(n));
			}

			// Fire event with the updated matches:
			OnMatchesModified(matchesToUpdate);
		}

		protected override void ResetBracketData()
		{
			base.ResetBracketData();

			if (null == GroupRankings)
			{
				GroupRankings = new List<List<IPlayerScore>>():
			}
			GroupRankings.Clear();
		}
		#endregion
	}
}
