using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IGroupStage : IBracket
	{
		#region Variables & Properties
		int NumberOfGroups { get; set; }
		#endregion

		#region Public Methods
		/// <summary>
		/// Get one of this stage's groups.
		/// Should only be used for viewing/display.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <returns>Bracket-type object; specified group</returns>
		IBracket GetGroup(int _groupNumber);
		#endregion
	}
}
