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
		// inherits List<IBracket> Groups
		// inherits int NumberOfGroups
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
			BracketType = BracketType.RRGROUP;
			NumberOfGroups = _numberOfGroups;
			MaxRounds = _numberOfRounds;
			ResetBracket();
			CreateBracket(_maxGamesPerMatch);
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
				throw new ArgumentNullException("_model");
			}

			List<TournamentUserModel> userModels = _model.TournamentUsersBrackets
				.OrderBy(tubm => tubm.Seed)
				.Select(tubm => tubm.TournamentUser)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (TournamentUserModel model in userModels)
			{
				Players.Add(new User(model));
			}

			this.Id = _model.BracketID;
			this.BracketType = BracketType.RRGROUP;
			this.IsFinalized = _model.Finalized;
			this.NumberOfGroups = _model.NumberOfGroups;
			this.MaxRounds = 0;
			ResetBracket();
			CreateBracket();

			// Find & update every Match:
			foreach (MatchModel model in _model.Matches)
			{
				RestoreMatch(model.MatchNumber, model);
#if false
				foreach (IBracket group in Groups)
				{
					if (group.Players.Select(p => p.Id).ToList()
						.Contains((int)(model.DefenderID)))
					{
						// Update Match's score:
						group.GetMatch(model.MatchNumber)
							.SetMaxGames((ushort)(model.MaxGames));
						//group.GetMatch(model.MatchNumber)
						//	.SetWinsNeeded((ushort)(model.WinsNeeded));

						List<GameModel> gModelList = model.Games
							.OrderBy(g => g.GameNumber).ToList();
						foreach (GameModel gmodel in gModelList)
						{
							group.AddGame(model.MatchNumber, new Game(gmodel));
						}
#if false
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
#endif
						break;
					}
				}
#endif
			}

			// Update the rankings:
			UpdateRankings();
			this.IsFinished = true;
			foreach (IBracket group in Groups)
			{
				if (!group.IsFinished)
				{
					this.IsFinished = false;
					break;
				}
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
			if (Players.Count < 2 ||
				NumberOfGroups > (Players.Count / 2) || NumberOfGroups < 2)
			{
				return;
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

			foreach (IBracket group in Groups)
			{
				NumberOfMatches += group.NumberOfMatches;
				NumberOfRounds = (NumberOfRounds < group.NumberOfRounds)
					? group.NumberOfRounds
					: this.NumberOfRounds;
				Rankings.AddRange(group.Rankings);
			}
		}

		public override void ResetMatches()
		{
			base.ResetMatches();
			UpdateRankings();
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

			Rankings.Sort(SortRankingScores);
			for (int i = 0; i < Rankings.Count; ++i)
			{
				Rankings[i].Rank = i + 1;
			}
		}
		#endregion
	}
}
