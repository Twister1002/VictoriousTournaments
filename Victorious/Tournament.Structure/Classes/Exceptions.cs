using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class DuplicateObjectException : Exception
	{
		public DuplicateObjectException() { }
	}

	public class InactiveMatchException : Exception
	{
		public InactiveMatchException() { }
	}
}
