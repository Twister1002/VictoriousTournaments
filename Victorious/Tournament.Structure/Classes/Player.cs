using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public abstract class Player : IPlayer
	{
		#region Variables & Properties
		public int Id
		{ get; protected set; }
		public string Name
		{ get; set; }
		#endregion

		#region Public Methods
		public abstract UserModel GetModel();
		#endregion
	}
}
