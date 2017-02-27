using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	/// <summary>
	/// Interface used to fill slots for Users or Teams.
	/// </summary>
	public interface IPlayer
	{
		#region Variables & Properties
		/// <summary>
		/// Id references the object's DB id.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Name of user/team.
		/// </summary>
		string Name { get; set; }
		#endregion

		#region Public Methods

		#endregion
	}
}
