using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	/// <summary>
	/// Interface used to fill slots for Users or Teams.
	/// </summary>
	public interface IPlayer
	{
		#region Variables & Properties
		int Id { get; }
		string Name { get; set; }
		#endregion

		#region Public Methods
		/// <summary>
		/// Create a Model for this Player.
		/// </summary>
		/// <returns>UserModel-type object</returns>
		UserModel GetModel();
		#endregion
	}
}
