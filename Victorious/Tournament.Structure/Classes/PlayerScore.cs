using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class PlayerScore : IPlayerScore
	{
		public int Id
		{ get; private set; }
		public string Name
		{ get; private set; }
		public int Score
		{ get; set; }
		public int Rank
		{ get; set; }

		public PlayerScore(int _id, string _name, int _score, int _rank)
		{
			Id = _id;
			Name = _name;
			Score = _score;
			Rank = _rank;
		}
		public PlayerScore()
			: this(0, "", -1, -1)
		{ }
	}
}
