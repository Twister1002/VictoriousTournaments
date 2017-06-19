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

		List<List<IMatch>> GetGroup(int _groupNumber);

		/// <summary>
		/// Gets all Matches in specified round, from specified group.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <returns>List of Matches in the round</returns>
		List<IMatch> GetRound(int _groupNumber, int _round);
		#endregion
	}
}
