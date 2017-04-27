using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class Match : IMatch
	{
		#region Variables & Properties
		public int Id
		{ get; private set; }
		public MatchModel Model
		{ get; private set; }
		public bool IsReady
		{ get; private set; }
		public bool IsFinished
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
			Model = new MatchModel();

			IsReady = false;
			IsFinished = false;
			MaxGames = 1;
			Model.MaxGames = 1;

			Players = new IPlayer[2] { null, null };
			Model.ChallengerID = Model.DefenderID = -1;
			WinnerSlot = PlayerSlot.unspecified;
			Model.WinnerID = -1;
			Games = new List<IGame>();
			Score = new int[2] { 0, 0 };
			Model.DefenderScore = Model.ChallengerScore = 0;

			RoundIndex = -1;
			MatchIndex = -1;
			MatchNumber = -1;
			PreviousMatchNumbers = new int[2] { -1, -1 };
			NextMatchNumber = -1;
			NextLoserMatchNumber = -1;
			Model.RoundIndex = Model.MatchIndex = Model.PrevChallengerMatchNumber = Model.PrevDefenderMatchNumber = Model.NextMatchNumber = Model.NextLoserMatchNumber = -1;
		}
		public Match(IMatch _match)
		{
			if (null == _match)
			{
				throw new ArgumentNullException("_match");
			}

			this.Id = _match.Id;
			this.Model = _match.Model;
			this.IsReady = _match.IsReady;
			this.IsFinished = _match.IsFinished;
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
		}
		public Match(MatchModel _m)
		{
			if (null == _m)
			{
				throw new ArgumentNullException("_m");
			}

			this.Id = _m.MatchID;
			this.Model = _m;
			this.MaxGames = (null == _m.MaxGames)
				? 1 : (int)(_m.MaxGames);
			int winsNeeded = MaxGames / 2 + 1;

			Players = new IPlayer[2];
			Players[(int)PlayerSlot.Defender] = (null == _m.Defender)
				? null : new User(_m.Defender);
			Players[(int)PlayerSlot.Challenger] = (null == _m.Challenger)
				? null : new User(_m.Challenger);
			IsReady = (null == Players[0] || null == Players[1])
				? false : true;

			Games = new List<IGame>();
			Score = new int[2] { 0, 0 };
			foreach (GameModel model in _m.Games.OrderBy(m => m.GameNumber))
			{
				this.AddGame(new Game(model));
			}
			//Score[(int)PlayerSlot.Defender] = (null == _m.DefenderScore)
			//	? 0 : (int)(_m.DefenderScore);
			//Score[(int)PlayerSlot.Challenger] = (null == _m.ChallengerScore)
			//	? 0 : (int)(_m.ChallengerScore);
			if (Score[0] > winsNeeded || Score[1] > winsNeeded)
			{
				throw new ScoreException
					("Score cannot be higher than the match allows!");
			}
			WinnerSlot = PlayerSlot.unspecified;
			if (Score[(int)PlayerSlot.Defender] == winsNeeded)
			{
				WinnerSlot = PlayerSlot.Defender;
			}
			else if (Score[(int)PlayerSlot.Challenger] == winsNeeded)
			{
				WinnerSlot = PlayerSlot.Challenger;
			}
			IsFinished = (PlayerSlot.unspecified == WinnerSlot)
				? false : true;

			RoundIndex = (int)(_m.RoundIndex);
			MatchIndex = (int)(_m.MatchIndex);
			MatchNumber = _m.MatchNumber;
			NextMatchNumber = (int)(_m.NextMatchNumber);
			NextLoserMatchNumber = (int)(_m.NextLoserMatchNumber);

			PreviousMatchNumbers = new int[2] { -1, -1 };
			PreviousMatchNumbers[(int)PlayerSlot.Defender] =
				(null == _m.PrevDefenderMatchNumber)
				? -1 : (int)(_m.PrevDefenderMatchNumber);
			PreviousMatchNumbers[(int)PlayerSlot.Challenger] =
				(null == _m.PrevChallengerMatchNumber)
				? -1 : (int)(_m.PrevChallengerMatchNumber);
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
				GameModel gm = new GameModel();
				gm.GameID = game.Id;
				gm.ChallengerID = game.PlayerIDs[(int)PlayerSlot.Challenger];
				gm.DefenderID = game.PlayerIDs[(int)PlayerSlot.Defender];
				gm.WinnerID = (PlayerSlot.unspecified == game.WinnerSlot)
					? (int)(game.WinnerSlot) : game.PlayerIDs[(int)(game.WinnerSlot)];
				gm.MatchID = this.Id;
				gm.GameNumber = game.GameNumber;
				gm.ChallengerScore = game.Score[(int)PlayerSlot.Challenger];
				gm.DefenderScore = game.Score[(int)PlayerSlot.Defender];

				model.Games.Add(gm);
			}

			return model;
		}

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
						switch ((PlayerSlot)i)
						{
							case (PlayerSlot.Defender):
								Model.DefenderID = _player.Id;
								break;
							case (PlayerSlot.Challenger):
								Model.ChallengerID = _player.Id;
								break;
						}

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
				Model.DefenderID = _newPlayer.Id;
				foreach (IGame game in Games)
				{
					game.PlayerIDs[(int)PlayerSlot.Defender] = _newPlayer.Id;
				}
			}
			else if (null != Players[(int)PlayerSlot.Challenger] &&
				_oldPlayerId == Players[(int)PlayerSlot.Challenger].Id)
			{
				Players[(int)PlayerSlot.Challenger] = _newPlayer;
				Model.ChallengerID = _newPlayer.Id;
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
					switch ((PlayerSlot)i)
					{
						case (PlayerSlot.Defender):
							Model.DefenderID = -1;
							break;
						case (PlayerSlot.Challenger):
							Model.ChallengerID = -1;
							break;
					}

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
			Model.ChallengerID = Model.DefenderID = -1;

			ResetScore();
			IsReady = false;
		}

		public GameModel AddGame(int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Cannot add games to an inactive match!");
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

			IGame game = new Game(this.Id, (Games.Count + 1));
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
			return game.GetModel();
		}
		public GameModel AddGame(int _defenderScore, int _challengerScore)
		{
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Cannot add games to an inactive match!");
			}
			if (_defenderScore < 0 || _challengerScore < 0)
			{
				throw new ScoreException
					("Score cannot be negative!");
			}

			IGame game = new Game(this.Id, (Games.Count + 1));
			for (int i = 0; i < 2; ++i)
			{
				game.PlayerIDs[i] = this.Players[i].Id;
			}
			game.Score[(int)PlayerSlot.Defender] = _defenderScore;
			game.Score[(int)PlayerSlot.Challenger] = _challengerScore;
			if (_defenderScore > _challengerScore)
			{
				game.WinnerSlot = PlayerSlot.Defender;
			}
			else if (_challengerScore > _defenderScore)
			{
				game.WinnerSlot = PlayerSlot.Challenger;
			}
			else
			{
				throw new NotImplementedException
					("Tie Games are not (yet) supported!");
			}

			AddWin(game.WinnerSlot);
			Games.Add(game);
			return game.GetModel();
		}
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
					RemoveGame(g);

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
		public GameModel RemoveLastGame()
		{
			return (RemoveGame(Games.Count - 1));
		}
		public void ResetScore()
		{
			if (null == Score)
			{
				Score = new int[2];
			}

			IsFinished = false;
			WinnerSlot = PlayerSlot.unspecified;
			Model.WinnerID = -1;
			Games.Clear();
			Score[0] = Score[1] = 0;
			Model.DefenderScore = Model.ChallengerScore = 0;
		}

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
			Model.MaxGames = this.MaxGames;
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
			Model.RoundIndex = _index;
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
			Model.MatchIndex = _index;
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
			Model.MatchNumber = _number;
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
				Model.PrevDefenderMatchNumber = _number;
			}
			else if ((PlayerSlot.unspecified == _slot || PlayerSlot.Challenger == _slot)
				&& PreviousMatchNumbers[(int)PlayerSlot.Challenger] < 0)
			{
				PreviousMatchNumbers[(int)PlayerSlot.Challenger] = _number;
				Model.PrevChallengerMatchNumber = _number;
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
			Model.NextMatchNumber = _number;
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
			Model.NextLoserMatchNumber = _number;
		}
		#endregion

		#region Private Methods
		private void AddGame(IGame _game)
		{
			if (null == _game)
			{
				throw new ArgumentNullException("_game");
			}

			_game.MatchId = this.Id;
			_game.GameNumber = (_game.GameNumber > 0)
				? _game.GameNumber : (Games.Count + 1);
			_game.PlayerIDs[(int)PlayerSlot.Defender] = this.Players[(int)PlayerSlot.Defender].Id;
			_game.PlayerIDs[(int)PlayerSlot.Challenger] = this.Players[(int)PlayerSlot.Challenger].Id;
			foreach (IGame game in Games)
			{
				if (game.Id == _game.Id || game.GameNumber == _game.GameNumber)
				{
					throw new DuplicateObjectException
						("New game cannot match an existing game!");
				}
			}

			AddWin(_game.WinnerSlot);
			Games.Add(_game);
		}
		private GameModel RemoveGame(int _index)
		{
			if (0 == Games.Count)
			{
				throw new GameNotFoundException
					("No Games to remove!");
			}
			if (_index < 0 || _index >= Games.Count)
			{
				throw new InvalidIndexException
					("Game index is out of range!");
			}

			GameModel removedGame = Games[_index].GetModel();
			SubtractWin(Games[_index].WinnerSlot);
			Games.RemoveAt(_index);

			return removedGame;
		}
		private void AddWin(PlayerSlot _slot)
		{
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be 0 or 1!");
			}
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

			Score[(int)_slot] += 1;
			switch (_slot)
			{
				case (PlayerSlot.Defender):
					Model.DefenderScore++;
					break;
				case (PlayerSlot.Challenger):
					Model.ChallengerScore++;
					break;
			}

			int winsNeeded = MaxGames / 2 + 1;
			if (Score[(int)_slot] >= winsNeeded)
			{
				WinnerSlot = _slot;
				Model.WinnerID = Players[(int)_slot].Id;
				IsFinished = true;
			}
			else if (Score[0] + Score[1] >= MaxGames)
			{
				WinnerSlot = PlayerSlot.unspecified;
				Model.WinnerID = -1;
				IsFinished = true;
			}
		}
		private void SubtractWin(PlayerSlot _slot)
		{
			if (_slot != PlayerSlot.Defender &&
				_slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("PlayerSlot must be 0 or 1!");
			}
			if (!IsReady)
			{
				throw new InactiveMatchException
					("Match is not begun; can't subtract wins!");
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
				Model.WinnerID = -1;
			}

			Score[(int)_slot] -= 1;
			switch (_slot)
			{
				case (PlayerSlot.Defender):
					Model.DefenderScore--;
					break;
				case (PlayerSlot.Challenger):
					Model.ChallengerScore--;
					break;
			}
		}
		#endregion
	}
}
