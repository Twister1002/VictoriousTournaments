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
			if (null == _name
				|| null == _first
				|| null == _last
				|| null == _email)
			{
				throw new NullReferenceException();
			}

			Id = _id;
			Name = _name;
			Firstname = _first;
			Lastname = _last;
			Email = _email;
		}
		public User(UserModel _u)
		{
			if (null == _u
				|| null == _u.Username
				|| null == _u.FirstName
				|| null == _u.LastName
				|| null == _u.Email)
			{
				throw new NullReferenceException();
			}

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
