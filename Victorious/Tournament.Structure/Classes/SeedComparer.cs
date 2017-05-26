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
			// NULL values sort to the back:
			if (!first.HasValue)
			{
				if (!second.HasValue)
				{
					// Dual null values randomize:
					return RandCompare();
				}

				return 1;
			}
			if (!second.HasValue)
			{
				return -1;
			}
		
			// Default: Both ints have value:
			int compare = (first.Value).CompareTo(second.Value);
			// Duplicate values randomize:
			return (0 == compare)
				? RandCompare() : compare;
		}

		private int RandCompare()
		{
			Random rng = new Random();
			int compare = rng.Next().CompareTo(rng.Next());
			return (0 == compare)
				? RandCompare() : compare;
		}
	}
}
