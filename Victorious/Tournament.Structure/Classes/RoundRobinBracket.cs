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
		// inherits List<IPlayerScore> Rankings
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		// inherits int NumberOfMatches
		#endregion

		#region Ctors
		public RoundRobinBracket(List<IPlayer> _players, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
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
			MaxRounds = _numberOfRounds;
			ResetBracket();
			CreateBracket();
		}
#if false
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
#endif
		public RoundRobinBracket()
			: this(new List<IPlayer>(), 0)
		{ }
		public RoundRobinBracket(BracketModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			BracketType = BracketTypeModel.BracketType.ROUNDROBIN;
			this.IsFinalized = _model.Finalized;

			List<UserModel> userModels = _model.UserSeeds
				.OrderBy(ubs => ubs.Seed)
				.Select(ubs => ubs.User)
				.ToList();
			this.Players = new List<IPlayer>();
			this.Rankings = new List<IPlayerScore>();
			foreach (UserModel model in userModels)
			{
				Players.Add(new User(model));
				Rankings.Add(new PlayerScore(model.UserID, model.Username, 0, 1));
			}
			
			this.MaxRounds = 0;
			ResetBracket();

			this.Matches = new Dictionary<int, IMatch>();
			foreach (MatchModel mm in _model.Matches)
			{
				IMatch match = new Match(mm);
				Matches.Add(match.MatchNumber, match);
				++NumberOfMatches;
				if (match.RoundIndex > NumberOfRounds)
				{
					this.NumberOfRounds = match.RoundIndex;
				}

				for (int i = 0; i < Rankings.Count; ++i)
				{
					if (Rankings[i].Id == match.Players[(int)PlayerSlot.Defender].Id)
					{
						Rankings[i].Score = Rankings[i].Score + match.Score[(int)PlayerSlot.Defender];
					}
					else if (Rankings[i].Id == match.Players[(int)PlayerSlot.Challenger].Id)
					{
						Rankings[i].Score = Rankings[i].Score + match.Score[(int)PlayerSlot.Challenger];
					}
				}
			}

			UpdateRankings();
			this.IsFinished = true;
			foreach (IMatch match in Matches.Values)
			{
				if (!match.IsFinished)
				{
					this.IsFinished = false;
					break;
				}
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
			Rankings = new List<IPlayerScore>();
			foreach (IPlayer player in Players)
			{
				Rankings.Add(new PlayerScore(player.Id, player.Name, 0, 1));
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
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (Rankings[i].Id == Matches[_matchNumber].Players[(int)_slot].Id)
				{
					Rankings[i].Score = Rankings[i].Score + 1;
					break;
				}
			}
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
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (Rankings[i].Id == Matches[_matchNumber].Players[(int)_slot].Id)
				{
					Rankings[i].Score = Rankings[i].Score - 1;
					break;
				}
			}
			UpdateRankings();

			IsFinished = IsFinished && Matches[_matchNumber].IsFinished;
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

			int defScore = Matches[_matchNumber].Score[(int)PlayerSlot.Defender];
			int chalScore = Matches[_matchNumber].Score[(int)PlayerSlot.Challenger];

			Matches[_matchNumber].ResetScore();
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (Rankings[i].Id == Matches[_matchNumber].Players[(int)PlayerSlot.Defender].Id)
				{
					Rankings[i].Score = Rankings[i].Score - defScore;
				}
				else if (Rankings[i].Id == Matches[_matchNumber].Players[(int)PlayerSlot.Challenger].Id)
				{
					Rankings[i].Score = Rankings[i].Score - chalScore;
				}
			}
			UpdateRankings();

			IsFinished = false;
		}
#endregion

#region Private Methods
		protected override void UpdateRankings()
		{
			Rankings.Sort((first, second) => -1 * (first.Score.CompareTo(second.Score)));
			Rankings[0].Rank = 1;

			int increment = 1;
			for (int i = 1; i < Rankings.Count; ++i)
			{
				if (Rankings[i].Score == Rankings[i - 1].Score)
				{
					++increment;
					Rankings[i].Rank = Rankings[i - 1].Rank;
				}
				else
				{
					Rankings[i].Rank = Rankings[i - 1].Rank + increment;
					increment = 1;
				}
			}
		}
#endregion
	}
}
