using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	/// <summary>
	/// These objects are used in Bracket Ranking systems.
	/// </summary>
	public interface IPlayerScore
	{
		#region Variables & Properties
		int Id { get; }
		string Name { get; }

		[System.Obsolete("use MatchScore&GameScore instead", false)]
		int Score { get; set; }
		int MatchScore { get; set; }
		int GameScore { get; set; }
		int OpponentsScore { get; set; }
		int PointsScore { get; set; }

		/// <summary>
		/// In the case of a "ranged" rank, returns the minimum.
		/// Ex: Rank 5-8 returns 5.
		/// </summary>
		int Rank { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Replace the Player's information.
		/// </summary>
		/// <param name="_id">New player ID</param>
		/// <param name="_name">New player name</param>
		void ReplacePlayerData(int _id, string _name);

		/// <summary>
		/// Add score values to (or subtract from) this PlayerScore.
		/// </summary>
		/// <param name="_matchScore">Match score change</param>
		/// <param name="_gameScore">Game score change</param>
		/// <param name="_pointsScore">Point score change</param>
		/// <param name="_addition">Add or subtract previous values</param>
		void AddToScore(int _matchScore, int _gameScore, int _pointsScore, bool _addition);

		/// <summary>
		/// Reset this object's Score values: match, game, and point scores
		/// </summary>
		void ResetScore();
		#endregion
	}
}
