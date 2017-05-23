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
		// int Id
		// BracketType BracketType
		// bool IsFinalized
		// bool IsFinished
		// List<IPlayer> Players
		// List<IPlayerScore> Rankings
		// int MaxRounds
		// Dictionary<int, IMatch> Matches
		// int NumberOfRounds
		// Dictionary<int, IMatch> LowerMatches -- unused
		// int NumberOfLowerRounds -- unused
		// IMatch GrandFinal -- unused
		// int NumberOfMatches
		// int MatchWinValue
		// int MatchTieValue
		#endregion

		#region Ctors
		public RoundRobinBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = new List<IPlayer>();
			if (_players.Count > 0 && _players[0] is User)
			{
				foreach (IPlayer p in _players)
				{
					Players.Add(new User(p as User));
				}
			}
			else if (_players.Count > 0 && _players[0] is Team)
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
			foreach (TournamentUserModel model in userModels)
			{
				Players.Add(new User(model));
				Rankings.Add(new PlayerScore(model.TournamentUserID, model.Name));
			}

			foreach (MatchModel mm in _model.Matches)
			{
				// Create the Match:
				IMatch match = new Match(mm);
				Matches.Add(match.MatchNumber, match);
				this.NumberOfRounds = Math.Max(NumberOfRounds, match.RoundIndex);

				// Get the Scores, and update Rankings:
				int defScore = 0, chalScore = 0;
				foreach (IGame game in match.Games)
				{
					defScore += game.Score[(int)PlayerSlot.Defender];
					chalScore += game.Score[(int)PlayerSlot.Challenger];
				}
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

				// Update Defender:
				int playerIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Defender].Id);
				Rankings[playerIndex].AddMatchOutcome
					(defOutcome, match.Score[(int)PlayerSlot.Defender], defScore, true);

				// Update Challenger:
				playerIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Challenger].Id);
				Rankings[playerIndex].AddMatchOutcome
					(chalOutcome, match.Score[(int)PlayerSlot.Challenger], chalScore, true);
			}
			NumberOfMatches = Matches.Count;

			UpdateRankings();
			this.IsFinished = true;
			foreach (IMatch match in Matches.Values)
			{
				if (!match.IsFinished)
				{
					this.IsFinished = false;
					break;
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
					IMatch match = new Match();
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
		#endregion

		#region Private Methods
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, PlayerSlot _formerMatchWinnerSlot, bool _resetManualWin = false)
		{
			IMatch match = GetMatch(_matchNumber);

			if (null == _games)
			{
				// Match winner was manually set. Apply a match win to his score (but no games!):
				int winnerIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)(match.WinnerSlot)].Id);
				Rankings[winnerIndex].AddMatchOutcome(Outcome.Win, 0, 0, true);
			}
			else if (_resetManualWin)
			{
				// Match had a manual winner: being removed.
				// Update score accordingly, ignoring individual game scores:
				int winnerIndex = Rankings.FindIndex
					(r => r.Id == match.Players[(int)_formerMatchWinnerSlot].Id);
				Rankings[winnerIndex].AddMatchOutcome(Outcome.Win, 0, 0, false);
			}
			else
			{
				// Standard case: Update score for new game(s):
				Outcome defOutcome = Outcome.Tie;
				Outcome chalOutcome = Outcome.Tie;
				int defenderGameScore = 0, defenderPointScore = 0;
				int challengerGameScore = 0, challengerPointScore = 0;

				// Calculate players' MatchScore updates:
				if ((PlayerSlot.Defender == _formerMatchWinnerSlot) ^ (PlayerSlot.Defender == match.WinnerSlot))
				{
					defOutcome = Outcome.Win;
					chalOutcome = Outcome.Loss;
				}
				else if ((PlayerSlot.Challenger == _formerMatchWinnerSlot) ^ (PlayerSlot.Challenger == match.WinnerSlot))
				{
					defOutcome = Outcome.Loss;
					chalOutcome = Outcome.Win;
				}

				// Calculate players' GameScore & PointScore updates:
				foreach (GameModel model in _games)
				{
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
					defenderPointScore += model.DefenderScore;
					challengerPointScore += model.ChallengerScore;
				}

				// Update Defender's score:
				int defIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Defender].Id);
				Rankings[defIndex].AddMatchOutcome(defOutcome, defenderGameScore, defenderPointScore, _isAddition);

				// Update Challenger's score:
				int chalIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Challenger].Id);
				Rankings[chalIndex].AddMatchOutcome(chalOutcome, challengerGameScore, challengerPointScore, _isAddition);
			}

			UpdateRankings();
		}
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			IsFinished = true;
			foreach (IMatch match in Matches.Values)
			{
				if (!match.IsFinished)
				{
					IsFinished = false;
					break;
				}
			}

			return (new List<MatchModel>());
		}
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			this.IsFinished = (IsFinished && GetMatch(_matchNumber).IsFinished);

			return (new List<MatchModel>());
		}

		protected override void UpdateRankings()
		{
			// Calculate MatchScore values for each player:
			int[] playerScores = new int[Rankings.Count];
			for (int p = 0; p < Rankings.Count; ++p)
			{
				Rankings[p].OpponentsScore = 0;
				playerScores[p] = Rankings[p].CalculateScore(MatchWinValue, MatchTieValue, 0);
			}
			// Calculate & Assign OpponentsPoints value for each player:
			foreach (IMatch match in Matches.Values.Where(m => m.IsFinished).ToList())
			{
				Rankings
					.Find(p => p.Id == match.Players[(int)PlayerSlot.Challenger].Id)
					.OpponentsScore
					+= playerScores[Rankings
					.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Defender].Id)];
				Rankings
					.Find(p => p.Id == match.Players[(int)PlayerSlot.Defender].Id)
					.OpponentsScore
					+= playerScores[Rankings
					.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Challenger].Id)];
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
