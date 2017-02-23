using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class User : Player
	{
		#region Variables & Properties
		public string Firstname
		{ get; set; }
		public string Lastname
		{ get; set; }
		public string Email
		{ get; set; }
		#endregion

		#region Ctors
		public User(string _name) : base(_name)
		{
			Firstname = "";
			Lastname = "";
			Email = "";
		}
		#endregion

		#region Public Methods

		#endregion
	}
}
