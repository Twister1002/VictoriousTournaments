using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public abstract class GroupStage : Bracket, IGroupStage
	{
		#region Variables & Properties
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
			GetMatchData(ref _matchNumber, out groupIndex);
			GameModel gameModel = Groups[groupIndex].AddGame(_matchNumber, _defenderScore, _challengerScore, _winnerSlot);

			UpdateScore(_matchNumber, gameModel, true);
			ApplyWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			GameModel gameModel = Groups[groupIndex].AddGame(_matchNumber, _defenderScore, _challengerScore);
			UpdateRankings();

			IsFinished = true;
			foreach (IBracket group in Groups)
			{
				if (!group.IsFinished)
				{
					IsFinished = false;
					break;
				}
			}

			return gameModel;
		}
		public override GameModel UpdateGame(int _matchNumber, int _gameNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			GameModel gameModel = Groups[groupIndex].UpdateGame(_matchNumber, _gameNumber, _defenderScore, _challengerScore, _winnerSlot);
			ApplyWinEffects(_matchNumber, _winnerSlot);
			return gameModel;
		}
		public override GameModel RemoveLastGame(int _matchNumber)
		{
			ApplyGameRemovalEffects(_matchNumber);
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			GameModel gameModel = Groups[groupIndex].RemoveLastGame(_matchNumber);
			UpdateScore(_matchNumber, gameModel, false);
			return gameModel;
		}
		public override void ResetMatchScore(int _matchNumber)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			Groups[groupIndex].ResetMatchScore(_matchNumber);
			UpdateRankings();

			IsFinished = false;
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
		protected override void UpdateScore(int _matchNumber, GameModel _game, bool _isAddition)
		{
			UpdateRankings();
		}
		protected override void ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			UpdateRankings();

			IsFinished = true;
			foreach (IBracket group in Groups)
			{
				if (!group.IsFinished)
				{
					IsFinished = false;
					break;
				}
			}
		}
		protected override void ApplyGameRemovalEffects(int _matchNumber)
		{
			IsFinished = false;
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
