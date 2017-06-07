using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tournaments = Tournament.Structure;
using Tournament.Structure;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public abstract class VictoriousController : Controller
    {
        protected Account account;
        public IUnitOfWork work;
        Dictionary<String, object> jsonResponse;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            LoadAccount(requestContext.HttpContext.Session);
        }

        public VictoriousController()
        {
            jsonResponse = new Dictionary<String, object>();
            work = new UnitOfWork();
        }

        protected override void Dispose(bool disposing)
        {
            work.Dispose();
        }

        public void LoadAccount(HttpSessionStateBase Session)
        {
            if (Session != null)
            {
                if (Session["User.UserId"] != null)
                {
                    account = new Account(work, (int)Session["User.UserId"]);
                }
                else
                {
                    account = new Account(work);
                }
            }
            else
            {
                account = null;
            }
        }

        public bool IsLoggedIn()
        {
            if (account != null && account.Model.AccountID > 0)
            {
                return true;
            }
            else
            {
                return false;
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

        protected object JsonMatchResponse(Models.Match match, bool includeGames)
        {
            List<object> gameData = new List<object>();

            IPlayer Challenger = match.Challenger;
            IPlayer Defender = match.Defender;

            if (includeGames)
            {
                foreach (IGame game in match.GetGames())
                {
                    gameData.Add(JsonGameResponse(game));
                }
            }

            return new
            {
                matchId = match.match.Id,
                matchNum = match.match.MatchNumber,
                ready = match.match.IsReady,
                finished = match.match.IsFinished,
                challenger = JsonPlayerDataResponse(Challenger, match.ChallengerScore()),
                defender = JsonPlayerDataResponse(Defender, match.DefenderScore()),
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