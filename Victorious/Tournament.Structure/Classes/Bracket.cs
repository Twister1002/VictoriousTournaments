using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public abstract class Bracket : IBracket
	{
		#region Variables & Properties
		public BracketTypeModel.BracketType BracketType
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
		#endregion

		#region Abstract Methods
		public abstract void CreateBracket(int _gamesPerMatch = 1);
		public abstract GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore);
		protected abstract void UpdateScore(int _matchNumber, GameModel _game, bool _isAddition);
		protected abstract void ApplyWinEffects(int _matchNumber, PlayerSlot _slot);
		protected abstract void ApplyGameRemovalEffects(int _matchNumber, GameModel _game, bool _wasFinished);
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

			if (null != GrandFinal &&
				GrandFinal.MatchNumber == _matchNumber)
			{
				GrandFinal = new Match(_model);
			}
			else if (null != Matches &&
				Matches.ContainsKey(_matchNumber))
			{
				Matches[_matchNumber] = new Match(_model);
			}
			else if (null != LowerMatches &&
				LowerMatches.ContainsKey(_matchNumber))
			{
				LowerMatches[_matchNumber] = new Match(_model);
			}
			else
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}
		}

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
				ResetBracket();
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
			ResetBracket();
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
			ResetBracket();
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
			ResetBracket();
		}
		public void ReplacePlayer(IPlayer _player, int _index)
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

			if (null != Players[_index])
			{
				// Replace existing Player in any Matches
				for (int n = 1; n <= NumberOfMatches; ++n)
				{
					try
					{
						GetMatch(n).ReplacePlayer(_player, Players[_index].Id);
					}
					catch (PlayerNotFoundException)
					{ }
				}
				// Replace existing Player in the Rankings
				for (int i = 0; i < Rankings.Count(); ++i)
				{
					if (Rankings[i].Id == Players[_index].Id)
					{
						int score = Rankings[i].Score;
						int rank = Rankings[i].Rank;
						Rankings[i] = new PlayerScore
							(_player.Id, _player.Name, score, rank);
					}
				}
			}

			Players[_index] = _player;
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
			ResetBracket();
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

			ResetBracket();
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
					ResetBracket();
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
			ResetBracket();
		}

		public virtual GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			GameModel gameModel = GetMatch(_matchNumber)
				.AddGame(_defenderScore, _challengerScore, _winnerSlot);
			UpdateScore(_matchNumber, gameModel, true);
			ApplyWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
		public virtual GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			bool wasFinished = GetMatch(_matchNumber).IsFinished;
			GameModel removedGame = GetMatch(_matchNumber).RemoveGameNumber(_gameNumber);
			ApplyGameRemovalEffects(_matchNumber, removedGame, wasFinished);
			UpdateScore(_matchNumber, removedGame, false);

			GameModel addedGame = GetMatch(_matchNumber)
				.AddGame(_defenderScore, _challengerScore, _winnerSlot);
			UpdateScore(_matchNumber, addedGame, true);
			ApplyWinEffects(_matchNumber, _winnerSlot);
			return addedGame;
		}
		public virtual GameModel RemoveLastGame(int _matchNumber)
		{
			bool wasFinished = GetMatch(_matchNumber).IsFinished;
			GameModel gameModel = GetMatch(_matchNumber).RemoveLastGame();
			ApplyGameRemovalEffects(_matchNumber, gameModel, wasFinished);
			UpdateScore(_matchNumber, gameModel, false);
			return gameModel;
		}
		public virtual void ResetMatchScore(int _matchNumber)
		{
			while (GetMatch(_matchNumber).Games.Count > 0)
			{
				RemoveLastGame(_matchNumber);
			}
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
		public void SetMaxGamesForWholeRound(int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}

			List<IMatch> round = GetRound(_round);
			foreach (IMatch match in round)
			{
				if (match.IsFinished)
				{
					throw new InactiveMatchException
						("One or more matches in this round is already finished!");
				}
			}

			foreach (IMatch match in round)
			{
				GetMatch(match.MatchNumber).SetMaxGames(_maxGamesPerMatch);
			}
		}
		public void SetMaxGamesForWholeLowerRound(int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}

			List<IMatch> round = GetLowerRound(_round);
			foreach (IMatch match in round)
			{
				if (match.IsFinished)
				{
					throw new InactiveMatchException
						("One or more matches in this round is already finished!");
				}
			}

			foreach (IMatch match in round)
			{
				GetMatch(match.MatchNumber).SetMaxGames(_maxGamesPerMatch);
			}
		}
		public virtual void ResetMatches()
		{
			for (int n = 1; n <= NumberOfMatches; ++n)
			{
				IMatch match = GetMatch(n);
				for (int i = 0; i < 2; ++i)
				{
					if (match.PreviousMatchNumbers[i] > -1 &&
						null != match.Players[i])
					{
						GetMatch(n).RemovePlayer(match.Players[i].Id);
					}
				}
				GetMatch(n).ResetScore();
			}
		}
		#endregion

		#region Private Methods
		protected virtual void ResetBracket()
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
		#endregion
	}
}
