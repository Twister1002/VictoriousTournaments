using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tournament.Structure;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public abstract class VictoriousController : Controller
    {
        protected AccountViewModel account;
        public static IUnitOfWork uow;
        Dictionary<String, object> jsonResponse;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            LoadAccount(requestContext.HttpContext.Session);
        }

        public VictoriousController()
        {
            jsonResponse = new Dictionary<String, object>();
            uow = new UnitOfWork();
        }

        protected override void Dispose(bool disposing)
        {
            uow.Dispose();
        }

        public void LoadAccount(HttpSessionStateBase Session)
        {
            if (Session != null)
            {
                if (Session["User.UserId"] != null)
                {
                    account = new AccountViewModel((int)Session["User.UserId"]);
                }
                else
                {
                    account = new AccountViewModel();
                }
            }
            else
            {
                account = null;
            }
        }

        public bool IsLoggedIn()
        {
            if (account != null && account.AccountId > 0)
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