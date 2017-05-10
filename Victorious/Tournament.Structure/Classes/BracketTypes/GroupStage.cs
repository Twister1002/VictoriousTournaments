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
		// inherits int Id
		// inherits BracketType BracketType
		// inherits bool IsFinalized
		// inherits bool IsFinished
		// inherits List<IPlayer> Players
		// inherits List<IPlayerScore> Rankings
		// inherits Dictionary<int, IMatch> Matches (null)
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		// inherits int NumberOfMatches
		protected List<IBracket> Groups
		{ get; set; }
		public int NumberOfGroups
		{ get; set; }
		#endregion

		#region Public Methods
		public override void RestoreMatch(int _matchNumber, MatchModel _model)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			Groups[groupIndex].RestoreMatch(_matchNumber, _model);
		}
		
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			int groupIndex;
			int fixedMatchNumber = _matchNumber;
			GetMatchData(ref fixedMatchNumber, out groupIndex);

			GameModel gameModel = Groups[groupIndex].AddGame(fixedMatchNumber, _defenderScore, _challengerScore, _winnerSlot);
			//UpdateScore(_matchNumber, gameModel, true);
			UpdateRankings();
			ApplyWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
		public override GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			int groupIndex;
			int fixedMatchNumber = _matchNumber;
			GetMatchData(ref fixedMatchNumber, out groupIndex);

			GameModel gameModel = Groups[groupIndex].UpdateGame(fixedMatchNumber, _gameNumber, _defenderScore, _challengerScore, _winnerSlot);
			UpdateRankings();
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
			UpdateRankings();
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
			UpdateRankings();
			return gameModel;
		}
		public override void SetMatchWinner(int _matchNumber, PlayerSlot _winnerSlot)
		{
			int groupIndex;
			int fixedMatchNumber = _matchNumber;
			GetMatchData(ref fixedMatchNumber, out groupIndex);

			Groups[groupIndex].SetMatchWinner(fixedMatchNumber, _winnerSlot);
			UpdateRankings();
			ApplyWinEffects(_matchNumber, _winnerSlot);
		}
		public override List<GameModel> ResetMatchScore(int _matchNumber)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			bool wasFinished = GetMatch(_matchNumber).IsFinished;

			List<GameModel> modelList = Groups[groupIndex].ResetMatchScore(_matchNumber);
			UpdateFinishStatus();
			UpdateRankings();
			return modelList;
		}

		public IBracket GetGroup(int _groupNumber)
		{
			if (null == Groups)
			{
				throw new NullReferenceException
					("No groups exist! Create a bracket first.");
			}
			if (_groupNumber < 1)
			{
				throw new InvalidIndexException
					("Group number must be greater than 0!");
			}
			if (_groupNumber > Groups.Count)
			{
				throw new BracketNotFoundException
					("Group not found! Invalid group number.");
			}

			return Groups[_groupNumber - 1];
		}
		public override List<IMatch> GetRound(int _round)
		{
			if (null == Groups)
			{
				throw new NullReferenceException
					("No groups exist! Create a bracket first.");
			}

			List<IMatch> ret = new List<IMatch>();
			foreach (IBracket group in Groups)
			{
				ret.AddRange(group.GetRound(_round));
			}
			return ret;
		}
		public override IMatch GetMatch(int _matchNumber)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			return Groups[groupIndex].GetMatch(_matchNumber);
		}
		public override void ResetMatches()
		{
			foreach (IBracket group in Groups)
			{
				group.ResetMatches();
			}
		}
		#endregion

		#region Private Methods
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, PlayerSlot _formerMatchWinnerSlot, bool _resetManualWin = false)
		{
			UpdateRankings();
		}
		protected override void ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			UpdateFinishStatus();
		}
		protected override void ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			if (PlayerSlot.unspecified == _formerMatchWinnerSlot)
			{
				this.IsFinished = false;
				return;
			}
			UpdateFinishStatus();
		}
		protected void UpdateFinishStatus()
		{
			this.IsFinished = true;
			foreach (IBracket group in Groups)
			{
				if (!group.IsFinished)
				{
					this.IsFinished = false;
					return;
				}
			}
		}
		protected override void ResetBracket()
		{
			base.ResetBracket();

			if (null == Groups)
			{
				Groups = new List<IBracket>();
			}
			Groups.Clear();
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
