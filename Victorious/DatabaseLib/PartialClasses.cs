using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;


namespace DatabaseLib
{
    public partial class VictoriousEntities : DbContext
    {
        
        public VictoriousEntities(string name) :
           base(name)
        {

        }

    }


    public partial class AccountModel
    {
        public const int FirstNameLength = 50;
        public const int LastNameLength = 50;
        public const int UsernameLength = 50;
        public const int EmailLength = 100;
        public const int PasswordLength = 255;

        partial void OnInit()
        {
            Tournaments = new Collection<TournamentModel>();
        }

        public ICollection<TournamentModel> Tournaments { get; set; }

        internal string GetFullName()
        {
            return this.FirstName + ' ' + this.LastName;
        }
    }

    public partial class AccountInviteModel
    {
        //partial void OnInit()
        //{

        //}

        internal string FullName()
        {
            return this.Account.FirstName + ' ' + this.Account.LastName;
        }
    }

    public partial class AccountSocialModel
    {
        public enum SocialProviders
        {
            FACEBOOK = 1
        }
    }

    public partial class TournamentInviteModel
    {
        //partial void OnInit()
        //{
        //    //Tournaments = new Collection<TournamentModel>();
        //}

    }

    public partial class TournamentModel
    {
        partial void OnInit()
        {
            CreatedOn = DateTime.Now;
            LastEditedOn = DateTime.Now;
        }

    }

    public partial class TournamentUserModel
    {
        partial void OnInit()
        {

        }
    }

    public partial class BracketModel
    {
        partial void OnInit()
        {
            Matches = new Collection<MatchModel>();
        }
    }

    public partial class MatchModel
    {
        partial void OnInit()
        {
            //Challenger = new TournamentUserModel();
            //Defender = new TournamentUserModel();
        }

        [NotMapped]
        public TournamentUserModel Challenger { get; set; }

        [NotMapped]
        public TournamentUserModel Defender { get; set; }


    }

    public partial class GameModel
    {
        //partial void OnInit()
        //{

        //}
    }

    /// <summary>
    /// Class that ties a TournamentUser to a specific Bracket within a Tournament. 
    /// Since a Tournament may have more than one bracket, this class allows a user to be placed in
    /// multiple Brackets without having conflicting info, such as their seed.
    /// </summary>
    public partial class TournamentUsersBracketModel
    {

    }

    /// <summary>
    /// A site-wide team. 
    /// </summary>
    public partial class SiteTeamModel
    {

    }

    /// <summary>
    /// Member of a SiteTeam.
    /// </summary>
    public partial class SiteTeamMemberModel
    {

    }
    
    /// <summary>
    /// A team that exists only as part of a Tournament. 
    /// </summary>
    public partial class TournamentTeamModel
    {

    }

    /// <summary>
    /// Ties a TournamentTeam to a specific Bracket within a Tournament. 
    /// Works very similarly to TournamentUsersBracketModel.
    /// </summary>
    public partial class TournamentTeamBracketModel
    {
        
    }

    /// <summary>
    /// Ties in with the tournament to display broadcasters with a tournament.
    /// </summary>
    public partial class TournamentBroadcasterModel
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public partial class BroadcastServiceModel
    {
        public enum BroadcastServices
        {
            TWITCH = 1
        }

        public String GetUri()
        {
            String uri = "";

            switch (this.ServiceID)
            {
                case (int)BroadcastServices.TWITCH:
                    uri = "https://player.twitch.tv/?channel={NAME}&autoplay=false";
                    break;
                default:
                    uri = "";
                    break;
            }

            return uri;
        }
    }
}
