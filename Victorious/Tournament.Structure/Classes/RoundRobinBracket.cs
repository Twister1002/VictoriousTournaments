using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class RoundRobinBracket : Bracket
	{
		#region Variables & Properties
		// inherits List<IPlayer> Players
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		// inherits int NumberOfMatches
		public int[] Scores
		{ get; private set; }
		#endregion

		#region Ctors
		public RoundRobinBracket(List<IPlayer> _players)
		{
			if (null == _players)
			{
				throw new NullReferenceException();
			}

			Players = _players;
			ResetBracket();
			CreateBracket();
		}
		public RoundRobinBracket(int _numPlayers)
		{
			Players = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				Players.Add(new User());
			}

			ResetBracket();
			CreateBracket();
		}
		public RoundRobinBracket()
			: this(new List<IPlayer>())
		{ }
		public RoundRobinBracket(BracketModel _model)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			ResetBracket();
			if (Players.Count < 2)
			{
				return;
			}
			//if (Players.Count % 2 > 0)
			//{
			//	throw new NotImplementedException();
			//}

			if (null == Scores)
			{
				Scores = new int[Players.Count];
			}
			for (int i = 0; i < Scores.Count(); ++i)
			{
				Scores[i] = 0;
			}

			Matches = new Dictionary<int, IMatch>();
			int totalRounds = (0 == Players.Count % 2)
				? Players.Count - 1 : Players.Count;
			int matchesPerRound = (int)(Players.Count * 0.5);
			for (int r = 0; r < totalRounds; ++r, ++NumberOfRounds)
			{
				for (int m = 0; m < matchesPerRound; ++m, ++NumberOfMatches)
				{
					IMatch match = new Match();
					match.SetMatchNumber(NumberOfMatches + 1);
					match.SetRoundIndex(r + 1);
					match.SetMatchIndex(m + 1);
					match.WinsNeeded = _winsPerMatch;
					match.AddPlayer(m + r);
					match.AddPlayer((Players.Count - 1 - m + r) % Players.Count);

					Matches.Add(match.MatchNumber, match);
				}
			}
		}

		public override void AddWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_matchNumber < 1)
			{
				throw new IndexOutOfRangeException();
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new KeyNotFoundException();
			}

			Matches[_matchNumber].AddWin(_slot);
			if (_slot == PlayerSlot.Defender)
			{
				Scores[Matches[_matchNumber].DefenderIndex()] += 1;
			}
			else if (_slot == PlayerSlot.Challenger)
			{
				Scores[Matches[_matchNumber].ChallengerIndex()] += 1;
			}
		}
		public override void SubtractWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_matchNumber < 1)
			{
				throw new IndexOutOfRangeException();
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new KeyNotFoundException();
			}

			Matches[_matchNumber].SubtractWin(_slot);
			if (_slot == PlayerSlot.Defender)
			{
				Scores[Matches[_matchNumber].DefenderIndex()] -= 1;
			}
			else if (_slot == PlayerSlot.Challenger)
			{
				Scores[Matches[_matchNumber].ChallengerIndex()] -= 1;
			}
		}
		public override void ResetMatchScore(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new IndexOutOfRangeException();
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new KeyNotFoundException();
			}

			Scores[Matches[_matchNumber].DefenderIndex()] -= Matches[_matchNumber].Score[(int)PlayerSlot.Defender];
			Scores[Matches[_matchNumber].ChallengerIndex()] -= Matches[_matchNumber].Score[(int)PlayerSlot.Challenger];
			Matches[_matchNumber].ResetScore();
		}

		public override void ResetBracket()
		{
			base.ResetBracket();

			//if (null != Scores)
			//{
			//	for (int i = 0; i < Scores.Count(); ++i)
			//	{
			//		Scores[i] = 0;
			//	}
			//}
			Scores = null;
		}
		#endregion
	}
}
