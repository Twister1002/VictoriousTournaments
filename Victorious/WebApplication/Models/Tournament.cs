using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Tournaments = Tournament.Structure;
using WebApplication.Models.ViewModels;
using WebApplication.Utility;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
	public class Tournament : Model, IViewModel<TournamentViewModel>
	{
		private int maxBrackets = 2;
		private bool TempFixMatchObjects = true;
		private Tournaments.ITournament Tourny;
		public TournamentViewModel viewModel { get; private set; }
		public TournamentModel Model { get; private set; }
		public List<TournamentModel> searched { get; private set; }

		public Tournament(IService service, int id) : base(service)
		{
			Retrieve(id);
			Init();
		}

		private void Init()
		{
			if (TempFixMatchObjects) MatchObjectFix();

			// Create the tournament
			Tourny = new Tournaments.Tournament(Model);

			searched = new List<TournamentModel>();
			SetupViewModel();
		}

		/// <summary>
		/// Gathers the information from the user and searches all tournaments on data given
		/// </summary>
		/// <param name="searchData">Key value pairs of information to search for.</param>
		public void Search(Dictionary<String, String> searchData)
		{
			if (searchData == null)
			{
				searchData = new Dictionary<String, String>();
			}

			List<String> safeParamList = new List<String>() {
				"Title",
				"GameTypeID",
				"PlatformTypeID",
				"PublicViewing",
				"PublicRegistration",
				"TournamentStartDate",
				"TournamentEndDate",
				"RegistrationStartDate",
				"RegistrationEndDate"
			};

			searchData = searchData.Where(k => safeParamList.Contains(k.Key) && k.Value != String.Empty).ToDictionary(k => k.Key, k => k.Value);

			searchData.Add("PublicViewing", "true");
			if (searchData.Keys.Contains("startDate"))
			{
				searchData.Add("TournamentStartDate", searchData["startDate"]);
				searchData.Add("RegistrationStartDate", searchData["startDate"]);

				if (searchData.Keys.Contains("endDate"))
				{
					searchData.Add("TournamentEndDate", DateTime.Now.ToShortDateString());
					searchData.Add("RegistrationEndDate", DateTime.Now.ToShortDateString());
				}
				else
				{
					searchData.Add("TournamentEndDate", DateTime.Now.ToShortDateString());
					searchData.Add("RegistrationEndDate", DateTime.Now.ToShortDateString());
				}
			}

			searched = services.Tournament.FindTournaments(searchData);
		}

		/// <summary>
		/// An accessor to get the tournament's core data
		/// </summary>
		/// <returns>An ITournament without a wrapper</returns>
		public Tournaments.ITournament GetTournament()
		{
			return Tourny;
		}

		#region Bracket
		/// <summary>
		/// Gets the bracket from the loaded tournament.
		/// </summary>
		/// <param name="bracketId">ID of the bracket</param>
		/// <returns>A Bracket wrapper class</returns>
		public Bracket GetBracket(int bracketId)
		{
			// Get the current bracket
			Tournaments.IBracket bracket = Tourny.Brackets.Single(x => x.Id == bracketId);

			return new Bracket(
				services,
				bracket,
				Model.Brackets.Single(x => x.BracketID == bracketId)
			);
		}

		/// <summary>
		/// Gathers all the brackets loaded into the Tournament class
		/// </summary>
		/// <returns>A List of all brackets in the tournament</returns>
		public List<Bracket> GetBrackets()
		{
			List<Bracket> brackets = new List<Bracket>();
			for (int i = 0; i < Tourny.Brackets.Count; i++)
			{
				Tournaments.IBracket bracket = Tourny.Brackets.ElementAt(i);
				Tournaments.IBracket nextBracket = Tourny.Brackets.ElementAtOrDefault(i + 1);

				brackets.Add(
					new Bracket(
						services,
						bracket,
						Model.Brackets.Single(x => x.BracketID == bracket.Id)
					)
				);
			}

			return brackets;
		}
		#endregion

		#region Match
		public Match GetMatch(int bracketId, int matchId)
		{
			int matchNum = Model.Brackets.Single(x => x.BracketID == bracketId).Matches.Single(x => x.MatchID == matchId).MatchNumber;
			return new Match(services, Tourny.Brackets.Single(x => x.Id == bracketId).GetMatch(matchNum));
		}
		#endregion

		#region Finalize
		/// <summary>
		/// This will finalize a bracket.
		/// </summary>
		/// <param name="bracketId">The ID of the bracket</param>
		/// <param name="roundData">The data that will be used to set the max amount of games.</param>
		/// <returns>True if the bracket was finalized or false if failed to save</returns>
		public bool FinalizeBracket(int bracketId, Dictionary<String, Dictionary<String, int>> roundData)
		{
			// Set variables
			BracketModel bracketModel = Model.Brackets.Single(x => x.BracketID == bracketId);
			Tournaments.IBracket bracket = Tourny.Brackets.Single(x => x.Id == bracketId);

			// Set max games for every round
			foreach (KeyValuePair<String, Dictionary<String, int>> roundInfo in roundData)
			{
				foreach (KeyValuePair<String, int> data in roundInfo.Value)
				{
					switch (roundInfo.Key)
					{
						case "upper":
							bracket.SetMaxGamesForWholeRound(int.Parse(data.Key), data.Value);
							break;
						case "lower":
							bracket.SetMaxGamesForWholeLowerRound(int.Parse(data.Key), data.Value);
							break;
						case "final":
							bracket.SetMaxGamesForWholeRound(int.Parse(data.Key), data.Value);
							break;
					}
				}
			}

			if (bracket.Validate())
			{
				// Update the necesarry information
				bracketModel.Finalized = true;
				Model.InProgress = true;

				// Update the database
				bracketModel.Matches = bracket.GetModel(Model.TournamentID).Matches;
				services.Tournament.UpdateBracket(bracketModel);
				services.Tournament.UpdateTournament(Model);
				return services.Save();
			}
			else
			{
				return false;
			}
		}

		private List<MatchModel> CreateMatches(Tournaments.IBracket bracket)
		{
			List<MatchModel> matches = new List<MatchModel>();

			// Add the matches to the database
			for (int i = 1; i <= bracket.NumberOfMatches; i++)
			{
				matches.Add(bracket.GetMatchModel(i));
			}

			return matches;
		}

		/// <summary>
		/// Will progress the tournament and add new players to the net bracket
		/// </summary>
		/// <param name="bracketId">ID of the bracket</param>
		/// <param name="isLocked">Lock or unlock the bracket</param>
		/// <returns>True if saved; false if not saved</returns>
		public bool LockBracket(int bracketId, bool isLocked)
		{
			Tournaments.IBracket currentBracket = Tourny.Brackets.Single(x => x.Id == bracketId);
			Tournaments.IBracket nextBracket = Tourny.Brackets.ElementAtOrDefault(Tourny.Brackets.FindIndex(x => x == currentBracket) + 1);

			// Make changes to the current bracket
			if (currentBracket != null)
			{
				BracketModel model = Model.Brackets.Single(x => x.BracketID == bracketId);
				model.IsLocked = isLocked;

				services.Tournament.UpdateBracket(model);
			}

			if (nextBracket != null)
			{
				BracketModel nextBracketModel = Model.Brackets.Single(x => x.BracketID == nextBracket.Id);

				List<TournamentUsersBracketModel> usersToProceed = new List<TournamentUsersBracketModel>();
				Tourny.AdvancePlayersByRanking(currentBracket, nextBracket);

				//nextBracketModel.TournamentUsersBrackets = nextBracket.GetModel(Model.TournamentID).TournamentUsersBrackets;

				foreach (Tournaments.IPlayer player in nextBracket.Players)
				{
					TournamentUsersBracketModel user = new TournamentUsersBracketModel()
					{
						BracketID = nextBracket.Id,
						TournamentID = Model.TournamentID,
						TournamentUserID = player.Id,
						Seed = nextBracket.GetPlayerSeed(player.Id)
					};

					usersToProceed.Add(user);
				}

				nextBracketModel.TournamentUsersBrackets = usersToProceed;
				services.Tournament.UpdateBracket(nextBracketModel);
			}

			return services.Save();
		}
		#endregion

		#region CRUD
		//TODO: Fix issue where tournamentCodes can collide and be repeatable.
		public bool Create(TournamentViewModel viewModel, Account account)
		{
			ApplyChanges(viewModel);
			// Exit the create if we detect there is an exception in the viewModel.
			if (viewModel.e != null)
			{
				SetupViewModel(viewModel);
				return false;
			}

			Model.LastEditedOn = DateTime.Now;
			Model.LastEditedByID = account.Model.AccountID;
			Model.CreatedOn = DateTime.Now;
			Model.CreatedByID = account.Model.AccountID;

			// Generate the Tournament Invite Codes
			Model.InviteCode = Codes.GenerateInviteCode();

			//Save the tournament First.
			services.Tournament.AddTournament(Model);
			AddUser(account, Permission.TOURNAMENT_CREATOR);
			//if (viewModel.BracketData != null) UpdateBrackets();
			bool tournamentSave = services.Save();

			// Create InviteModel
			TournamentInviteModel inviteModel = new TournamentInviteModel()
			{
				TournamentInviteCode = Model.InviteCode,
				DateCreated = DateTime.Now,
				TournamentID = Model.TournamentID,
				NumberOfUses = 0
			};

			// Add the Invite Model

			services.Tournament.AddTournamentInvite(inviteModel);
			bool TournamentInviteSave = services.Save();

			return tournamentSave && TournamentInviteSave;
		}

		public void Retrieve(int id)
		{
			Model = services.Tournament.GetTournament(id);
			if (Model == null) Model = new TournamentModel();
		}

		public bool Update(TournamentViewModel viewModel, int accountId)
		{
			ApplyChanges(viewModel);
			// Exit the create if we detect there is an exception in the viewModel.
			if (viewModel.e != null)
			{
				SetupViewModel(viewModel);
				return false;
			}
			Model.LastEditedByID = accountId;
			Model.LastEditedOn = DateTime.Now;

			services.Tournament.UpdateTournament(Model);

			if (services.Save())
			{
				return true;
			}
			else
			{
				SetFields();
				return false;
			}
		}

		public bool Delete()
		{
			List<BracketModel> brackets = Model.Brackets.ToList();
			List<TournamentUserModel> tournamentUsers = Model.TournamentUsers.ToList();

			// Delete all the brackets
			foreach (BracketModel bracket in brackets)
			{
				List<TournamentUsersBracketModel> users = bracket.TournamentUsersBrackets.ToList();

				// Delete all the seeded users
				foreach (TournamentUsersBracketModel user in users)
				{
					services.Tournament.DeleteTournamentUsersBracket(user.TournamentUserID, bracket.BracketID);
				}

				services.Tournament.DeleteBracket(bracket.BracketID);
			}

			// Delete all the users
			foreach (TournamentUserModel user in tournamentUsers)
			{
				services.Tournament.DeleteTournamentUser(user.TournamentUserID);
			}

			// Delete the Invite code
			services.Tournament.DeleteTournamentInvite(Model.InviteCode);

			// Delete the tournament
			services.Tournament.DeleteTournament(Model.TournamentID);

			return services.Save();
		}

		public void UpdateBrackets()
		{
			int updates = Math.Max(viewModel.BracketData.Count, Model.Brackets.Count);
			List<BracketModel> updatedBrackets = new List<BracketModel>();

			for (int i = 0; i < updates; i++)
			{
				BracketViewModel newBracket = viewModel.BracketData.ElementAtOrDefault(i);
				BracketModel bracketModel = Model.Brackets.ElementAtOrDefault(i);
				//List<TournamentUsersBracketModel> users = new List<TournamentUsersBracketModel>();

				if (newBracket != null)
				{
					if (bracketModel != null)
					{
						// We just need to update the data
						bracketModel.BracketTypeID = newBracket.BracketTypeID;
						bracketModel.MaxRounds = newBracket.NumberOfRounds;
						bracketModel.NumberOfGroups = newBracket.NumberOfGroups;
						bracketModel.NumberPlayersAdvance = newBracket.NumberPlayersAdvance;

						updatedBrackets.Add(bracketModel);
						//services.Tournament.UpdateBracket(bracketModel);
					}
					else if (bracketModel == null)
					{
						// We need to add this bracket
						bracketModel = new BracketModel()
						{
							MaxRounds = newBracket.NumberOfRounds,
							BracketTypeID = newBracket.BracketTypeID,
							NumberOfGroups = newBracket.NumberOfGroups,
							NumberPlayersAdvance = newBracket.NumberPlayersAdvance,
							Finalized = false,
							TournamentID = Model.TournamentID
						};

						updatedBrackets.Add(bracketModel);
						//services.Tournament.AddBracket(bracketModel);
					}
				}
			}

			Model.Brackets = updatedBrackets;
		}

		private void UpdatePlayers()
		{
			foreach (BracketModel bracketModel in Model.Brackets)
			{
				// Update the Players
				if (bracketModel.TournamentUsersBrackets.Count != Model.TournamentUsers.Where(x => IsUserParticipant(x.TournamentUserID)).Count())
				{
					for (int j = 0; j < Model.TournamentUsers.Count; j++)
					{
						TournamentUserModel user = Model.TournamentUsers.ElementAt(j);
						TournamentUsersBracketModel model = new TournamentUsersBracketModel()
						{
							TournamentUserID = user.TournamentUserID,
							BracketID = bracketModel.BracketID,
							TournamentID = Model.TournamentID,
							Seed = j + 1
						};
						//bracketModel.TournamentUsersBrackets.Add(model);

						if (!IsUserAdministrator(user.TournamentUserID))
						{
							services.Tournament.AddTournamentUsersBracket(model);
						}
					}
				}
			}
		}

		public void UpdateSeeds(Dictionary<String, int> players, int bracketId)
		{
			foreach (KeyValuePair<String, int> player in players)
			{
				TournamentUsersBracketModel user = Model.Brackets.Single(x => x.BracketID == bracketId)
					.TournamentUsersBrackets.SingleOrDefault(z => z.TournamentUserID == int.Parse(player.Key));
				if (user != null)
				{
					user.Seed = player.Value;
					services.Tournament.UpdateTournamentUsersBracket(user);
				}
				else
				{
					user = new TournamentUsersBracketModel()
					{
						Seed = player.Value,
						TournamentUserID = int.Parse(player.Key),
						TournamentID = Model.TournamentID,
						BracketID = bracketId
					};

					services.Tournament.AddTournamentUsersBracket(user);
				}
			}

			// Pass updated seeds to IBracket, which will fix any invalid values:
			Tourny.Brackets.First()
				.SetNewPlayerlist(Model.Brackets.First().TournamentUsersBrackets);

			// Give correct values to all user seeds in the first BracketModel.
			// This prevents two users having the same seed and being randomly sorted.
			BracketModel bracketModel = Model.Brackets.First();
			foreach (TournamentUsersBracketModel userModel in bracketModel.TournamentUsersBrackets)
			{
				int newPlayerSeed = Tourny.Brackets
					.Single(b => b.Id == bracketModel.BracketID)
					.GetPlayerSeed(userModel.TournamentUserID);

				if (newPlayerSeed != userModel.Seed)
				{
					userModel.Seed = newPlayerSeed;
					services.Tournament.UpdateTournamentUsersBracket(userModel);
				}
			}
#if false
			foreach (TournamentTeamBracketModel teamModel in bracketModel.TournamentTeamBrackets)
			{
				int newPlayerSeed = Tourny.Brackets
					.Single(b => b.Id == bracketModel.BracketID)
					.GetPlayerSeed(teamModel.TournamentTeamID);

				if (newPlayerSeed != teamModel.Seed)
				{
					teamModel.Seed = newPlayerSeed;
					services.Tournament.UpdateTournamentTeamsBracket(teamModel);
				}
			}
#endif

			services.Save();
		}
		#endregion

		#region AddUsers
		/// <summary>
		/// Adds a user to the tournament
		/// </summary>
		/// <param name="name">The name of the user</param>
		/// <returns>The user model of the created user</returns>
		public TournamentUserModel AddUser(String name)
		{
			bool isEmail = false;
			TournamentUserModel userModel = null;

			if (!isEmail)
			{
				userModel = new TournamentUserModel()
				{
					Name = name,
					PermissionLevel = (int)Permission.TOURNAMENT_STANDARD,
					TournamentID = Model.TournamentID
				};
			}

			return AddUserToTournament(userModel) ? userModel : null;
		}

		public bool AddUser(Account account, Permission permission)
		{
			// Verify this user doesn't exist in the tournament
			if (!Model.TournamentUsers.Any(x => x.AccountID == account.Model.AccountID))
			{
				TournamentUserModel tournamentUserModel = new TournamentUserModel()
				{
					AccountID = account.Model.AccountID,
					Name = account.Model.Username,
					PermissionLevel = (int)permission,
					TournamentID = Model.TournamentID
				};

				return AddUserToTournament(tournamentUserModel);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Adds a user to the tournament of the first bracket.
		/// </summary>
		/// <param name="model">The user model to be added to the tournament</param>
		/// <returns>True if saved; false if save failed</returns>
		private bool AddUserToTournament(TournamentUserModel model)
		{
			// Add the user to the tournament
			services.Tournament.AddTournamentUser(model);

			if (model.PermissionLevel == (int)Permission.TOURNAMENT_STANDARD)
			{
				// Add user to the beginning bracket
				BracketModel bracket = Model.Brackets.ElementAt(0);
				int? seedData = bracket.TournamentUsersBrackets.Max(x => x.Seed);
				int seed = seedData != null ? seedData.Value + 1 : 1;

				TournamentUsersBracketModel bracketUser = new TournamentUsersBracketModel()
				{
					TournamentID = Model.TournamentID,
					TournamentUserID = model.TournamentUserID,
					Seed = seed,
					BracketID = bracket.BracketID
				};

				services.Tournament.AddTournamentUsersBracket(bracketUser);
			}

			return services.Save();
		}
		#endregion

		#region RemoveUsers
		/// <summary>
		/// Removes a user by the user's accuontID
		/// </summary>
		/// <param name="accountId">ID of user's accuont</param>
		/// <returns>True if saved; false is not</returns>
		public bool RemoveUser(int accountId)
		{
			TournamentUserModel user = Model.TournamentUsers.First(x => x.AccountID == accountId);

			services.Tournament.DeleteTournamentUser(user.TournamentUserID);
			return services.Save();
		}

		/// <summary>
		/// Removes a user by the name 
		/// </summary>
		/// <param name="username">The name of the user to remove</param>
		/// <returns>True if saved; false is not</returns>
		public bool RemoveUser(String username)
		{
			TournamentUserModel user = Model.TournamentUsers.First(x => x.Name == username);

			services.Tournament.DeleteTournamentUser(user.TournamentUserID);
			return services.Save();
		}
		#endregion

		#region Permissions
		#region Account
		public Permission GetAccountPermission(int accountId)
		{
			TournamentUserModel user = Model.TournamentUsers.SingleOrDefault(x => x.AccountID == accountId);
			if (user != null)
			{
				return (Permission)user.PermissionLevel;
			}

			return Permission.NONE;
		}

		public bool IsParticipent(int accountId)
		{
			Permission permission = GetAccountPermission(accountId);
			if (permission == Permission.TOURNAMENT_STANDARD ||
				permission == Permission.TOURNAMENT_ADMINISTRATOR ||
				permission == Permission.TOURNAMENT_CREATOR)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsAdmin(int accountId)
		{
			Permission permission = GetAccountPermission(accountId);
			if (permission == Permission.TOURNAMENT_ADMINISTRATOR ||
				permission == Permission.TOURNAMENT_CREATOR)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsCreator(int accountId)
		{
			return GetAccountPermission(accountId) == Permission.TOURNAMENT_CREATOR;
		}
		#endregion

		#region User
		public Permission GetUserPermission(int tournamentUserId)
		{
			TournamentUserModel user = Model.TournamentUsers.Single(x => x.TournamentUserID == tournamentUserId);
			if (user != null)
			{
				return (Permission)user.PermissionLevel;
			}

			return Permission.NONE;
		}

		public bool IsUserParticipant(int tournamentUserId)
		{
			Permission permission = GetUserPermission(tournamentUserId);
			if (permission == Permission.TOURNAMENT_STANDARD) //||
															  //    permission == Permission.TOURNAMENT_ADMINISTRATOR ||
															  //    permission == Permission.TOURNAMENT_CREATOR)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsUserAdministrator(int tournamentUserId)
		{
			Permission permission = GetUserPermission(tournamentUserId);
			if (permission == Permission.TOURNAMENT_ADMINISTRATOR ||
				permission == Permission.TOURNAMENT_CREATOR)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsUserCreator(int tournamentUserId)
		{
			Permission permission = GetUserPermission(tournamentUserId);
			if (permission == Permission.TOURNAMENT_CREATOR)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		public Dictionary<String, int> PermissionAction(int accountId, int tournamentUserId, String action)
		{
			TournamentUserModel targetAccount = Model.TournamentUsers.First(x => x.TournamentUserID == tournamentUserId);
			bool accountIsAdmin = IsAdmin(accountId);
			bool accountIsCreator = IsCreator(accountId);

			Dictionary<String, int> permissionActions = new Dictionary<string, int>();
			permissionActions.Add("Demote", 0);
			permissionActions.Add("Promote", 0);
			permissionActions.Add("Remove", 0);
			permissionActions.Add("Permission", -1);

			switch (action)
			{
				case "promote":
					switch ((Permission)targetAccount.PermissionLevel)
					{
						case Permission.TOURNAMENT_STANDARD:
							if (accountIsCreator)
							{
								targetAccount.PermissionLevel = (int)Permission.TOURNAMENT_ADMINISTRATOR;
								services.Tournament.UpdateTournamentUser(targetAccount);
								permissionActions["Demote"] = 1;
							}
							break;
						case Permission.TOURNAMENT_ADMINISTRATOR:
							break;
						case Permission.TOURNAMENT_CREATOR:
							break;
					}
					break;
				case "remove":
				case "demote":
					switch ((Permission)targetAccount.PermissionLevel)
					{
						case Permission.TOURNAMENT_STANDARD:
							if (accountIsAdmin)
							{
								targetAccount.PermissionLevel = (int)Permission.NONE;
								services.Tournament.DeleteTournamentUser(targetAccount.TournamentUserID);
							}
							break;
						case Permission.TOURNAMENT_ADMINISTRATOR:
							if (accountIsCreator)
							{
								targetAccount.PermissionLevel = (int)Permission.TOURNAMENT_STANDARD;
								services.Tournament.UpdateTournamentUser(targetAccount);
								permissionActions["Remove"] = 1;
								permissionActions["Promote"] = 1;
							}
							break;
						case Permission.TOURNAMENT_CREATOR:
							break;
					}
					break;
				default:
					switch ((Permission)targetAccount.PermissionLevel)
					{
						case Permission.TOURNAMENT_STANDARD:
							if (targetAccount.AccountID != null)
							{
								if (accountIsCreator)
								{
									permissionActions["Remove"] = 1;
									permissionActions["Promote"] = 1;
								}
								else if (accountIsAdmin)
								{
									permissionActions["Remove"] = 1;
								}
							}
							else
							{
								if (accountIsAdmin || accountIsCreator)
								{
									permissionActions["Remove"] = 1;
								}
							}
							break;
						case Permission.TOURNAMENT_ADMINISTRATOR:
							if (accountIsCreator)
							{
								permissionActions["Demote"] = 1;
							}
							break;
						case Permission.TOURNAMENT_CREATOR:
							break;
					}
					break;
			}

			permissionActions["Permission"] = targetAccount.PermissionLevel != null ? (int)targetAccount.PermissionLevel : -1;

			if (services.Save())
			{
				return permissionActions;
			}
			else
			{
				return null;
			}
		}
		#endregion

		#region Check-ins
		public bool isAccountCheckedIn(int accountId)
		{
			TournamentUserModel userModel = Model.TournamentUsers.SingleOrDefault(x => x.AccountID == accountId);
			bool checkedIn = userModel.IsCheckedIn != null ? (bool)userModel.IsCheckedIn : false;
			return checkedIn;
		}

		public bool isUserCheckedIn(int tournamentUserId)
		{
			bool checkedIn = false;
			TournamentUserModel userModel = Model.TournamentUsers.SingleOrDefault(x => x.TournamentUserID == tournamentUserId);

			if (userModel != null)
			{
				checkedIn = userModel.IsCheckedIn != null ? (bool)userModel.IsCheckedIn : false;
			}
			return checkedIn;
		}

		public bool CheckAccountIn(int accountId)
		{
			TournamentUserModel userModel = Model.TournamentUsers.SingleOrDefault(x => x.AccountID == accountId);

			services.Tournament.CheckUserIn(userModel.TournamentUserID);
			return services.Save();
		}

		public bool CheckUserIn(int tournamentUserId)
		{
			TournamentUserModel userModel = Model.TournamentUsers.SingleOrDefault(x => x.TournamentUserID == tournamentUserId);

			services.Tournament.CheckUserIn(userModel.TournamentUserID);
			return services.Save();
		}
		#endregion

		#region Helper
		/// <summary>
		/// Returns a list of users that are registered to this tournament.
		/// </summary>
		/// <returns></returns>
		public List<TournamentUserModel> GetRegisteredUsers()
		{
			return Model.TournamentUsers.OrderBy(x => x.PermissionLevel).ToList();
		}

		/// <summary>
		/// Gets the registration of the user's model.
		/// </summary>
		/// <param name="tournamentUserId">ID of the user </param>
		/// <returns>Tournament level user model or null</returns>
		public TournamentUserModel GetUserModel(int tournamentUserId)
		{
			return Model.TournamentUsers.SingleOrDefault(x => x.TournamentUserID == tournamentUserId);
		}

		/// <summary>
		/// Gets the seed of the user
		/// </summary>
		/// <param name="bracketId">The bracket's ID</param>
		/// <param name="userId">ID of the user</param>
		/// <returns>The seed value</returns>
		public int GetUserSeed(int bracketId, int userId)
		{
			int? seed = -1;

			seed = Model.Brackets.Single(x => x.BracketID == bracketId)
				.TournamentUsersBrackets.SingleOrDefault(x => x.TournamentUserID == userId)?.Seed;

			if (seed != null)
			{
				return seed.Value;
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Gets a list of all participants in the tournament.
		/// </summary>
		/// <returns></returns>
		public List<TournamentUserModel> GetParticipants()
		{
			return Model.TournamentUsers.Where(x => x.PermissionLevel == (int)Permission.TOURNAMENT_STANDARD).ToList();
		}

		/// <summary>
		/// Determins if this tournament is editable or not
		/// </summary>
		/// <returns>True if can edit; false if not</returns>
		public bool CanEdit()
		{
			return !Model.InProgress ? true : false;
		}

		/// <summary>
		/// Determins if the user is registered to the tournament
		/// </summary>
		/// <param name="accountId">The Account ID of the user</param>
		/// <returns>True if registered; false is not</returns>
		public bool isRegistered(int accountId)
		{
			return Model.TournamentUsers.Any(x => x.AccountID == accountId);
		}

		/// <summary>
		/// Determins if the user can register or not
		/// </summary>
		/// <returns>True if can register; false is not</returns>
		public bool CanRegister()
		{
			if (Model.RegistrationStartDate < DateTime.Now && Model.RegistrationEndDate > DateTime.Now)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void MatchObjectFix()
		{
			foreach (BracketModel bracket in Model.Brackets)
			{
				foreach (MatchModel match in bracket.Matches)
				{
					if (match.Defender == null)
					{
						match.Defender = services.Tournament.GetTournamentUser(match.DefenderID);
					}
					if (match.Challenger == null)
					{
						match.Challenger = services.Tournament.GetTournamentUser(match.ChallengerID);
					}
				}
			}
		}
		#endregion

		#region ViewModel
		/// <summary>
		/// This will setup the base of the viewmodel without the Model's information
		/// </summary>
		public void SetupViewModel()
		{
			viewModel = new TournamentViewModel();

			// Set the field's original data
			viewModel.BracketTypes = services.Type.GetAllBracketTypes().Where(x => x.IsActive == true).ToList();
			viewModel.GameTypes = services.Type.GetAllGameTypes();
			viewModel.PlatformTypes = services.Type.GetAllPlatforms();
			viewModel.PublicViewing = true;
			viewModel.BracketData = new List<BracketViewModel>();
			viewModel.NumberOfRounds = Enumerable.Range(0, 20).ToList();
			viewModel.NumberOfGroups = Enumerable.Range(0, 10).ToList();
			viewModel.NumberPlayersAdvance = Enumerable.Range(4, 20).ToList();

			viewModel.RegistrationStartDate = DateTime.Now;
			viewModel.RegistrationEndDate = DateTime.Now.AddDays(1);
			viewModel.TournamentStartDate = DateTime.Now.AddDays(3);
			viewModel.TournamentEndDate = DateTime.Now.AddDays(4);
			viewModel.CheckinStartDate = DateTime.Now.AddDays(1);
			viewModel.CheckinEndDate = DateTime.Now.AddDays(2);
		}

		/// <summary>
		/// Helper method to set the default data for a view model passed in.
		/// </summary>
		/// <param name="viewModel">The model to set default data</param>
		public void SetupViewModel(TournamentViewModel viewModel)
		{
			// Set the field's original data
			viewModel.BracketTypes = services.Type.GetAllBracketTypes().Where(x => x.IsActive == true).ToList();
			viewModel.GameTypes = services.Type.GetAllGameTypes();
			viewModel.PlatformTypes = services.Type.GetAllPlatforms();
			viewModel.NumberOfRounds = Enumerable.Range(0, 20).ToList();
			viewModel.NumberOfGroups = Enumerable.Range(0, 10).ToList();
			viewModel.NumberPlayersAdvance = Enumerable.Range(4, 20).ToList();

			viewModel.RegistrationStartDate = DateTime.Now;
			viewModel.RegistrationEndDate = DateTime.Now.AddDays(1);
			viewModel.TournamentStartDate = DateTime.Now.AddDays(3);
			viewModel.TournamentEndDate = DateTime.Now.AddDays(4);
			viewModel.CheckinStartDate = DateTime.Now.AddDays(1);
			viewModel.CheckinEndDate = DateTime.Now.AddDays(2);
		}

		/// <summary>
		/// This will apply the changes from the viewModel to the model for saving
		/// </summary>
		/// <param name="viewModel">Saves all data from the viewModel to the Model</param>
		public void ApplyChanges(TournamentViewModel viewModel)
		{
			// Tournament Stuff
			Model.Title = viewModel.Title;
			Model.Description = viewModel.Description;
			Model.GameTypeID = viewModel.GameTypeID;
			Model.PlatformID = viewModel.PlatformTypeID;
			Model.PublicViewing = viewModel.PublicViewing;
			Model.PublicRegistration = viewModel.PublicRegistration;

			// Adding Dates and Times
			Model.RegistrationStartDate = viewModel.RegistrationStartDate + viewModel.RegistrationStartTime.TimeOfDay;
			Model.RegistrationEndDate = viewModel.RegistrationEndDate + viewModel.RegistrationEndTime.TimeOfDay;
			Model.TournamentStartDate = viewModel.TournamentStartDate + viewModel.TournamentStartTime.TimeOfDay;
			Model.TournamentEndDate = viewModel.TournamentEndDate + viewModel.TournamentEndTime.TimeOfDay;
			Model.CheckInBegins = viewModel.CheckinStartDate + viewModel.CheckinStartTime.TimeOfDay;
			Model.CheckInEnds = viewModel.CheckinEndDate + viewModel.CheckinEndTime.TimeOfDay;

			if (viewModel.BracketData != null)
			{
				if (viewModel.BracketData.Count <= maxBrackets)
				{
					// Give the class viewModel the viewModel data
					this.viewModel.BracketData = viewModel.BracketData;

					// Add the bracket data
					UpdateBrackets();
				}
				else
				{
					viewModel.e = new Exception("You may only have " + maxBrackets + " or less brackets.");
				}
			}
		}

		/// <summary>
		/// Sets the viewModel's data based on the Model's data
		/// </summary>
		public void SetFields()
		{
			viewModel.Title = Model.Title;
			viewModel.Description = Model.Description;
			viewModel.GameTypeID = Model.GameTypeID;
			viewModel.PlatformTypeID = Model.PlatformID;
			viewModel.PublicViewing = Model.PublicViewing;
			viewModel.PublicRegistration = Model.PublicRegistration;

			// Dates
			viewModel.RegistrationStartDate = Model.RegistrationStartDate;
			viewModel.RegistrationEndDate = Model.RegistrationEndDate;
			viewModel.TournamentStartDate = Model.TournamentStartDate;
			viewModel.TournamentEndDate = Model.TournamentEndDate;
			viewModel.CheckinStartDate = Model.CheckInBegins;
			viewModel.CheckinEndDate = Model.CheckInEnds;

			// Times
			viewModel.RegistrationStartTime = Model.RegistrationStartDate;
			viewModel.RegistrationEndTime = Model.RegistrationEndDate;
			viewModel.TournamentStartTime = Model.TournamentStartDate;
			viewModel.TournamentEndTime = Model.TournamentEndDate;
			viewModel.CheckinStartTime = Model.CheckInBegins;
			viewModel.CheckinEndTime = Model.CheckInEnds;

			// Bracket data
			foreach (BracketModel bracket in Model.Brackets)
			{
				viewModel.BracketData.Add(new BracketViewModel()
				{
					BracketTypeID = bracket.BracketTypeID,
					NumberOfRounds = bracket.MaxRounds,
					NumberOfGroups = bracket.NumberOfGroups,
					NumberPlayersAdvance = bracket.NumberPlayersAdvance
				});
			}
		}
		#endregion
	}
}