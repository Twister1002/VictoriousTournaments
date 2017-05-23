using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public enum PlayerSlot
	{
		unspecified = -1,
		Defender = 0,
		Challenger = 1
	};
	public enum Record
	{
		Wins = 0,
		Losses = 1,
		Ties = 2
	};
	public enum Outcome
	{
		Loss,
		Tie,
		Win
	};

	public class PlayerRecord
	{
		public int Wins
		{ get; private set; }
		public int Ties
		{ get; private set; }
		public int Losses
		{ get; private set; }

		public PlayerRecord()
		{
			Reset();
		}

		public void AddOutcome(Outcome _outcome, bool _isAddition)
		{
			int add = (_isAddition) ? 1 : -1;
			switch (_outcome)
			{
				case Outcome.Win:
					this.Wins += add;
					break;
				case Outcome.Tie:
					this.Ties += add;
					break;
				case Outcome.Loss:
					this.Losses += add;
					break;
			}
		}
		public void Reset()
		{
			Wins = Ties = Losses = 0;
		}
	}
}
