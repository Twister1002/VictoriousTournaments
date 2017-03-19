﻿using System;
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
		public int MaxRounds
		{ get; set; }
		#endregion

		#region Ctors
		public RoundRobinBracket(List<IPlayer> _players, int _numRounds = 0)
		{
			if (null == _players)
			{
				throw new NullReferenceException();
			}

			Players = _players;
			MaxRounds = _numRounds;
			ResetBracket();
			CreateBracket();
		}
		public RoundRobinBracket(int _numPlayers, int _numRounds = 0)
		{
			Players = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				Players.Add(new User());
			}

			MaxRounds = _numRounds;
			ResetBracket();
			CreateBracket();
		}
		public RoundRobinBracket()
			: this(new List<IPlayer>(), 0)
		{ }
		public RoundRobinBracket(BracketModel _model)
		{
			if (null == _model)
			{
				throw new NullReferenceException();
			}

			List<UserModel> userModels = _model.UserSeeds
				.OrderBy(ubs => ubs.Seed)
				.Select(ubs => ubs.User)
				.ToList();
			Players = new List<IPlayer>();
			foreach (UserModel um in userModels)
			{
				Players.Add(new User(um));
			}

			Scores = new int[Players.Count];
			for (int i = 0; i < Scores.Count(); ++i)
			{
				Scores[i] = 0;
			}
			MaxRounds = 0;

			ResetBracket();
			Matches = new Dictionary<int, IMatch>();
			foreach (MatchModel mm in _model.Matches)
			{
				IMatch match = new Match(mm, Players);
				Matches.Add(match.MatchNumber, match);
				++NumberOfMatches;
				if (match.RoundIndex > NumberOfRounds)
				{
					NumberOfRounds = match.RoundIndex;
				}

				Scores[match.DefenderIndex()] += match.Score[(int)PlayerSlot.Defender];
				Scores[match.ChallengerIndex()] += match.Score[(int)PlayerSlot.Challenger];
			}
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
			if (MaxRounds > 0 && MaxRounds < totalRounds)
			{
				// NOTE: this sets a limit on the number of created rounds
				// it does NOT randomize the rounds
				// should that be included in future?
				totalRounds = MaxRounds;
			}
			int matchesPerRound = (int)(Players.Count * 0.5);
			for (int r = 0; r < totalRounds; ++r, ++NumberOfRounds)
			{
				for (int m = 0; m < matchesPerRound; ++m, ++NumberOfMatches)
				{
					IMatch match = new Match();
					match.SetMatchNumber(NumberOfMatches + 1);
					match.SetRoundIndex(r + 1);
					match.SetMatchIndex(m + 1);
					match.SetWinsNeeded(_winsPerMatch);
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

			int defScore = Matches[_matchNumber].Score[(int)PlayerSlot.Defender];
			int chalScore = Matches[_matchNumber].Score[(int)PlayerSlot.Challenger];

			Matches[_matchNumber].ResetScore();
			Scores[Matches[_matchNumber].DefenderIndex()] -= defScore;
			Scores[Matches[_matchNumber].ChallengerIndex()] -= chalScore;
		}

		public override void ResetBracket()
		{
			base.ResetBracket();

			Scores = null;
		}
		#endregion
	}
}
