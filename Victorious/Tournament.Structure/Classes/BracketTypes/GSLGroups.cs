using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class GSLGroups : GroupStage
	{
		private class GSLBracket : DoubleElimBracket
		{
			#region Variables & Properties
			// inherits int Id
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
				//BracketType = BracketType.GSL;
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
#if false
			public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
			{
				GameModel gameModel = null;
				if (GetMatch(_matchNumber).NextMatchNumber <= NumberOfMatches)
				{
					gameModel = base.AddGame(_matchNumber, _defenderScore, _challengerScore, _winnerSlot);
				}
				else
				{
					gameModel = GetMatch(_matchNumber).AddGame(_defenderScore, _challengerScore, _winnerSlot);
					AddWinEffects(_matchNumber, _winnerSlot);
				}
				return gameModel;
			}
#endif
			#endregion

			#region Private Methods
			protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, PlayerSlot _formerMatchWinnerSlot, bool _resetManualWin = false)
			{
				if (!_isAddition)
				{
					UpdateRankings();
					return;
				}

				int nextWinnerNumber;
				int nextLoserNumber;
				IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

				if (match.NextMatchNumber <= NumberOfMatches)
				{
					// Case 1: Not a final/endpoint match. Treat like a DEB:
					base.UpdateScore(_matchNumber, _games, _isAddition, _formerMatchWinnerSlot, _resetManualWin);
					return;
				}

				if (match.IsFinished)
				{
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;

					if (nextLoserNumber > 0)
					{
						// Case 2: UB Finals.
						// Add winner to top of Rankings:
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name,
							1));
					}
					else
					{
						// Case 3: LB Finals.
						// Add both players to Rankings:
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name,
							2));
						Rankings.Add(new PlayerScore
							(match.Players[(int)loserSlot].Id,
							match.Players[(int)loserSlot].Name,
							3));
					}
				}

				Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
			}
			protected override void ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
			{
				int nextWinnerNumber;
				int nextLoserNumber;
				IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

				if (match.NextMatchNumber <= NumberOfMatches)
				{
					// Case 1: Not a final/endpoint match. Treat like a DEB:
					base.ApplyWinEffects(_matchNumber, _slot);
					return;
				}

				if (match.IsFinished)
				{
					if (nextLoserNumber > 0)
					{
						// Case 2: UB Finals.
						// Advance loser to lower bracket:
						PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
							? PlayerSlot.Challenger
							: PlayerSlot.Defender;
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
						// Check upper bracket completion:
						if (GetRound(NumberOfRounds)[0].IsFinished)
						{
							this.IsFinished = true;
						}
					}
				}
			}
			// void ApplyGameRemovalEffects() just uses DEB's version.

			protected override void RemovePlayerFromFutureMatches(int _matchNumber, int _playerId)
			{
				if (_matchNumber > NumberOfMatches)
				{
					return;
				}
				base.RemovePlayerFromFutureMatches(_matchNumber, _playerId);
			}
			protected override void UpdateRankings()
			{
				base.UpdateRankings();

				IMatch upperFinal = GetRound(NumberOfRounds)[0];
				if (upperFinal.IsFinished)
				{
					Rankings.Add(new PlayerScore
						(upperFinal.Players[(int)(upperFinal.WinnerSlot)].Id,
						upperFinal.Players[(int)(upperFinal.WinnerSlot)].Name,
						1));
				}
				IMatch lowerFinal = GetLowerRound(NumberOfLowerRounds)[0];
				if (lowerFinal.IsFinished)
				{
					PlayerSlot loserSlot = (PlayerSlot.Defender == lowerFinal.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					Rankings.Add(new PlayerScore
						(lowerFinal.Players[(int)(lowerFinal.WinnerSlot)].Id,
						lowerFinal.Players[(int)(lowerFinal.WinnerSlot)].Name,
						2));
					Rankings.Add(new PlayerScore
						(lowerFinal.Players[(int)loserSlot].Id,
						lowerFinal.Players[(int)loserSlot].Name,
						3));
				}

				Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
			}
			#endregion
		}

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
				throw new BracketException
					("Must have 4 or 8 players per group!");
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
			BracketType = BracketType.GSLGROUP;
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

			List<TournamentUserModel> userModels = _model.TournamentUsersBrackets
				.OrderBy(tubm => tubm.Seed, new SeedComparer())
				.Select(tubm => tubm.TournamentUser)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (TournamentUserModel model in userModels)
			{
				Players.Add(new User(model));
			}

			this.Id = _model.BracketID;
			this.BracketType = BracketType.GSLGROUP;
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

			foreach (IBracket group in Groups)
			{
				NumberOfMatches += group.NumberOfMatches;
				NumberOfRounds = (NumberOfRounds < group.NumberOfRounds)
					? group.NumberOfRounds
					: this.NumberOfRounds;
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
