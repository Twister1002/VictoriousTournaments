﻿using System;
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
			//public int Id
			//public BracketType BracketType
			//public bool IsFinalized
			//public bool IsFinished
			//public List<IPlayer> Players
			//public List<IPlayerScore> Rankings
			//public int MaxRounds = 0
			//protected Dictionary<int, Match> Matches
			//public int NumberOfRounds
			//protected Dictionary<int, Match> LowerMatches
			//public int NumberOfLowerRounds
			//protected Match grandFinal = null
			//public IMatch GrandFinal = null
			//public int NumberOfMatches
			//protected int MatchWinValue = 0
			//protected int MatchTieValue = 0
			//protected List<IBracket> Groups = empty
			//public int NumberOfGroups = 0
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
				grandFinal = null;
				--NumberOfMatches;
			}
			#endregion

			#region Private Methods
			protected override List<MatchModel> ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
			{
				List<MatchModel> alteredMatches = new List<MatchModel>();

				int nextWinnerNumber;
				int nextLoserNumber;
				IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

				if (match.NextMatchNumber <= NumberOfMatches)
				{
					// Case 1: Not a final/endpoint match. Treat like a DEB:
					alteredMatches.AddRange(base.ApplyWinEffects(_matchNumber, _slot));
				}
				else if (match.IsFinished)
				{
					if (nextLoserNumber > 0)
					{
						// Case 2: UB Finals.
						// Advance loser to lower bracket:
						PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
							? PlayerSlot.Challenger
							: PlayerSlot.Defender;
						GetInternalMatch(nextLoserNumber).AddPlayer
							(match.Players[(int)loserSlot], PlayerSlot.Defender);
						alteredMatches.Add(GetMatchModel(nextLoserNumber));
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

				return alteredMatches;
			}
			// void ApplyGameRemovalEffects() just uses DEB's version.
			protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
			{
				if (!_isAddition)
				{
					RecalculateRankings();
					return;
				}

				int nextWinnerNumber;
				int nextLoserNumber;
				IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

				if (match.NextMatchNumber <= NumberOfMatches)
				{
					// Case 1: Not a final/endpoint match. Treat like a DEB:
					base.UpdateScore(_matchNumber, _games, _isAddition, _oldMatch);
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

			protected override List<MatchModel> RemovePlayerFromFutureMatches(int _matchNumber, int _playerId)
			{
				if (_matchNumber > NumberOfMatches)
				{
					return new List<MatchModel>();
				}
				return base.RemovePlayerFromFutureMatches(_matchNumber, _playerId);
			}

			protected override void RecalculateRankings()
			{
				base.RecalculateRankings();

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

			Players = _players;
			Id = 0;
			BracketType = BracketType.GSLGROUP;
			NumberOfGroups = _numberOfGroups;

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
			foreach (TournamentUserModel userModel in userModels)
			{
				Players.Add(new Player(userModel));
			}

			this.Id = _model.BracketID;
			this.BracketType = _model.BracketType.Type;
			this.IsFinalized = _model.Finalized;
			this.NumberOfGroups = _model.NumberOfGroups;

			CreateBracket();
			// Find & update every Match:
			foreach (MatchModel matchModel in _model.Matches)
			{
				GetInternalMatch(matchModel.MatchNumber)
					.SetFromModel(matchModel);
			}

			// Update the Rankings:
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
				NumberOfRounds = Math.Max(this.NumberOfRounds, group.NumberOfRounds);
			}
		}
		#endregion

		#region Private Methods
		protected override void RecalculateRankings()
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
