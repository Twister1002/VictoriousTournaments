#define CHOOSE_PAIRING

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
		//public int Id
		//public BracketType BracketType
		//public bool IsFinalized
		//public bool IsFinished
		//public List<IPlayer> Players
		//public List<IPlayerScore> Rankings
		//public int MaxRounds
		//protected Dictionary<int, Match> Matches
		//public int NumberOfRounds
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue
		//protected int MatchTieValue
		private List<Matchup> Matchups
		{ get; set; }
		private List<int> PlayerByes
		{ get; set; }
		private PairingMethod PairingMethod
		{ get; set; }
		private int ActiveRound
		{ get; set; }
		#endregion

		#region Ctors
		public SwissBracket(List<IPlayer> _players, PairingMethod _pairing = PairingMethod.Slide, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.SWISS;
			PairingMethod = _pairing;

			MaxRounds = _numberOfRounds;
			if (Players.Count > 8 && MaxRounds > (int)(Players.Count * 0.5))
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
			for (int r = 1; r <= NumberOfRounds; ++r)
			{
				List<IMatch> round = GetRound(r);
				if (round.Any(m => m.Players.Contains(null)))
				{
					// This round isn't populated yet. Break out:
					break;
				}

				List<int> playersInRound = new List<int>();
				foreach (IMatch match in round)
				{
					// For each populated Match, add a Matchup to the private list:
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
							Rankings[rIndex].AddMatchOutcome(Outcome.Win, true);
							break;
						}
					}
				}

				ActiveRound = r;
			}

			// Determine the Pairing Method from examining Rnd 1:
			this.PairingMethod = PairingMethod.Slide;
			if (NumberOfMatches > 0 && ActiveRound > 0)
			{
#if CHOOSE_PAIRING
				int firstPlayerIndex = (0 == PlayerByes.Count)
					? 0 : 1;
				IMatch firstPlayerMatch = GetRound(1)
					.Where(m => m.Players.Select(p => p.Id).Contains(this.Players[firstPlayerIndex].Id))
					.First();
				int secondPlayerId = firstPlayerMatch.Players
					.Select(p => p.Id)
					.Where(i => i != this.Players[firstPlayerIndex].Id)
					.First();
				if (Players[1 + firstPlayerIndex].Id == secondPlayerId)
				{
					// Top two seeds are matched up:
					PairingMethod = PairingMethod.Adjacent;
				}
				else if (Players.Last().Id == secondPlayerId)
				{
					// Top seed is paired against bottom seed:
					PairingMethod = PairingMethod.Fold;
				}
#endif
				if (PlayerByes.Count > 0)
				{
					// If we added points for byes, we need to update rankings:
					UpdateRankings();
				}
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();
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

			// Calculate round count:
			NumberOfRounds = MaxRounds;
			if (0 == NumberOfRounds)
			{
				while (Math.Pow(2, NumberOfRounds) < Players.Count)
				{
					++NumberOfRounds;
				}
			}
			// Create empty matches:
			int matchesPerRound = (int)(Players.Count * 0.5);
			for (int r = 1; r <= NumberOfRounds; ++r)
			{
				for (int m = 1; m <= matchesPerRound; ++m)
				{
					Match match = new Match();
					match.SetMatchNumber(++NumberOfMatches);
					match.SetRoundIndex(r);
					match.SetMatchIndex(m);
					match.SetMaxGames(_gamesPerMatch);

					Matches.Add(match.MatchNumber, match);
				}
			}

			// Populate first-round matches:
			AddSwissRound(_gamesPerMatch);
#if false
			int divisionPoint = Players.Count / 2;
			NumberOfRounds = 1;
			for (int m = 0; m < divisionPoint; ++m, ++NumberOfMatches)
			{
				Match match = new Match();
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

		public override void ResetMatches()
		{
			List<MatchModel> clearedMatches = RemoveFutureRounds(0);
			ActiveRound = 1;
			RecalculateRankings();
			OnMatchesModified(clearedMatches);
		}

		public override bool CheckForTies()
		{
			return false;
		}
		public override bool GenerateTiebreakers()
		{
			throw new NotImplementedException
				("Can't create tiebreakers for a Swiss bracket!");
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
		#endregion

		#region Private Methods
		protected override void ResetBracketData()
		{
			base.ResetBracketData();

			ActiveRound = 0;
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

		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			bool makeNewRound = !(GetRound(ActiveRound).Any(m => !m.IsFinished));
			List<MatchModel> modelList = new List<MatchModel>();

			if (makeNewRound)
			{
				// If active round is finished, try to generate a new round.
				if (AddSwissRound(GetMatch(_matchNumber).MaxGames))
				{
					List<IMatch> addedRound = GetRound(ActiveRound);
					foreach (IMatch match in addedRound)
					{
						modelList.Add(GetMatchModel(match));
					}
				}
				else
				{
					// Failed to generate new round.
					// Clean up all the excess stuff:
					List<MatchModel> deletedMatches = new List<MatchModel>();

					List<Match> extraMatches = Matches.Values
						.Where(m => m.RoundIndex > ActiveRound)
						.ToList();
					foreach (Match match in extraMatches)
					{
						deletedMatches.Add(GetMatchModel(match));
						Matches.Remove(match.MatchNumber);
					}

					NumberOfRounds = ActiveRound;
					NumberOfMatches = Matches.Count;
					IsFinished = true;

					OnRoundDeleted(deletedMatches);
				}
			}

			return modelList;
		}
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			IMatch match = GetMatch(_matchNumber);
			List<MatchModel> alteredMatches = new List<MatchModel>();

			if (!(match.IsFinished) &&
				((PlayerSlot.unspecified != _formerMatchWinnerSlot) ||
				(_games.Count + match.Score[0] + match.Score[1] >= match.MaxGames)))
			{
				// This removal invalidates future matches. Delete them:
				alteredMatches = RemoveFutureRounds(match.RoundIndex);
				this.IsFinished = false;
			}
			else
			{
				base.ApplyGameRemovalEffects(_matchNumber, _games, _formerMatchWinnerSlot);
			}

			return alteredMatches;
		}
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			IMatch match = GetMatch(_matchNumber);
			if (!(match.IsFinished))
			{
				bool oldMatchFinished = _oldMatch.IsManualWin;
				if (!oldMatchFinished)
				{
					if (_oldMatch.WinnerID.HasValue && _oldMatch.WinnerID > -1)
					{
						oldMatchFinished = true;
					}
					else if (_oldMatch.DefenderScore + _oldMatch.ChallengerScore >= match.MaxGames)
					{
						oldMatchFinished = true;
					}
				}

				if (oldMatchFinished)
				{
					// We just invalidated future match results.
					// Instead of regular updating, we need to reset/recalculate:
					RecalculateRankings();
				}
			}
			else
			{
				base.UpdateScore(_matchNumber, _games, _isAddition, _oldMatch);
			}
		}

		protected override void RecalculateRankings()
		{
			base.RecalculateRankings();

			if (PlayerByes.Count > 0)
			{
				foreach (int playerId in PlayerByes)
				{
					Rankings.Find(r => r.Id == playerId)
						.AddMatchOutcome(Outcome.Win, true);
				}
				UpdateRankings();
			}
		}

		private bool AddSwissRound(int _gamesPerMatch)
		{
			// Don't try to populate nonexistent rounds:
			if (ActiveRound >= NumberOfRounds)
			{
				return false;
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

				Matchup newMatchup = new Matchup(i, player2index, (1 + ActiveRound));
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

			// Populate the new round of Matches:
			for (int i = 0; i < newRoundMatchups.Count; ++i)
			{
				int matchNum = (i + 1 + (newRoundMatchups.Count * ActiveRound));

				Matches[matchNum].AddPlayer(Players[newRoundMatchups[i].DefenderIndex]);
				Matches[matchNum].AddPlayer(Players[newRoundMatchups[i].ChallengerIndex]);
			}
			Matchups.AddRange(newRoundMatchups);
			++ActiveRound;

			// Now that we have a new legal round...
			// Award points to the player with a bye, if there is one:
			if (PlayerByes.Count > 0)
			{
				int rIndex = Rankings.FindIndex(r => r.Id == PlayerByes[PlayerByes.Count - 1]);
				Rankings[rIndex].AddMatchOutcome(Outcome.Win, true);
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
					Rankings[i].CalculateScore(MatchWinValue, MatchTieValue, 0) < Rankings[prevIndex].CalculateScore(MatchWinValue, MatchTieValue, 0))
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
#if CHOOSE_PAIRING
				List<Matchup> groupYmatchups = CreatePairingsList(_groups[groupNumberY].Count);
#else
				List<Matchup> groupYmatchups = new List<Matchup>();
				int divisionPoint = (int)(_groups[groupNumberY].Count * 0.5);
				for (int i = 0; i < divisionPoint; ++i)
				{
					// Make fake "preferred" matchups for the players in this group, for use later:
					// SLIDE pairing:
					groupYmatchups.Add(new Matchup(i, i + divisionPoint, -1));
				}
#endif
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
#if CHOOSE_PAIRING
						List<Matchup> groupXmatchups = CreatePairingsList(_groups[groupNumberX].Count);
#else
						List<Matchup> groupXmatchups = new List<Matchup>();
						divisionPoint = (int)(_groups[groupNumberX].Count * 0.5);
						for (int i = 0; i < divisionPoint; ++i)
						{
							// Make fake "preferred" matchups for the players in this group:
							// SLIDE pairing:
							groupXmatchups.Add(new Matchup(i, i + divisionPoint, -1));
						}
#endif
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

		private List<Matchup> CreatePairingsList(int _numPlayers)
		{
			List<Matchup> matchups = new List<Matchup>();
			int divisionPoint = (int)(_numPlayers * 0.5);
			switch (PairingMethod)
			{
				case PairingMethod.Adjacent:
					for (int i = 0; i < _numPlayers; i += 2)
					{
						matchups.Add(new Matchup(i, i + 1, -1));
					}
					break;
				case PairingMethod.Fold:
					for (int i = 0; i < divisionPoint; ++i)
					{
						matchups.Add(new Matchup(i, _numPlayers - i, -1));
					}
					break;
				case PairingMethod.Slide:
					for (int i = 0; i < divisionPoint; ++i)
					{
						matchups.Add(new Matchup(i, divisionPoint + i, -1));
					}
					break;
			}

			return matchups;
		}

		private List<MatchModel> RemoveFutureRounds(int _currentRoundIndex)
		{
			List<MatchModel> clearedMatches = new List<MatchModel>();
			List<int> deletedGameIDs = new List<int>();

			// Recursive call on all rounds after this one:
			int nextRoundIndex = 1 + _currentRoundIndex;
			if (nextRoundIndex > NumberOfRounds)
			{
				return clearedMatches;
			}
			clearedMatches.AddRange(RemoveFutureRounds(nextRoundIndex));

			if (_currentRoundIndex >= 0)
			{
				// Reset all Matches in this round:
				List<IMatch> nextRound = GetRound(nextRoundIndex);
				foreach (Match match in nextRound)
				{
					if (!(match.Players.Contains(null)) ||
						match.Games.Count > 0 ||
						match.IsManualWin)
					{
						deletedGameIDs.AddRange(match.Games.Select(g => g.Id));
						if (nextRoundIndex > 1)
						{
							match.ResetPlayers(); // This also resets score.
						}
						else
						{
							// Keep Players if Round = 1:
							match.ResetScore();
						}
						clearedMatches.Add(GetMatchModel(match));
					}
				}
				if (nextRoundIndex > 1)
				{
					// Also delete associated Matchups and Bye:
					Matchups.RemoveAll(m => m.RoundNumber == nextRoundIndex);
					if (PlayerByes.Count == nextRoundIndex)
					{
						// Remove the last Bye in the list:
						PlayerByes.RemoveAt(PlayerByes.Count - 1);
					}

					// Update bracket Properties:
					ActiveRound = _currentRoundIndex;
				}

				OnGamesDeleted(deletedGameIDs);
			}

			return clearedMatches;
		}
		#endregion
	}
}
