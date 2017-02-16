using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class Player : IPlayer
	{
		// Variables
		private uint id;
		private string username;
		private string firstname;
		private string lastname;
		private string email;

		// Properties
		public uint Id
		{
			get { return id; }
		}
		public string Username
		{
			get { return username; }
			set { username = value; }
		}
		public string Firstname
		{
			get { return firstname; }
			set { firstname = value; }
		}
		public string Lastname
		{
			get { return lastname; }
			set { lastname = value; }
		}
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		// Ctors
		public Player(uint _id)
		{
			id = _id;
			username = "";
			firstname = "";
			lastname = "";
			email = "";
		}

		// Methods

	}
}
