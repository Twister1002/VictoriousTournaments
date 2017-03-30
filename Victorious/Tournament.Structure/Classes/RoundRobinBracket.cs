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
		// inherits BracketType BracketType
		// inherits bool IsFinished
		// inherits List<IPlayer> Players
		// inherits int[] Rankings
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		// inherits int NumberOfMatches
		public Dictionary<int, uint> Scores
		{ get; private set; }
		public int MaxRounds
		{ get; set; }
		#endregion

		#region Ctors
		public RoundRobinBracket(List<IPlayer> _players, int _numRounds = 0)
		{
			if (null == _players)
			{
				throw new NullReferenceException
					("Playerlist cannot be null!");
			}

			Players = new List<IPlayer>();
			if (_players.Count > 0 && _players[0] is User)
			{
				foreach (IPlayer p in _players)
				{
					Players.Add(new User(p as User));
				}
			}
			else if (_players.Count > 0 && _players[0] is Team)
			{
				foreach (IPlayer p in _players)
				{
					Players.Add(new Team(p as Team));
				}
			}
			else
			{
				Players = _players;
			}

			BracketType = BracketTypeModel.BracketType.ROUNDROBIN;
			MaxRounds = _numRounds;
			ResetBracket();
			CreateBracket();
		}
		public RoundRobinBracket(int _numPlayers, int _numRounds = 0)
		{
			if (_numPlayers < 0)
			{
				throw new ArgumentOutOfRangeException
					("_numPlayers", "Can't have negative players!");
			}

			BracketType = BracketTypeModel.BracketType.ROUNDROBIN;
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
				throw new NullReferenceException
					("Bracket Model cannot be null!");
			}

			BracketType = BracketTypeModel.BracketType.ROUNDROBIN;

			List<UserModel> userModels = _model.UserSeeds
				.OrderBy(ubs => ubs.Seed)
				.Select(ubs => ubs.User)
				.ToList();
			Players = new List<IPlayer>();
			foreach (UserModel model in userModels)
			{
				Players.Add(new User(model));
			}

			MaxRounds = 0;
			ResetBracket();

			Matches = new Dictionary<int, IMatch>();
			foreach (MatchModel mm in _model.Matches)
			{
				IMatch match = new Match(mm);
				Matches.Add(match.MatchNumber, match);
				++NumberOfMatches;
				if (match.RoundIndex > NumberOfRounds)
				{
					NumberOfRounds = match.RoundIndex;
				}

				Scores[match.Players[(int)PlayerSlot.Defender].Id] += match.Score[(int)PlayerSlot.Defender];
				Scores[match.Players[(int)PlayerSlot.Challenger].Id] += match.Score[(int)PlayerSlot.Challenger];
			}

			UpdateRankings();
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

			Matches = new Dictionary<int, IMatch>();
			int totalRounds = (0 == Players.Count % 2)
				? Players.Count - 1 : Players.Count;

			// Randomly choose which rounds to "remove"
			// (only applies if MaxRounds is capped)
			List<int> roundsToRemove = new List<int>();
			if (MaxRounds > 0 && MaxRounds < totalRounds)
			{
				int roundsDiff = totalRounds - MaxRounds;
				Random rng = new Random();
				while (roundsToRemove.Count < roundsDiff)
				{
					int randomRound = rng.Next(totalRounds);
					if (!roundsToRemove.Contains(randomRound))
					{
						roundsToRemove.Add(randomRound);
					}
				}
			}

			// Create all the matchups:
			int matchesPerRound = (int)(Players.Count * 0.5);
			for (int r = 0; r < totalRounds; ++r)
			{
				if (roundsToRemove.Contains(r))
				{
					continue;
				}
				++NumberOfRounds;

				for (int m = 0; m < matchesPerRound; ++m, ++NumberOfMatches)
				{
					IMatch match = new Match();
					match.SetMatchNumber(NumberOfMatches + 1);
					//match.SetRoundIndex(r + 1 - roundsSkipped);
					match.SetRoundIndex(NumberOfRounds);
					match.SetMatchIndex(m + 1);
					match.SetWinsNeeded(_winsPerMatch);
					match.AddPlayer(Players[(m + r) % Players.Count]);
					match.AddPlayer(Players[(Players.Count - 1 - m + r) % Players.Count]);

					Matches.Add(match.MatchNumber, match);
				}
			}
		}

		public override void AddWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}

			Matches[_matchNumber].AddWin(_slot);
			Scores[Matches[_matchNumber].Players[(int)_slot].Id] += 1;

			UpdateRankings();
			IsFinished = true;
			foreach (IMatch match in Matches.Values)
			{
				if (!match.IsFinished)
				{
					IsFinished = false;
					break;
				}
			}
		}
		public override void SubtractWin(int _matchNumber, PlayerSlot _slot)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}

			Matches[_matchNumber].SubtractWin(_slot);
			Scores[Matches[_matchNumber].Players[(int)_slot].Id] -= 1;

			IsFinished = IsFinished && Matches[_matchNumber].IsFinished;
			UpdateRankings();
		}
		public override void ResetMatchScore(int _matchNumber)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			if (!Matches.ContainsKey(_matchNumber))
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}

			uint defScore = Matches[_matchNumber].Score[(int)PlayerSlot.Defender];
			uint chalScore = Matches[_matchNumber].Score[(int)PlayerSlot.Challenger];

			Matches[_matchNumber].ResetScore();
			Scores[Matches[_matchNumber].Players[(int)PlayerSlot.Defender].Id] -= defScore;
			Scores[Matches[_matchNumber].Players[(int)PlayerSlot.Challenger].Id] -= chalScore;

			IsFinished = false;
			UpdateRankings();
		}
		#endregion

		#region Private Methods
		protected override void UpdateRankings()
		{
			Rankings = Players
				.Select(p => p.Id)
				.OrderByDescending(id => Scores[id])
				.ToArray();
		}

		protected override void ResetBracket()
		{
			base.ResetBracket();

			Scores = new Dictionary<int, uint>();
			for (int i = 0; i < Players.Count; ++i)
			{
				Scores.Add(Players[i].Id, 0);
			}
		}
		#endregion
	}
}
