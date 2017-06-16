using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<AccountInviteModel> AccountInviteRepo { get; }
        IRepository<AccountModel> AccountRepo { get; }
        IRepository<BracketModel> BracketRepo { get; }
        IRepository<BracketTypeModel> BracketTypeRepo { get; }
        IRepository<GameModel> GameRepo { get; }
        IRepository<GameTypeModel> GameTypeRepo { get; }
        IRepository<MatchModel> MatchRepo { get; }
        IRepository<PlatformModel> PlatformRepo { get; }
        IRepository<TournamentModel> TournamentRepo { get; }
        IRepository<TournamentUserModel> TournamentUserRepo { get; }
        IRepository<TournamentInviteModel> TournamentInviteRepo { get; }
        IRepository<TournamentUsersBracketModel> TournamentUsersBracketRepo { get; }
        IRepository<TournamentTeamModel> TournamentTeamRepo { get; }
        IRepository<TournamentTeamMemberModel> TournamentTeamMemberRepo { get; }
        IRepository<TournamentTeamBracketModel> TournamentTeamBracketRepo { get; }
        IRepository<SiteTeamModel> SiteTeamRepo { get; }
        IRepository<SiteTeamMemberModel> SiteTeamMemberRepo { get; }
        IRepository<MailingList> MailingListRepo { get; }

        void Refresh();

        bool Save();



    }
}