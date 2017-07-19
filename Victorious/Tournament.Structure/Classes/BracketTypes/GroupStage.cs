using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public abstract class GroupStage : Bracket, IGroupStage
	{
		#region Variables & Properties
		//public int Id
		//public BracketType BracketType
		//public bool IsFinalized
		//public bool IsFinished
		//public List<IPlayer> Players
		//public List<IPlayerScore> Rankings
		//public int AdvancingPlayers
		//public int MaxRounds
		//protected Dictionary<int, Match> Matches = empty
		//public int NumberOfRounds
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue
		//protected int MatchTieValue

		public int NumberOfGroups
		{ get; set; }

		/// <summary>
		/// A list of PlayerScores for each group.
		/// Each group is sorted.
		/// Each PlayerScore is a copied reference from the main Rankings.
		/// These can be referenced with GetGroupRanking().
		/// </summary>
		protected List<List<IPlayerScore>> GroupRankings
		{ get; set; }
		#endregion

		#region Public Methods
		/// <summary>
		/// Verifies this bracket's status is legal.
		/// This is called before allowing play to begin.
		/// </summary>
		/// <returns>true if okay, false if errors</returns>
		public override bool Validate()
		{
			if (false == base.Validate())
			{
				return false;
			}

			if (NumberOfGroups < 2 ||
				NumberOfGroups * 2 > NumberOfPlayers())
			{
				return false;
			}

			return true;
		}

		#region Player Methods
		/// <summary>
		/// Removes a Player from the playerlist,
		/// and replaces him with a given Player.
		/// The new Player inherits the old's seed value.
		/// The removed Player is replaced in all groups, Matches, Games, & Rankings
		/// by the new Player.
		/// May fire MatchesModified event, if updates happen.
		/// If the Player-to-replace's index is invalid, an exception is thrown.
		/// </summary>
		/// <param name="_player">Player to add</param>
		/// <param name="_index">Index (in playerlist) of Player to remove</param>
		public override void ReplacePlayer(IPlayer _player, int _index)
		{
			int? oldPlayerId = Players[_index]?.Id;

			base.ReplacePlayer(_player, _index);

			// After replacing the old player,
			// we also need to find & replace him in the group-specific Rankings:
			foreach (List<IPlayerScore> groupRanks in GroupRankings)
			{
				int i = groupRanks.FindIndex(r => r.Id == oldPlayerId.Value);
				if (i > -1)
				{
					// The player will always only be in one group.
					// After we find it, replace him and break out.
					groupRanks[i].ReplacePlayerData(_player.Id, _player.Name);
					break;
				}
			}
		}
		#endregion
#if false
		#region Match & Game Methods
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			int groupIndex;
			int fixedMatchNumber = _matchNumber;
			GetMatchData(ref fixedMatchNumber, out groupIndex);

			GameModel gameModel = Groups[groupIndex].AddGame(fixedMatchNumber, _defenderScore, _challengerScore, _winnerSlot);
			//UpdateScore(_matchNumber, gameModel, true);
			RecalculateRankings();
			ApplyWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
		public override GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			int groupIndex;
			int fixedMatchNumber = _matchNumber;
			GetMatchData(ref fixedMatchNumber, out groupIndex);

			GameModel gameModel = Groups[groupIndex].UpdateGame(fixedMatchNumber, _gameNumber, _defenderScore, _challengerScore, _winnerSlot);
			RecalculateRankings();
			ApplyWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}

		public override GameModel RemoveLastGame(int _matchNumber)
		{
			PlayerSlot matchWinnerSlot = GetMatch(_matchNumber).WinnerSlot;
			int groupIndex;
			int fixedMatchNumber = _matchNumber;
			GetMatchData(ref fixedMatchNumber, out groupIndex);

			GameModel gameModel = Groups[groupIndex].RemoveLastGame(fixedMatchNumber);
			ApplyGameRemovalEffects(_matchNumber, new List<GameModel>() { gameModel }, matchWinnerSlot);
			//UpdateScore(fixedMatchNumber, new List<GameModel>() { gameModel }, false, matchWinnerSlot);
			RecalculateRankings();
			return gameModel;
		}
		public override GameModel RemoveGameNumber(int _matchNumber, int _gameNumber)
		{
			PlayerSlot matchWinnerSlot = GetMatch(_matchNumber).WinnerSlot;
			int groupIndex;
			int fixedMatchNumber = _matchNumber;
			GetMatchData(ref fixedMatchNumber, out groupIndex);

			GameModel gameModel = Groups[groupIndex].RemoveGameNumber(fixedMatchNumber, _gameNumber);
			ApplyGameRemovalEffects(_matchNumber, new List<GameModel>() { gameModel }, matchWinnerSlot);
			//UpdateScore(fixedMatchNumber, new List<GameModel>() { gameModel }, false, matchWinnerSlot);
			RecalculateRankings();
			return gameModel;
		}

		public override void SetMatchWinner(int _matchNumber, PlayerSlot _winnerSlot)
		{
			int groupIndex;
			int fixedMatchNumber = _matchNumber;
			GetMatchData(ref fixedMatchNumber, out groupIndex);

			Groups[groupIndex].SetMatchWinner(fixedMatchNumber, _winnerSlot);
			RecalculateRankings();
			ApplyWinEffects(_matchNumber, _winnerSlot);
		}

		public override List<GameModel> ResetMatchScore(int _matchNumber)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			bool wasFinished = GetMatch(_matchNumber).IsFinished;

			List<GameModel> modelList = Groups[groupIndex].ResetMatchScore(_matchNumber);
			UpdateFinishStatus();
			RecalculateRankings();
			return modelList;
		}
		#endregion
#endif
		#region Accessors
		/// <summary>
		/// Returns the number of (upper bracket) rounds
		/// in the specified group.
		/// If group number is negative, an exception is thrown.
		/// If group number is too high, returns 0.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <returns>Number of rounds</returns>
		public int NumberOfRoundsInGroup(int _groupNumber)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			List<int> roundNums = Matches.Values
				.Where(m => m.GroupNumber == _groupNumber)
				.Select(m => m.RoundIndex).ToList();

			if (roundNums.Any())
			{
				return roundNums.Max();
			}
			// Else: (_groupNumber is too high)
			return 0;
		}

		public int NumberOfLowerRoundsInGroup(int _groupNumber)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			List<int> roundNums = Matches.Values
				.Where(m => m.GroupNumber == _groupNumber)
				.Select(m => m.RoundIndex).ToList();

			if (roundNums.Any())
			{
				return roundNums.Max();
			}
			// Else: (_groupNumber is too high)
			return 0;
		}
		
		/// <summary>
		/// Creates a Model of this Bracket's current state.
		/// Any contained objects (Players, Matches) are also converted into Models.
		/// </summary>
		/// <param name="_tournamentID">ID of containing Tournament</param>
		/// <returns>Matching BracketModel</returns>
		public override BracketModel GetModel(int _tournamentID)
		{
			BracketModel model = base.GetModel(_tournamentID);
			model.NumberOfGroups = this.NumberOfGroups;

			return model;
		}

		/// <summary>
		/// Gets the ordered Rankings list for the given group.
		/// If the given group number is <1, an exception is thrown.
		/// If the group number is out of range, an empty list is returned.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <returns>Sorted list of IPlayerScore rankings objects</returns>
		public List<IPlayerScore> GetGroupRanking(int _groupNumber)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}
			if (_groupNumber > NumberOfGroups)
			{
				return new List<IPlayerScore>();
			}

			return GroupRankings[_groupNumber - 1];
		}

		/// <summary>
		/// Gets all Matches in a specified round, from the given group.
		/// If the group number is <1, an exception is thrown.
		/// If the round number is <1, an exception is thrown.
		/// If either is otherwise out-of-range, an empty list is returned.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <returns>Sorted list of IMatches in the given group's round</returns>
		public List<IMatch> GetRound(int _groupNumber, int _round)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			return GetRound(_round)
				.Where(m => m.GroupNumber == _groupNumber)
				.ToList();
		}

		/// <summary>
		/// Gets all Matches in a specified round, from the given group.
		/// If the group number is <1, an exception is thrown.
		/// If the round number is <1, an exception is thrown.
		/// If either is otherwise out-of-range, an empty list is returned.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <returns>Sorted list of IMatches in the given group's round</returns>
		public List<IMatch> GetLowerRound(int _groupNumber, int _round)
		{
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number cannot be less than 1!");
			}

			return GetLowerRound(_round)
				.Where(m => m.GroupNumber == _groupNumber)
				.ToList();
		}
		#endregion
		#region Mutators
		/// <summary>
		/// Sets the max number of games per match for one round,
		/// in the specified group.
		/// If Max Games is invalid, an exception is thrown.
		/// If the group number is <1, an exception is thrown.
		/// If the round number is <1, an exception is thrown.
		/// If any matches are already finished, an exception is thrown.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <param name="_maxGamesPerMatch">How many Games each Match may last</param>
		public void SetMaxGamesForWholeRound(int _groupNumber, int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}

			List<IMatch> round = GetRound(_groupNumber, _round);
			if (round.Any(m => m.IsFinished))
			{
				throw new InactiveMatchException
					("One or more matches in this round is already finished!");
			}

			foreach (Match match in round)
			{
				match.SetMaxGames(_maxGamesPerMatch);
			}
		}

		/// <summary>
		/// Sets the max number of games per match for one round,
		/// in the specified group.
		/// If Max Games is invalid, an exception is thrown.
		/// If the group number is <1, an exception is thrown.
		/// If the round number is <1, an exception is thrown.
		/// If any matches are already finished, an exception is thrown.
		/// </summary>
		/// <param name="_groupNumber">1-indexed</param>
		/// <param name="_round">1-indexed</param>
		/// <param name="_maxGamesPerMatch">How many Games each Match may last</param>
		public void SetMaxGamesForWholeLowerRound(int _groupNumber, int _round, int _maxGamesPerMatch)
		{
			if (_maxGamesPerMatch < 1)
			{
				throw new ScoreException
					("Games per match cannot be less than 1!");
			}

			List<IMatch> round = GetLowerRound(_groupNumber, _round);
			if (round.Any(m => m.IsFinished))
			{
				throw new InactiveMatchException
					("One or more matches in this round is already finished!");
			}

			foreach (Match match in round)
			{
				match.SetMaxGames(_maxGamesPerMatch);
			}
		}
		#endregion
		#endregion
	}
}
