using System;
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
		protected Dictionary<int, IMatch> Matches
		{ get; set; }
		public int NumberOfRounds
		{ get; protected set; }
		protected Dictionary<int, IMatch> LowerMatches
		{ get; set; }
		public int NumberOfLowerRounds
		{ get; protected set; }
		public IMatch GrandFinal
		{ get; protected set; }
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
		protected void OnMatchesModified(BracketEventArgs _e)
		{
			MatchesModified?.Invoke(this, _e);
		}
		protected void OnMatchesModified(List<MatchModel> _modelList)
		{
			if (null != _modelList && _modelList.Count > 0)
			{
				OnMatchesModified(new BracketEventArgs(_modelList
					.OrderBy(m => m.MatchNumber).ToList()));
			}
		}
		protected void OnGamesDeleted(BracketEventArgs _e)
		{
			GamesDeleted?.Invoke(this, _e);
		}
		protected void OnGamesDeleted(List<IGame> _games)
		{
			if (null != _games && _games.Count > 0)
			{
				OnGamesDeleted(new BracketEventArgs(_games.Select(g => g.Id).ToList()));
			}
		}
		protected void OnGamesDeleted(List<int> _gameIDs)
		{
			if (null != _gameIDs && _gameIDs.Count > 0)
			{
				OnGamesDeleted(new BracketEventArgs(_gameIDs));
			}
		}
		#endregion

		#region Abstract Methods
		public abstract void CreateBracket(int _gamesPerMatch = 1);
		protected abstract void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, PlayerSlot _formerMatchWinnerSlot, bool _resetManualWin = false);
		protected abstract List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot);
		protected abstract List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot);
		protected abstract void UpdateRankings();
		#endregion

		#region Public Methods
		public virtual void RestoreMatch(int _matchNumber, MatchModel _model)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			IMatch match = GetMatch(_matchNumber);
			match = new Match(_model);
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
			if (null == Players || Players.Count < 2)
			{
				if (null == Players)
				{
					Players = new List<IPlayer>();
				}
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
				if ((Players[rolls[key]] is User))
				{
					pList.Add(new User(Players[rolls[key]] as User));
				}
				else if ((Players[rolls[key]] is Team))
				{
					pList.Add(new Team(Players[rolls[key]] as Team));
				}
				else
				{
					pList.Add(Players[rolls[key]]);
				}
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
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}

			Players.Clear();
			foreach (IPlayer player in _players)
			{
				if (player is User)
				{
					Players.Add(new User(player as User));
				}
				else if (player is Team)
				{
					Players.Add(new Team(player as Team));
				}
				else
				{
					Players.Add(player);
				}
			}
			DeleteBracketData();
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

			if (_player is User)
			{
				Players.Add(new User(_player as User));
			}
			else if (_player is Team)
			{
				Players.Add(new Team(_player as Team));
			}
			else
			{
				Players.Add(_player);
			}
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
						IMatch match = GetMatch(n);
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
		public void SwapPlayers(int _index1, int _index2)
		{
			if (_index1 < 0 || _index1 >= Players.Count
				|| _index2 < 0 || _index2 >= Players.Count)
			{
				throw new InvalidIndexException
					("Invalid index; outside Playerlist bounds.");
			}

			if (Players[_index1] is User)
			{
				User tmp = new User(Players[_index1] as User);
				Players[_index1] = new User(Players[_index2] as User);
				Players[_index2] = tmp;
			}
			else if (Players[_index1] is Team)
			{
				Team tmp = new Team(Players[_index1] as Team);
				Players[_index1] = new Team(Players[_index2] as Team);
				Players[_index2] = tmp;
			}
			else
			{
				IPlayer tmp = Players[_index1];
				Players[_index1] = Players[_index2];
				Players[_index2] = tmp;
			}
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

			if (Players[0] is User)
			{
				User tmp = new User(Players[_oldIndex] as User);
				if (_oldIndex > _newIndex)
				{
					for (int i = _oldIndex; i > _newIndex; --i)
					{
						Players[i] = new User(Players[i - 1] as User);
					}
				}
				else // _oldIndex < _newIndex
				{
					for (int i = _oldIndex; i < _newIndex; ++i)
					{
						Players[i] = new User(Players[i + 1] as User);
					}
				}
				Players[_newIndex] = tmp;
			}
			else if (Players[0] is Team)
			{
				Team tmp = new Team(Players[_oldIndex] as Team);
				if (_oldIndex > _newIndex)
				{
					for (int i = _oldIndex; i > _newIndex; --i)
					{
						Players[i] = new Team(Players[i - 1] as Team);
					}
				}
				else // _oldIndex < _newIndex
				{
					for (int i = _oldIndex; i < _newIndex; ++i)
					{
						Players[i] = new Team(Players[i + 1] as Team);
					}
				}
				Players[_newIndex] = tmp;
			}
			else
			{
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
			}

			DeleteBracketData();
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
			IMatch match = GetMatch(_matchNumber);
			PlayerSlot oldWinnerSlot = match.WinnerSlot;

			// Add the new Game and update Bracket & Rankings:
			GameModel gameModel = match
				.AddGame(_defenderScore, _challengerScore, _winnerSlot);
			UpdateScore(_matchNumber, new List<GameModel>() { gameModel }, true, oldWinnerSlot);
			List<MatchModel> alteredMatches = ApplyWinEffects(_matchNumber, _winnerSlot);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
			// Return a Model of the new Game:
			return gameModel;
		}
		public virtual GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			IMatch match = GetMatch(_matchNumber);
			int gameIndex = match.Games.FindIndex(g => g.GameNumber == _gameNumber);
			if (gameIndex < 0)
			{
				// Case 1: Game doesn't exist:
				throw new GameNotFoundException
					("Game not found; Game Number may be invalid!");
			}
			if (match.Games[gameIndex].WinnerSlot == _winnerSlot)
			{
				// Case 2: Game winner won't change.
				// Just modify the game's score:
				match.Games[gameIndex].Score[(int)PlayerSlot.Defender] = _defenderScore;
				match.Games[gameIndex].Score[(int)PlayerSlot.Challenger] = _challengerScore;

				////////////////////////////////
				// NOTE : DOES NOT UPDATE RANKINGS!
				////////////////////////////////

				// Fire Event with the changed Match data:
				OnMatchesModified(new List<MatchModel> { GetMatchModel(match) });
				// Return a Model of the altered Game:
				GameModel gameModel = match.Games[gameIndex].GetModel();
				gameModel.MatchID = match.Id;
				return gameModel;
			}
			else
			{
				// Case 3: Game winner changes:
				throw new NotImplementedException
					("Can't update this Game with new values! Try removing and adding instead.");
			}
#if false
			PlayerSlot matchWinnerSlot = GetMatch(_matchNumber).WinnerSlot;
			List<GameModel> modelList = new List<GameModel>();

			modelList.Add(GetMatch(_matchNumber).RemoveGameNumber(_gameNumber));
			List<MatchModel> clearedMatches = ApplyGameRemovalEffects(_matchNumber, modelList, matchWinnerSlot);
			UpdateScore(_matchNumber, modelList, false, matchWinnerSlot);

			GameModel addedGame = GetMatch(_matchNumber)
				.AddGame(_defenderScore, _challengerScore, _winnerSlot);
			UpdateScore(_matchNumber, new List<GameModel>() { addedGame }, true, _winnerSlot);
			List<MatchModel> alteredMatches = ApplyWinEffects(_matchNumber, _winnerSlot);

			foreach (MatchModel model in clearedMatches)
			{
				if (!alteredMatches.Any(m => m.MatchID == model.MatchID))
				{
					alteredMatches.Add(model);
				}
			}
			OnMatchesModified(alteredMatches);
			return addedGame;
#endif
		}
		public virtual GameModel RemoveLastGame(int _matchNumber)
		{
			IGame lastGame = GetMatch(_matchNumber).Games.LastOrDefault();
			if (null == lastGame)
			{
				throw new GameNotFoundException
					("No Games to remove!");
			}
			return (RemoveGameNumber(_matchNumber, lastGame.GameNumber));
#if false
			IMatch match = GetMatch(_matchNumber);
			PlayerSlot oldWinnerSlot = match.WinnerSlot;
			List<GameModel> modelList = new List<GameModel>();

			// Remove the Game and update the Bracket & Rankings:
			modelList.Add(match.RemoveLastGame());
			List<MatchModel> alteredMatches = ApplyGameRemovalEffects(_matchNumber, modelList, oldWinnerSlot);
			UpdateScore(_matchNumber, modelList, false, oldWinnerSlot);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
			// Return a Model of the removed Game:
			return modelList[0];
#endif
		}
		public virtual GameModel RemoveGameNumber(int _matchNumber, int _gameNumber)
		{
			IMatch match = GetMatch(_matchNumber);
			PlayerSlot winnerSlot = match.WinnerSlot;
			List<GameModel> modelList = new List<GameModel>();

			// Remove the Game and update the Bracket & Rankings:
			modelList.Add(match.RemoveGameNumber(_gameNumber));
			List<MatchModel> alteredMatches = ApplyGameRemovalEffects(_matchNumber, modelList, winnerSlot);
			UpdateScore(_matchNumber, modelList, false, winnerSlot);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
			// Return a Model of the removed Game:
			return modelList[0];
		}

		public virtual void SetMatchWinner(int _matchNumber, PlayerSlot _winnerSlot)
		{
			IMatch match = GetMatch(_matchNumber);
			bool winnerChange = (_winnerSlot != match.WinnerSlot);
			List<GameModel> modelList = new List<GameModel>();

			// Reset the match, and save the games:
			if (winnerChange)
			{
				modelList = ResetMatchScore(_matchNumber);
			}
			else
			{
				modelList = match.ResetScore();
			}

			// Set the match winner, THEN re-add the games:
			match.SetWinner(_winnerSlot);
			foreach (GameModel model in modelList)
			{
				PlayerSlot winSlot = (model.DefenderID == model.WinnerID)
					? PlayerSlot.Defender : PlayerSlot.unspecified;
				winSlot = (model.ChallengerID == model.WinnerID)
					? PlayerSlot.Challenger : winSlot;

				match.AddGame(model.DefenderScore, model.ChallengerScore, winSlot);
			}
			// Update the Bracket & Rankings:
			UpdateScore(_matchNumber, null, true, PlayerSlot.unspecified);
			List<MatchModel> alteredMatches = ApplyWinEffects(_matchNumber, _winnerSlot);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
		}
		public virtual List<GameModel> ResetMatchScore(int _matchNumber)
		{
			IMatch match = GetMatch(_matchNumber);
			PlayerSlot winnerSlot = match.WinnerSlot;
			bool wasManualWin = match.IsManualWin;

			// Reset the Match's score, remove any Games, update Bracket & Rankings:
			List<GameModel> modelList = match.ResetScore();
			List<MatchModel> alteredMatches = ApplyGameRemovalEffects(_matchNumber, modelList, winnerSlot);
			UpdateScore(_matchNumber, modelList, false, winnerSlot, wasManualWin);

			// Fire Event with any Matches that changed:
			alteredMatches.Add(GetMatchModel(match));
			OnMatchesModified(alteredMatches);
			// Return Models of any removed Games:
			return modelList;
		}
		#endregion

		#region Accessors
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
				.ToList();
			return ret;
		}
		public virtual IMatch GetMatch(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			
			if (null != GrandFinal &&
				GrandFinal.MatchNumber == _matchNumber)
			{
				return GrandFinal;
			}
			if (null != Matches &&
				Matches.ContainsKey(_matchNumber))
			{
				return Matches[_matchNumber];
			}
			if (null != LowerMatches &&
				LowerMatches.ContainsKey(_matchNumber))
			{
				return LowerMatches[_matchNumber];
			}

			throw new MatchNotFoundException
				("Match not found; match number may be invalid.");
		}
		public MatchModel GetMatchModel(int _matchNumber)
		{
			MatchModel model = GetMatch(_matchNumber).GetModel();
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

			List<IMatch> round = GetRound(_round);
			if (round.Any(m => m.IsFinished))
			{
				throw new InactiveMatchException
					("One or more matches in this round is already finished!");
			}
#if false
			foreach (IMatch match in round)
			{
				if (match.IsFinished)
				{
					throw new InactiveMatchException
						("One or more matches in this round is already finished!");
				}
			}
#endif

			foreach (IMatch match in round)
			{
				GetMatch(match.MatchNumber).SetMaxGames(_maxGamesPerMatch);
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
#if false
			foreach (IMatch match in round)
			{
				if (match.IsFinished)
				{
					throw new InactiveMatchException
						("One or more matches in this round is already finished!");
				}
			}
#endif

			foreach (IMatch match in round)
			{
				GetMatch(match.MatchNumber).SetMaxGames(_maxGamesPerMatch);
			}
		}
		#endregion

		public virtual void ResetMatches()
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();
			List<int> deletedGameIDs = new List<int>();

			for (int n = 1; n <= NumberOfMatches; ++n)
			{
				IMatch match = GetMatch(n);
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
		#endregion

		#region Private Methods
		protected virtual void ResetBracketData()
		{
			if (null == Matches)
			{
				Matches = new Dictionary<int, IMatch>();
			}
			if (null == LowerMatches)
			{
				LowerMatches = new Dictionary<int, IMatch>();
			}
			if (null == Rankings)
			{
				Rankings = new List<IPlayerScore>();
			}

			IsFinished = false;
			Matches.Clear();
			LowerMatches.Clear();
			GrandFinal = null;
			NumberOfRounds = NumberOfLowerRounds = 0;
			NumberOfMatches = 0;
			Rankings.Clear();
		}
		protected void DeleteBracketData()
		{
			List<MatchModel> alteredMatches = new List<MatchModel>();
			List<int> deletedGameIDs = new List<int>();
			if (NumberOfMatches > 0 && null != Matches)
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

		protected MatchModel GetMatchModel(IMatch _match)
		{
			MatchModel model = _match.GetModel();
			model.BracketID = this.Id;
			return model;
		}

		protected int SortRankingScores(IPlayerScore first, IPlayerScore second)
		{
			// Rankings sorting: MatchScore > OpponentsScore > GameScore > PointsScore > initial Seeding
			int compare = -1 * ((first.Wins * MatchWinValue + first.Ties * MatchTieValue)
				.CompareTo(second.Wins * MatchWinValue + second.Ties * MatchTieValue));
			compare = (compare != 0)
				? compare : -1 * (first.OpponentsScore.CompareTo(second.OpponentsScore));
			compare = (compare != 0)
				? compare : -1 * (first.GameScore.CompareTo(second.GameScore));
			compare = (compare != 0)
				? compare : -1 * (first.PointsScore.CompareTo(second.PointsScore));
			return (compare != 0)
				? compare : GetPlayerSeed(first.Id).CompareTo(GetPlayerSeed(second.Id));
		}
		#endregion
	}
}
