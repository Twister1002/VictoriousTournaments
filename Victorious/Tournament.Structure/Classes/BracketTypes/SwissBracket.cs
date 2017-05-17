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
		public int DefenderIndex { get; private set; }
		public int ChallengerIndex { get; private set; }
		public int RoundNumber { get; private set; }
		public Matchup(int _defIndex, int _chalIndex, int _roundNumber)
		{
			DefenderIndex = _defIndex;
			ChallengerIndex = _chalIndex;
			RoundNumber = _roundNumber;
		}

		public bool ContainsInt(int _int)
		{
			if (DefenderIndex == _int || ChallengerIndex == _int)
			{
				return true;
			}
			return false;
		}
		public bool HasMatchingPlayers(Matchup _m)
		{
			return (this.ContainsInt(_m.DefenderIndex) &&
				this.ContainsInt(_m.ChallengerIndex));
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
			BracketType = BracketType.SWISS;
			MaxRounds = _numberOfRounds;
			if (Players.Count > 8 && MaxRounds > (Players.Count / 2))
			{
				MaxRounds = Players.Count / 2;
			}
			else if (Players.Count <= 8 && MaxRounds >= Players.Count)
			{
				MaxRounds = Players.Count - 1;
			}
			
			CreateBracket(_maxGamesPerMatch);
		}
		public SwissBracket()
			: this(new List<IPlayer>())
		{ }
		public SwissBracket(BracketModel _model)
			: base(_model)
		{
			//BracketType = BracketType.SWISS;

			for (int r = 1; r <= NumberOfRounds; ++r)
			{
				List<IMatch> round = GetRound(r);
				List<int> playersInRound = new List<int>();

				foreach (IMatch match in round)
				{
					// For each Match, add a Matchup to the private list:
					int defIndex = Players.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Defender].Id);
					int chalIndex = Players.FindIndex(p => p.Id == match.Players[(int)PlayerSlot.Challenger].Id);

					playersInRound.Add(defIndex);
					playersInRound.Add(chalIndex);
					Matchups.Add(new Matchup(defIndex, chalIndex, r));
				}
				if (playersInRound.Count < Players.Count)
				{
					// Find the player with a bye this round (if exists),
					// add him to Byes list, and award points:
					for (int i = 0; i < Players.Count; ++i)
					{
						if (!(playersInRound.Contains(i)))
						{
							PlayerByes.Add(Players[i].Id);
							int rIndex = Rankings.FindIndex(p => p.Id == Players[i].Id);
							Rankings[rIndex].AddToScore(MatchWinValue, 0, 0, true);
							break;
						}
					}
				}
			}

			if (PlayerByes.Count > 0)
			{
				// If we added points for byes, we need to update rankings:
				UpdateRankings();
			}
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

			foreach (IPlayer player in Players)
			{
				Rankings.Add(new PlayerScore(player.Id, player.Name));
			}

			// Create first-round matches:
			AddSwissRound(_gamesPerMatch);
#if false
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
#endif
		}

		public override void ReplacePlayer(IPlayer _player, int _index)
		{
			int oldId = Players[_index].Id;
			base.ReplacePlayer(_player, _index);

			// Replace the old player's ID in the Byes list:
			for (int i = 0; i < PlayerByes.Count; ++i)
			{
				if (PlayerByes[i] == oldId)
				{
					PlayerByes[i] = _player.Id;
				}
			}
		}

		public override GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			throw new NotImplementedException
				("Cannot update games in a Swiss bracket! (yet)");
		}
		public override GameModel RemoveLastGame(int _matchNumber)
		{
			if (GetMatch(_matchNumber).RoundIndex < (NumberOfRounds - 1))
			{
				throw new BracketException
					("Cannot affect matches too far back in Swiss brackets!");
			}
			return base.RemoveLastGame(_matchNumber);
		}
		public override GameModel RemoveGameNumber(int _matchNumber, int _gameNumber)
		{
			if (GetMatch(_matchNumber).RoundIndex < (NumberOfRounds - 1))
			{
				throw new BracketException
					("Cannot affect matches too far back in Swiss brackets!");
			}
			return base.RemoveGameNumber(_matchNumber, _gameNumber);
		}

		public override List<GameModel> ResetMatchScore(int _matchNumber)
		{
			if (GetMatch(_matchNumber).RoundIndex < (NumberOfRounds - 1))
			{
				throw new BracketException
					("Cannot affect matches too far back in Swiss brackets!");
			}
			return base.ResetMatchScore(_matchNumber);
		}

		public override void ResetMatches()
		{
			base.ResetMatches();
			Matchups.Clear();
			PlayerByes.Clear();

			for (int n = 1; n <= NumberOfMatches; ++n)
			{
				// Create Matchups for all first-round Matches:
				if (Matches[n].RoundIndex == 1)
				{
					int defIndex = Players.FindIndex
						(p => p.Id == Matches[n].Players[(int)PlayerSlot.Defender].Id);
					int chalIndex = Players.FindIndex
						(p => p.Id == Matches[n].Players[(int)PlayerSlot.Challenger].Id);
					Matchups.Add(new Matchup(defIndex, chalIndex, 1));
				}
				// Delete any Matches after first round:
				else
				{
					Matches.Remove(n);
				}
			}
			NumberOfMatches = Matches.Count;
			NumberOfRounds = 1;

			if (NumberOfPlayers() % 2 > 0)
			{
				// If Playercount is odd, find the player with a bye:
				for (int p = 0; p < NumberOfPlayers(); ++p)
				{
					bool isByeIndex = true;
					foreach (Matchup matchup in Matchups)
					{
						if (matchup.ContainsInt(p))
						{
							isByeIndex = false;
							break;
						}
					}

					if (isByeIndex)
					{
						// Add player to the Byes list and update his score:
						PlayerByes.Add(Players[p].Id);
						int rIndex = Rankings.FindIndex(r => r.Id == Players[p].Id);
						Rankings[rIndex].AddToScore(MatchWinValue, 0, 0, true);
						UpdateRankings();
						break;
					}
				}
			}
		}
		#endregion

		#region Private Methods
		protected override void ResetBracket()
		{
			base.ResetBracket();

			if (null == Matchups)
			{
				Matchups = new List<Matchup>();
			}
			if (null == PlayerByes)
			{
				PlayerByes = new List<int>();
			}
			Matchups.Clear();
			PlayerByes.Clear();
		}

		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, PlayerSlot _formerMatchWinnerSlot, bool _resetManualWin = false)
		{
			IMatch match = GetMatch(_matchNumber);
			if (!(match.IsFinished) &&
				((PlayerSlot.unspecified != _formerMatchWinnerSlot) ||
				(_games.Count + match.Score[0] + match.Score[1] >= match.MaxGames)))
			{
				// We just invalidated future match results.
				// Instead of regular updating, we need to reset/recalculate:
				RecalculateRankings();
				UpdateRankings();
			}
			else
			{
				base.UpdateScore(_matchNumber, _games, _isAddition, _formerMatchWinnerSlot, _resetManualWin);
			}
		}
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			base.ApplyWinEffects(_matchNumber, _slot);
			if (this.IsFinished)
			{
				// If all matches are finished, try to generate a new round.
				// If successful, reset IsFinished:
				if (AddSwissRound(GetMatch(_matchNumber).MaxGames))
				{
					IsFinished = false;

					List<MatchModel> modelList = new List<MatchModel>();
					List<IMatch> addedRound = GetRound(NumberOfRounds);
					foreach (IMatch match in addedRound)
					{
						modelList.Add(GetMatchModel(match.MatchNumber));
					}
					OnRoundAdded(new BracketEventArgs(modelList));
				}
			}

			return (new List<MatchModel>());
		}
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			IMatch match = GetMatch(_matchNumber);
			if (!(match.IsFinished) &&
				((PlayerSlot.unspecified != _formerMatchWinnerSlot) ||
				(_games.Count + match.Score[0] + match.Score[1] >= match.MaxGames)))
			{
				// This removal invalidates future matches. Delete them:
				List<MatchModel> removedMatches = RemoveFutureRounds(match.RoundIndex);
				this.IsFinished = false;
				OnRoundDeleted(new BracketEventArgs(removedMatches));
			}
			else
			{
				base.ApplyGameRemovalEffects(_matchNumber, _games, _formerMatchWinnerSlot);
			}

			return (new List<MatchModel>());
		}

		private bool AddSwissRound(int _gamesPerMatch)
		{
			// Check against user-input MaxRounds:
			if (MaxRounds > 0)
			{
				if (NumberOfRounds >= MaxRounds)
				{
					return false;
				}
			}
			// No user input: Check against default max rounds:
			else
			{
				int totalRounds = 0;
				while (Math.Pow(2, totalRounds) < Players.Count)
				{
					++totalRounds;
				}
				if (NumberOfRounds >= totalRounds)
				{
					return false;
				}
			}

			// Get possible matchups and heuristics:
			List<List<int>> scoreBrackets = CreateGroups();
			List<int[]> possibleMatches = GetHeuristicEdges(scoreBrackets);

			// Access the Python weight-matching script:
			var engine = IronPython.Hosting.Python.CreateEngine();
			var scope = engine.CreateScope();
			engine.ExecuteFile("mwmatching.py", scope);
			dynamic maxWeightMatching = scope.GetVariable("maxWeightMatching");
			// Determine the next round of matchups:
			IronPython.Runtime.List pySolution = maxWeightMatching(possibleMatches, true);

			// Make sure all the new matchups are Swiss-legal:
			List<Matchup> newRoundMatchups = new List<Matchup>();
			for (int i = 0; i < pySolution.Count; ++i)
			{
				// 'i' = 'player 1 index'
				// 'pySolution[i]' = 'player 2 index'
				int player2index = Convert.ToInt32(pySolution[i]);
				if (i == player2index)
				{
					// Player is matched against himself!
					return false;
				}
				else if (i > player2index)
				{
					continue;
				}

				Matchup newMatchup = new Matchup(i, player2index, (1 + NumberOfRounds));
				foreach (Matchup m in Matchups)
				{
					if (m.HasMatchingPlayers(newMatchup))
					{
						// This is a rematch from a previous round!
						return false;
					}
				}
				foreach (Matchup m in newRoundMatchups)
				{
					if (m.ContainsInt(newMatchup.DefenderIndex) ||
						m.ContainsInt(newMatchup.ChallengerIndex))
					{
						// A player has multiple matchups this round!
						return false;
					}
				}
				newRoundMatchups.Add(newMatchup);
			}
			if (newRoundMatchups.Count < (NumberOfPlayers() / 2))
			{
				// Not enough Matchups were created!
				// (probably means: unable to create enough legal matches)
				return false;
			}

			// Add the new round, and create Match objects:
			++NumberOfRounds;
			int mIndex = 1;
			foreach (Matchup matchup in newRoundMatchups)
			{
				IMatch match = new Match();
				match.SetMatchNumber(++NumberOfMatches);
				match.SetRoundIndex(NumberOfRounds);
				match.SetMatchIndex(mIndex++);
				match.SetMaxGames(_gamesPerMatch);
				match.AddPlayer(Players[matchup.DefenderIndex]);
				match.AddPlayer(Players[matchup.ChallengerIndex]);

				Matches.Add(match.MatchNumber, match);
			}
			Matchups.AddRange(newRoundMatchups);

			// Now that we have a new legal round...
			// Award points to the player with a bye, if there is one:
			if (PlayerByes.Count > 0)
			{
				int rIndex = Rankings.FindIndex(r => r.Id == PlayerByes[PlayerByes.Count - 1]);
				Rankings[rIndex].AddToScore(MatchWinValue, 0, 0, true);
				UpdateRankings();
			}

			return true;
		}
		private List<List<int>> CreateGroups()
		{
			// If playercount is odd, find the top-ranked player to give a Bye
			// (no player should have >1 bye)
			int currentByeId = -1;
			if (Players.Count % 2 > 0)
			{
				foreach (int id in Rankings.Select(r => r.Id))
				{
					if (!PlayerByes.Contains(id))
					{
						PlayerByes.Add(id);
						currentByeId = id;
						break;
					}
				}
			}

			// Create score-brackets (groups) of players, separated by their MatchScore:
			List<List<int>> groups = new List<List<int>>();
			for (int i = 0; i < Rankings.Count; ++i)
			{
				if (PlayerByes.Count > 0 &&
					currentByeId == Rankings[i].Id)
				{
					// This player has a bye this round. Do not add him to groups!
					continue;
				}

				int prevIndex = i - 1;
				if (PlayerByes.Count > 0 &&
					prevIndex >= 0 &&
					currentByeId == Rankings[prevIndex].Id)
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

				// Add player's index in the Players array:
				// this value represents the player in these heuristic methods.
				groups[groups.Count - 1].Add
					(Players.FindIndex(p => p.Id == Rankings[i].Id));
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
			// Make sure each group has an even playercount
			for (int i = 0; i < groups.Count; ++i)
			{
				if (groups[i].Count % 2 > 0)
				{
					// If group.count is odd, take top player out of next group,
					// and shift him up to current group:
					int playerIndex = groups[i + 1][0];
					groups[i].Add(playerIndex);
					groups[i + 1].RemoveAt(0);
				}
			}

			return groups;
		}
		private List<int[]> GetHeuristicEdges(List<List<int>> _groups)
		{
			int numCompetitors = NumberOfPlayers();
			numCompetitors = (0 == numCompetitors % 2)
				? numCompetitors : (numCompetitors - 1);
			// competitors array is used to reference player-indexes when there's a bye:
			int[] competitors = new int[numCompetitors];
			for (int i = 0, pOffset = 0; i < numCompetitors; ++i)
			{
				if (PlayerByes.Count > 0 &&
					PlayerByes[PlayerByes.Count - 1] == Players[i].Id)
				{
					++pOffset;
				}
				competitors[i] = i + pOffset;
			}

			// Create a grid to store h-values for all possible matchups
			int[,] heuristicGrid = new int[numCompetitors, numCompetitors];
			for (int y = 0; y < numCompetitors; ++y)
			{
				int playerYindex = -1, groupNumberY = -1;
				for (int g = 0; g < _groups.Count; ++g)
				{
					playerYindex = _groups[g].FindIndex(num => num == competitors[y]);

					if (playerYindex > -1)
					{
						groupNumberY = g;
						break;
					}
				}

				List<Matchup> groupYmatchups = new List<Matchup>();
				int divisionPoint = Convert.ToInt32(_groups[groupNumberY].Count * 0.5);
				for (int i = 0; i < divisionPoint; ++i)
				{
					// Make fake "preferred" matchups for the players in this group, for use later:
					// SLIDE pairing:
					groupYmatchups.Add(new Matchup(i, i + divisionPoint, -1));
				}
				int matchupYindex = groupYmatchups.FindIndex(m => m.ContainsInt(playerYindex));

				for (int x = 0; x < numCompetitors; ++x)
				{
					if (x == y)
					{
						// Can't play against self! Add heuristic=100M:
						heuristicGrid[y, x] = 100000000;
						continue;
					}

					int playerXindex = -1, groupNumberX = -1;
					for (int g = 0; g < _groups.Count; ++g)
					{
						playerXindex = _groups[g].FindIndex(num => num == competitors[x]);

						if (playerXindex > -1)
						{
							groupNumberX = g;
							break;
						}
					}

					// Add heuristic=20 for each group-line crossed for this matchup:
					heuristicGrid[y, x] = Math.Abs(groupNumberX - groupNumberY) * 20;

					// Add heuristic=1 for each slot *within group* away from preferred matchup:
					int split = 0;
					if (groupNumberY == groupNumberX)
					{
						int idealMatchup = (groupYmatchups[matchupYindex].DefenderIndex == playerYindex)
							? groupYmatchups[matchupYindex].ChallengerIndex
							: groupYmatchups[matchupYindex].DefenderIndex;
						split = Math.Abs(idealMatchup - playerXindex);
					}
					else
					{
						List<Matchup> groupXmatchups = new List<Matchup>();
						divisionPoint = Convert.ToInt32(_groups[groupNumberX].Count * 0.5);
						for (int i = 0; i < divisionPoint; ++i)
						{
							// Make fake "preferred" matchups for the players in this group:
							// SLIDE pairing:
							groupXmatchups.Add(new Matchup(i, i + divisionPoint, -1));
						}

						int matchupXindex, idealMatchup;
						if (groupNumberY > groupNumberX)
						{
							// First player is "adding onto" BACK of groupX
							matchupXindex = groupXmatchups
								.FindIndex(m => m.ContainsInt(_groups[groupNumberX].Count - 1));
							idealMatchup =
								(groupXmatchups[matchupXindex].DefenderIndex == _groups[groupNumberX].Count - 1)
								? groupXmatchups[matchupXindex].ChallengerIndex
								: groupXmatchups[matchupXindex].DefenderIndex;
						}
						else // if (groupNumberY < groupNumberX)
						{
							// PlayerY is "adding onto" FRONT of groupX
							matchupXindex = groupXmatchups.FindIndex(m => m.ContainsInt(0));
							idealMatchup =
								(groupXmatchups[matchupXindex].DefenderIndex == 0)
								? groupXmatchups[matchupXindex].ChallengerIndex
								: groupXmatchups[matchupXindex].DefenderIndex;
						}
						split = Math.Abs(idealMatchup - playerXindex);
					}
					heuristicGrid[y, x] += split;

					// Check for Rematch:
					foreach (Matchup matchup in Matchups)
					{
						if (matchup.ContainsInt(competitors[x]) &&
							matchup.ContainsInt(competitors[y]))
						{
							// Rematch found. Add heuristic=100K:
							heuristicGrid[y, x] += 100000;
							break;
						}
					}
				}
			}

			// Translate the big grid into a list of heuristic "edges":
			// Each node is a player, each edge is a possible matchup
			List<int[]> heuristicGraph = new List<int[]>();

			for (int y = 0; y < (numCompetitors - 1); ++y)
			{
				for (int x = 1; x < numCompetitors; ++x)
				{
					if (y >= x)
					{
						// Skip edges we've found, and self-edges
						continue;
					}

					// Edge: [Defender index, Challenger index, negative heuristic value]
					// The sign is flipped on the h-value for use with the weight-function later
					int[] edge = new int[3]
					{
						competitors[y],
						competitors[x],
						(-1 * (heuristicGrid[y, x] + heuristicGrid[x, y]))
					};
					heuristicGraph.Add(edge);
				}
			}

			return heuristicGraph;
		}

		private List<MatchModel> RemoveFutureRounds(int _currentRoundIndex)
		{
			List<MatchModel> removedMatches = new List<MatchModel>();

			// Recursive call on all rounds after this one:
			int nextRoundIndex = 1 + _currentRoundIndex;
			if (nextRoundIndex > NumberOfRounds)
			{
				return removedMatches;
			}
			removedMatches.AddRange(RemoveFutureRounds(nextRoundIndex));

			// Get & delete all Matches in this round:
			List<int> nextRoundMatchNumbers = GetRound(nextRoundIndex)
				.Select(m => m.MatchNumber)
				.ToList();
			foreach (int n in nextRoundMatchNumbers)
			{
				removedMatches.Add(GetMatchModel(n));
				Matches.Remove(n);
			}
			// Also delete associated Matchups and Bye:
			Matchups.RemoveAll(m => m.RoundNumber == nextRoundIndex);
			if (PlayerByes.Count > 0)
			{
				PlayerByes.RemoveAt(PlayerByes.Count - 1);
			}

			// Update bracket Properties:
			NumberOfMatches = Matches.Count;
			NumberOfRounds = _currentRoundIndex;

			return removedMatches;
		}

		private void RecalculateRankings()
		{
			foreach (IPlayerScore player in Rankings)
			{
				player.ResetScore();
			}

			foreach (int playerId in PlayerByes)
			{
				Rankings.Find(r => r.Id == playerId)
					.AddToScore(MatchWinValue, 0, 0, true);
			}
			foreach (IMatch match in Matches.Values)
			{
				// Case 1: ManualWin:
				if (match.IsManualWin)
				{
					// Just add a MatchWin to winning player:
					Rankings.Find(r => r.Id == match.Players[(int)match.WinnerSlot].Id)
						.AddToScore(MatchWinValue, 0, 0, true);
				}
				// Case 2: Match has at least 1 completed Game:
				else if (match.Games.Count > 0)
				{
					// Total each player's game-wins and points:
					int defGameScore = 0, defPointsScore = 0;
					int chalGameScore = 0, chalPointsScore = 0;
					foreach (IGame game in match.Games)
					{
						defGameScore += game.Score[(int)PlayerSlot.Defender];
						chalGameScore += game.Score[(int)PlayerSlot.Challenger];
						switch (game.WinnerSlot)
						{
							case (PlayerSlot.Defender):
								++defGameScore;
								break;
							case (PlayerSlot.Challenger):
								++chalGameScore;
								break;
						}
					}

					// Add each player's totals to their scores:
					Rankings.Find(r => r.Id == match.Players[(int)PlayerSlot.Defender].Id)
						.AddToScore((PlayerSlot.Defender == match.WinnerSlot) ? MatchWinValue : 0,
						defGameScore, defPointsScore, true);
					Rankings.Find(r => r.Id == match.Players[(int)PlayerSlot.Challenger].Id)
						.AddToScore((PlayerSlot.Challenger == match.WinnerSlot) ? MatchWinValue : 0,
						chalGameScore, chalPointsScore, true);
				}
				// Case 3: Match has no Games played:
				else
				{
					// Do nothing! Take a break!
				}
			}
		}

#if false
		private void CheckAndRemoveNextRound(int _nextRoundIndex)
		{
			// TODO : this should probably be implemented at some point
			if (_nextRoundIndex > NumberOfRounds)
			{
				return;
			}
			CheckAndRemoveNextRound(1 + _nextRoundIndex);

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
		}
#endif
#endregion
	}
}
