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
			MatchWinValue = 2;
			MatchTieValue = 1;

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
			if (NumberOfGroups < 2 ||
				NumberOfGroups * 2 > Players.Count)
			{
				return;
			}

			List<IBracket> groups = new List<IBracket>();

			List<List<IPlayer>> playerGroups = DividePlayersIntoGroups();
			GroupRankings.Capacity = NumberOfGroups;

			for (int g = 0; g < playerGroups.Count; ++g)
			{
				groups.Add(new RoundRobinBracket(playerGroups[g], _gamesPerMatch, MaxRounds));
#if false
				GroupRankings.Add(new List<IPlayerScore>());
				GroupRankings[g].Capacity = playerGroups[g].Count;
				foreach (IPlayer player in playerGroups[g])
				{
					GroupRankings[g].Add(new PlayerScore(player.Id, player.Name));
					Rankings.Add(GroupRankings[g][GroupRankings[g].Count - 1]);
				}
#endif
			}

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

		public override void ResetMatches()
		{
			base.ResetMatches();

			Rankings.Clear();
			foreach (List<IPlayerScore> group in GroupRankings)
			{
				foreach (IPlayerScore playerScore in group)
				{
					playerScore.Rank = 1;
					playerScore.ResetScore();
				}

				Rankings.AddRange(group);
			}
		}

		public override bool CheckForTies()
		{
			for (int g = 0; g < NumberOfGroups; ++g)
			{
				if (Matches.Values
					.Where(m => m.GroupNumber == 1 + g)
					.All(m => m.IsFinished))
				{
					int[] scores = new int[GroupRankings[g].Count];
					for (int i = 0; i < GroupRankings[g].Count; ++i)
					{
						scores[i] = GroupRankings[g][i]
							.CalculateScore(MatchWinValue, MatchTieValue, 0);

						if (i > 0 && scores[i] == scores[i - 1])
						{
							// Found a tie. We're finished:
							return true;
						}
					}
				}
			}
			
			return false;
		}
		public override bool GenerateTiebreakers()
		{
			throw new NotImplementedException();
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
			this.NumberOfRounds = Matches.Values
				.Select(m => m.RoundIndex)
				.Max();
			this.IsFinished = Matches.Values
				.All(m => m.IsFinished);

			List<List<IPlayer>> playerGroups = DividePlayersIntoGroups();
			for (int g = 0; g < playerGroups.Count; ++g)
			{
				GroupRankings.Add(new List<IPlayerScore>());
				foreach (IPlayer player in playerGroups[g])
				{
					IPlayerScore pScore = new PlayerScore(player.Id, player.Name);
					Rankings.Add(pScore);
					GroupRankings[g].Add(pScore);
				}
			}
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
			IMatch match = GetMatch(_matchNumber);

			if (null == _games)
			{
				// Case 1: Match winner was manually set.
				// Apply outcome to rankings (but no games!):
				int winnerIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)(match.WinnerSlot)].Id);
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger : PlayerSlot.Defender;
				int loserIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)loserSlot].Id);

				Rankings[winnerIndex].AddMatchOutcome(Outcome.Win, true);
				Rankings[loserIndex].AddMatchOutcome(Outcome.Loss, true);
			}
			else if (_oldMatch.IsManualWin && !(match.IsManualWin))
			{
				// Case 2: Match had a manual winner: being removed.
				// Update rankings accordingly, ignoring individual game scores:
				int loserId = (_oldMatch.WinnerID == _oldMatch.DefenderID)
					? _oldMatch.ChallengerID : _oldMatch.DefenderID;

				Rankings.Find(r => r.Id == _oldMatch.WinnerID)
					.AddMatchOutcome(Outcome.Win, false);
				Rankings.Find(r => r.Id == loserId)
					.AddMatchOutcome(Outcome.Loss, false);
			}
			else
			{
				// Standard case: Update score for new game(s):
				int defIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Defender].Id);
				int chalIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Challenger].Id);

				// First, find out if the Match's Outcome is changing:
				bool oldMatchFinished = _oldMatch.IsManualWin;
				if (!oldMatchFinished)
				{
					if (_oldMatch.WinnerID.HasValue && _oldMatch.WinnerID > -1)
					{
						oldMatchFinished = true;
					}
					else if (_oldMatch.DefenderScore + _oldMatch.ChallengerScore >= match.MaxGames)
					{
						oldMatchFinished = true;
					}
				}
				if (match.IsFinished != oldMatchFinished)
				{
					// Match Outcome has changed.
					PlayerSlot oldWinnerSlot = (_oldMatch.WinnerID == _oldMatch.DefenderID)
						? PlayerSlot.Defender : PlayerSlot.unspecified;
					oldWinnerSlot = (_oldMatch.WinnerID == _oldMatch.ChallengerID)
						? PlayerSlot.Challenger : oldWinnerSlot;

					// Find the Outcome type to update:
					Outcome defenderOutcome = Outcome.Tie;
					Outcome challengerOutcome = Outcome.Tie;
					if (PlayerSlot.Defender == match.WinnerSlot ||
						PlayerSlot.Defender == oldWinnerSlot)
					{
						defenderOutcome = Outcome.Win;
						challengerOutcome = Outcome.Loss;
					}
					else if (PlayerSlot.Challenger == match.WinnerSlot ||
						PlayerSlot.Challenger == oldWinnerSlot)
					{
						defenderOutcome = Outcome.Loss;
						challengerOutcome = Outcome.Win;
					}

					// Add/subtract the Match Outcome:
					Rankings[defIndex].AddMatchOutcome(defenderOutcome, _isAddition);
					Rankings[chalIndex].AddMatchOutcome(challengerOutcome, _isAddition);
				}

				// Now, calculate the Score updates:
				if (_games.Count > 0 && !(match.IsManualWin))
				{
					int defenderGameScore = 0, defenderPointScore = 0;
					int challengerGameScore = 0, challengerPointScore = 0;

					foreach (GameModel model in _games)
					{
						// GameScore +1 for wins, -1 for losses
						if (model.WinnerID == model.DefenderID)
						{
							++defenderGameScore;
							--challengerGameScore;
						}
						else if (model.WinnerID == model.ChallengerID)
						{
							--defenderGameScore;
							++challengerGameScore;
						}
						// PointScore adjusts for "points" in each Game
						defenderPointScore += model.DefenderScore;
						challengerPointScore += model.ChallengerScore;
					}

					// Apply the Score updates:
					Rankings[defIndex].UpdateScores
						(defenderGameScore, defenderPointScore, _isAddition);
					Rankings[chalIndex].UpdateScores
						(challengerGameScore, challengerPointScore, _isAddition);
				}
			}

			UpdateRankings();
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

			foreach (List<IPlayerScore> rankGroup in GroupRankings)
			{
				rankGroup.Sort(SortRankingScores);
				for (int i = 0; i < rankGroup.Count; ++i)
				{
					rankGroup[i].Rank = i + 1;
				}
			}

			Rankings.Sort(SortRankingRanks);
		}

		protected override void ResetBracketData()
		{
			base.ResetBracketData();

			if (null == GroupRankings)
			{
				GroupRankings = new List<List<IPlayerScore>>();
			}
			GroupRankings.Clear();
		}
		#endregion
	}
}
