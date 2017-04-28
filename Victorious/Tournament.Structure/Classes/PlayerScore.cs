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
		public int GameScore
		{ get; set; }
		public int PointsScore
		{ get; set; }
		public int Rank
		{ get; set; }
		#endregion

		public PlayerScore(int _id, string _name, int _score, int _rank)
		{
			this.Id = _id;
			this.Name = _name;
			this.Score = _score;
			this.MatchScore = this.GameScore = this.PointsScore = _score;
			this.Rank = _rank;
		}
		public PlayerScore()
			: this(0, "", -1, 0)
		{ }

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
		}
	}
}
