using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class StepladderBracket : KnockoutBracket
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
		//protected Dictionary<int, Match> LowerMatches = empty
		//public int NumberOfLowerRounds = 0
		//protected Match grandFinal = null
		//public IMatch GrandFinal = null
		//public int NumberOfMatches
		//protected int MatchWinValue = 0
		//protected int MatchTieValue = 0
		#endregion

		#region Ctors
		public StepladderBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			//BracketType = BracketType.STEP;

			CreateBracket(_maxGamesPerMatch);
		}
		public StepladderBracket()
			: this(new List<IPlayer>())
		{ }
		public StepladderBracket(BracketModel _model)
		{
			if (null == _model)
			{
				throw new ArgumentNullException("_model");
			}

			this.Id = _model.BracketID;
			this.BracketType = _model.BracketType.Type;
			this.IsFinalized = _model.Finalized;

			List<TournamentUserModel> userModels = _model.TournamentUsersBrackets
				.OrderBy(u => u.Seed, new SeedComparer())
				.Select(u => u.TournamentUser)
				.ToList();
			this.Players = new List<IPlayer>();
			foreach (TournamentUserModel userModel in userModels)
			{
				Players.Add(new Player(userModel));
			}

			ResetBracketData();
			if (_model.Matches.Count > 0)
			{
				foreach (MatchModel mm in _model.Matches)
				{
					Matches.Add(mm.MatchNumber, new Match(mm));
				}
				this.NumberOfMatches = Matches.Count;
				this.NumberOfRounds = Matches.Count;
			}

			RecalculateRankings();
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();
			if (_gamesPerMatch < 1)
			{
				throw new BracketException
					("Games per Match must be greater than 0!");
			}
			else if (0 == _gamesPerMatch % 2)
			{
				throw new BracketException
					("Games per Match must be ODD in an elimination bracket!");
			}
			if (Players.Count < 2)
			{
				return;
			}

			// Create Matches and assign Players:
			int playerIndex = Players.Count - 2;
			for (int i = 1; playerIndex >= 0; ++i, --playerIndex)
			{
				Match match = new Match();
				match.SetMatchNumber(i);
				match.SetRoundIndex(i);
				match.SetMatchIndex(1);
				match.SetMaxGames(_gamesPerMatch);
				match.AddPlayer(Players[playerIndex], PlayerSlot.Defender);

				Matches.Add(match.MatchNumber, match);
			}
			Matches[1].AddPlayer(Players.Last(), PlayerSlot.Challenger);

			// Set Bracket fields:
			NumberOfMatches = Matches.Count;
			NumberOfRounds = Matches.Count;

			// Tie Matches together:
			for (int m = 1; m <= NumberOfMatches; ++m)
			{
				if (m > 1)
				{
					Matches[m].AddPreviousMatchNumber(m - 1, PlayerSlot.Challenger);
				}
				if (m < NumberOfMatches)
				{
					Matches[m].SetNextMatchNumber(m + 1);
				}
			}
		}
		#endregion

		#region Private Methods
		protected override int CalculateRank(int _matchNumber)
		{
			return (2 + NumberOfMatches - _matchNumber);
		}

		protected override void RecalculateRankings()
		{
			if (null == Rankings)
			{
				Rankings = new List<IPlayerScore>();
			}
			Rankings.Clear();

			if (NumberOfMatches > 0)
			{
				foreach (Match match in Matches.Values.OrderBy(m => m.MatchNumber))
				{
					if (!(match.IsFinished))
					{
						break;
					}

					// Add each losing Player to the Rankings:
					int rank = 2 + NumberOfMatches - match.MatchNumber;
					IPlayer losingPlayer = match.Players[
						(PlayerSlot.Defender == match.WinnerSlot)
						? (int)PlayerSlot.Challenger
						: (int)PlayerSlot.Defender];
					Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));
				}

				if (Matches[NumberOfMatches].IsFinished)
				{
					// Add Finals winner to Rankings:
					IPlayer winningPlayer = Matches[NumberOfMatches]
						.Players[(int)Matches[NumberOfMatches].WinnerSlot];
					Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, 1));
					this.IsFinished = true;
				}

				Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
			}
		}
		#endregion
	}
}
