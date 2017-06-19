using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatabaseLib;

namespace Tournament.Structure
{
	public class SingleElimBracket : KnockoutBracket
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
		public SingleElimBracket(List<IPlayer> _players, int _maxGamesPerMatch = 1)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}

			Players = _players;
			Id = 0;
			BracketType = BracketType.SINGLE;

			CreateBracket(_maxGamesPerMatch);
		}
		public SingleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public SingleElimBracket(BracketModel _model)
		{
			SetDataFromModel(_model);

			int totalUBMatches = Players.Count - 1;
			if (_model.Matches.Count > 0)
			{
				foreach (MatchModel mm in _model.Matches.OrderBy(m => m.MatchNumber))
				{
					if (mm.MatchNumber <= totalUBMatches)
					{
						Matches.Add(mm.MatchNumber, new Match(mm));
					}
					else
					{
						// Match doesn't belong in upper bracket, so break out:
						break;
					}
				}
				this.NumberOfMatches = Matches.Count;
				this.NumberOfRounds = Matches.Values
					.Max(m => m.RoundIndex);
			}

			if (BracketType.SINGLE == BracketType)
			{
				RecalculateRankings();

				if (this.IsFinalized && false == Validate())
				{
					throw new BracketValidationException
						("Bracket is Finalized but not Valid!");
				}
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

			#region Create the Bracket
			int totalMatches = Players.Count - 1;
			int numMatches = 0;
			List<List<Match>> roundList = new List<List<Match>>();

			// Create the Matches
			for (int r = 0; numMatches < totalMatches; ++r)
			{
				roundList.Add(new List<Match>());
				for (int i = 0;
					i < Math.Pow(2, r) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					Match m = new Match();
					m.SetMaxGames(_gamesPerMatch);
					roundList[r].Add(m);
				}
			}

			// Assign Match Numbers
			int matchNum = 1;
			for (int r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (Match match in roundList[r])
				{
					match.SetMatchNumber(matchNum++);
				}
			}

			// Tie Matches Together
			for (int r = 0; r + 1 < roundList.Count; ++r)
			{
				if (roundList[r + 1].Count == (roundList[r].Count * 2))
				{
					// "Normal" rounds: twice as many matchups
					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int currNum = roundList[r][m].MatchNumber;

						// Assign prev/next matchup numbers
						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2].MatchNumber);
						roundList[r + 1][m * 2].SetNextMatchNumber(currNum);

						roundList[r][m].AddPreviousMatchNumber(roundList[r + 1][m * 2 + 1].MatchNumber);
						roundList[r + 1][m * 2 + 1].SetNextMatchNumber(currNum);
					}
				}
				// Else: round is abnormal. Ignore it for now (we'll handle it later)
			}
			#endregion

			#region Assign the Players
			// Assign top two seeds to final match
			int pIndex = 0;
			roundList[0][0].AddPlayer(Players[pIndex++]);
			roundList[0][0].AddPlayer(Players[pIndex++]);

			for (int r = 0; r + 1 < roundList.Count; ++r)
			{
				// We're shifting back one player for each match in the prev round
				int prevRoundMatches = roundList[r + 1].Count;

				if ((roundList[r].Count * 2) > prevRoundMatches)
				{
					// Abnormal round ahead: we need to allocate prevMatchIndexes
					// to correctly distribute bye seeds

					int prevMatchNumber = 1;

					for (int m = 0; m < roundList[r].Count; ++m)
					{
						int[] playerIndexes = new int[2] { -1, -1 };
						for (int i = 0; i < Players.Count; ++i)
						{
							if (Players[i].Equals(roundList[r][m].Players[0]))
							{
								playerIndexes[0] = i;
							}
							else if (Players[i].Equals(roundList[r][m].Players[1]))
							{
								playerIndexes[1] = i;
							}
						}

						int playersToMove = 0;
						foreach (int p in playerIndexes)
						{
							if (p >= pIndex - prevRoundMatches)
							{
								++playersToMove;
							}
						}
						for (int i = 0; i < playerIndexes.Length; ++i)
						{
							if (playerIndexes[i] >= pIndex - prevRoundMatches)
							{
								roundList[r][m].AddPreviousMatchNumber(prevMatchNumber,
									(2 == playersToMove)
									? (PlayerSlot)i : PlayerSlot.Challenger);
								roundList[r + 1][prevMatchNumber - 1].SetNextMatchNumber
									(roundList[r][m].MatchNumber);
								++prevMatchNumber;
							}
						}
					}
				}

				for (int m = 0; m < roundList[r].Count; ++m)
				{
					// For each match, shift/reassign all teams to the prev bracket level
					// If prev level is abnormal, only shift 1 (or 0) teams
					foreach (int n in roundList[r][m].PreviousMatchNumbers)
					{
						if (n > 0)
						{
							ReassignPlayers(roundList[r][m], roundList[r + 1]);
							break;
						}
					}
				}

				for (int prePlayers = pIndex - 1; prePlayers >= 0; --prePlayers)
				{
					for (int m = 0; m < prevRoundMatches; ++m)
					{
						if (roundList[r + 1][m].Players.Contains(Players[prePlayers]))
						{
							// Add prev round's teams (according to seed) from the master list
							roundList[r + 1][m].AddPlayer(Players[pIndex++]);
							break;
						}
					}
				}
			}
			#endregion

			#region Set Bracket Member Variables
			// Move bracket data to member variables (Matches dictionary)
			NumberOfRounds = roundList.Count;
			for (int r = 0; r < roundList.Count; ++r)
			{
				for (int m = 0; m < roundList[r].Count; ++m)
				{
					roundList[r][m].SetRoundIndex(roundList.Count - r);
					roundList[r][m].SetMatchIndex(m + 1);
					Matches.Add(roundList[r][m].MatchNumber, roundList[r][m]);
				}
			}
			NumberOfMatches = Matches.Count;
			#endregion
		}
		#endregion

		#region Private Methods
		protected override int CalculateRank(int _matchNumber)
		{
			int round = GetInternalMatch(_matchNumber).RoundIndex;
			return Convert.ToInt32(Math.Pow(2, NumberOfRounds - round) + 1);
		}

		private void ReassignPlayers(Match _currMatch, List<Match> _prevRound)
		{
			if (null == _currMatch ||
				0 == (_prevRound?.Count ?? 0))
			{
				throw new NullReferenceException
					("NULL error in calling ReassignPlayers()...");
			}

			int playersToMove = 2;
			foreach (int n in _currMatch.PreviousMatchNumbers)
			{
				if (n < 0)
				{
					--playersToMove;
				}
			}
			foreach (Match match in _prevRound)
			{
				for (int i = 0; i < _currMatch.PreviousMatchNumbers.Length; ++i)
				{
					if (match.MatchNumber == _currMatch.PreviousMatchNumbers[i])
					{
						match.AddPlayer(_currMatch.Players[i]);
						_currMatch.RemovePlayer(_currMatch.Players[i].Id);
						--playersToMove;
					}
				}
				if (0 == playersToMove)
				{
					break;
				}
			}
		}
		#endregion
	}
}
