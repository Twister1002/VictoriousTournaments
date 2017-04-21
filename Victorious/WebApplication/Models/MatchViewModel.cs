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
            Match = new Match();
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

        public IPlayer Challenger()
        {
            return Match.Players[(int)PlayerSlot.Challenger] != null ? Match.Players[(int)PlayerSlot.Challenger] : new User() { Name = "Unknown Challenger" };
        }

        public int ChallengerScore()
        {
            return Match.Score[(int)PlayerSlot.Challenger];
        }

        public IPlayer Defender()
        {
            return Match.Players[(int)PlayerSlot.Defender] != null ? Match.Players[(int)PlayerSlot.Defender] : new User() { Name = "Unknown Defender" };
        }

        public int DefenderScore()
        {
            return Match.Score[(int)PlayerSlot.Defender];
        }
    }
}