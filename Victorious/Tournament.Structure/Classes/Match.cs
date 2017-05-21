﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class Match : IMatch
	{
		#region Variables & Properties
		public int Id
		{ get; private set; }
		public bool IsReady
		{ get; private set; }
		public bool IsFinished
		{ get; private set; }
		public bool IsManualWin
		{ get; private set; }
		public int MaxGames
		{ get; private set; }
		public IPlayer[] Players
		{ get; private set; }
		public PlayerSlot WinnerSlot
		{ get; private set; }
		public List<IGame> Games
		{ get; private set; }
		public int[] Score
		{ get; private set; }
		public int RoundIndex
		{ get; private set; }
		public int MatchIndex
		{ get; private set; }
		public int MatchNumber
		{ get; private set; }
		public int[] PreviousMatchNumbers
		{ get; private set; }
		public int NextMatchNumber
		{ get; private set; }
		public int NextLoserMatchNumber
		{ get; private set; }
		#endregion

		#region Ctors
		public Match()
		{
			Id = 0;

			IsReady = false;
			IsFinished = false;
			IsManualWin = false;
			MaxGames = 1;

			Players = new IPlayer[2] { null, null };
			WinnerSlot = PlayerSlot.unspecified;
			Games = new List<IGame>();
			Score = new int[2] { 0, 0 };

			RoundIndex = -1;
			MatchIndex = -1;
			MatchNumber = -1;
			PreviousMatchNumbers = new int[2] { -1, -1 };
			NextMatchNumber = -1;
			NextLoserMatchNumber = -1;
		}
		public Match(IMatch _match)
		{
			throw new NotImplementedException
				("Copy ctor hasn't been used, so isn't up-to-date.");
#if false
			if (null == _match)
			{
				throw new ArgumentNullException("_match");
			}

			this.Id = _match.Id;
			this.IsReady = _match.IsReady;
			this.IsFinished = _match.IsFinished;
			this.IsManualWin = _match.IsManualWin; // NOTE : This needs fixing.
			this.MaxGames = _match.MaxGames;
			this.WinnerSlot = _match.WinnerSlot;
			this.RoundIndex = _match.RoundIndex;
			this.MatchIndex = _match.MatchIndex;
			this.MatchNumber = _match.MatchNumber;
			this.NextMatchNumber = _match.NextMatchNumber;
			this.NextLoserMatchNumber = _match.NextLoserMatchNumber;

			this.Players = new IPlayer[2];
			this.Score = new int[2];
			this.PreviousMatchNumbers = new int[2];
			for (int i = 0; i < 2; ++i)
			{
				if (_match.Players[i] is User)
				{
					this.Players[i] = new User(_match.Players[i] as User);
				}
				else if (_match.Players[i] is Team)
				{
					this.Players[i] = new Team(_match.Players[i] as Team);
				}

				//this.Score[i] = _match.Score[i];
				this.PreviousMatchNumbers[i] = _match.PreviousMatchNumbers[i];
			}

			this.Games = new List<IGame>();
			foreach (IGame game in _match.Games.OrderBy(g => g.GameNumber))
			{
				//this.Games.Add(game);
				this.AddGame(game);
			}
#endif
		}
		public Match(MatchModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.MatchID;
			this.MaxGames = (null == _model.MaxGames)
				? 1 : (int)(_model.MaxGames);

			this.Players = new IPlayer[2];
			Players[(int)PlayerSlot.Defender] = (null == _model.Defender)
				? null : new User(_model.Defender);
			Players[(int)PlayerSlot.Challenger] = (null == _model.Challenger)
				? null : new User(_model.Challenger);
			this.IsReady = (null == Players[0] || null == Players[1])
				? false : true;

			this.Games = new List<IGame>();
			this.Score = new int[2] { 0, 0 };
			this.IsManualWin = false;
			if (_model.IsManualWin)
			{
				PlayerSlot winnerSlot = (_model.WinnerID == _model.DefenderID)
					? PlayerSlot.Defender : PlayerSlot.Challenger;
				SetWinner(winnerSlot);
			}

			foreach (GameModel model in _model.Games.OrderBy(m => m.GameNumber))
			{
				AddGame(new Game(model));
			}
			int winsNeeded = MaxGames / 2 + 1;
			if (Score[0] > winsNeeded || Score[1] > winsNeeded)
			{
				throw new ScoreException
					("Score cannot be higher than the match allows!");
			}
			if (!this.IsManualWin)
			{
				this.WinnerSlot = PlayerSlot.unspecified;
				if (Score[(int)PlayerSlot.Defender] == winsNeeded)
				{
					this.WinnerSlot = PlayerSlot.Defender;
				}
				else if (Score[(int)PlayerSlot.Challenger] == winsNeeded)
				{
					this.WinnerSlot = PlayerSlot.Challenger;
				}
				this.IsFinished = (PlayerSlot.unspecified == WinnerSlot)
					? false : true;
			}

			this.RoundIndex = (int)(_model.RoundIndex);
			this.MatchIndex = (int)(_model.MatchIndex);
			this.MatchNumber = _model.MatchNumber;
			this.NextMatchNumber = (int)(_model.NextMatchNumber);
			this.NextLoserMatchNumber = (int)(_model.NextLoserMatchNumber);

			this.PreviousMatchNumbers = new int[2] { -1, -1 };
			PreviousMatchNumbers[(int)PlayerSlot.Defender] =
				(null == _model.PrevDefenderMatchNumber)
				? -1 : (int)(_model.PrevDefenderMatchNumber);
			PreviousMatchNumbers[(int)PlayerSlot.Challenger] =
				(null == _model.PrevChallengerMatchNumber)
				? -1 : (int)(_model.PrevChallengerMatchNumber);
		}
		#endregion

		#region Public Methods
		public MatchModel GetModel()
		{
			MatchModel model = new MatchModel();

			model.MatchID = this.Id;
			model.RoundIndex = this.RoundIndex;
			model.MatchIndex = this.MatchIndex;
			model.MatchNumber = this.MatchNumber;
			model.NextMatchNumber = this.NextMatchNumber;
			model.NextLoserMatchNumber = this.NextLoserMatchNumber;
			model.PrevDefenderMatchNumber = this.PreviousMatchNumbers[(int)PlayerSlot.Defender];
			model.PrevChallengerMatchNumber = this.PreviousMatchNumbers[(int)PlayerSlot.Challenger];
			model.MaxGames = this.MaxGames;

			model.ChallengerID = (null != Players[(int)PlayerSlot.Challenger])
				? Players[(int)PlayerSlot.Challenger].Id : -1;
			model.DefenderID = (null != Players[(int)PlayerSlot.Defender])
				? Players[(int)PlayerSlot.Defender].Id : -1;
			model.WinnerID = (PlayerSlot.unspecified == WinnerSlot)
				? (int)this.WinnerSlot
				: Players[(int)WinnerSlot].Id;
			model.ChallengerScore = this.Score[(int)PlayerSlot.Challenger];
			model.DefenderScore = this.Score[(int)PlayerSlot.Defender];

			model.Games = new List<GameModel>();
			foreach (IGame game in this.Games)
			{
				GameModel gm = game.GetModel();
				gm.MatchID = this.Id;
				model.Games.Add(gm);
			}

			return model;
		}

		#region Player Methods
		public void AddPlayer(IPlayer _player, PlayerSlot _slot = PlayerSlot.unspecified)
		{
			if (_slot != PlayerSlot.unspecified &&
				_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be -1, 0, or 1!");
			}
			if ((null != Players[0] && Players[0].Id == _player.Id) ||
				(null != Players[1] && Players[1].Id == _player.Id))
			{
				throw new DuplicateObjectException
					("Match already contains this Player!");
			}

			for (int i = 0; i < 2; ++i)
			{
				if ((int)_slot == i || _slot == PlayerSlot.unspecified)
				{
					if (null == Players[i])
					{
						Players[i] = _player;

						if (null != Players[0] && null != Players[1])
						{
							IsReady = true;
						}
						return;
					}
				}
			}

			throw new SlotFullException
				("Match cannot add Player; there is already a Player in this Slot!");
		}
		public void ReplacePlayer(IPlayer _newPlayer, int _oldPlayerId)
		{
			if (null == _newPlayer)
			{
				throw new ArgumentNullException("_newPlayer");
			}

			if (null != Players[(int)PlayerSlot.Defender] &&
				_oldPlayerId == Players[(int)PlayerSlot.Defender].Id)
			{
				Players[(int)PlayerSlot.Defender] = _newPlayer;
				foreach (IGame game in Games)
				{
					game.PlayerIDs[(int)PlayerSlot.Defender] = _newPlayer.Id;
				}
			}
			else if (null != Players[(int)PlayerSlot.Challenger] &&
				_oldPlayerId == Players[(int)PlayerSlot.Challenger].Id)
			{
				Players[(int)PlayerSlot.Challenger] = _newPlayer;
				foreach (IGame game in Games)
				{
					game.PlayerIDs[(int)PlayerSlot.Challenger] = _newPlayer.Id;
				}
			}
			else
			{
				throw new PlayerNotFoundException
					("Player not found in this Match!");
			}
		}
		public void RemovePlayer(int _playerId)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (null != Players[i] && Players[i].Id == _playerId)
				{
					Players[i] = null;

					ResetScore();
					IsReady = false;
					return;
				}
			}

			throw new PlayerNotFoundException
				("Player not found in this Match!");
		}
		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new IPlayer[2];
			}
			Players[0] = Players[1] = null;

			ResetScore();
			IsReady = false;
		}
		#endregion

		#region Game & Score Methods
		public GameModel AddGame(int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Cannot add games to an inactive match!");
			}
			if (IsFinished && !IsManualWin)
			{
				throw new InactiveMatchException
					("Cannot add games to a finished match!");
			}
			if (PlayerSlot.unspecified == _winnerSlot)
			{
				throw new NotImplementedException
					("No ties allowed / enter a winner slot!");
			}
			if (_defenderScore < 0 || _challengerScore < 0)
			{
				throw new ScoreException
					("Score cannot be negative!");
			}

			List<int> gameNumbers = Games.Select(g => g.GameNumber).ToList();
			int gameNum = 1;
			// Find the lowest (positive) Game Number we can add:
			while (gameNumbers.Contains(gameNum))
			{
				++gameNum;
			}
			IGame game = new Game(this.Id, gameNum);
			// Add Game's data (players and score):
			for (int i = 0; i < 2; ++i)
			{
				game.PlayerIDs[i] = this.Players[i].Id;
			}
			game.Score[(int)PlayerSlot.Defender] = _defenderScore;
			game.Score[(int)PlayerSlot.Challenger] = _challengerScore;
			game.WinnerSlot = _winnerSlot;

			if (!IsFinished)
			{
				if (PlayerSlot.Defender == _winnerSlot ||
					PlayerSlot.Challenger == _winnerSlot)
				{
					AddWin(game.WinnerSlot);
				}
			}

			Games.Add(game);
			Games.Sort((first, second) => first.GameNumber.CompareTo(second.GameNumber));
			return game.GetModel();
		}
#if false
		public GameModel UpdateGame(int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			if (_gameNumber < 1)
			{
				throw new InvalidIndexException
					("Game Number must be positive!");
			}
			if (PlayerSlot.unspecified == _winnerSlot)
			{
				throw new NotImplementedException
					("No ties allowed / enter a winner slot!");
			}
			if (_defenderScore < 0 || _challengerScore < 0)
			{
				throw new ScoreException
					("Score cannot be negative!");
			}

			for (int g = 0; g < Games.Count; ++g)
			{
				if (Games[g].GameNumber == _gameNumber)
				{
					RemoveGameNumber(g);

					IGame game = new Game(this.Id, _gameNumber);
					for (int i = 0; i < 2; ++i)
					{
						game.PlayerIDs[i] = this.Players[i].Id;
					}
					game.Score[(int)PlayerSlot.Defender] = _defenderScore;
					game.Score[(int)PlayerSlot.Challenger] = _challengerScore;
					game.WinnerSlot = _winnerSlot;

					if (PlayerSlot.Defender == _winnerSlot ||
						PlayerSlot.Challenger == _winnerSlot)
					{
						AddWin(game.WinnerSlot);
					}
					Games.Add(game);
					Games.Sort((first, second) => first.GameNumber.CompareTo(second.GameNumber));
					return (game.GetModel());
				}
			}

			throw new GameNotFoundException
				("Game not found; Game Number may be invalid!");
		}
#endif
		public GameModel RemoveLastGame()
		{
			int index = Games.Count - 1;
			if (index < 0)
			{
				throw new GameNotFoundException
					("No Games to remove!");
			}

			return (RemoveGameNumber(Games[index].GameNumber));
		}
		public GameModel RemoveGameNumber(int _gameNumber)
		{
			for (int index = 0; index < Games.Count; ++index)
			{
				if (Games[index].GameNumber == _gameNumber)
				{
					GameModel removedGame = Games[index].GetModel();
					if (!IsManualWin)
					{
						SubtractWin(Games[index].WinnerSlot);
					}
					Games.RemoveAt(index);
					return removedGame;
				}
			}

			throw new GameNotFoundException
				("Game not found; Game Number may be invalid!");
		}
		public void SetWinner(PlayerSlot _winnerSlot)
		{
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Cannot set a winner for an inactive match!");
			}
			if (IsFinished || Games.Count > 0)
			{
				throw new InactiveMatchException
					("Can't set winner! Reset match first.");
			}

			if (PlayerSlot.Defender == _winnerSlot)
			{
				Score[(int)PlayerSlot.Defender] = -1;
				Score[(int)PlayerSlot.Challenger] = 0;
			}
			else if (PlayerSlot.Challenger == _winnerSlot)
			{
				Score[(int)PlayerSlot.Defender] = 0;
				Score[(int)PlayerSlot.Challenger] = -1;
			}
			else
			{
				throw new InvalidSlotException
					("Winner must be Defender or Challenger!");
			}

			WinnerSlot = _winnerSlot;
			IsFinished = true;
			IsManualWin = true;
		}
		public List<GameModel> ResetScore()
		{
			if (null == Score)
			{
				Score = new int[2];
			}

			IsFinished = IsManualWin = false;
			Score[0] = Score[1] = 0;
			WinnerSlot = PlayerSlot.unspecified;

			List<GameModel> modelList = new List<GameModel>();
			foreach (IGame game in Games)
			{
				modelList.Add(game.GetModel());
			}
			Games.Clear();
			return modelList;
		}
		#endregion

		#region Mutators
		public void SetMaxGames(int _numberOfGames)
		{
			if (IsFinished)
			{
				throw new InactiveMatchException
					("Match is finished; cannot change victory conditions.");
			}
			if (_numberOfGames < 1)
			{
				throw new ScoreException
					("Total games cannot be less than 1!");
			}

			MaxGames = _numberOfGames;
		}
		public void SetRoundIndex(int _index)
		{
			if (RoundIndex > -1)
			{
				throw new AlreadyAssignedException
					("Round Index is already set!");
			}
			if (_index < 1)
			{
				throw new InvalidIndexException
					("Round Index cannot be less than 1!");
			}

			RoundIndex = _index;
		}
		public void SetMatchIndex(int _index)
		{
			if (MatchIndex > -1)
			{
				throw new AlreadyAssignedException
					("Match Index is already set!");
			}
			if (_index < 1)
			{
				throw new InvalidIndexException
					("Match Index cannot be less than 1!");
			}

			MatchIndex = _index;
		}
		public void SetMatchNumber(int _number)
		{
			if (MatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Match Number is already set!");
			}
			if (_number < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			MatchNumber = _number;
		}
		public void AddPreviousMatchNumber(int _number, PlayerSlot _slot = PlayerSlot.unspecified)
		{
			if (_number < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			if ((PlayerSlot.unspecified == _slot || PlayerSlot.Defender == _slot)
				&& PreviousMatchNumbers[(int)PlayerSlot.Defender] < 0)
			{
				PreviousMatchNumbers[(int)PlayerSlot.Defender] = _number;
			}
			else if ((PlayerSlot.unspecified == _slot || PlayerSlot.Challenger == _slot)
				&& PreviousMatchNumbers[(int)PlayerSlot.Challenger] < 0)
			{
				PreviousMatchNumbers[(int)PlayerSlot.Challenger] = _number;
			}
			else
			{
				throw new AlreadyAssignedException
					("Previous Match Numbers are already set!");
			}
		}
		public void SetNextMatchNumber(int _number)
		{
			if (NextMatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Next Match Number is already set!");
			}
			if (_number < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			NextMatchNumber = _number;
		}
		public void SetNextLoserMatchNumber(int _number)
		{
			if (NextLoserMatchNumber > -1)
			{
				throw new AlreadyAssignedException
					("Next Loser Match Number is already set!");
			}
			if (_number < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			NextLoserMatchNumber = _number;
		}
		#endregion
		#endregion

		#region Private Methods
		private void AddGame(IGame _game)
		{
			if (null == _game)
			{
				throw new ArgumentNullException("_game");
			}

			AddWin(_game.WinnerSlot);
			Games.Add(_game);
		}
		private void AddWin(PlayerSlot _slot)
		{
			if (IsFinished)
			{
				throw new InactiveMatchException
					("Match is finished; can't add more wins!");
			}
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Match is not begun; can't add a win!");
			}
			if (PlayerSlot.unspecified == _slot)
			{
				// Adding a tie: do nothing to Score
				return;
			}
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be 0 or 1!");
			}

			Score[(int)_slot] += 1;

			int winsNeeded = MaxGames / 2 + 1;
			if (Score[(int)_slot] >= winsNeeded)
			{
				// One player has enough game-wins to win the match
				WinnerSlot = _slot;
				IsFinished = true;
			}
			else if (Score[0] + Score[1] >= MaxGames)
			{
				// MaxGames has been played without a winner: Match is over (TIE)
				WinnerSlot = PlayerSlot.unspecified;
				IsFinished = true;
			}
		}
		private void SubtractWin(PlayerSlot _slot)
		{
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Match is not begun; can't subtract wins!");
			}
			if (PlayerSlot.unspecified == _slot)
			{
				// Removing a tie: do nothing to Score
				return;
			}
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be 0 or 1!");
			}
			if (Score[(int)_slot] <= 0)
			{
				throw new ScoreException
					("Score is already 0; can't subtract wins!");
			}

			if (WinnerSlot == _slot)
			{
				IsFinished = false;
				WinnerSlot = PlayerSlot.unspecified;
			}

			Score[(int)_slot] -= 1;
		}
		#endregion
	}
}
