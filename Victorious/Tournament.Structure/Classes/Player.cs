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
		//private uint id;
		//public uint Id
		//{
		//	get { return id; }
		//}
		public string Name
		{ get; set; }
		#endregion

		#region Ctors
		public Player(/*uint _id,*/ string _name)
		{
			//id = _id;
			Name = _name;
		}
		#endregion

		#region Public Methods

		#endregion
	}
}
