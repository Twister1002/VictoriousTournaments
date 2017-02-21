using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class Player : IPlayer
	{
		#region Variables & Properties
		private uint id;
		public uint Id
		{
			get { return id; }
		}
		public string Username
		{ get; set; }
		public string Firstname
		{ get; set; }
		public string Lastname
		{ get; set; }
		public string Email
		{ get; set; }
		#endregion

		// Ctors
		public Player(uint _id)
		{
			id = _id;
			Username = "";
			Firstname = "";
			Lastname = "";
			Email = "";
		}
		public Player(uint _id, string _username, string _firstname, string _lastname, string _email)
		{
			id = _id;
			Username = _username;
			Firstname = _firstname;
			Lastname = _lastname;
			Email = _email;
		}

		// Methods

	}
}
