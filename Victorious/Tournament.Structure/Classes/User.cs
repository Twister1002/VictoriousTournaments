﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class User : Player
	{
		#region Variables & Properties
		// inherits int Id
		// inherits string Name
		public string Firstname
		{ get; set; }
		public string Lastname
		{ get; set; }
		public string Email
		{ get; set; }
		#endregion

		#region Ctors
		public User()
			: this(-1, "", "", "", "")
		{ }
		public User(int _id, string _name, string _first, string _last, string _email)
		{
			Id = _id;
			Name = _name;
			Firstname = _first;
			Lastname = _last;
			Email = _email;
		}
		public User(UserModel _u)
		{
			Id = _u.UserID;
			Name = _u.Username;
			Firstname = _u.FirstName;
			Lastname = _u.LastName;
			Email = _u.Email;
		}
		#endregion

		#region Public Methods

		#endregion
	}
}
