using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class RoundRobinBracket : Bracket
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
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		// inherits int NumberOfMatches
		protected int MatchWinValue
		{ get; set; }
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
			BracketType = BracketTypeModel.BracketType.ROUNDROBIN;
			MaxRounds = _numberOfRounds;
			MatchWinValue = 2;
			ResetBracket();
			CreateBracket(_maxGamesPerMatch);
		}
#if false
		public RoundRobinBracket(int _numPlayers, int _numRounds = 0)
		{
			if (_numPlayers < 0)
			{
				throw new ArgumentOutOfRangeException
					("_numPlayers", "Can't have negative players!");
			}

			BracketType = BracketTypeModel.BracketType.ROUNDROBIN;
			Players = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				Players.Add(new User());
			}

			MaxRounds = _numRounds;
			ResetBracket();
			CreateBracket();
		}
#endif
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
			this.BracketType = BracketTypeModel.BracketType.ROUNDROBIN;
			this.IsFinalized = _model.Finalized;
			this.MaxRounds = 0;
			this.MatchWinValue = 2;
			ResetBracket();

			List<UserModel> userModels = _model.UserSeeds
				.OrderBy(ubs => ubs.Seed)
				.Select(ubs => ubs.User)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (UserModel model in userModels)
			{
				Players.Add(new User(model));
				Rankings.Add(new PlayerScore(model.UserID, model.Username, 0, 1));
			}

			foreach (MatchModel mm in _model.Matches)
			{
				// Create the Match:
				IMatch match = new Match(mm);
				Matches.Add(match.MatchNumber, match);
				++NumberOfMatches;
				if (match.RoundIndex > NumberOfRounds)
				{
					this.NumberOfRounds = match.RoundIndex;
				}

				// Get the Scores, and update Rankings:
				int defScore = 0, chalScore = 0;
				foreach (IGame game in match.Games)
				{
					defScore += game.Score[(int)PlayerSlot.Defender];
					chalScore += game.Score[(int)PlayerSlot.Challenger];
				}

				int defIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Defender].Id);
				Rankings[defIndex].Score += match.Score[(int)PlayerSlot.Defender];
				Rankings[defIndex].AddToScore(
					(PlayerSlot.Defender == match.WinnerSlot) ? MatchWinValue : 0
					, match.Score[(int)PlayerSlot.Defender], defScore, true);

				int chalIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)PlayerSlot.Challenger].Id);
				Rankings[chalIndex].Score += match.Score[(int)PlayerSlot.Challenger];
				Rankings[chalIndex].AddToScore(
					(PlayerSlot.Challenger == match.WinnerSlot) ? MatchWinValue : 0
					, match.Score[(int)PlayerSlot.Challenger], chalScore, true);
			}

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
			ResetBracket();
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
				Rankings.Add(new PlayerScore(player.Id, player.Name, 0, 1));
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

#if false
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			GameModel gameModel = GetMatch(_matchNumber).AddGame(_defenderScore, _challengerScore, _winnerSlot);
			if (_defenderScore == _challengerScore)
			{
				throw new NotImplementedException
					("Tie games are not (yet) supported!");
			}
			AddWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
#endif
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}

			GameModel gameModel = Matches[_matchNumber].AddGame(_defenderScore, _challengerScore);
			if (_defenderScore == _challengerScore)
			{
				throw new NotImplementedException
					("Tie games are not (yet) supported!");
			}
			PlayerSlot gameWinnerSlot = (_defenderScore > _challengerScore)
				? PlayerSlot.Defender : PlayerSlot.Challenger;
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (Rankings[i].Id == Matches[_matchNumber].Players[(int)gameWinnerSlot].Id)
				{
					Rankings[i].Score += 1;
					break;
				}
			}
			UpdateRankings();

			IsFinished = true;
			foreach (IMatch match in Matches.Values)
			{
				if (!match.IsFinished)
				{
					IsFinished = false;
					break;
				}
			}

			return gameModel;
		}
#if false
		public override GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			IMatch match = GetMatch(_matchNumber);
			bool gameFound = false;
			foreach (IGame game in match.Games)
			{
				if (game.GameNumber == _gameNumber)
				{
					gameFound = true;
					for (int i = 0; i < Rankings.Count; ++i)
					{
						if (Rankings[i].Id == match.Players[(int)(game.WinnerSlot)].Id)
						{
							Rankings[i].Score -= 1;
							break;
						}
					}
					break;
				}
			}
			if (!gameFound)
			{
				throw new GameNotFoundException
					("Game not found; Game Number may be invalid!");
			}

			GameModel gameModel = GetMatch(_matchNumber).UpdateGame(_gameNumber, _defenderScore, _challengerScore, _winnerSlot);
			ApplyWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
		public override void RemoveLastGame(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}

			IGame removedGame = Matches[_matchNumber].RemoveLastGame();
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (Rankings[i].Id == removedGame.PlayerIDs[(int)(removedGame.WinnerSlot)])
				{
					Rankings[i].Score = Rankings[i].Score - 1;
					break;
				}
			}
			UpdateRankings();

			IsFinished = IsFinished && Matches[_matchNumber].IsFinished;
		}
		public override void ResetMatchScore(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}

			int defScore = Matches[_matchNumber].Score[(int)PlayerSlot.Defender];
			int chalScore = Matches[_matchNumber].Score[(int)PlayerSlot.Challenger];

			Matches[_matchNumber].ResetScore();
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (Rankings[i].Id == Matches[_matchNumber].Players[(int)PlayerSlot.Defender].Id)
				{
					Rankings[i].Score = Rankings[i].Score - defScore;
				}
				else if (Rankings[i].Id == Matches[_matchNumber].Players[(int)PlayerSlot.Challenger].Id)
				{
					Rankings[i].Score = Rankings[i].Score - chalScore;
				}
			}
			UpdateRankings();

			IsFinished = false;
		}
#endif

		public override void ResetMatches()
		{
			base.ResetMatches();
			foreach (IPlayerScore ps in Rankings)
			{
				ps.Rank = 1;
				ps.Score = 0;
			}
		}
		#endregion

		#region Private Methods
		protected override void UpdateScore(int _matchNumber, GameModel _game, bool _isAddition, bool _wasFinished)
		{
			if (null == _game)
			{
				// Match winner was manually set. Apply a match win to his score:
				IMatch match = GetMatch(_matchNumber);
				int winnerIndex = Rankings.FindIndex(r => r.Id == match.Players[(int)(match.WinnerSlot)].Id);
				Rankings[winnerIndex].AddToScore(MatchWinValue, 0, 0, true);
			}
			else
			{
				//PlayerSlot matchWinner = GetMatch(_matchNumber).WinnerSlot;
				PlayerSlot gameWinner = (_game.DefenderID == _game.WinnerID)
					? PlayerSlot.Defender : PlayerSlot.Challenger;
				bool matchFinishChange = _wasFinished ^ GetMatch(_matchNumber).IsFinished;

				// Update Defender's score:
				int defIndex = Rankings.FindIndex(r => r.Id == _game.DefenderID);
				bool defenderUpdate = matchFinishChange && (PlayerSlot.Defender == gameWinner);
				Rankings[defIndex].Score += (_isAddition)
					? Convert.ToInt16(defenderUpdate) * MatchWinValue
					: -1 * Convert.ToInt16(defenderUpdate) * MatchWinValue;
				Rankings[defIndex].AddToScore(Convert.ToInt16(defenderUpdate) * MatchWinValue, 1, _game.DefenderScore, _isAddition);

				// Update Challenger's score:
				int chalIndex = Rankings.FindIndex(r => r.Id == _game.ChallengerID);
				bool challengerUpdate = matchFinishChange && (PlayerSlot.Challenger == gameWinner);
				Rankings[chalIndex].Score += (_isAddition)
					? Convert.ToInt16(challengerUpdate) * MatchWinValue
					: -1 * Convert.ToInt16(challengerUpdate) * MatchWinValue;
				Rankings[chalIndex].AddToScore(Convert.ToInt16(challengerUpdate) * MatchWinValue, 1, _game.ChallengerScore, _isAddition);
			}

			UpdateRankings();
		}
		protected override void ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
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
		}
		protected override void ApplyGameRemovalEffects(int _matchNumber, GameModel _game, bool _wasFinished)
		{
			this.IsFinished = (IsFinished && GetMatch(_matchNumber).IsFinished);
		}

		protected override void UpdateRankings()
		{
			Rankings.Sort(SortRankingScores);
#if false
			Rankings.Sort((first, second) =>
			{
				// Rankings sorting: MatchScore > GameScore > PointsScore > initial Seeding
				int compare = -1 * (first.MatchScore.CompareTo(second.MatchScore));
				compare = (compare != 0)
					? compare : -1 * (first.GameScore.CompareTo(second.GameScore));
				compare = (compare != 0)
					? compare : -1 * (first.PointsScore.CompareTo(second.PointsScore));
				return (compare != 0)
					? compare : GetPlayerSeed(first.Id).CompareTo(GetPlayerSeed(second.Id));
			});
#endif
			for (int i = 0; i < Rankings.Count; ++i)
			{
				Rankings[i].Rank = i + 1;
			}
		}
#endregion
	}
}
