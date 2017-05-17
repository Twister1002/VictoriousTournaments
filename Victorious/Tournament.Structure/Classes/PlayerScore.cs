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
		public int Score
		{ get; set; }
		public int MatchScore
		{ get; set; }
		public int OpponentsScore
		{ get; set; }
		public int GameScore
		{ get; set; }
		public int PointsScore
		{ get; set; }
		public int Rank
		{ get; set; }
		#endregion

		/// <summary>
		/// Constructor for KNOCKOUT-type brackets.
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
			MatchScore = OpponentsScore = GameScore = PointsScore = -1;
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
			MatchScore = OpponentsScore = GameScore = PointsScore = 0;
		}
		public PlayerScore()
			: this(0, "")
		{ }

		public void ReplacePlayerData(int _id, string _name)
		{
			this.Id = _id;
			this.Name = _name;
		}
		public void AddToScore(int _matchScore, int _gameScore, int _pointsScore, bool _addition)
		{
			if (_addition)
			{
				this.MatchScore += _matchScore;
				this.GameScore += _gameScore;
				this.PointsScore += _pointsScore;
			}
			else
			{
				this.MatchScore -= _matchScore;
				this.GameScore -= _gameScore;
				this.PointsScore -= _pointsScore;
			}
		}
		public void ResetScore()
		{
			MatchScore = GameScore = PointsScore = 0;
			OpponentsScore = 0;
		}
	}
}
