﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataLib;

namespace Tournament.Structure
{
	public class Tournament : ITournament
	{
		#region Variables & Properties
		public string Title
		{ get; set; }
		public string Description
		{ get; set; }
		public List<IPlayer> Players
		{ get; private set; }
		public List<IBracket> Brackets
		{ get; private set; }
		public float PrizePool
		{ get; set; }
		public bool IsPublic
		{ get; set; }
		#endregion

		#region Ctors
		public Tournament(string _title, string _description, List<IPlayer> _players, List<IBracket> _brackets, float _pool, bool _isPublic)
		{
			Title = _title;
			Description = _description;
			Players = _players;
			Brackets = _brackets;
			PrizePool = _pool;
			IsPublic = _isPublic;
		}
		public Tournament()
			: this("", "", new List<IPlayer>(), new List<IBracket>(), 0.0f, false)
		{ }
		public Tournament(TournamentModel _model)
		{
			if (null == _model)
			{
				throw new NullReferenceException
					("Tournament model cannot be null!");
			}

			Title = _model.Title;
			Description = _model.Description;

			Players = new List<IPlayer>();
			foreach (UserModel model in _model.Users)
			{
				Players.Add(new User(model));
			}

			Brackets = new List<IBracket>();
			foreach (BracketModel bModel in _model.Brackets)
			{
				switch ((BracketTypeModel.BracketType)bModel.BracketType.BracketTypeID)
				{
					case (BracketTypeModel.BracketType.SINGLE):
						AddBracket(new SingleElimBracket(bModel));
						break;
					case (BracketTypeModel.BracketType.DOUBLE):
						AddBracket(new DoubleElimBracket(bModel));
						break;
					// More here eventually...
					default:
						break;
				}
			}

			PrizePool = (null == _model.TournamentRules.PrizePurse)
				? 0.0f : (float)(_model.TournamentRules.PrizePurse);
			IsPublic = (null == _model.TournamentRules.IsPublic)
				? false : (bool)(_model.TournamentRules.IsPublic);
		}
		#endregion

		#region Public Methods
		public int NumberOfPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			return Players.Count;
		}
		public void SetNewPlayerlist(List<IPlayer> _players)
		{
			if (null == _players)
			{
				throw new ArgumentNullException("_players");
			}
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}

			Players.Clear();
			Players = _players;
			foreach (IBracket bracket in Brackets)
			{
				bracket.ResetPlayers();
			}
		}
		public void AdvancePlayersByRanking(int _initialBracketIndex, int _newBracketIndex, int _numberOfPlayers = 0)
		{
			if (_initialBracketIndex < 0 || _initialBracketIndex >= Brackets.Count)
			{
				throw new InvalidIndexException
					("Initial Bracket Index is out of range!");
			}
			if (_newBracketIndex < 0 || _newBracketIndex >= Brackets.Count)
			{
				throw new InvalidIndexException
					("New Bracket Index is out of range!");
			}
			if (!Brackets[_initialBracketIndex].IsFinished)
			{
				throw new BracketException
					("Can't retrieve seeds from an unfinished bracket!");
			}

			int maxPlayers = (_numberOfPlayers > 0)
				? _numberOfPlayers
				: Brackets[_initialBracketIndex].Rankings.Count;
			List<IPlayer> pList = new List<IPlayer>();
			foreach (IPlayerScore pScore in Brackets[_initialBracketIndex].Rankings)
			{
				pList.Add(Brackets[_initialBracketIndex].Players
					.Find(p => p.Id == pScore.Id));
				if (pList.Count == maxPlayers)
				{
					break;
				}
			}

			Brackets[_newBracketIndex].SetNewPlayerlist(pList);
		}
		public void AddPlayer(IPlayer _player)
		{
			if (null == _player)
			{
				throw new NullReferenceException
					("New Player cannot be null!");
			}
			if (null == Players)
			{
				throw new NullReferenceException
					("Playerlist is null; this shouldn't happen...");
			}
			if (Players.Contains(_player))
			{
				throw new DuplicateObjectException
					("Tournament already contains this Player!");
			}

			Players.Add(_player);
		}
		public void ReplacePlayer(IPlayer _player, int _index)
		{
			if (null == _player)
			{
				throw new NullReferenceException
					("New Player cannot be null!");
			}
			if (_index < 0 || _index >= Players.Count)
			{
				throw new InvalidIndexException
					("Can't replace; Index is out of playerlist's bounds!");
			}

			if (null != Players[_index])
			{
				int pId = Players[_index].Id;
				foreach (IBracket bracket in Brackets)
				{
					for (int i = 0; i < bracket.Players.Count; ++i)
					{
						if (bracket.Players[i].Id == pId)
						{
							bracket.ReplacePlayer(_player, i);
							break;
						}
					}
				}
			}

			Players[_index] = _player;
		}
		public void RemovePlayer(IPlayer _player)
		{
			if (null == _player)
			{
				throw new NullReferenceException
					("Parameter cannot be null!");
			}
			if (null == Players)
			{
				throw new NullReferenceException
					("Playerlist is null; this shouldn't happen...");
			}
			if (!Players.Remove(_player))
			{
				throw new PlayerNotFoundException
					("Player not found in this tournament!");
			}
		}
		public void ResetPlayers()
		{
			if (null == Players)
			{
				Players = new List<IPlayer>();
			}
			Players.Clear();
		}

		public int NumberOfBrackets()
		{
			if (null == Brackets)
			{
				Brackets = new List<IBracket>();
			}
			return Brackets.Count;
		}
		public void AddBracket(IBracket _bracket)
		{
			if (null == _bracket)
			{
				throw new NullReferenceException
					("New bracket cannot be null!");
			}
			if (null == Brackets)
			{
				throw new NullReferenceException
					("Bracket list is null; this shouldn't happen...");
			}
			if (Brackets.Contains(_bracket))
			{
				throw new DuplicateObjectException
					("Tournament already contains this Bracket!");
			}

			Brackets.Add(_bracket);
		}
		public void RemoveBracket(IBracket _bracket)
		{
			if (null == _bracket)
			{
				throw new NullReferenceException
					("Parameter cannot be null!");
			}
			if (null == Brackets)
			{
				throw new NullReferenceException
					("Bracket list is null; this shouldn't happen...");
			}
			if (!Brackets.Remove(_bracket))
			{
				throw new BracketNotFoundException
					("Bracket not found in this tournament!");
			}
		}
		public void ResetBrackets()
		{
			if (null == Brackets)
			{
				Brackets = new List<IBracket>();
			}
			Brackets.Clear();
		}

		#region Bracket Creation Methods
		public void AddSingleElimBracket(List<IPlayer> _playerList)
		{
			Brackets.Add(new SingleElimBracket(_playerList));
		}
#if false
		public void AddSingleElimBracket(int _numPlayers)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddSingleElimBracket(pList);
		}
#endif
		public void AddDoubleElimBracket(List<IPlayer> _playerList)
		{
			Brackets.Add(new DoubleElimBracket(_playerList));
		}
#if false
		public void AddDoubleElimBracket(int _numPlayers)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddDoubleElimBracket(pList);
		}
#endif

		public void AddRoundRobinBracket(List<IPlayer> _playerList, int _numRounds = 0)
		{
			Brackets.Add(new RoundRobinBracket(_playerList, _numRounds));
		}
#if false
		public void AddRoundRobinBracket(int _numPlayers, int _numRounds = 0)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddRoundRobinBracket(pList, _numRounds);
		}
#endif
		public void AddGroupStageBracket(List<IPlayer> _playerList, int _numGroups = 2)
		{
			Brackets.Add(new RoundRobinGroups(_playerList, _numGroups));
		}
#if false
		public void AddGroupStageBracket(int _numPlayers, int _numGroups = 2)
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < _numPlayers; ++i)
			{
				pList.Add(new User());
			}
			AddGroupStageBracket(pList, _numGroups);
		}
#endif
		#endregion
		#endregion

		#region Private Methods

		#endregion
	}
}
