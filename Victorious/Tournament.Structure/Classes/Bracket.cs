﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public abstract class Bracket : IBracket
	{
		#region Variables & Properties
		public int Id
		{ get; protected set; }
		public BracketType BracketType
		{ get; protected set; }
		public bool IsFinalized
		{ get; protected set; }
		public bool IsFinished
		{ get; protected set; }
		public List<IPlayer> Players
		{ get; protected set; }
		public List<IPlayerScore> Rankings
		{ get; protected set; }
		public int MaxRounds
		{ get; set; }
		protected Dictionary<int, Match> Matches
		{ get; set; }
		public int NumberOfRounds
		{ get; protected set; }
		protected Dictionary<int, Match> LowerMatches
		{ get; set; }
		public int NumberOfLowerRounds
		{ get; protected set; }
		protected Match grandFinal
		{ get; set; }
		public IMatch GrandFinal
		{ get { return (grandFinal as IMatch); } }
		public int NumberOfMatches
		{ get; protected set; }
		protected int MatchWinValue
		{ get; set; }
		protected int MatchTieValue
		{ get; set; }
		#endregion

		#region Events
		public event EventHandler<BracketEventArgs> RoundAdded;
		public event EventHandler<BracketEventArgs> RoundDeleted;
		public event EventHandler<BracketEventArgs> MatchesModified;
		public event EventHandler<BracketEventArgs> GamesDeleted;
		protected void OnRoundAdded(BracketEventArgs _e)
		{
			RoundAdded?.Invoke(this, _e);
		}
		protected void OnRoundDeleted(BracketEventArgs _e)
		{
			RoundDeleted?.Invoke(this, _e);
		}
		protected void OnRoundDeleted(List<MatchModel> _modelList)
		{
			if ((_modelList?.Count ?? 0) > 0)
			{
				OnRoundDeleted(new BracketEventArgs(_modelList));
			}
		}
		protected void OnMatchesModified(BracketEventArgs _e)
		{
			MatchesModified?.Invoke(this, _e);
		}
		protected void OnMatchesModified(List<MatchModel> _modelList)
		{
			if ((_modelList?.Count ?? 0) > 0)
			{
				OnMatchesModified(new BracketEventArgs(_modelList));
			}
		}
		protected void OnGamesDeleted(BracketEventArgs _e)
		{
			GamesDeleted?.Invoke(this, _e);
		}
		protected void OnGamesDeleted(List<IGame> _games)
		{
			if ((_games?.Count ?? 0) > 0)
			{
				OnGamesDeleted(new BracketEventArgs(_games
					.Select(g => g.Id).ToList()));
			}
		}
		protected void OnGamesDeleted(List<int> _gameIDs)
		{
			if ((_gameIDs?.Count ?? 0) > 0)
			{
				OnGamesDeleted(new BracketEventArgs(_gameIDs));
			}
		}
		#endregion

		#region Abstract Methods
		public abstract void CreateBracket(int _gamesPerMatch = 1);

		public abstract bool CheckForTies();
		public abstract bool GenerateTiebreakers();

		protected abstract List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot);
		protected abstract List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot);
		protected abstract void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch);
		protected abstract void RecalculateRankings();
		protected abstract void UpdateRankings();
		#endregion

		#region Public Methods
		public virtual void ResetMatches()
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();
			List<int> deletedGameIDs = new List<int>();

			for (int n = 1; n <= NumberOfMatches; ++n)
			{
				Match match = GetInternalMatch(n);
				bool affected = false;

				if (match.IsManualWin || match.Games.Count > 0)
				{
					// Populate the list for GamesDeleted event:
					affected = true;
					deletedGameIDs.AddRange(match.Games.Select(g => g.Id));
				}
				for (int i = 0; i < 2; ++i)
				{
					// Remove Players (but only if they advanced into this match):
					if (match.PreviousMatchNumbers[i] > -1 &&
						null != match.Players[i])
					{
						affected = true;
						match.RemovePlayer(match.Players[i].Id);
					}
				}

				match.ResetScore();
				if (affected)
				{
					// Populate the list for MatchesModified event:
					alteredMatches.Add(GetMatchModel(match));
				}
			}
			IsFinished = false;
			IsFinalized = false;

			OnGamesDeleted(deletedGameIDs);
			OnMatchesModified(alteredMatches);
		}

		#region Player Methods
		public int NumberOfPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			return Players.Count;
		}
		public int GetPlayerSeed(int _playerId)
		{
			if (null == Players)
			{
				throw new NullReferenceException
					("Players is null. This shouldn't happen...");
			}

			for (int i = 0; i < Players.Count; ++i)
			{
				if (_playerId == Players[i].Id)
				{
					return (i + 1);
				}
			}

			throw new PlayerNotFoundException
				("Player not found in this Bracket!");
		}

		public void RandomizeSeeds()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			if (Players.Count < 2)
			{
				DeleteBracketData();
				return;
			}

			List<IPlayer> pList = new List<IPlayer>();

			// Get random rolls for each player
			// (match rolls -> player-index)
			Random rng = new Random();
			Dictionary<int, int> rolls = new Dictionary<int, int>();
			for (int i = 0; i < Players.Count; ++i)
			{
				int rand = -1;
				while (rand < 0 || rolls.ContainsKey(rand))
				{
					rand = rng.Next(Players.Count * 3);
				}
				rolls.Add(rand, i);
			}

			// Sort rolls in list, then add Players to new list in order
			List<int> rollsList = rolls.Keys
				.OrderByDescending(v => v)
				.ToList();
			foreach (int key in rollsList)
			{
				pList.Add(Players[rolls[key]]);
			}

			Players = pList;
			DeleteBracketData();
		}

		public void SetNewPlayerlist(List<IPlayer> _players)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			DeleteBracketData();
		}
		public void SetNewPlayerlist(ICollection<TournamentUsersBracketModel> _players)
		{
			List<TournamentUserModel> userModels = _players
				.OrderBy(p => p.Seed, new SeedComparer())
				.Select(p => p.TournamentUser)
				.ToList();

			List<IPlayer> playerList = new List<IPlayer>();
			foreach (TournamentUserModel model in userModels)
			{
				playerList.Add(new Player(model));
			}
			SetNewPlayerlist(playerList);
		}

		public void AddPlayer(IPlayer _player)
		{
			if (null == _player)
			{
				throw new ArgumentNullException("_player");
			}
			if (null == Players)
			{
				throw new NullReferenceException
					("Players is null. This shouldn't happen...");
			}
			if (Players.Contains(_player))
			{
				throw new DuplicateObjectException
					("Bracket already contains this Player!");
			}

			Players.Add(_player);
			DeleteBracketData();
		}
		public virtual void ReplacePlayer(IPlayer _player, int _index)
		{
			if (null == _player)
			{
				throw new ArgumentNullException("_player");
			}
			if (_index < 0 || _index >= Players.Count)
			{
				throw new InvalidIndexException
					("Invalid index; outside Playerlist bounds.");
			}

			List<MatchModel> alteredMatches = new List<MatchModel>();

			if (null != Players[_index])
			{
				// Replace existing Player in any Matches
				for (int n = 1; n <= NumberOfMatches; ++n)
				{
					try
					{
						Match match = GetInternalMatch(n);
						match.ReplacePlayer(_player, Players[_index].Id);
						alteredMatches.Add(GetMatchModel(match));
					}
					catch (PlayerNotFoundException)
					{ }
				}
				// Replace existing Player in the Rankings
				for (int i = 0; i < Rankings.Count(); ++i)
				{
					if (Rankings[i].Id == Players[_index].Id)
					{
						Rankings[i].ReplacePlayerData(_player.Id, _player.Name);
						break;
					}
				}
			}

			Players[_index] = _player;
			OnMatchesModified(alteredMatches);
		}
		public void RemovePlayer(int _playerId)
		{
			if (null == Players)
			{
				throw new NullReferenceException
					("Playerlist is null. This shouldn't happen...");
			}

			for (int i = 0; i < Players.Count; ++i)
			{
				if (Players[i].Id == _playerId)
				{
					Players.RemoveAt(i);
					DeleteBracketData();
					return;
				}
			}
			throw new PlayerNotFoundException
				("Player not found in this Bracket!");
		}

		public void SwapPlayers(int _index1, int _index2)
		{
			if (_index1 < 0 || _index1 >= Players.Count
				|| _index2 < 0 || _index2 >= Players.Count)
			{
				throw new InvalidIndexException
					("Invalid index; outside Playerlist bounds.");
			}

			IPlayer tmp = Players[_index1];
			Players[_index1] = Players[_index2];
			Players[_index2] = tmp;

			DeleteBracketData();
		}
		public void ReinsertPlayer(int _oldIndex, int _newIndex)
		{
			if (_oldIndex < 0 || _oldIndex >= Players.Count
				|| _newIndex < 0 || _newIndex >= Players.Count)
			{
				throw new InvalidIndexException
					("Invalid index; outside Playerlist bounds.");
			}
			if (_oldIndex == _newIndex)
			{
				return;
			}

			IPlayer tmp = Players[_oldIndex];
			if (_oldIndex > _newIndex)
			{
				for (int i = _oldIndex; i > _newIndex; --i)
				{
					Players[i] = Players[i - 1];
				}
			}
			else // _oldIndex < _newIndex
			{
				for (int i = _oldIndex; i < _newIndex; ++i)
				{
					Players[i] = Players[i + 1];
				}
			}
			Players[_newIndex] = tmp;

			DeleteBracketData();
		}

		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			Players.Clear();
			DeleteBracketData();
		}
		#endregion

		#region Match & Game Methods
		public virtual GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			Match match = GetInternalMatch(_matchNumber);
			MatchModel oldModel = GetMatchModel(match);

			// Add the new Game and update Bracket & Rankings:
			GameModel gameModel = match
				.AddGame(_defenderScore, _challengerScore, _winnerSlot);
			UpdateScore(_matchNumber, new List<GameModel>() { gameModel }, true, oldModel);
			List<MatchModel> alteredMatches = ApplyWinEffects(_matchNumber, _winnerSlot);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
			// Return a Model of the new Game:
			return gameModel;
		}
		public virtual GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			Match match = GetInternalMatch(_matchNumber);
			int gameIndex = match.Games.FindIndex(g => g.GameNumber == _gameNumber);
			if (gameIndex < 0)
			{
				// Case 1: Game doesn't exist:
				throw new GameNotFoundException
					("Game not found; Game Number may be invalid!");
			}

			MatchModel oldMatchModel = GetMatchModel(match);
			List<MatchModel> alteredMatches = new List<MatchModel>();
			List<GameModel> alteredGames = new List<GameModel>();

			if (match.Games[gameIndex].WinnerSlot == _winnerSlot)
			{
				// Case 2: Game winner won't change.

				// Subtract old scores from rankings:
				GameModel oldGame = match.Games[gameIndex].GetModel();
				UpdateScore(_matchNumber, new List<GameModel> { oldGame }, false, oldMatchModel);

				// Update the Game (and Match):
				match.Games[gameIndex].Score[(int)PlayerSlot.Defender] = _defenderScore;
				match.Games[gameIndex].Score[(int)PlayerSlot.Challenger] = _challengerScore;
				alteredGames.Add(match.Games[gameIndex].GetModel());
				alteredGames[0].MatchID = match.Id;

				// Add new scores to rankings:
				UpdateScore(_matchNumber, alteredGames, true, oldMatchModel);
			}
			else
			{
				// Case 3: Game winner changes.

				// Remove (and save) the current Game:
				GameModel removedGame = RemoveGameNumber(_matchNumber, _gameNumber);
				UpdateScore(_matchNumber, new List<GameModel> { removedGame }, false, oldMatchModel);
				oldMatchModel = GetMatchModel(match);

				// Update the Game's values:
				removedGame.DefenderScore = _defenderScore;
				removedGame.ChallengerScore = _challengerScore;
				removedGame.WinnerID = (PlayerSlot.Defender == _winnerSlot)
					? removedGame.DefenderID : -1;
				removedGame.WinnerID = (PlayerSlot.Challenger == _winnerSlot)
					? removedGame.ChallengerID : removedGame.WinnerID;

				// Add the updated Game back to the Match:
				alteredGames.Add(match.AddGame(removedGame));
				// Update the Bracket (Scores and progression):
				UpdateScore(_matchNumber, alteredGames, true, oldMatchModel);
				alteredMatches.AddRange(ApplyWinEffects(_matchNumber, _winnerSlot));
			}

			// Fire Event with any changed Matches:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
			// Return a Model of the updated Game:
			return alteredGames[0];
		}

		public virtual GameModel RemoveLastGame(int _matchNumber)
		{
			IGame lastGame = GetInternalMatch(_matchNumber).Games.LastOrDefault();
			if (null == lastGame)
			{
				throw new GameNotFoundException
					("No Games to remove!");
			}
			return (RemoveGameNumber(_matchNumber, lastGame.GameNumber));
		}
		public virtual GameModel RemoveGameNumber(int _matchNumber, int _gameNumber)
		{
			Match match = GetInternalMatch(_matchNumber);
			MatchModel oldMatchModel = GetMatchModel(match);
			PlayerSlot winnerSlot = match.WinnerSlot;
			List<GameModel> modelList = new List<GameModel>();

			// Remove the Game and update the Bracket & Rankings:
			modelList.Add(match.RemoveGameNumber(_gameNumber));
			List<MatchModel> alteredMatches = ApplyGameRemovalEffects(_matchNumber, modelList, winnerSlot);
			UpdateScore(_matchNumber, modelList, false, oldMatchModel);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(new BracketEventArgs
				(alteredMatches, modelList.Select(g => g.GameID).ToList()));
			// Return a Model of the removed Game:
			return modelList[0];
		}

		public virtual void SetMatchWinner(int _matchNumber, PlayerSlot _winnerSlot)
		{
			Match match = GetInternalMatch(_matchNumber);
			MatchModel oldMatchModel = GetMatchModel(match);
			bool winnerChange = (_winnerSlot != match.WinnerSlot);
			List<GameModel> modelList = new List<GameModel>();

			if (PlayerSlot.unspecified != match.WinnerSlot ||
				match.Games.Count > 0)
			{
				// Save the games:
				if (winnerChange)
				{
					// Reset the Match AND other affected Matches:
					modelList = ResetMatchScore(_matchNumber);
					//RecalculateRankings();
				}
				else
				{
					// Reset just the one Match:
					modelList = match.ResetScore();
					UpdateScore(_matchNumber, modelList, false, oldMatchModel);
				}
			}

			// Set the match winner, THEN re-add the games:
			oldMatchModel = GetMatchModel(match);
			match.SetWinner(_winnerSlot);
			foreach (GameModel model in modelList)
			{
				match.AddGame(model);
			}
			// Update the Bracket & Rankings:
			UpdateScore(_matchNumber, null, true, oldMatchModel);
			List<MatchModel> alteredMatches = ApplyWinEffects(_matchNumber, _winnerSlot);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
		}

		public virtual List<GameModel> ResetMatchScore(int _matchNumber)
		{
			Match match = GetInternalMatch(_matchNumber);
			PlayerSlot winnerSlot = match.WinnerSlot;
			MatchModel oldMatchModel = GetMatchModel(match);

			// Reset the Match's score, remove any Games, update Bracket & Rankings:
			List<GameModel> modelList = match.ResetScore();
			List<MatchModel> alteredMatches = ApplyGameRemovalEffects(_matchNumber, modelList, winnerSlot);
			UpdateScore(_matchNumber, modelList, false, oldMatchModel);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(new BracketEventArgs
				(alteredMatches, modelList.Select(g => g.GameID).ToList()));
			// Return Models of any removed Games:
			return modelList;
		}
		#endregion

		#region Accessors
		public virtual BracketModel GetModel(int _tournamentID = 0)
		{
			BracketModel model = new BracketModel();
			model.TournamentID = _tournamentID;
			model.BracketID = this.Id;
			model.BracketTypeID = Convert.ToInt32(this.BracketType);
			model.Finalized = this.IsFinalized;
			model.NumberOfGroups = 0;
			model.MaxRounds = this.MaxRounds;

			model.BracketType = new BracketTypeModel();
			model.BracketType.BracketTypeID = model.BracketTypeID;
			model.BracketType.Type = this.BracketType;
			model.BracketType.TypeName = this.BracketType.ToString("f");

			//model.TournamentUsersBrackets = new List<TournamentUsersBracketModel>();
			foreach (IPlayer player in Players)
			{
				TournamentUsersBracketModel m = player.GetTournamentUsersBracketModel(this.Id, GetPlayerSeed(player.Id));
				model.TournamentUsersBrackets.Add(m);
			}

			//model.Matches = new List<MatchModel>();
			if (!(this is IGroupStage))
			{
				for (int n = 1; n <= NumberOfMatches; ++n)
				{
					model.Matches.Add(GetMatchModel(n));
				}
			}

			return model;
		}

		public virtual List<IMatch> GetRound(int _round)
		{
			if (null == Matches)
			{
				throw new NullReferenceException
					("Matches doesn't exist! Create a bracket first.");
			}
			if (_round < 1)
			{
				throw new InvalidIndexException
					("Round index cannot be less than 1!");
			}

			List<IMatch> ret = Matches.Values
				.Where(m => m.RoundIndex == _round)
				.OrderBy(m => m.MatchIndex)
				.Cast<IMatch>()
				.ToList();
			return ret;
		}
		public List<IMatch> GetLowerRound(int _round)
		{
			if (null == LowerMatches)
			{
				throw new NullReferenceException
					("Lower Round doesn't exist!");
			}
			if (_round < 1)
			{
				throw new InvalidIndexException
					("Round index cannot be less than 1!");
			}

			List<IMatch> ret = LowerMatches.Values
				.Where(m => m.RoundIndex == _round)
				.OrderBy(m => m.MatchIndex)
				.Cast<IMatch>()
				.ToList();
			return ret;
		}
		public virtual IMatch GetMatch(int _matchNumber)
		{
			return (GetInternalMatch(_matchNumber) as IMatch);
		}
		public MatchModel GetMatchModel(int _matchNumber)
		{
			MatchModel model = GetInternalMatch(_matchNumber).GetModel();
			model.BracketID = this.Id;
			return model;
		}
		#endregion
		#region Mutators
		public virtual void SetMaxGamesForWholeRound(int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}

			List<IMatch> round = null;
			if (null != grandFinal && _round == 1 + NumberOfRounds)
			{
				round = new List<IMatch>() { grandFinal };
			}
			else
			{
				round = GetRound(_round);
			}

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
		public virtual void SetMaxGamesForWholeLowerRound(int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}

			List<IMatch> round = GetLowerRound(_round);
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
		protected virtual void ResetBracketData()
		{
			if (null == Matches)
			{
				Matches = new Dictionary<int, Match>();
			}
			if (null == LowerMatches)
			{
				LowerMatches = new Dictionary<int, Match>();
			}
			if (null == Rankings)
			{
				Rankings = new List<IPlayerScore>();
			}

			IsFinished = false;
			Matches.Clear();
			LowerMatches.Clear();
			grandFinal = null;
			NumberOfRounds = NumberOfLowerRounds = 0;
			NumberOfMatches = 0;
			Rankings.Clear();
		}
		protected void DeleteBracketData()
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();
			List<int> deletedGameIDs = new List<int>();
			if (Matches?.Count > 0)
			{
				for (int n = 1; n <= NumberOfMatches; ++n)
				{
					MatchModel match = GetMatchModel(n);
					alteredMatches.Add(match);
					deletedGameIDs.AddRange(match.Games.Select(g => g.GameID));
				}
			}

			ResetBracketData();

			OnGamesDeleted(deletedGameIDs);
			OnMatchesModified(alteredMatches);
		}

		protected virtual Match GetInternalMatch(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}

			if (grandFinal?.MatchNumber == _matchNumber)
			{
				return grandFinal;
			}
			if (Matches?.ContainsKey(_matchNumber) ?? false)
			{
				return Matches[_matchNumber];
			}
			if (LowerMatches?.ContainsKey(_matchNumber) ?? false)
			{
				return LowerMatches[_matchNumber];
			}

			throw new MatchNotFoundException
				("Match not found; match number may be invalid.");
		}
		protected MatchModel GetMatchModel(IMatch _match)
		{
			MatchModel model = (_match as Match).GetModel();
			model.BracketID = this.Id;
			return model;
		}

		protected int SortRankingScores(IPlayerScore first, IPlayerScore second)
		{
			// Rankings sorting: MatchScore > OpponentsScore > GameScore > PointsScore > initial Seeding
			int compare = -1 * (first.CalculateScore(MatchWinValue, MatchTieValue, 0)
				.CompareTo(second.CalculateScore(MatchWinValue, MatchTieValue, 0)));
			compare = (compare != 0)
				? compare : -1 * (first.OpponentsScore.CompareTo(second.OpponentsScore));
			compare = (compare != 0)
				? compare : -1 * (first.GameScore.CompareTo(second.GameScore));
			compare = (compare != 0)
				? compare : -1 * (first.PointsScore.CompareTo(second.PointsScore));
			return (compare != 0)
				? compare : GetPlayerSeed(first.Id).CompareTo(GetPlayerSeed(second.Id));
		}
		protected int SortRankingRanks(IPlayerScore first, IPlayerScore second)
		{
			int compare = first.Rank.CompareTo(second.Rank);
			return (compare != 0)
				? compare
				: GetPlayerSeed(first.Id).CompareTo(GetPlayerSeed(second.Id));
		}
		#endregion
	}
}
