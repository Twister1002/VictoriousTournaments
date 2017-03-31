using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class MatchViewModel : MatchFields
    {
        public MatchModel matchModel { get; private set; }
        public BracketModel bracketModel { get; private set; }
        public TournamentModel tournyModel { get; private set; }

        public MatchViewModel()
        {
            matchModel = new MatchModel();
        }

        public MatchViewModel(int tournamentId, int matchId)
        {
            tournyModel = db.GetTournamentById(tournamentId);
            matchModel = db.GetMatchById(matchId);
            SetFields();
        }

        public override void ApplyChanges(int userId)
        {
            matchModel.ChallengerScore  = this.ChallengerScore;
            matchModel.DefenderScore    = this.DefenderScore;
            matchModel.WinnerID         = this.WinnerID;
        }

        public override void SetFields()
        {
            this.ChallengerScore    = matchModel.ChallengerScore;
            this.DefenderScore      = matchModel.DefenderScore;
            this.WinnerID           = matchModel.WinnerID;
        }
    }
}