﻿using System;
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
		int PointsScore { get; set; }

		/// <summary>
		/// In the case of a "ranged" rank, returns the minimum.
		/// Ex: Rank 5-8 returns 5.
		/// </summary>
		int Rank { get; set; }
		#endregion

		#region Methods
		void AddToScore(int _matchScore, int _gameScore, int _pointsScore, bool _addition);
		void ResetScore();
		#endregion
	}
}
