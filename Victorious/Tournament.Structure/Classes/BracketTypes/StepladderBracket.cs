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
			BracketType = BracketType.STEP;

			CreateBracket(_maxGamesPerMatch);
		}
		public StepladderBracket()
			: this(new List<IPlayer>())
		{ }
		public StepladderBracket(BracketModel _model)
		{
			SetDataFromModel(_model);

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

			if (this.IsFinalized && false == Validate())
			{
				throw new BracketValidationException
					("Bracket is Finalized but not Valid!");
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();
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
		#endregion
	}
}
