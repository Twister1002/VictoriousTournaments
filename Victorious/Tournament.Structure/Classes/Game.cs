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
		{ get; private set; }
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
			throw new NotImplementedException();
		}
		#endregion

		#region Public Methods
		public GameModel GetModel()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
