using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IPlayer
	{
		uint Id { get; }
		string Username { get; set; }
		string Firstname { get; set; }
		string Lastname { get; set; }
		string Email { get; set; }
		//phonenumber
		//createdOn
		//lastLogin
	}
}
