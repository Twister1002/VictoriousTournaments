using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class PlayerScore : IPlayerScore
	{
		#region Variables & Properties
		public int Id
		{ get; private set; }
		public string Name
		{ get; private set; }

		private PlayerRecord MatchRecord
		{ get; set; }
		public int Wins
		{ get { return MatchRecord.Wins; } }
		public int W
		{ get { return MatchRecord.Wins; } }
		public int Ties
		{ get { return MatchRecord.Ties; } }
		public int T
		{ get { return MatchRecord.Ties; } }
		public int Losses
		{ get { return MatchRecord.Losses; } }
		public int L
		{ get { return MatchRecord.Losses; } }

		public int MatchScore
		{ get { return CalculateScore(2, 1, 0); } }
		public int OpponentsScore
		{ get; set; }
		public int GameScore
		{ get; set; }
		public int PointsScore
		{ get; set; }
		public int Rank
		{ get; set; }
		#endregion

		#region Ctors
		/// <summary>
		/// Constructor for ELIMINATION-type brackets.
		/// Sets Score=-1.
		/// </summary>
		/// <param name="_id">Player ID</param>
		/// <param name="_name">Player name</param>
		/// <param name="_rank">Player's rank</param>
		public PlayerScore(int _id, string _name, int _rank)
		{
			this.Id = _id;
			this.Name = _name;
			this.Rank = _rank;
			MatchRecord = new PlayerRecord();
			OpponentsScore = GameScore = PointsScore = -1;
		}
		/// <summary>
		/// Constructor for SCORE-based brackets.
		/// Sets Rank=1 and Score=0.
		/// </summary>
		/// <param name="_id">Player ID</param>
		/// <param name="_name">Player name</param>
		public PlayerScore(int _id, string _name)
		{
			this.Id = _id;
			this.Name = _name;
			Rank = 1;
			MatchRecord = new PlayerRecord();
			OpponentsScore = GameScore = PointsScore = 0;
		}
		public PlayerScore()
			: this(0, "")
		{ }
		#endregion

		#region Public Methods
		public int[] GetRecord()
		{
			int[] record = new int[3];
			record[(int)Record.Wins] = Wins;
			record[(int)Record.Losses] = Losses;
			record[(int)Record.Ties] = Ties;
			return record;
		}

		public void ReplacePlayerData(int _id, string _name)
		{
			this.Id = _id;
			this.Name = _name;
		}

		public void AddMatchOutcome(Outcome _outcome, int _gameScore, int _pointsScore, bool _isAddition)
		{
			int add = (_isAddition) ? 1 : -1;
			MatchRecord.AddOutcome(_outcome, _isAddition);
			GameScore += (_gameScore * add);
			PointsScore += (_pointsScore * add);
		}

		public int CalculateScore(int _matchWinValue, int _matchTieValue, int _matchLossValue)
		{
			int score = 0;
			score += (MatchRecord.Wins * _matchWinValue);
			score += (MatchRecord.Ties * _matchTieValue);
			score += (MatchRecord.Losses * _matchLossValue);
			return score;
		}
		public void ResetScore()
		{
			MatchRecord.Reset();
			GameScore = PointsScore = 0;
			OpponentsScore = 0;
		}

		public void AddToScore(int _matchScore, int _gameScore, int _pointsScore, bool _isAddition)
		{
			switch (_matchScore)
			{
				case 2:
					AddMatchOutcome(Outcome.Win, _gameScore, _pointsScore, _isAddition);
					break;
				case 1:
					AddMatchOutcome(Outcome.Tie, _gameScore, _pointsScore, _isAddition);
					break;
				case 0:
					AddMatchOutcome(Outcome.Loss, _gameScore, _pointsScore, _isAddition);
					break;
			}
		}
		#endregion
	}
}
