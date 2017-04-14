using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tournament.Structure;

namespace WebApplication.Models
{
    public class MatchViewModel : MatchFields
    {
        public MatchModel Model { get; private set; }
        public IMatch Match { get; private set; }

        public MatchViewModel()
        {
            Model = new MatchModel();
        }

        public MatchViewModel(IMatch match)
        {
            Match = match;
            Model = Match.GetModel();
        }

        public MatchViewModel(int matchId)
        {
            Model = db.GetMatchById(matchId);
            if (Model != null)
            {
                Match = new Match(Model);
            }
            else
            {
                Match = new Match();
            }
        }

        public override void ApplyChanges(int userId)
        {
            Model.ChallengerScore = this.ChallengerScore;
            Model.DefenderScore = this.DefenderScore;
            Model.WinnerID = this.WinnerID;
        }

        public override void SetFields()
        {
            this.ChallengerScore = Model.ChallengerScore;
            this.DefenderScore = Model.DefenderScore;
            this.WinnerID = Model.WinnerID;
        }

        public void AddGame(int defenderScore, int challengerScore)
        {
            Match.AddGame(defenderScore, challengerScore);

            db.AddGame(Model, new GameModel()
            {
                ChallengerID = Model.ChallengerID,
                DefenderID = Model.DefenderID,
                ChallengerScore = challengerScore,
                DefenderScore = defenderScore,
                GameID = -1,
                MatchID = Model.MatchID,
                WinnerID = -1,
                GameNumber = -1
            });
        }
    }
}