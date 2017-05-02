using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tournament.Structure;

namespace WebApplication.Controllers
{
    public abstract class VictoriousController : Controller
    {
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
            List<object> gameData = new List<object>();
            IPlayer challenger = match.Players[(int)PlayerSlot.Challenger] != null ?
                match.Players[(int)PlayerSlot.Challenger] :
                new User() { Name = "From Match "+match.PreviousMatchNumbers[(int)PlayerSlot.Challenger] };

            IPlayer defender = match.Players[(int)PlayerSlot.Defender] != null ?
                match.Players[(int)PlayerSlot.Defender] :
                new User() { Name = "From Match " + match.PreviousMatchNumbers[(int)PlayerSlot.Defender] };
            
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
                challenger = JsonPlayerDataResponse(challenger, match.Score[(int)PlayerSlot.Challenger]),
                defender = JsonPlayerDataResponse(defender, match.Score[(int)PlayerSlot.Defender]),
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