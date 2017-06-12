// #define ENABLE_TIEBREAKERS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class RoundRobinBracket : Bracket
	{
		#region Variables & Properties
		//public int Id
		//public BracketType BracketType
		//public bool IsFinalized
		//public bool IsFinished
		//public List<IPlayer> Players
		//public List<IPlayerScore> Rankings
		//public int MaxRounds
		//protected Dictionary<int, Match> Matches
		//public int NumberOfRounds
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue
		//protected int MatchTieValue
		#endregion

		#region Ctors
		public RoundRobinBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.ROUNDROBIN;
			MaxRounds = _numberOfRounds;
			MatchWinValue = 2;
			MatchTieValue = 1;

			CreateBracket(_maxGamesPerMatch);
		}
		public RoundRobinBracket()
			: this(new List<IPlayer>())
		{ }
		public RoundRobinBracket(BracketModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.BracketID;
			this.BracketType = _model.BracketType.Type;
			this.IsFinalized = _model.Finalized;
			this.MaxRounds = _model.MaxRounds;
			this.MatchWinValue = 2;
			this.MatchTieValue = 1;
			ResetBracketData();

			List<TournamentUserModel> userModels = _model.TournamentUsersBrackets
				.OrderBy(tubm => tubm.Seed, new SeedComparer())
				.Select(tubm => tubm.TournamentUser)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (TournamentUserModel userModel in userModels)
			{
				Players.Add(new Player(userModel));
				Rankings.Add(new PlayerScore(userModel.TournamentUserID, userModel.Name));
			}

			foreach (MatchModel mm in _model.Matches)
			{
				// Create the Match:
				Matches.Add(mm.MatchNumber, new Match(mm));
			}
			this.NumberOfMatches = Matches.Count;
			this.NumberOfRounds = 0;

			this.IsFinished = false;
			if (NumberOfMatches > 0)
			{
				this.NumberOfRounds = Matches.Values
					.Select(m => m.RoundIndex)
					.Last();
				if (Matches.Values.Any(m => !m.IsFinished))
				{
					this.IsFinished = false;
				}
				else
				{
					this.IsFinished = true;
				}
			}

			RecalculateRankings();
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();
			if (_gamesPerMatch < 1)
			{
				throw new BracketException
					("Games Per Match must be positive!");
			}
			if (Players.Count < 2)
			{
				return;
			}
			foreach (IPlayer player in Players)
			{
				Rankings.Add(new PlayerScore(player.Id, player.Name));
			}

			int totalRounds = (0 == Players.Count % 2)
				? Players.Count - 1 : Players.Count;

			// Randomly choose which rounds to "remove"
			// (only applies if MaxRounds is capped)
			List<int> roundsToRemove = new List<int>();
			if (MaxRounds > 0 && MaxRounds < totalRounds)
			{
				int roundsDiff = totalRounds - MaxRounds;
				Random rng = new Random();
				while (roundsToRemove.Count < roundsDiff)
				{
					int randomRound = rng.Next(totalRounds);
					if (!roundsToRemove.Contains(randomRound))
					{
						roundsToRemove.Add(randomRound);
					}
				}
			}

			// Create all the matchups:
			int matchesPerRound = (int)(Players.Count * 0.5);
			for (int r = 0; r < totalRounds; ++r)
			{
				if (roundsToRemove.Contains(r))
				{
					continue;
				}
				++NumberOfRounds;

				for (int m = 0; m < matchesPerRound; ++m, ++NumberOfMatches)
				{
					Match match = new Match();
					match.SetMatchNumber(NumberOfMatches + 1);
					match.SetRoundIndex(NumberOfRounds);
					match.SetMatchIndex(m + 1);
					match.SetMaxGames(_gamesPerMatch);
					match.AddPlayer(Players[(m + r) % Players.Count]);
					match.AddPlayer(Players[(Players.Count - 1 - m + r) % Players.Count]);

					Matches.Add(match.MatchNumber, match);
				}
			}
		}

		public override void ResetMatches()
		{
			base.ResetMatches();
			foreach (IPlayerScore ps in Rankings)
			{
				ps.Rank = 1;
				ps.ResetScore();
			}
		}

		public override bool CheckForTies()
		{
			if (!IsFinished)
			{
				throw new BracketException
					("Bracket isn't finished yet!");
			}

			// Calculate W/L scores for all players:
			int[] scores = new int[Rankings.Count];
			for (int i = 0; i < Rankings.Count; ++i)
			{
				scores[i] = Rankings[i].CalculateScore(MatchWinValue, MatchTieValue, 0);

				if (i > 0 && scores[i] == scores[i - 1])
				{
					// Found a tie. We're finished:
					return true;
				}
			}
			// No ties found.
			return false;
		}
		public override bool GenerateTiebreakers()
		{
			if (!IsFinished)
			{
				throw new BracketException
					("Bracket isn't finished yet!");
			}

			// Calculate W/L scores for all players:
			int[] scores = new int[Rankings.Count];
			for (int i = 0; i < Rankings.Count; ++i)
			{
				scores[i] = Rankings[i].CalculateScore(MatchWinValue, MatchTieValue, 0);
			}

			// Create a "group" for any tied players:
			List<List<int>> tiedGroups = new List<List<int>>();
			for (int i = 0; i < Rankings.Count - 1;)
			{
				int j = i + 1;
				for (; j < Rankings.Count; ++j)
				{
					if (scores[i] != scores[j])
					{
						break;
					}

					if (i + 1 == j)
					{
						tiedGroups.Add(new List<int>());
						tiedGroups[tiedGroups.Count - 1].Add(Rankings[i].Id);
					}
					tiedGroups[tiedGroups.Count - 1].Add(Rankings[j].Id);
				}
				i = j;
			}
			if (0 == tiedGroups.Count)
			{
				// No ties: bracket is finished, so just leave:
				return false;
			}
			// else:
			this.IsFinished = false;
			List<MatchModel> newMatchModels = new List<MatchModel>();

			// Create a bracket for each group:
			List<IBracket> tiebreakerBrackets = new List<IBracket>();
			foreach (List<int> group in tiedGroups)
			{
				List<IPlayer> pList = new List<IPlayer>();
				foreach (int id in group)
				{
					pList.Add(Players.Find(p => p.Id == id));
				}
				tiebreakerBrackets.Add(new RoundRobinBracket(pList));
			}

			// "Copy" matches from new pseudo-brackets onto the end of this bracket:
			for (int r = 1; ; ++r) // Run through once for each new round
			{
				// Round-by-round, make a list of matches to add:
				List<IMatch> currRound = new List<IMatch>();
				foreach (IBracket bracket in tiebreakerBrackets.Where(b => b.NumberOfRounds >= r))
				{
					currRound.AddRange(bracket.GetRound(r));
				}
				if (0 == currRound.Count)
				{
					// No more new matches; break out:
					break;
				}

				// Add a new round and copy applicable matches:
				this.NumberOfRounds++;
				for (int m = 0; m < currRound.Count; ++m)
				{
					Match match = new Match();
					match.SetMatchNumber(++NumberOfMatches);
					match.SetRoundIndex(NumberOfRounds);
					match.SetMatchIndex(m + 1);
					match.AddPlayer(currRound[m].Players[0]);
					match.AddPlayer(currRound[m].Players[1]);

					Matches.Add(match.MatchNumber, match);
					// Also add a model, for firing the event:
					newMatchModels.Add(GetMatchModel(match));
				}
			}

			// Fire event to notify that new Matches were made, and return:
			OnRoundAdded(new BracketEventArgs(newMatchModels));
			return true;
		}
		#endregion

		#region Private Methods
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			this.IsFinished = !(Matches.Values.Any(m => !m.IsFinished));
#if ENABLE_TIEBREAKERS
			// Check for, and create, Tiebreaker matches:
			if (IsFinished && BracketType.ROUNDROBIN == this.BracketType)
			{
				// Calculate W/L scores for all players:
				int[] scores = new int[Rankings.Count];
				for (int i = 0; i < Rankings.Count; ++i)
				{
					scores[i] = Rankings[i].CalculateScore(MatchWinValue, MatchTieValue, 0);
				}

				// Create a "group" for any tied players:
				List<List<int>> tiedGroups = new List<List<int>>();
				for (int i = 0; i < Rankings.Count - 1; )
				{
					int j = i + 1;
					for ( ; j < Rankings.Count; ++j)
					{
						if (scores[i] != scores[j])
						{
							break;
						}

						if (i + 1 == j)
						{
							tiedGroups.Add(new List<int>());
							tiedGroups[tiedGroups.Count - 1].Add(Rankings[i].Id);
						}
						tiedGroups[tiedGroups.Count - 1].Add(Rankings[j].Id);
					}
					i = j;
				}
				if (0 == tiedGroups.Count)
				{
					// No ties: bracket is finished, so just leave:
					return (new List<MatchModel>());
				}
				// else:
				this.IsFinished = false;
				List<MatchModel> newMatchModels = new List<MatchModel>();

				// Create a bracket for each group:
				List<IBracket> tiebreakerBrackets = new List<IBracket>();
				foreach (List<int> group in tiedGroups)
				{
					List<IPlayer> pList = new List<IPlayer>();
					foreach (int id in group)
					{
						pList.Add(Players.Find(p => p.Id == id));
					}
					tiebreakerBrackets.Add(new RoundRobinBracket(pList));
				}

				// "Copy" matches from new pseudo-brackets onto the end of this bracket:
				int r = 0;
				while (true) // Run through once for each new round
				{
					// Round-by-round, make a list of matches to add:
					++r;
					List<IMatch> currRound = new List<IMatch>();
					foreach (IBracket bracket in tiebreakerBrackets.Where(b => b.NumberOfRounds >= r))
					{
						currRound.AddRange(bracket.GetRound(r));
					}
					if (0 == currRound.Count)
					{
						// No more new matches; break out:
						break;
					}

					// Add a new round and copy applicable matches:
					this.NumberOfRounds++;
					for (int m = 0; m < currRound.Count; ++m)
					{
						Match match = new Match();
						match.SetMatchNumber(++NumberOfMatches);
						match.SetRoundIndex(NumberOfRounds);
						match.SetMatchIndex(m + 1);
						match.AddPlayer(currRound[m].Players[0]);
						match.AddPlayer(currRound[m].Players[1]);

						Matches.Add(match.MatchNumber, match);
						// Also add a model, for firing the event:
						newMatchModels.Add(GetMatchModel(match));
					}
				}

				// Fire event to notify that new Matches were made:
				OnRoundAdded(new BracketEventArgs(newMatchModels));
			}
#endif
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
			foreach (IPlayerScore ps in Rankings)
			{
				ps.Rank = 1;
				ps.ResetScore();
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

			// Sort the list and apply Ranks:
			Rankings.Sort(SortRankingScores);
			for (int i = 0; i < Rankings.Count; ++i)
			{
				Rankings[i].Rank = i + 1;
			}
		}
		#endregion
	}
}
