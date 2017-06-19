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
		protected List<List<IPlayerScore>> GroupRankings
		{ get; set; }
		#endregion

		#region Public Methods
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
		public override void ResetMatches()
		{
			base.ResetMatches();

			Rankings.Clear();
			foreach (List<IPlayerScore> group in GroupRankings)
			{
				foreach (IPlayerScore playerScore in group)
				{
					playerScore.Rank = 1;
					playerScore.ResetScore();
				}

				Rankings.AddRange(group);
			}
		}

		#region Player Methods
		public override void ReplacePlayer(IPlayer _player, int _index)
		{
			int? oldPlayerId = Players[_index]?.Id;

			base.ReplacePlayer(_player, _index);

			foreach (List<IPlayerScore> groupRanks in GroupRankings)
			{
				int i = groupRanks.FindIndex(r => r.Id == oldPlayerId.Value);
				if (i > -1)
				{
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
		public override BracketModel GetModel(int _tournamentID = 0)
		{
			BracketModel model = base.GetModel(_tournamentID);
			model.NumberOfGroups = this.NumberOfGroups;

			return model;
		}

		public List<List<IMatch>> GetGroup(int _groupNumber)
		{
			if (0 == (Matches?.Count ?? 0))
			{
				throw new NullReferenceException
					("No groups exist! Create a bracket first.");
			}
			if (_groupNumber < 1 || _groupNumber > NumberOfGroups)
			{
				throw new BracketNotFoundException
					("Group not found! Invalid group number.");
			}

			List<List<IMatch>> ret = new List<List<IMatch>>();

			List<Match> group = Matches.Values
				.Where(m => m.GroupNumber == _groupNumber)
				//.OrderBy(m => m.MatchNumber)
				.ToList();
			for (int r = 1; r <= NumberOfRounds; ++r)
			{
				List<IMatch> round = group
					.Where(m => m.RoundIndex == r)
					//.OrderBy(m => m.MatchNumber)
					.Cast<IMatch>()
					.ToList();

				if (0 == round.Count)
				{
					break;
				}
				ret.Add(round);
			}

			return ret;
		}
		public List<IMatch> GetRound(int _groupNumber, int _round)
		{
			List<List<IMatch>> group = GetGroup(_groupNumber);

			if (_round < 1 || _round > group.Count)
			{
				throw new InvalidIndexException
					("Round Number invalid!");
			}

			return group[_round - 1];
		}
		#endregion
		#endregion

		#region Private Methods
		protected override void SetDataFromModel(BracketModel _model)
		{
			base.SetDataFromModel(_model);
			this.NumberOfGroups = _model.NumberOfGroups;

			foreach (MatchModel matchModel in _model.Matches)
			{
				Matches.Add(matchModel.MatchNumber, new Match(matchModel));
			}

			this.IsFinished = Matches.Values
				.All(m => m.IsFinished);
			RecalculateRankings();

			if (this.IsFinalized && false == Validate())
			{
				throw new BracketValidationException
					("Bracket is Finalized but not Valid!");
			}
		}


		protected override void RecalculateRankings()
		{
			Rankings.Clear();
			foreach (IBracket group in Groups)
			{
				Rankings.AddRange(group.Rankings);
			}
			Rankings.Sort(SortRankingRanks);
		}
		protected override void UpdateRankings()
		{
			RecalculateRankings();
		}

		protected void UpdateFinishStatus()
		{
			this.IsFinished = Groups.All(g => g.IsFinished);
		}

		protected override void ResetBracketData()
		{
			base.ResetBracketData();

			if (null == Groups)
			{
				Groups = new List<IBracket>();
			}
			Groups.Clear();
		}

		protected override Match GetInternalMatch(int _matchNumber)
		{
			return (GetMatch(_matchNumber) as Match);
		}
		protected void GetMatchData(ref int _matchNumber, out int _groupIndex)
		{
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match Number cannot be less than 1!");
			}

			for (_groupIndex = 0; _groupIndex < Groups.Count; ++_groupIndex)
			{
				if (_matchNumber < 1)
				{
					break;
				}
				if (_matchNumber <= Groups[_groupIndex].NumberOfMatches)
				{
					return;
				}
				else
				{
					_matchNumber -= Groups[_groupIndex].NumberOfMatches;
				}
			}

			throw new MatchNotFoundException
				("Match not found; match number may be invalid.");
		}
		#endregion
	}
}
