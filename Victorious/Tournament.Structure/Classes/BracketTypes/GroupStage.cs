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
		protected List<IBracket> Groups
		{ get; set; }
		public int NumberOfGroups
		{ get; set; }
		#endregion

		#region Public Methods
		public override void ResetMatches()
		{
			foreach (IBracket group in Groups)
			{
				group.ResetMatches();
			}
			IsFinished = false;
			IsFinalized = false;
			RecalculateRankings();
		}

		#region Player Methods
		public override void ReplacePlayer(IPlayer _player, int _index)
		{
			if (null == _player)
			{
				throw new ArgumentNullException("_player");
			}
			if (_index < 0 || _index >= Players.Count)
			{
				throw new InvalidIndexException
					("Invalid index; outside Playerlist bounds.");
			}
			if (null == Players[_index])
			{
				throw new NotImplementedException
					("Player to replace doesn't exist!");
			}

			// Find existing Player's group, and replace him inside:
			foreach (IBracket group in Groups)
			{
				int innerGroupIndex = group.Players
					.FindIndex(p => p.Id == this.Players[_index].Id);
				if (innerGroupIndex > -1)
				{
					group.ReplacePlayer(_player, innerGroupIndex);
					break;
				}
			}

			// Replace existing Player in Rankings and Playerlist:
			int rankIndex = Rankings.FindIndex(r => r.Id == Players[_index].Id);
			if (rankIndex > -1)
			{
				Rankings[rankIndex].ReplacePlayerData(_player.Id, _player.Name);
			}
			this.Players[_index] = _player;
		}
		#endregion

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

		#region Accessors
		public override BracketModel GetModel(int _tournamentID = 0)
		{
			throw new NotImplementedException();
#if false
			BracketModel model = base.GetModel(_tournamentID);
			model.NumberOfGroups = this.NumberOfGroups;
			return model;
#endif
		}

		public IBracket GetGroup(int _groupNumber)
		{
			if (null == Groups)
			{
				throw new NullReferenceException
					("No groups exist! Create a bracket first.");
			}
			if (_groupNumber < 1 || _groupNumber > Groups.Count)
			{
				throw new BracketNotFoundException
					("Group not found! Invalid group number.");
			}

			return Groups[_groupNumber - 1];
		}
		public override List<IMatch> GetRound(int _round)
		{
			List<IMatch> ret = new List<IMatch>();
			for (int i = 0; i < Groups.Count; ++i)
			{
				ret.AddRange(GetRound(i + 1, _round));
			}
			return ret;
		}
		public List<IMatch> GetRound(int _groupNumber, int _round)
		{
			if (null == Groups)
			{
				throw new NullReferenceException
					("No groups exist! Create a bracket first.");
			}

			return (GetGroup(_groupNumber).GetRound(_round));
		}
		public override IMatch GetMatch(int _matchNumber)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			return Groups[groupIndex].GetMatch(_matchNumber);
		}
		#endregion
		#endregion

		#region Private Methods
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			UpdateFinishStatus();
			return (new List<MatchModel>());
		}
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			if (PlayerSlot.unspecified == _formerMatchWinnerSlot)
			{
				this.IsFinished = false;
			}
			else
			{
				UpdateFinishStatus();
			}
			return (new List<MatchModel>());
		}
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			UpdateRankings();
		}

		protected override void RecalculateRankings()
		{
			Rankings.Clear();
			foreach (IBracket group in Groups)
			{
				Rankings.AddRange(group.Rankings);
			}
		}
		protected override void UpdateRankings()
		{
			RecalculateRankings();
		}

		protected void UpdateFinishStatus()
		{
			if (Groups.Any(g => !g.IsFinished))
			{
				this.IsFinished = false;
			}
			else
			{
				this.IsFinished = true;
			}
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
