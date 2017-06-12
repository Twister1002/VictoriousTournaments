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
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			List<TournamentUserModel> userModels = _model.TournamentUsersBrackets
				.OrderBy(tubm => tubm.Seed, new SeedComparer())
				.Select(tubm => tubm.TournamentUser)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (TournamentUserModel userModel in userModels)
			{
				Players.Add(new Player(userModel));
			}

			this.Id = _model.BracketID;
			this.BracketType = _model.BracketType.Type;
			this.IsFinalized = _model.Finalized;
			this.NumberOfGroups = _model.NumberOfGroups;
			this.MaxRounds = _model.MaxRounds;

			CreateBracket();
			// Find & update every Match:
			foreach (MatchModel matchModel in _model.Matches)
			{
				GetInternalMatch(matchModel.MatchNumber)
					.SetFromModel(matchModel);
			}

			// Update the rankings:
			RecalculateRankings();
			UpdateFinishStatus();
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
			if (Players.Count < 2 ||
				NumberOfGroups < 2 ||
				NumberOfGroups > (int)(Players.Count * 0.5))
			{
				throw new BracketException
					("Not enough Players per Group!");
			}

			for (int b = 0; b < NumberOfGroups; ++b)
			{
				List<IPlayer> pList = new List<IPlayer>();
				for (int p = 0; (p + b) < Players.Count; p += NumberOfGroups)
				{
					pList.Add(Players[p + b]);
				}

				Groups.Add(new RoundRobinBracket(pList, _gamesPerMatch, MaxRounds));
			}
			SubscribeToGroupEvents();

			foreach (IBracket group in Groups)
			{
				NumberOfMatches += group.NumberOfMatches;
				NumberOfRounds = Math.Max(this.NumberOfRounds, group.NumberOfRounds);
				Rankings.AddRange(group.Rankings);
			}
			Rankings.Sort(SortRankingScores);
		}

		public override bool CheckForTies()
		{
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
		protected override void AddRounds(object _sender, BracketEventArgs _args)
		{
			// Base method relays the RoundsAdded event:
			base.AddRounds(_sender, _args);

			// Get the highest MatchNumber from the new matches:
			int highestNewMatchNum = _args.UpdatedMatches
				.Select(m => m.MatchNumber)
				.OrderByDescending(n => n)
				.First();
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
