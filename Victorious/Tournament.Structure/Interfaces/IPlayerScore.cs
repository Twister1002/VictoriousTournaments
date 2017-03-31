using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IPlayerScore
	{
		string Name { get; }
		int Id { get; }
		int Score { get; set; }
		int Rank { get; set; }
	}
}
