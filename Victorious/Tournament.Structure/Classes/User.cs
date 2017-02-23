using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class User : Player
	{
		#region Properties
		public string Firstname
		{ get; set; }
		public string Lastname
		{ get; set; }
		public string Email
		{ get; set; }
		#endregion

		// Ctors
		public User(string _name) : base(_name)
		{
			Firstname = "";
			Lastname = "";
			Email = "";
		}

		// Methods

	}
}
