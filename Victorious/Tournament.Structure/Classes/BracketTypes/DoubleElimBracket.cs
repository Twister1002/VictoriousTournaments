using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class DoubleElimBracket : SingleElimBracket
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
		//protected Match grandFinal
		//public IMatch GrandFinal
		//public int NumberOfMatches
		//protected int MatchWinValue = 0
		//protected int MatchTieValue = 0
		#endregion

		#region Ctors
		public DoubleElimBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
			: base(_players, _maxGamesPerMatch)
		{
			BracketType = BracketType.DOUBLE;
		}
		public DoubleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public DoubleElimBracket(BracketModel _model)
			: base(_model)
		{
			this.NumberOfLowerRounds = 0;
			if (_model.Matches.Count > 0)
			{
				if (CalculateTotalLowerBracketMatches(Players.Count) > 0)
				{
					int numOfGrandFinal = _model.Matches.Count;

					foreach (MatchModel mm in _model.Matches.OrderBy(m => m.MatchNumber))
					{
						if (Matches.ContainsKey(mm.MatchNumber))
						{
							// Case 1: match is upper bracket:
							continue;
						}
						if (mm.MatchNumber == numOfGrandFinal)
						{
							// Case 2: match is grand final:
							this.grandFinal = new Match(mm);
						}
						else
						{
							// Case 3: match is lower bracket:
							Match match = new Match(mm);
							LowerMatches.Add(match.MatchNumber, match);
							this.NumberOfLowerRounds = Math.Max(NumberOfLowerRounds, match.RoundIndex);
						}
					}
				}
				this.NumberOfMatches = Matches.Count + LowerMatches.Count;
				if (null != grandFinal)
				{
					++NumberOfMatches;
				}
			}

			RecalculateRankings();
			if (grandFinal?.IsFinished ?? false)
			{
				this.IsFinished = true;
			}

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
			if (Players.Count < 4)
			{
				return;
			}

			base.CreateBracket(_gamesPerMatch);

			List<List<Match>> roundList = new List<List<Match>>();
			int totalMatches = CalculateTotalLowerBracketMatches(Players.Count);
			int numMatches = 0;
			int r = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				roundList.Add(new List<Match>());
				for (int i = 0;
					i < Math.Pow(2, r / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					Match m = new Match();
					m.SetMaxGames(_gamesPerMatch);
					roundList[r].Add(m);
				}
				++r;
			}

			// Assign Match Numbers
			int matchNum = 1 + Matches.Count;
			for (r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (Match match in roundList[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			// Tie Matches Together
			bool flipSeeds = true;
			for (r = roundList.Count - 2; r >= 0; --r)
			{
				bool rIndexIsEven = (0 == r % 2) ? true : false;
				if (rIndexIsEven && roundList[r + 1].Count == roundList[r].Count)
				{
					// Round is "normal," but one team is coming from Upper Bracket
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						List<IMatch> upperRound = GetRound(NumberOfRounds - (r / 2));
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup indexes
						if (flipSeeds)
						{
							roundList[r][m].AddPreviousMatchNumber(upperRound[upperRound.Count - 1 - m].MatchNumber);
							Matches[upperRound[upperRound.Count - 1 - m].MatchNumber].SetNextLoserMatchNumber(currNum);
						}
						else
						{
							roundList[r][m].AddPreviousMatchNumber(upperRound[m].MatchNumber);
							Matches[upperRound[m].MatchNumber].SetNextLoserMatchNumber(currNum);
						}
						// ************* THIS ISN'T QUITE RIGHT (RE-SEED ORDER)

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m].MatchNumber);
						roundList[r + 1][m].SetNextMatchNumber(currNum);
					}

					flipSeeds = !flipSeeds;
				}
				else if (!rIndexIsEven && roundList[r + 1].Count == (roundList[r].Count * 2))
				{
					// Round is "normal"
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup indexes
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2].MatchNumber);
						roundList[r + 1][m * 2].SetNextMatchNumber(currNum);

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2 + 1].MatchNumber);
						roundList[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				else
				{
					// Round is abnormal. Case is not possible
					// (unless we later decide to include it)
				}

				flipSeeds = !flipSeeds;
			}

			r = roundList.Count - 1;
			if (r >= 0)
			{
				// We have enough teams to have created a Lower Bracket.
				// Manually update the first Lower round,
				// and create a Grand Final match

				for (int m = 0; m < roundList[r].Count; ++m)
				{
					List<IMatch> upperRound = GetRound(NumberOfRounds - (r / 2 + 1));
					int currNum = roundList[r][m].MatchNumber;

					// Assign prev/next matchup indexes for FIRST round
					// (both teams come from Upper Bracket)
					roundList[r][m].AddPreviousMatchNumber(upperRound[m * 2].MatchNumber);
					Matches[upperRound[m * 2].MatchNumber].SetNextLoserMatchNumber(currNum);

					roundList[r][m].AddPreviousMatchNumber(upperRound[m * 2 + 1].MatchNumber);
					Matches[upperRound[m * 2 + 1].MatchNumber].SetNextLoserMatchNumber(currNum);
				}

				// Create a Grand Final
				grandFinal = new Match();
				grandFinal.SetMatchNumber(matchNum);
				grandFinal.SetMaxGames(_gamesPerMatch);
				grandFinal.SetRoundIndex(1);
				grandFinal.SetMatchIndex(1);
				grandFinal.AddPreviousMatchNumber(Matches.Count);
				grandFinal.AddPreviousMatchNumber(roundList[0][0].MatchNumber);
				// Connect Final matches to Grand Final
				roundList[0][0].SetNextMatchNumber(grandFinal.MatchNumber);
				Matches[Matches.Count].SetNextMatchNumber(grandFinal.MatchNumber);

				// Move new bracket data to member variables (LowerMatches dictionary)
				NumberOfLowerRounds = roundList.Count;
				for (r = 0; r < roundList.Count; ++r)
				{
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						roundList[r][m].SetRoundIndex(roundList.Count - r);
						roundList[r][m].SetMatchIndex(m + 1);
						LowerMatches.Add(roundList[r][m].MatchNumber, roundList[r][m]);
					}
				}
				NumberOfMatches += (LowerMatches.Count + 1);
			}
		}
		public override bool Validate()
		{
			if ((Players?.Count ?? 0) < 4)
			{
				return false;
			}

			return base.Validate();
		}
		#endregion

		#region Private Methods
		protected override int CalculateRank(int _matchNumber)
		{
			int rank = 2; // 2 = GrandFinal loser

			if (LowerMatches?.ContainsKey(_matchNumber) ?? false)
			{
				Match match = GetInternalMatch(_matchNumber);
				rank = NumberOfMatches - GetLowerRound(match.RoundIndex)[0].MatchNumber + 2;
			}
			else if (Matches?.ContainsKey(_matchNumber) ?? false)
			{
				rank = Convert.ToInt32(Math.Pow(2, NumberOfRounds - 1) + 1);
			}

			return rank;
		}
		protected override void RecalculateRankings()
		{
			// The base method adds all LB losers to Rankings.
			// It also handles the Grand Final.
			base.RecalculateRankings();

			// If there's no "Play-in" round, we're done:
			if (0 == NumberOfMatches || Matches[1].NextLoserMatchNumber > 0)
			{
				return;
			}

			// Get the play-in round:
			List<IMatch> firstRound = GetRound(1)
				.Where(m => m.IsFinished && m.NextLoserMatchNumber > 0)
				.ToList();
			foreach (IMatch match in firstRound)
			{
				// Add each losing Player to the Rankings:
				int rank = CalculateRank(match.MatchNumber);
				IPlayer losingPlayer = match.Players[
					(PlayerSlot.Defender == match.WinnerSlot)
					? (int)PlayerSlot.Challenger
					: (int)PlayerSlot.Defender];
				Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));
			}
		}

		private int CalculateTotalLowerBracketMatches(int _numPlayers)
		{
			if (_numPlayers < 4)
			{
				return 0;
			}

			int normalizedPlayers = 2;
			while (true)
			{
				int next = normalizedPlayers * 2;
				if (next <= _numPlayers)
				{
					normalizedPlayers = next;
				}
				else
				{
					break;
				}
			}
			return (normalizedPlayers - 2);
		}
		#endregion
	}
}
