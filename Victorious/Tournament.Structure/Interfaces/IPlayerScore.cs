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

		/// <summary>
		/// -1 when not applicable. (should not be displayed)
		/// </summary>
		int Score { get; set; }

		/// <summary>
		/// In the case of a "ranged" rank, returns the minimum.
		/// Example: Rank 5-8 returns 5.
		/// </summary>
		int Rank { get; set; }
		#endregion
	}
}
