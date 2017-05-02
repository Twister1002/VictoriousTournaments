using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class SwissBracket : RoundRobinBracket
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
		public SwissBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1, int _numberOfRounds = 0)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
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
			//BracketType = BracketTypeModel.BracketType.SWISS;
			MaxRounds = _numberOfRounds;
			ResetBracket();
			CreateBracket(_maxGamesPerMatch);
		}
		public SwissBracket()
			: this(new List<IPlayer>())
		{ }
		public SwissBracket(BracketModel _model)
			: base(_model)
		{
			//BracketType = BracketTypeModel.BracketType.SWISS;
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
			if (Players.Count < 2)
			{
				return;
			}
			if (0 != Players.Count % 2)
			{
				throw new BracketException
					("Swiss brackets must have an even number of players!");
			}

			foreach (IPlayer player in Players)
			{
				Rankings.Add(new PlayerScore(player.Id, player.Name, 0, 1));
			}

			// Create first-round matches:
			int divisionPoint = Players.Count / 2;
			NumberOfRounds = 1;
			for (int m = 0; m < divisionPoint; ++m, ++NumberOfMatches)
			{
				IMatch match = new Match();
				match.SetMatchNumber(NumberOfMatches + 1);
				match.SetRoundIndex(NumberOfRounds);
				match.SetMatchIndex(m + 1);
				match.SetMaxGames(_gamesPerMatch);
				match.AddPlayer(Players[m]);
				match.AddPlayer(Players[m + divisionPoint]);

				Matches.Add(match.MatchNumber, match);
			}
		}

#if false
		public override GameModel AddGame(int _matchNumber, int _defenderScore, int _challengerScore, PlayerSlot _winnerSlot)
		{
			GameModel gameModel = base.AddGame(_matchNumber, _defenderScore, _challengerScore, _winnerSlot);

			if (IsFinished)
			{
				IsFinished = !(AddNewRound(GetMatch(_matchNumber).MaxGames));
			}
			return gameModel;
		}
		public override void RemoveLastGame(int _matchNumber)
		{
			base.RemoveLastGame(_matchNumber);

			IMatch currMatch = GetMatch(_matchNumber);
			if (!currMatch.IsFinished)
			{
				CheckAndRemoveNextRound(1 + currMatch.RoundIndex);
			}
		}
		public override void ResetMatchScore(int _matchNumber)
		{
			base.ResetMatchScore(_matchNumber);

			CheckAndRemoveNextRound(1 + GetMatch(_matchNumber).RoundIndex);
		}
#endif
		#endregion

		#region Private Methods
		protected override void ApplyWinEffects(int _matchNumber, PlayerSlot _slot)
		{
			base.ApplyWinEffects(_matchNumber, _slot);
			if (this.IsFinished)
			{
				IsFinished = !(AddNewRound(GetMatch(_matchNumber).MaxGames));
			}
		}
		protected override void ApplyGameRemovalEffects(int _matchNumber, List<GameModel> _games, PlayerSlot _formerMatchWinnerSlot)
		{
			if (PlayerSlot.unspecified != _formerMatchWinnerSlot &&
				(PlayerSlot.unspecified == GetMatch(_matchNumber).WinnerSlot))
			{
				CheckAndRemoveNextRound(1 + GetMatch(_matchNumber).RoundIndex);
			}
			base.ApplyGameRemovalEffects(_matchNumber, _games, _formerMatchWinnerSlot);
		}
		private bool AddNewRound(int _gamesPerMatch)
		{
			if (MaxRounds > 0 && NumberOfRounds >= MaxRounds)
			{
				return false;
			}
			int totalRounds = 0;
			while (Math.Pow(2, totalRounds) < Players.Count)
			{
				++totalRounds;
			}
			if (NumberOfRounds >= totalRounds)
			{
				return false;
			}

			++NumberOfRounds;
			int mIndex = 1;
			int divisionPoint = Players.Count / 2;
			if (2 == NumberOfRounds && 0 == divisionPoint % 2)
			{
				int secondDivision = divisionPoint / 2;
				for (int i = 0; i < secondDivision; ++i)
				{
					for (int j = 0; j <= divisionPoint; j += divisionPoint)
					{
						IMatch match = new Match();
						match.SetMatchNumber(++NumberOfMatches);
						match.SetRoundIndex(NumberOfRounds);
						match.SetMatchIndex(mIndex++);
						match.SetMaxGames(_gamesPerMatch);
						match.AddPlayer(Players.Find(p => p.Id == Rankings[i + j].Id));
						match.AddPlayer(Players.Find(p => p.Id == Rankings[i + j + secondDivision].Id));

						Matches.Add(match.MatchNumber, match);
					}
				}
			}
			else
			{
				for (int i = 0; i < Players.Count; i += 2)
				{
					IMatch match = new Match();
					match.SetMatchNumber(++NumberOfMatches);
					match.SetRoundIndex(NumberOfRounds);
					match.SetMatchIndex(mIndex++);
					match.SetMaxGames(_gamesPerMatch);
					match.AddPlayer(Players.Find(p => p.Id == Rankings[i].Id));
					match.AddPlayer(Players.Find(p => p.Id == Rankings[i + 1].Id));

					Matches.Add(match.MatchNumber, match);
				}
			}

			return true;
		}

		private void CheckAndRemoveNextRound(int _nextRoundIndex)
		{
#if false
			if (_nextRoundIndex > NumberOfRounds)
			{
				return;
			}

			bool deleteNextRound = true;
			List<IMatch> nextRound = GetRound(_nextRoundIndex);
			foreach (IMatch match in nextRound)
			{
				if (match.Games.Count > 0)
				{
					deleteNextRound = false;
					break;
				}
			}

			if (deleteNextRound)
			{
				foreach (IMatch match in nextRound)
				{
					Matches.Remove(match.MatchNumber);
					--NumberOfMatches;
				}
				--NumberOfRounds;
			}
#endif
		}
		#endregion
	}
}
