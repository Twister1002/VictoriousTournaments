﻿using System;
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
					.Select(m => m.RoundIndex)
					.Max();
			}

			if (BracketType.SINGLE == BracketType)
			{
				RecalculateRankings();
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(int _gamesPerMatch = 1)
		{
			ResetBracketData();
			if (_gamesPerMatch < 1)
			{
				throw new BracketException
					("Games Per Match must be greater than 0!");
			}
			else if (_gamesPerMatch % 2 == 0)
			{
				throw new BracketException
					("Games/Match must be odd in an elimination bracket!");
			}
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
		protected override void UpdateScore(int _matchNumber, List<GameModel> _games, bool _isAddition, MatchModel _oldMatch)
		{
			if (_isAddition && _oldMatch.WinnerID.GetValueOrDefault(-1) < 0)
			{
				int nextWinnerNumber;
				int nextLoserNumber;
				Match match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

				if (match.IsFinished)
				{
					// Add losing Player to Rankings:
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;
					int rank = (int)(Math.Pow(2, NumberOfRounds - match.RoundIndex) + 1);

					Rankings.Add(new PlayerScore
						(match.Players[(int)loserSlot].Id,
						match.Players[(int)loserSlot].Name,
						rank));
					if (nextWinnerNumber < 0)
					{
						// Finals match: Add winner to Rankings:
						Rankings.Add(new PlayerScore
							(match.Players[(int)(match.WinnerSlot)].Id,
							match.Players[(int)(match.WinnerSlot)].Name,
							1));
						IsFinished = true;
					}
					Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				}
			}
			else if (!_isAddition && _oldMatch.WinnerID.GetValueOrDefault(-1) > -1)
			{
				RecalculateRankings();
			}
		}

		protected override void RecalculateRankings()
		{
			if (null == Rankings)
			{
				Rankings = new List<IPlayerScore>();
			}
			Rankings.Clear();

			foreach (Match match in Matches.Values)
			{
				if (match.IsFinished)
				{
					// Add losing Player to the Rankings:
					int rank = (int)(Math.Pow(2, (NumberOfRounds - match.RoundIndex)) + 1);
					IPlayer losingPlayer = match.Players[
						(PlayerSlot.Defender == match.WinnerSlot)
						? (int)PlayerSlot.Challenger
						: (int)PlayerSlot.Defender];
					Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, rank));
				}
			}
			if (NumberOfMatches > 0 && Matches[NumberOfMatches].IsFinished)
			{
				// Add Finals winner to Rankings:
				IPlayer winningPlayer = Matches[NumberOfMatches]
					.Players[(int)Matches[NumberOfMatches].WinnerSlot];
				Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, 1));
				this.IsFinished = true;
			}

			Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
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
