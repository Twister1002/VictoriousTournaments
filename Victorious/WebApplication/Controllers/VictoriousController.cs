using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tournament.Structure;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public abstract class VictoriousController : Controller
    {
        protected AccountViewModel account;
        Dictionary<String, object> jsonResponse;

        public VictoriousController()
        {
            jsonResponse = new Dictionary<String, object>();
            account = new AccountViewModel();
        }

        public void LoadAccount(HttpSessionStateBase session)
        {
            if (Session != null)
            {
                if (Session["User.UserId"] != null)
                {
                    account = new AccountViewModel((int)Session["User.UserId"]);
                }
            }
        }

        public int ConvertToInt(String x)
        {
            int i = -1;
            int.TryParse(x, out i);
            return i;
        }

        protected object JsonPlayerDataResponse(IPlayer player, int score = -1)
        {
            return new
            {
                id = player.Id,
                name = player.Name,
                score = score
            };
        }

        protected object JsonMatchResponse(IMatch match, bool includeGames)
        {
            MatchViewModel matchModel = new MatchViewModel(match);
            List<object> gameData = new List<object>();

            IPlayer Challenger = matchModel.Challenger;
            IPlayer Defender = matchModel.Defender;

            if (includeGames)
            {
                foreach (IGame game in match.Games)
                {
                    gameData.Add(JsonGameResponse(game));
                }
            }

            return new
            {
                matchId = match.Id,
                matchNum = match.MatchNumber,
                ready = match.IsReady,
                finished = match.IsFinished,
                challenger = JsonPlayerDataResponse(Challenger, match.Score[(int)PlayerSlot.Challenger]),
                defender = JsonPlayerDataResponse(Defender, match.Score[(int)PlayerSlot.Defender]),
                games = gameData
            };
        }

        protected object JsonGameResponse(IGame game)
        {
            return new
            {
                id = game.Id,
                gameNum = game.GameNumber,
                matchId = game.MatchId,
                challenger = new
                {
                    id = game.PlayerIDs[(int)PlayerSlot.Challenger],
                    score = game.Score[(int)PlayerSlot.Challenger]
                },
                defender = new
                {
                    id = game.PlayerIDs[(int)PlayerSlot.Defender],
                    score = game.Score[(int)PlayerSlot.Defender]
                }
            };
        }
    }
}