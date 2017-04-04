using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class RoundRobinGroups : Bracket
	{
		#region Variables & Properties
		// inherits BracketType BracketType
		// inherits bool IsFinished
		// inherits List<IPlayer> Players
		// inherits List<IPlayerScore> Rankings
		// inherits Dictionary<int, IMatch> Matches (null)
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches (null)
		// inherits int NumberOfLowerRounds (0)
		// inherits IMatch GrandFinal (null)
		// inherits int NumberOfMatches
		private List<IBracket> Groups
		{ get; set; }
		public int NumberOfGroups
		{ get; set; }
		#endregion

		#region Ctors
		public RoundRobinGroups(List<IPlayer> _players, int _numberOfGroups, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}
			if (_numberOfGroups < 2)
			{
				throw new ArgumentOutOfRangeException
					("_numberOfGroups", "Must have more than 1 group!");
			}
			if (_numberOfGroups > (_players.Count / 2))
			{
				throw new ArgumentOutOfRangeException
					("_numberOfGroups", "Must have at least two players per group!");
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

			//BracketType = BracketTypeModel.BracketType.RRGROUP;
			NumberOfGroups = _numberOfGroups;
			MaxRounds = _numberOfRounds;
			ResetBracket();
			CreateBracket();
		}
#if false
		public RoundRobinGroups(int _numberOfPlayers, int _numberOfGroups)
		{
			if (_numberOfPlayers < 0)
			{
				throw new ArgumentOutOfRangeException
					("_numberOfPlayers", "Can't have negative players!");
			}
			if (_numberOfGroups < 2)
			{
				throw new ArgumentOutOfRangeException
					("_numberOfGroups", "Must have more than 1 group!");
			}
			if (_numberOfGroups > (_numberOfPlayers / 2))
			{
				throw new ArgumentOutOfRangeException
					("_numberOfGroups", "Must have at least two players per group!");
			}

			Players = new List<IPlayer>();
			for (int i = 0; i < _numberOfPlayers; ++i)
			{
				Players.Add(new User());
			}

			//BracketType = BracketTypeModel.BracketType.RRGROUP;
			NumberOfGroups = _numberOfGroups;
			ResetBracket();
			CreateBracket();
		}
#endif
		public RoundRobinGroups()
			: this(new List<IPlayer>(), 0, 0)
		{ }
		public RoundRobinGroups(BracketModel _model)
		{
			if (null == _model)
			{
				throw new NullReferenceException
					("Bracket Model canot be null!");
			}

			//BracketType = BracketTypeModel.BracketType.RRGROUP;

			List<UserModel> userModels = _model.UserSeeds
				.OrderBy(ubs => ubs.Seed)
				.Select(ubs => ubs.User)
				.ToList();
			Players = new List<IPlayer>();
			foreach (UserModel model in userModels)
			{
				Players.Add(new User(model));
			}

			//NumberOfGroups = _model.NumberOfGroups;
			MaxRounds = 0; // ********************
			ResetBracket();
			CreateBracket();

			// Find & update every Match:
			foreach (MatchModel model in _model.Matches)
			{
				foreach (IBracket group in Groups)
				{
					if (group.Players.Select(p => p.Id).ToList()
						.Contains((int)(model.DefenderID)))
					{
						// Update Match's score:
						group.GetMatch(model.MatchNumber)
							.SetWinsNeeded((ushort)(model.WinsNeeded));

						if (model.DefenderScore < model.ChallengerScore)
						{
							for (int i = 0; i < model.DefenderScore; ++i)
							{
								group.AddWin(model.MatchNumber, PlayerSlot.Defender);
							}
							for (int i = 0; i < model.ChallengerScore; ++i)
							{
								group.AddWin(model.MatchNumber, PlayerSlot.Challenger);
							}
						}
						else
						{
							for (int i = 0; i < model.ChallengerScore; ++i)
							{
								group.AddWin(model.MatchNumber, PlayerSlot.Challenger);
							}
							for (int i = 0; i < model.DefenderScore; ++i)
							{
								group.AddWin(model.MatchNumber, PlayerSlot.Defender);
							}
						}

						break;
					}
				}
			}

			// Update the rankings:
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
#endregion

#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			ResetBracket();
			if (Players.Count < 2 || 
				NumberOfGroups > (Players.Count / 2) || NumberOfGroups < 2)
			{
				return;
			}

			Groups = new List<IBracket>();
			for (int b = 0; b < NumberOfGroups; ++b)
			{
				List<IPlayer> pList = new List<IPlayer>();
				for (int p = 0; (p + b) < Players.Count; p += NumberOfGroups)
				{
					pList.Add(Players[p + b]);
				}

				Groups.Add(new RoundRobinBracket(pList, MaxRounds));
			}

			Rankings = new List<IPlayerScore>();
			foreach (IBracket group in Groups)
			{
				NumberOfMatches += group.NumberOfMatches;
				NumberOfRounds = (NumberOfRounds < group.NumberOfRounds)
					? group.NumberOfRounds
					: NumberOfRounds;
				Rankings.AddRange(group.Rankings);
			}
		}

		public override void AddWin(int _matchNumber, PlayerSlot _slot)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			Groups[groupIndex].AddWin(_matchNumber, _slot);
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
		public override void SubtractWin(int _matchNumber, PlayerSlot _slot)
		{
			int groupIndex;
			GetMatchData(ref _matchNumber, out groupIndex);
			Groups[groupIndex].SubtractWin(_matchNumber, _slot);
			UpdateRankings();

			IsFinished = (IsFinished && Groups[groupIndex].IsFinished);
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
#endregion

#region Private Methods
		protected override void UpdateRankings()
		{
			Rankings.Clear();
			foreach (IBracket group in Groups)
			{
				Rankings.AddRange(group.Rankings);
			}
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

		protected override void ResetBracket()
		{
			base.ResetBracket();

			Groups = null;
		}

		private void GetMatchData(ref int _matchNumber, out int _groupIndex)
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
