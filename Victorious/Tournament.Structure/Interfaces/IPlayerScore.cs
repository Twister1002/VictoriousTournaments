using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IPlayerScore
	{
		string Name { get; }
		int Id { get; }

		/// <summary>
		/// -1 when not applicable. (should not be displayed)
		/// </summary>
		int Score { get; set; }

		/// <summary>
		/// In the case of a "ranged" rank, returns the minimum.
		/// Example: Rank 5-8 returns 5.
		/// </summary>
		int Rank { get; set; }
	}
}
