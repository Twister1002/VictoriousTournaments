using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public interface IGroupStage : IBracket
	{
		int NumberOfGroups { get; set; }

		IBracket GetGroup(int _groupNumber);
	}
}
