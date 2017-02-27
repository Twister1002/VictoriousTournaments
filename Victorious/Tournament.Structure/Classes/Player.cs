using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	/// <summary>
	/// Abstract Parent of User and Team,
	/// used to denote slots for either.
	/// </summary>
	public abstract class Player : IPlayer
	{
		#region Variables & Properties
		public int Id
		{ get; protected set; }
		public string Name
		{ get; set; }
		#endregion

		#region Public Methods

		#endregion
	}
}
