using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IGame
	{
		#region Variables & Properties
		int Id { get; }
		int MatchId { get; }

		/// <summary>
		/// Number of Game within this Match.
		/// </summary>
		int GameNumber { get; set; }

		int[] PlayerIDs { get; set; }

		/// <summary>
		/// Slot of winner: Defender or Challenger.
		/// </summary>
		PlayerSlot WinnerSlot { get; set; }

		int[] Score { get; set; }
		#endregion
	}
}
