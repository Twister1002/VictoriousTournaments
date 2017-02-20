using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClassData;

namespace ConsoleTester
{
	class Program
	{
		static void Main(string[] args)
		{
			Tournament t = new Tournament(8);
			t.PrintInfo();

			Console.ReadLine();
		}
	}
}
