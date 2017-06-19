using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class RoundRobinGroups : GroupStage
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
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue
		//protected int MatchTieValue
		//protected List<IBracket> Groups
		//public int NumberOfGroups
		#endregion

		#region Ctors
		public RoundRobinGroups(List<IPlayer> _players, int _numberOfGroups, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.RRGROUP;
			NumberOfGroups = _numberOfGroups;
			MaxRounds = _numberOfRounds;

			CreateBracket(_maxGamesPerMatch);
		}
		public RoundRobinGroups()
			: this(new List<IPlayer>(), 0, 0)
		{ }
		public RoundRobinGroups(BracketModel _model)
		{
			SetDataFromModel(_model);
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();
			List<IBracket> groups = new List<IBracket>();

			for (int b = 0; b < NumberOfGroups; ++b)
			{
				List<IPlayer> pList = new List<IPlayer>();
				for (int p = 0; (p + b) < Players.Count; p += NumberOfGroups)
				{
					pList.Add(Players[p + b]);
				}

				groups.Add(new RoundRobinBracket(pList, _gamesPerMatch, MaxRounds));
			}
			
			for (int g = 0; g < groups.Count; ++g)
			{
				for (int m = 1; m <= groups[g].NumberOfMatches; ++m)
				{
					++NumberOfMatches;
					IMatch currMatch = groups[g].GetMatch(m);

					if (0 == g)
					{
						Matches.Add(currMatch.MatchNumber, (currMatch as Match));
					}
					else
					{
						Match match = new Match();
						match.SetMaxGames(currMatch.MaxGames);
						match.SetRoundIndex(currMatch.RoundIndex);
						match.SetMatchIndex(currMatch.MatchIndex);
						match.SetMatchNumber(NumberOfMatches);
						match.AddPlayer(currMatch.Players[(int)PlayerSlot.Defender]);
						match.AddPlayer(currMatch.Players[(int)PlayerSlot.Challenger]);

						Matches.Add(match.MatchNumber, match);
					}
					Matches[NumberOfMatches].SetGroupNumber(g + 1);
				}

				Rankings.AddRange(groups[g].Rankings);
			}

			NumberOfRounds = Matches.Values
				.Select(m => m.RoundIndex)
				.Max();
			Rankings.Sort(SortRankingRanks);
		}

		public override bool CheckForTies()
		{
			for (int g = 1; g <= NumberOfGroups; ++g)
			{
				IBracket 
			}


			if (!(Groups.Any(g => g.IsFinished)))
			{
				throw new BracketException
					("No groups are finished yet!");
			}

			foreach (IBracket group in Groups.Where(g => g.IsFinished))
			{
				if (group.CheckForTies())
				{
					return true;
				}
			}
			return false;
		}
		public override bool GenerateTiebreakers()
		{
			if (!(Groups.Any(g => g.IsFinished)))
			{
				throw new BracketException
					("No groups are finished yet!");
			}

			bool addedMatches = false;
			foreach (IBracket group in Groups.Where(g => g.IsFinished))
			{
				addedMatches |= group.GenerateTiebreakers();
			}

			return addedMatches;
		}
		#endregion

		#region Private Methods
		protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			this.IsFinished = Matches.Values
				.All(m => m.IsFinished);
			return (new List<MatchModel>());
		}
		protected override List<MatchModel> ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			this.IsFinished = (IsFinished && GetMatch(_matchNumber).IsFinished);
			return (new List<MatchModel>());
		}
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			UpdateRankings();
		}
		protected override void AddRounds(object _sender, BracketEventArgs _args)
		{
			// Base method relays the RoundsAdded event:
			base.AddRounds(_sender, _args);

			// Update Matches and Rounds:
			NumberOfMatches += _args.UpdatedMatches.Count;
			NumberOfRounds += _args.UpdatedMatches
				.Select(m => m.RoundIndex).Distinct()
				.Count();
			// Get the highest MatchNumber from the new matches:
			int highestNewMatchNum = _args.UpdatedMatches
				.Select(m => m.MatchNumber)
				.Max();

			// Make a list of all the matches with "updated" match numbers:
			List<MatchModel> matchesToUpdate = new List<MatchModel>();
			for (int n = highestNewMatchNum + 1; n <= NumberOfMatches; ++n)
			{
				matchesToUpdate.Add(GetMatchModel(n));
			}

			// Fire event with the updated matches:
			OnMatchesModified(matchesToUpdate);
		}
		#endregion
	}
}
