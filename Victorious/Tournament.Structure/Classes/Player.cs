using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
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
		
		public Player(/*uint _id,*/ string _name)
		{
			//id = _id;
			Name = _name;
		}

		// Methods

	}
}
