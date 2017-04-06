﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class DoubleElimBracket : SingleElimBracket
	{
		#region Variables & Properties
		// inherits BracketType BracketType
		// inherits bool IsFinished
		// inherits List<IPlayer> Players
		// inherits List<IPlayerScore> Rankings
		// inherits Dictionary<int, IMatch> Matches
		// inherits int NumberOfRounds
		// inherits Dictionary<int, IMatch> LowerMatches
		// inherits int NumberOfLowerRounds
		// inherits IMatch GrandFinal
		// inherits int NumberOfMatches
		#endregion

		#region Ctors
		public DoubleElimBracket(List<IPlayer> _players)
			: base(_players)
		{
			BracketType = BracketTypeModel.BracketType.DOUBLE;
		}
#if false
		public DoubleElimBracket(int _numPlayers)
			: base(_numPlayers)
		{
			BracketType = BracketTypeModel.BracketType.DOUBLE;
		}
#endif
		public DoubleElimBracket()
			: this(new List<IPlayer>())
		{ }
		public DoubleElimBracket(BracketModel _model)
			: base(_model)
		{
			this.IsFinished = false;
			this.BracketType = BracketTypeModel.BracketType.DOUBLE;

			if (CalculateTotalLowerBracketMatches(Players.Count) > 0)
			{
				int numOfGrandFinal = _model.Matches.Count - 1;
				int i = 0;

				this.LowerMatches = new Dictionary<int, IMatch>();
				foreach (MatchModel mm in _model.Matches)
				{
					if (i >= Matches.Count
						&& i < numOfGrandFinal)
					{
						IMatch match = new Match(mm);
						if (match.RoundIndex > NumberOfLowerRounds)
						{
							this.NumberOfLowerRounds = match.RoundIndex;
						}
						LowerMatches.Add(match.MatchNumber, match);
						++NumberOfMatches;
					}
					else if (i == numOfGrandFinal)
					{
						this.GrandFinal = new Match(mm);
						++NumberOfMatches;
					}

					++i;
				}
			}

			UpdateRankings();
			if (GrandFinal.IsFinished)
			{
				IPlayer winningPlayer = GrandFinal.Players[(int)GrandFinal.WinnerSlot];
				Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, -1, 1));
				IPlayer losingPlayer = GrandFinal.Players[
					(PlayerSlot.Defender == GrandFinal.WinnerSlot)
					? (int)PlayerSlot.Challenger
					: (int)PlayerSlot.Defender];
				Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, 2));

				Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
				this.IsFinished = true;
			}
		}
		#endregion

		#region Public Methods
		public override void CreateBracket(ushort _winsPerMatch = 1)
		{
			base.CreateBracket(_winsPerMatch);
			if (0 == NumberOfMatches)
			{
				return;
			}

			List<List<IMatch>> roundList = new List<List<IMatch>>();
			int totalMatches = CalculateTotalLowerBracketMatches(Players.Count);
			int numMatches = 0;
			int r = 0;

			// Create the Matches
			while (numMatches < totalMatches)
			{
				roundList.Add(new List<IMatch>());
				for (int i = 0;
					i < Math.Pow(2, r / 2) && numMatches < totalMatches;
					++i, ++numMatches)
				{
					// Add new matchups per round
					// (rounds[0] is the final match)
					IMatch m = new Match();
					m.SetWinsNeeded(_winsPerMatch);
					roundList[r].Add(m);
				}
				++r;
			}

			// Assign Match Numbers
			int matchNum = 1 + Matches.Count;
			for (r = roundList.Count - 1; r >= 0; --r)
			{
				foreach (IMatch match in roundList[r])
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
				GrandFinal = new Match();
				GrandFinal.SetMatchNumber(matchNum);
				GrandFinal.SetWinsNeeded(_winsPerMatch);
				GrandFinal.SetRoundIndex(1);
				GrandFinal.SetMatchIndex(1);
				GrandFinal.AddPreviousMatchNumber(Matches.Count);
				GrandFinal.AddPreviousMatchNumber(roundList[0][0].MatchNumber);
				// Connect Final matches to Grand Final
				roundList[0][0].SetNextMatchNumber(GrandFinal.MatchNumber);
				Matches[Matches.Count].SetNextMatchNumber(GrandFinal.MatchNumber);

				// Move new bracket data to member variables (LowerMatches dictionary)
				NumberOfLowerRounds = roundList.Count;
				LowerMatches = new Dictionary<int, IMatch>();
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
			else
			{
				LowerMatches = null;
				GrandFinal = null;
			}
		}

		public override void AddGame(int _matchNumber, IGame _game)
		{
			base.AddGame(_matchNumber, _game);

			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);
			if (!match.IsFinished)
			{
				return;
			}

			PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
				? PlayerSlot.Challenger
				: PlayerSlot.Defender;
			if (nextLoserNumber > 0)
			{
				// Advance the losing player:
				IMatch nextMatch = GetMatch(nextLoserNumber);
				for (int i = 0; i < nextMatch.PreviousMatchNumbers.Length; ++i)
				{
					if (_matchNumber == nextMatch.PreviousMatchNumbers[i])
					{
						GetMatch(nextLoserNumber).AddPlayer(match.Players[(int)loserSlot], (PlayerSlot)i);
						break;
					}
				}
			}
			else
			{
				// Add losing player to Rankings:
				int rank = -1;
				if (null != LowerMatches && LowerMatches.ContainsKey(_matchNumber))
				{
					rank = NumberOfMatches - GetLowerRound(match.RoundIndex)[0].MatchNumber + 2;
				}
				else if (null != Matches && Matches.ContainsKey(_matchNumber))
				{
					rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);
				}
				else if (null != GrandFinal && GrandFinal.MatchNumber == _matchNumber)
				{
					rank = 2;
				}

				Rankings.Add(new PlayerScore
					(match.Players[(int)loserSlot].Id, match.Players[(int)loserSlot].Name, -1, rank));
				Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
			}

		}
		public override void RemoveLastGame(int _matchNumber)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.IsFinished)
			{
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				RemovePlayerFromFutureMatches
					(nextLoserNumber, ref match.Players[(int)loserSlot]);
			}

			base.RemoveLastGame(_matchNumber);
		}

		public override void AddWin(int _matchNumber, PlayerSlot _slot)
		{
			base.AddWin(_matchNumber, _slot);

			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);
			if (!match.IsFinished)
			{
				return;
			}
#if false
			GetMatch(_matchNumber).AddWin(_slot);
			if (!match.IsFinished)
			{
				return;
			}

			if (nextWinnerNumber > 0)
			{
				// Advance the winning player:
				IMatch nextMatch = GetMatch(nextWinnerNumber);
				for (int i = 0; i < nextMatch.PreviousMatchNumbers.Count; ++i)
				{
					if (_matchNumber == nextMatch.PreviousMatchNumbers[i])
					{
						PlayerSlot newSlot = (1 == nextMatch.PreviousMatchNumbers.Count)
							? PlayerSlot.Challenger : (PlayerSlot)i;
						GetMatch(nextWinnerNumber).AddPlayer(match.Players[(int)_slot], newSlot);
						break;
					}
				}
			}
			else
			{
				// Add winner to Rankings:
				Rankings.Add(new PlayerScore
					(match.Players[(int)_slot].Id, match.Players[(int)_slot].Name, -1, 1));
				IsFinished = true;
			}
#endif
			PlayerSlot loserSlot = (PlayerSlot.Defender == _slot)
				? PlayerSlot.Challenger
				: PlayerSlot.Defender;
			if (nextLoserNumber > 0)
			{
				// Advance the losing player:
				IMatch nextMatch = GetMatch(nextLoserNumber);
				for (int i = 0; i < nextMatch.PreviousMatchNumbers.Length; ++i)
				{
					if (_matchNumber == nextMatch.PreviousMatchNumbers[i])
					{
						GetMatch(nextLoserNumber).AddPlayer(match.Players[(int)loserSlot], (PlayerSlot)i);
						break;
					}
				}
			}
			else
			{
				// Add losing player to Rankings:
				int rank = -1;
				if (null != LowerMatches && LowerMatches.ContainsKey(_matchNumber))
				{
					rank = NumberOfMatches - GetLowerRound(match.RoundIndex)[0].MatchNumber + 2;
				}
				else if (null != Matches && Matches.ContainsKey(_matchNumber))
				{
					rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);
				}
				else if (null != GrandFinal && GrandFinal.MatchNumber == _matchNumber)
				{
					rank = 2;
				}

				Rankings.Add(new PlayerScore
					(match.Players[(int)loserSlot].Id, match.Players[(int)loserSlot].Name, -1, rank));
				Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
			}
#if false
			try
			{
				// Check the Upper Bracket:
				base.AddWin(_matchNumber, _slot);

				if (_slot == Matches[_matchNumber].WinnerSlot)
				{
					// If this is the Upper Final, move winner to the Grand Final
					if (Matches[_matchNumber].RoundIndex == NumberOfRounds)
					{
						GrandFinal.AddPlayer
							(Matches[_matchNumber].Players[(int)_slot], PlayerSlot.Defender);
					}

					if (Matches[_matchNumber].NextLoserMatchNumber > 0)
					{
						// "Advance" the loser
						int nmNumber = Matches[_matchNumber].NextLoserMatchNumber;
						for (int i = 0; i < LowerMatches[nmNumber].PreviousMatchNumbers.Count; ++i)
						{
							if (LowerMatches[nmNumber].PreviousMatchNumbers[i] == _matchNumber)
							{
								LowerMatches[nmNumber].AddPlayer((PlayerSlot.Defender == _slot)
									? Matches[_matchNumber].Players[(int)PlayerSlot.Challenger]
									: Matches[_matchNumber].Players[(int)PlayerSlot.Defender]
									, (PlayerSlot)i);
							}
						}
					}
					else
					{
						// Loser doesn't advance from play-in round,
						// so update the rankings instead.
						int rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);
						IPlayer losingPlayer = Matches[_matchNumber].Players[
							(PlayerSlot.Defender == _slot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, rank));
					}
				}

				return;
			}
			catch (MatchNotFoundException)
			{
				// Check the Grand Final:
				if (_matchNumber == GrandFinal.MatchNumber)
				{
					GrandFinal.AddWin(_slot);
					if (GrandFinal.IsFinished)
					{
						// Add both Players to the Rankings:
						IPlayer winningPlayer = GrandFinal.Players[(int)_slot];
						Rankings.Add(new PlayerScore(winningPlayer.Id, winningPlayer.Name, -1, 1));
						IPlayer losingPlayer = GrandFinal.Players[
							(PlayerSlot.Defender == _slot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, 2));

						Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
						IsFinished = true;
					}
					return;
				}

				// Check the Lower Bracket:
				if (LowerMatches.ContainsKey(_matchNumber))
				{
					LowerMatches[_matchNumber].AddWin(_slot);

					// If Match is over:
					if (_slot == LowerMatches[_matchNumber].WinnerSlot)
					{
						int nmNumber = LowerMatches[_matchNumber].NextMatchNumber;
						if (nmNumber == GrandFinal.MatchNumber)
						{
							// Winner to Grand Final:
							GrandFinal.AddPlayer
								(LowerMatches[_matchNumber].Players[(int)_slot], PlayerSlot.Challenger);
						}
						else
						{
							// Winner to next Match:
							for (int i = 0; i < LowerMatches[nmNumber].PreviousMatchNumbers.Count; ++i)
							{
								if (_matchNumber == LowerMatches[nmNumber].PreviousMatchNumbers[i])
								{
									PlayerSlot newSlot = (1 == LowerMatches[nmNumber].PreviousMatchNumbers.Count)
										? PlayerSlot.Challenger : (PlayerSlot)i;
									LowerMatches[nmNumber].AddPlayer((PlayerSlot.Defender == _slot)
										? LowerMatches[_matchNumber].Players[(int)PlayerSlot.Defender]
										: LowerMatches[_matchNumber].Players[(int)PlayerSlot.Challenger]
										, newSlot);
									break;
								}
							}
						}

						// Add losing Player to the Rankings:
						int rank = NumberOfMatches - GetLowerRound(LowerMatches[_matchNumber].RoundIndex)[0].MatchNumber + 2;
						IPlayer losingPlayer = LowerMatches[_matchNumber].Players[
							(PlayerSlot.Defender == _slot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, rank));

						Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
					}

					return;
				}
			}

			throw new MatchNotFoundException
				("Match not found; match number may be invalid.");
#endif
		}
		public override void SubtractWin(int _matchNumber, PlayerSlot _slot)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (_slot == match.WinnerSlot)
			{
				PlayerSlot loserSlot = (PlayerSlot.Defender == _slot)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				RemovePlayerFromFutureMatches
					(nextLoserNumber, ref match.Players[(int)loserSlot]);
			}

			base.SubtractWin(_matchNumber, _slot);
#if false
			if (_slot != PlayerSlot.Defender && _slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("Player slot must be 0 or 1!");
			}

			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);
			bool needToUpdateRankings = false;

			if (_slot == match.WinnerSlot)
			{
				needToUpdateRankings = true;
				// Remove advanced players from future matches:
				PlayerSlot loserSlot = (PlayerSlot.Defender == _slot)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				RemovePlayerFromFutureMatches
					(match.NextLoserMatchNumber, ref match.Players[(int)loserSlot]);
				RemovePlayerFromFutureMatches
					(match.NextMatchNumber, ref match.Players[(int)_slot]);
			}

			// Subtract the win and update rankings:
			GetMatch(_matchNumber).SubtractWin(_slot);
			if (needToUpdateRankings)
			{
				IsFinished = false;
				UpdateRankings();
			}
#endif
#if false
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}
			if (_slot != PlayerSlot.Defender && _slot != PlayerSlot.Challenger)
			{
				throw new InvalidSlotException
					("Player slot must be 0 or 1!");
			}

			bool needToUpdateRankings = (_slot == GetMatch(_matchNumber).WinnerSlot);

			// Check the Upper Bracket:
			if (Matches.ContainsKey(_matchNumber))
			{
				if (_slot == Matches[_matchNumber].WinnerSlot)
				{
					PlayerSlot loserSlot = (_slot == PlayerSlot.Defender)
						? PlayerSlot.Challenger : PlayerSlot.Defender;
					// Remove any advanced Players from future matches:
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextMatchNumber,
						ref Matches[_matchNumber].Players[(int)_slot]);
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextLoserMatchNumber,
						ref Matches[_matchNumber].Players[(int)loserSlot]);
				}

				Matches[_matchNumber].SubtractWin(_slot);
			}
			// Check the Lower Bracket:
			else if (LowerMatches.ContainsKey(_matchNumber))
			{
				if (_slot == LowerMatches[_matchNumber].WinnerSlot)
				{
					// Remove any advanced Players from future matches:
					RemovePlayerFromFutureMatches
						(LowerMatches[_matchNumber].NextMatchNumber,
						ref LowerMatches[_matchNumber].Players[(int)_slot]);
				}

				LowerMatches[_matchNumber].SubtractWin(_slot);
			}
			// Check the Grand Final:
			else if (GrandFinal.MatchNumber == _matchNumber)
			{
				GrandFinal.SubtractWin(_slot);
			}
			else
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}

			if (needToUpdateRankings)
			{
				IsFinished = false;
				UpdateRankings();
			}
#endif
		}

		public override void ResetMatchScore(int _matchNumber)
		{
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.IsFinished)
			{
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				RemovePlayerFromFutureMatches
					(match.NextLoserMatchNumber, ref match.Players[(int)loserSlot]);
			}

			base.ResetMatchScore(_matchNumber);
#if false
			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);
			bool needToUpdateRankings = false;

			if (match.IsFinished)
			{
				needToUpdateRankings = true;
				// Remove advanced players from future matches:
				PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
					? PlayerSlot.Challenger
					: PlayerSlot.Defender;
				RemovePlayerFromFutureMatches
					(match.NextMatchNumber, ref match.Players[(int)match.WinnerSlot]);
				RemovePlayerFromFutureMatches
					(match.NextLoserMatchNumber, ref match.Players[(int)loserSlot]);
			}

			// Reset score and update rankings:
			GetMatch(_matchNumber).ResetScore();
			if (needToUpdateRankings)
			{
				IsFinished = false;
				UpdateRankings();
			}
#endif
#if false
			if (_matchNumber < 1)
			{
				throw new InvalidIndexException
					("Match number cannot be less than 1!");
			}

			bool needToUpdateRankings = GetMatch(_matchNumber).IsFinished;

			// Check the Grand Final:
			if (GrandFinal.MatchNumber == _matchNumber)
			{
				GrandFinal.ResetScore();
			}
			// Check the Upper Bracket:
			else if (Matches.ContainsKey(_matchNumber))
			{
				if (Matches[_matchNumber].IsFinished)
				{
					PlayerSlot loserSlot = (PlayerSlot.Defender == Matches[_matchNumber].WinnerSlot)
						? PlayerSlot.Challenger : PlayerSlot.Defender;
					// Remove any advanced Players from future Matches
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextMatchNumber,
						ref Matches[_matchNumber].Players[(int)(Matches[_matchNumber].WinnerSlot)]);
					RemovePlayerFromFutureMatches
						(Matches[_matchNumber].NextLoserMatchNumber,
						ref Matches[_matchNumber].Players[(int)loserSlot]);
				}

				Matches[_matchNumber].ResetScore();
			}
			// Check the Lower Bracket:
			else if (LowerMatches.ContainsKey(_matchNumber))
			{
				if (LowerMatches[_matchNumber].IsFinished)
				{
					// Remove any advanced Players from future Matches
					RemovePlayerFromFutureMatches
						(LowerMatches[_matchNumber].NextMatchNumber,
						ref LowerMatches[_matchNumber].Players[(int)(LowerMatches[_matchNumber].WinnerSlot)]);
				}

				LowerMatches[_matchNumber].ResetScore();
			}
			else
			{
				throw new MatchNotFoundException
					("Match not found; match number may be invalid.");
			}

			if (needToUpdateRankings)
			{
				IsFinished = false;
				UpdateRankings();
			}
#endif
		}
#endregion

#region Private Methods
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

		protected override void RemovePlayerFromFutureMatches(int _matchNumber, ref IPlayer _player)
		{
			if (_matchNumber < 1 || null == _player)
			{
				return;
			}

			int nextWinnerNumber;
			int nextLoserNumber;
			IMatch match = GetMatchData(_matchNumber, out nextWinnerNumber, out nextLoserNumber);

			if (match.Players.Contains(_player))
			{
				if (match.IsFinished)
				{
					PlayerSlot loserSlot = (PlayerSlot.Defender == match.WinnerSlot)
						? PlayerSlot.Challenger
						: PlayerSlot.Defender;

					RemovePlayerFromFutureMatches
						(nextWinnerNumber, ref match.Players[(int)(match.WinnerSlot)]);
					RemovePlayerFromFutureMatches
						(nextLoserNumber, ref match.Players[(int)loserSlot]);
				}

				GetMatch(_matchNumber).RemovePlayer(_player.Id);
			}
#if false
			// Check the Grand Final:
			if (GrandFinal.MatchNumber == _matchNumber)
			{
				try
				{
					GrandFinal.RemovePlayer(_player.Id);
				}
				catch (PlayerNotFoundException)
				{ }
				return;
			}
			// Check the Upper Bracket:
			else if (Matches.ContainsKey(_matchNumber))
			{
				if (Matches[_matchNumber].Players.Contains(_player))
				{
					if (Matches[_matchNumber].IsFinished)
					{
						PlayerSlot loserSlot = (PlayerSlot.Defender == Matches[_matchNumber].WinnerSlot)
							? PlayerSlot.Challenger : PlayerSlot.Defender;
						// Remove any advanced Players from future Matches:
						RemovePlayerFromFutureMatches
							(Matches[_matchNumber].NextMatchNumber,
							ref Matches[_matchNumber].Players[(int)(Matches[_matchNumber].WinnerSlot)]);
						RemovePlayerFromFutureMatches
							(Matches[_matchNumber].NextLoserMatchNumber,
							ref Matches[_matchNumber].Players[(int)loserSlot]);
					}

					Matches[_matchNumber].RemovePlayer(_player.Id);
				}
			}
			// Check the Lower Bracket:
			else if (LowerMatches.ContainsKey(_matchNumber))
			{
				if (LowerMatches[_matchNumber].Players.Contains(_player))
				{
					// Remove any advanced Players from future Matches:
					if (LowerMatches[_matchNumber].IsFinished)
					{
						RemovePlayerFromFutureMatches
							(LowerMatches[_matchNumber].NextMatchNumber,
							ref LowerMatches[_matchNumber].Players[(int)(LowerMatches[_matchNumber].WinnerSlot)]);
					}

					LowerMatches[_matchNumber].RemovePlayer(_player.Id);
				}
			}
			else
			{
				throw new MatchNotFoundException
					("Recursive method called an invalid match number.");
			}
#endif
		}

		protected override void UpdateRankings()
		{
			Rankings.Clear();

			for (int r = 1; r <= NumberOfLowerRounds; ++r)
			{
				int numberFinishedMatches = 0;
				List<IMatch> round = GetLowerRound(r);
				int rank = NumberOfMatches - round[0].MatchNumber + 2;

				foreach (IMatch match in round)
				{
					if (match.IsFinished)
					{
						++numberFinishedMatches;
						// Add losing Player to the Rankings:
						IPlayer losingPlayer = match.Players[
							(PlayerSlot.Defender == match.WinnerSlot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, rank));
					}
				}
				if (numberFinishedMatches < 2)
				{
					break;
				}
			}

			// Special case check: DEB has a play-in round
			if (Matches[1].NextLoserMatchNumber < 1)
			{
				int rank = (int)(Math.Pow(2, NumberOfRounds - 1) + 1);

				foreach (IMatch match in GetRound(1))
				{
					if (match.IsFinished)
					{
						IPlayer losingPlayer = match.Players[
							(PlayerSlot.Defender == match.WinnerSlot)
							? (int)PlayerSlot.Challenger
							: (int)PlayerSlot.Defender];
						Rankings.Add(new PlayerScore(losingPlayer.Id, losingPlayer.Name, -1, rank));
					}
				}
			}

			Rankings.Sort((first, second) => first.Rank.CompareTo(second.Rank));
		}
#endregion
	}
}
