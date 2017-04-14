using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class Game : IGame
	{
		#region Variables & Properties
		public int Id
		{ get; private set; }
		public int MatchId
		{ get; set; }
		public int GameNumber
		{ get; set; }
		public int[] PlayerIDs
		{ get; set; }
		public PlayerSlot WinnerSlot
		{ get; set; }
		public int[] Score
		{ get; set; }
		#endregion

		#region Ctors
		public Game(int _matchId, int _gameNumber, int[] _playerIDs, PlayerSlot _winnerSlot, int[] _score)
		{
			if (null == _playerIDs)
			{
				throw new ArgumentNullException("_playerIDs");
			}
			if (null == _score)
			{
				throw new ArgumentNullException("_score");
			}

			Id = -1;
			MatchId = _matchId;
			GameNumber = _gameNumber;

			PlayerIDs = new int[2] { -1, -1 };
			Score = new int[2] { 0, 0 };
			for (int i = 0; i < 2; ++i)
			{
				PlayerIDs[i] = _playerIDs[i];
				Score[i] = _score[i];
			}
			WinnerSlot = _winnerSlot;
		}
		public Game(int _matchId, int _gameNumber)
			: this(_matchId, _gameNumber, new int[2], PlayerSlot.unspecified, new int[2])
		{ }
		public Game()
			: this(-1, 0)
		{ }
		public Game(GameModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.GameID;
			this.MatchId = (int)(_model.MatchID);
			this.GameNumber = _model.GameNumber;

			this.PlayerIDs = new int[2] { _model.DefenderID, _model.ChallengerID };
			this.Score = new int[2] { _model.DefenderScore, _model.ChallengerScore };
			if (_model.WinnerID == PlayerIDs[(int)PlayerSlot.Defender])
			{
				this.WinnerSlot = PlayerSlot.Defender;
			}
			else if (_model.WinnerID == PlayerIDs[(int)PlayerSlot.Challenger])
			{
				this.WinnerSlot = PlayerSlot.Challenger;
			}
			else
			{
				this.WinnerSlot = PlayerSlot.unspecified;
			}
		}
		#endregion

		#region Public Methods
		public GameModel GetModel()
		{
			GameModel model = new GameModel();
			model.GameID = this.Id;
			model.ChallengerID = this.PlayerIDs[(int)PlayerSlot.Challenger];
			model.DefenderID = this.PlayerIDs[(int)PlayerSlot.Defender];
			model.WinnerID = (PlayerSlot.unspecified == this.WinnerSlot)
				? -1 : this.PlayerIDs[(int)WinnerSlot];
			model.MatchID = this.MatchId;
			model.GameNumber = this.GameNumber;
			model.ChallengerScore = this.Score[(int)PlayerSlot.Challenger];
			model.DefenderScore = this.Score[(int)PlayerSlot.Defender];

			return model;
		}
		#endregion
	}
}
