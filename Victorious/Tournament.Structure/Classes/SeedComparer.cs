using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class SeedComparer : IComparer<int?>
	{
		public int Compare(int? first, int? second)
		{
			if (!first.HasValue)
			{
				if (!second.HasValue)
				{
					Random rng = new Random();
					return Compare(rng.Next(), rng.Next());
				}

				return 1;
			}
			if (!second.HasValue)
			{
				return -1;
			}
		
			// Default: Both ints have value:
			return ((first.Value).CompareTo(second.Value));
		}
	}
}
