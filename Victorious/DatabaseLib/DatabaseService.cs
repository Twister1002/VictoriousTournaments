using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class DatabaseService
    {
        IUnitOfWork unitOfWork;
        IRepository<AccountInviteModel> accountInviteRepo;
        IRepository<AccountModel> accountRepo;
        IRepository<BracketModel> bracketRepo;
        IRepository<BracketTypeModel> bracketTypeRepo;
        IRepository<GameModel> gameRepo;
        IRepository<GameTypeModel> gameTypeModel;
        IRepository<MatchModel> matchRepo;
        IRepository<PlatformModel> platformRepo;
        IRepository<TournamentModel> tournamentRepo;
        IRepository<TournamentUserModel> tournamentUserRepo;
        IRepository<TournamentInviteModel> tournamentInviteRepo;
        IRepository<TournamentUsersBracketModel> tournamentUsersBracketRepo;
        
        public DatabaseService()
        {
           
        }


    }
}
