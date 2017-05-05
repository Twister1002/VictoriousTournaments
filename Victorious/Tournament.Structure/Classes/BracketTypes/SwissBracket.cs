using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	internal struct Matchup
	{
		public int DefenderId { get; private set; }
		public int ChallengerId { get; private set; }
		public Matchup(int _defId, int _chalId)
		{
			DefenderId = _defId;
			ChallengerId = _chalId;
		}
		public bool ContainsInt(int _int)
		{
			if (DefenderId == _int || ChallengerId == _int)
			{
				return true;
			}
			return false;
		}
	}

	public class SwissBracket : RoundRobinBracket
	{
		#region Variables & Properties
		// inherits int Id
		// inherits BracketType BracketType
		// inherits bool IsFinalized
		// inherits bool IsFinished
		// inherits List<IPlayer> Players
		// inherits List<IPlayerScore> Rankings
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		// inherits int NumberOfMatches
		private List<Matchup> Matchups
		{ get; set; }
		private List<int> PlayerByes
		{ get; set; }
		#endregion

		#region Ctors
		public SwissBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
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

			Id = 0;
			//BracketType = BracketTypeModel.BracketType.SWISS;
			MaxRounds = _numberOfRounds;
			ResetBracket();
			CreateBracket(_maxGamesPerMatch);
		}
		public SwissBracket()
			: this(new List<IPlayer>())
		{ }
		public SwissBracket(BracketModel _model)
			: base(_model)
		{
			//BracketType = BracketTypeModel.BracketType.SWISS;
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracket();
			if (_gamesPerMatch < 1)
			{
				throw new BracketException
					("Games Per Match must be positive!");
			}
			if (Players.Count < 2)
			{
				return;
			}
			if (0 != Players.Count % 2)
			{
				throw new BracketException
					("Swiss brackets must have an even number of players!");
			}

			foreach (IPlayer player in Players)
			{
				Rankings.Add(new PlayerScore(player.Id, player.Name, 0, 1));
			}

			// Create first-round matches:
			int divisionPoint = Players.Count / 2;
			NumberOfRounds = 1;
			for (int m = 0; m < divisionPoint; ++m, ++NumberOfMatches)
			{
				IMatch match = new Match();
				match.SetMatchNumber(NumberOfMatches + 1);
				match.SetRoundIndex(NumberOfRounds);
				match.SetMatchIndex(m + 1);
				match.SetMaxGames(_gamesPerMatch);
				match.AddPlayer(Players[m]);
				match.AddPlayer(Players[m + divisionPoint]);

				Matches.Add(match.MatchNumber, match);
			}
		}

#if false
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			GameModel gameModel = base.AddGame(_matchNumber, _defenderScore, _challengerScore, _winnerSlot);

			if (IsFinished)
			{
				IsFinished = !(AddNewRound(GetMatch(_matchNumber).MaxGames));
			}
			return gameModel;
		}
		public override void RemoveLastGame(int _matchNumber)
		{
			base.RemoveLastGame(_matchNumber);

			IMatch currMatch = GetMatch(_matchNumber);
			if (!currMatch.IsFinished)
			{
				CheckAndRemoveNextRound(1 + currMatch.RoundIndex);
			}
		}
		public override void ResetMatchScore(int _matchNumber)
		{
			base.ResetMatchScore(_matchNumber);

			CheckAndRemoveNextRound(1 + GetMatch(_matchNumber).RoundIndex);
		}
#endif
		#endregion

		#region Private Methods
		protected override void ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			base.ApplyWinEffects(_matchNumber, _slot);
			if (this.IsFinished)
			{
				IsFinished = !(AddNewRound(GetMatch(_matchNumber).MaxGames));
			}
		}
		protected override void ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			IMatch match = GetMatch(_matchNumber);
			if (!(match.IsFinished) &&
				((PlayerSlot.unspecified != _formerMatchWinnerSlot) ||
				(_games.Count + match.Score[0] + match.Score[1] >= match.MaxGames)))
			{
				CheckAndRemoveNextRound(1 + GetMatch(_matchNumber).RoundIndex);
			}
			base.ApplyGameRemovalEffects(_matchNumber, _games, _formerMatchWinnerSlot);
		}

		private List<List<int>> CreateGroups()
		{
			// If playercount is odd, find the top-ranked player to give a Bye
			if (Players.Count % 2 > 0)
			{
				foreach (int id in Rankings.Select(r => r.Id))
				{
					if (!PlayerByes.Contains(id))
					{
						PlayerByes.Add(id);
						// Add a "match win" to the player with a bye:
						int index = Rankings.FindIndex(r => r.Id == id);
						Rankings[index].AddToScore(MatchWinValue, 0, 0, true);
						break;
					}
				}
			}
			int byeIndex = PlayerByes.Count - 1;

			// Create score-brackets (groups) of players, separated by their MatchScore:
			List<List<int>> groups = new List<List<int>>();
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (PlayerByes.Count > 0 &&
					PlayerByes[PlayerByes.Count - 1] == Rankings[i].Id)
				{
					// This player has a bye this round. Do not add him to groups!
					continue;
				}

				int prevIndex = i - 1;
				if (PlayerByes.Count > 0 &&
					prevIndex >= 0 &&
					PlayerByes[PlayerByes.Count - 1] == Rankings[prevIndex].Id)
				{
					// Prev player has a bye this round. Decrement the index:
					--prevIndex;
				}
				if (prevIndex < 0 ||
					Rankings[i].MatchScore < Rankings[prevIndex].MatchScore)
				{
					// New MatchPoints value = Add a new group:
					groups.Add(new List<int>());
				}
				groups[groups.Count - 1].Add(Rankings[i].Id);
			}

			// Sort the players within each group according to their accumulated opponents' scores
#if false
			foreach (List<int> group in groups)
			{
				group.Sort(
					(first, second) =>
					Rankings.FindIndex(r => r.Id == first)
					.CompareTo(Rankings.FindIndex(r => r.Id == second)));
			}
#endif
			// Make sure each group has an event playercount
			for (int i = 0; i < groups.Count; ++i)
			{
				if (groups[i].Count % 2 > 0)
				{
					// If group.count is odd, take top player out of next group,
					// and shift him up to current group:
					int id = groups[i + 1][0];
					groups[i].Add(id);
					groups[i + 1].RemoveAt(0);
				}
			}

			return groups;
		}

		private int[,] CreateHeuristicGrid(List<List<int>> _groups)
		{
			int numCompetitors = NumberOfPlayers();
			numCompetitors = (0 == numCompetitors % 2)
				? numCompetitors : (numCompetitors - 1);
			int[,] grid = new int[numCompetitors, numCompetitors];

			for (int y = 0; y < numCompetitors; ++y)
			{
				int playerYid = -1, groupNumberY = 0;
				for (int g = 0, playerNum = 0; g < _groups.Count; ++g)
				{
					if (playerNum + _groups[g].Count < y)
					{
						playerNum += _groups[g].Count;
					}
					else
					{
						groupNumberY = g + 1;
						playerYid = _groups[g][y - playerNum];
						break;
					}
				}

				List<Matchup> matchupList = new List<Matchup>();
				for (int i = 0; i < (_groups[groupNumberY].Count - 1) - i; ++i)
				{
					// Make fake matchups for the players in this group, for use later:
					matchupList.Add(new Matchup(i, _groups[groupNumberY].Count - 1 - i));
				}
				int matchupYindex = matchupList.FindIndex(m => m.ContainsInt(playerYid));

				for (int x = 0; x < numCompetitors; ++x)
				{
					if (x == y)
					{
						// Can't play against self! Add heuristic=100M:
						grid[y, x] = 100000000;
						continue;
					}

					int playerXid = -1, groupNumberX = 0;
					for (int g = 0, playerNum = 0; g < _groups.Count; ++g)
					{
						if (playerNum + _groups[g].Count < x)
						{
							playerNum += _groups[g].Count;
						}
						else
						{
							groupNumberX = g + 1;
							playerXid = _groups[g][x - playerNum];
							break;
						}
					}

					// Add heuristic=20 for each group-line crossed for this matchup:
					grid[y, x] = Math.Abs(groupNumberX - groupNumberY) * 20;

					// Add heuristic=1 for each slot *within group* away from preferred matchup:
					int split = 0;
					if (groupNumberY > groupNumberX)
					{
						split = _groups[groupNumberX].FindIndex(id => id == playerXid);
					}
					else if (groupNumberY < groupNumberX)
					{
						split = _groups[groupNumberX].FindIndex(id => id == playerXid);
						split = (_groups[groupNumberX].Count - 1) - split;
					}
					else // groupNumberY == groupNumberX
					{
						int idealMatchup = (matchupList[matchupYindex].DefenderId == playerYid)
							? matchupList[matchupYindex].ChallengerId
							: matchupList[matchupYindex].DefenderId;
						split = Math.Abs(idealMatchup - x);
					}
					grid[y, x] += split;

					// Check for Rematch:
					foreach (IMatch match in Matches.Values)
					{
						if ((match.Players[0].Id == playerXid && match.Players[1].Id == playerYid) ||
							(match.Players[0].Id == playerYid && match.Players[1].Id == playerXid))
						{
							// Rematch found. Add heuristic=100K:
							grid[y, x] += 100000;
							break;
						}
					}
				}
			}

			return grid;
		}

		private bool AddNewRound(int _gamesPerMatch)
		{
			if (MaxRounds > 0 && NumberOfRounds >= MaxRounds)
			{
				return false;
			}
			int totalRounds = 0;
			while (Math.Pow(2, totalRounds) < Players.Count)
			{
				++totalRounds;
			}
			if (NumberOfRounds >= totalRounds)
			{
				return false;
			}

			++NumberOfRounds;
			int mIndex = 1;
			int divisionPoint = Players.Count / 2;
			if (2 == NumberOfRounds && 0 == divisionPoint % 2)
			{
				int secondDivision = divisionPoint / 2;
				for (int i = 0; i < secondDivision; ++i)
				{
					for (int j = 0; j <= divisionPoint; j += divisionPoint)
					{
						IMatch match = new Match();
						match.SetMatchNumber(++NumberOfMatches);
						match.SetRoundIndex(NumberOfRounds);
						match.SetMatchIndex(mIndex++);
						match.SetMaxGames(_gamesPerMatch);
						match.AddPlayer(Players.Find(p => p.Id == Rankings[i + j].Id));
						match.AddPlayer(Players.Find(p => p.Id == Rankings[i + j + secondDivision].Id));

						Matches.Add(match.MatchNumber, match);
					}
				}
			}
			else
			{
				for (int i = 0; i < Players.Count; i += 2)
				{
					IMatch match = new Match();
					match.SetMatchNumber(++NumberOfMatches);
					match.SetRoundIndex(NumberOfRounds);
					match.SetMatchIndex(mIndex++);
					match.SetMaxGames(_gamesPerMatch);
					match.AddPlayer(Players.Find(p => p.Id == Rankings[i].Id));
					match.AddPlayer(Players.Find(p => p.Id == Rankings[i + 1].Id));

					Matches.Add(match.MatchNumber, match);
				}
			}

			return true;
		}

		private void CheckAndRemoveNextRound(int _nextRoundIndex)
		{
#if false
			if (_nextRoundIndex > NumberOfRounds)
			{
				return;
			}

			bool deleteNextRound = true;
			List<IMatch> nextRound = GetRound(_nextRoundIndex);
			foreach (IMatch match in nextRound)
			{
				if (match.Games.Count > 0)
				{
					deleteNextRound = false;
					break;
				}
			}

			if (deleteNextRound)
			{
				foreach (IMatch match in nextRound)
				{
					Matches.Remove(match.MatchNumber);
					--NumberOfMatches;
				}
				--NumberOfRounds;
			}
#endif
		}
		#endregion
	}
}
