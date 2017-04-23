﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class GSLGroups : GroupStage
	{
		private class GSLBracket : DoubleElimBracket
		{
			#region Variables & Properties
			// inherits BracketType BracketType
			// inherits bool IsFinalized
			// inherits bool IsFinished
			// inherits List<IPlayer> Players
			// inherits List<IPlayerScore> Rankings
			// inherits Dictionary<int, IMatch> Matches
			// inherits int NumberOfRounds
			// inherits Dictionary<int, IMatch> LowerMatches (null)
			// inherits int NumberOfLowerRounds (0)
			// inherits IMatch GrandFinal (null)
			// inherits int NumberOfMatches
			#endregion

			#region Ctors
			public GSLBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
				: base(_players, _maxGamesPerMatch)
			{
				//BracketType = DataLib.BracketTypeModel.BracketType.GSL;
			}
			public GSLBracket()
				: this(new List<IPlayer>())
			{ }
			public GSLBracket(BracketModel _model)
			{
				throw new NotImplementedException();
			}
			#endregion

			#region Public Methods
			public override void CreateBracket(int _gamesPerMatch = 1)
			{
				if (4 != Players.Count &&
					8 != Players.Count)
				{
					throw new BracketException
						("Each GSL-style bracket must have exactly 4 or 8 players!");
				}

				base.CreateBracket(_gamesPerMatch);
				GrandFinal = null;
				--NumberOfMatches;
			}

			public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
			{
				if (GetMatch(_matchNumber).NextMatchNumber <= NumberOfMatches)
				{
					// Case 1: Not a final/endpoint match.
					return base.AddGame(_matchNumber, _defenderScore, _challengerScore, _winnerSlot);
				}

				GameModel gameModel = GetMatch(_matchNumber).AddGame(_defenderScore, _challengerScore, _winnerSlot);
				int nextWinnerNumber;
				int nextLoserNumber;
				IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;

				if (match.IsFinished)
				{
					if (nextLoserNumber > 0)
					{
						// Case 2: UB Finals.

						// Add winner to top of Rankings:
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name, -1, 1));
						// Advance loser to lower bracket:
						GetMatch(nextLoserNumber).AddPlayer
							(match.Players[(int)loserSlot], PlayerSlot.Defender);
						// Check lower bracket completion:
						if (GetLowerRound(NumberOfLowerRounds)[0].IsFinished)
						{
							this.IsFinished = true;
						}
					}
					else
					{
						// Case 3: LB Finals.

						// Add both players to Rankings:
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name, -1, 2));
						Rankings.Add(new PlayerScore
							(match.Players[(int)loserSlot].Id,
							match.Players[(int)loserSlot].Name, -1, 3));
						// Check upper bracket completion:
						if (GetRound(NumberOfRounds)[0].IsFinished)
						{
							this.IsFinished = true;
						}
					}

					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				}

				return gameModel;
			}
			#endregion

			#region Private Methods
			protected override void RemovePlayerFromFutureMatches(int _matchNumber, ref IPlayer _player)
			{
				if (_matchNumber > NumberOfMatches)
				{
					return;
				}
				base.RemovePlayerFromFutureMatches(_matchNumber, ref _player);
			}
			protected override void UpdateRankings()
			{
				base.UpdateRankings();

				IMatch upperFinal = GetRound(NumberOfRounds)[0];
				if (upperFinal.IsFinished)
				{
					Rankings.Add(new PlayerScore
						(upperFinal.Players[(int)(upperFinal.WinnerSlot)].Id,
						upperFinal.Players[(int)(upperFinal.WinnerSlot)].Name, -1, 1));
				}
				IMatch lowerFinal = GetLowerRound(NumberOfLowerRounds)[0];
				if (lowerFinal.IsFinished)
				{
					PlayerSlot loserSlot = (PlayerSlot.Defender == lowerFinal.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					Rankings.Add(new PlayerScore
						(lowerFinal.Players[(int)(lowerFinal.WinnerSlot)].Id,
						lowerFinal.Players[(int)(lowerFinal.WinnerSlot)].Name, -1, 2));
					Rankings.Add(new PlayerScore
						(lowerFinal.Players[(int)loserSlot].Id,
						lowerFinal.Players[(int)loserSlot].Name, -1, 3));
				}

				Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
			}
			#endregion
		}

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
		// inherits List<IBracket> Groups
		// inherits int NumberOfGroups
		#endregion

		#region Ctors
		public GSLGroups(List<IPlayer> _players, int _numberOfGroups, int _maxGamesPerMatch = 1)
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
			if (_numberOfGroups * 4 != _players.Count &&
				_numberOfGroups * 8 != _players.Count)
			{
				throw new ArgumentOutOfRangeException
					("_numberOfGroups", "Must have 4 or 8 players per group!");
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

			BracketType = BracketTypeModel.BracketType.GSLGROUP;
			NumberOfGroups = _numberOfGroups;
			ResetBracket();
			CreateBracket(_maxGamesPerMatch);

		}
		public GSLGroups()
			: this(new List<IPlayer>(), 2)
		{ }
		public GSLGroups(BracketModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			List<UserModel> userModels = _model.UserSeeds
				.OrderBy(ubs => ubs.Seed)
				.Select(ubs => ubs.User)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (UserModel model in userModels)
			{
				Players.Add(new User(model));
			}

			this.BracketType = BracketTypeModel.BracketType.GSLGROUP;
			this.IsFinalized = _model.Finalized;
			this.NumberOfGroups = _model.NumberOfGroups;
			ResetBracket();
			CreateBracket();

			foreach (MatchModel model in _model.Matches)
			{
				RestoreMatch(model.MatchNumber, model);
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
				NumberOfGroups < 2 ||
				(NumberOfGroups * 4 != Players.Count && NumberOfGroups * 8 != Players.Count))
			{
				throw new BracketException
					("Must have 4 or 8 Players Per Group!");
			}

			for (int b = 0; b < NumberOfGroups; ++b)
			{
				List<IPlayer> pList = new List<IPlayer>();
				for (int p = 0; (p + b) < Players.Count; p += NumberOfGroups)
				{
					pList.Add(Players[p + b]);
				}

				Groups.Add(new GSLBracket(pList, _gamesPerMatch));
			}
		}

		public override void ResetMatches()
		{
			base.ResetMatches();
			Rankings.Clear();
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
			Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
		}
		#endregion
	}
}