using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;
using Tournament.Structure;
using Tournaments = Tournament.Structure;
using WebApplication.Utility;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public class Match
    {
        //MatchViewModel viewModel;
        IService services;

        public IMatch match { get; private set; }
        public IPlayer Challenger { get; private set; }
        public IPlayer Defender { get; private set; }

        public Match(IService services, IMatch match)
        {
            this.services = services;
            this.match = match;
            Init();
        }

        private void Init()
        {
            Challenger = match.Players[(int)PlayerSlot.Challenger];
            Defender = match.Players[(int)PlayerSlot.Defender];

            if (Challenger == null)
            {
                int prevMatchNum = match.PreviousMatchNumbers[(int)PlayerSlot.Challenger];
                String name = prevMatchNum == -1 ? "" : "Match " + prevMatchNum.ToString();

                Challenger = new Player()
                {
                    Name = name
                };
            }

            if (Defender == null)
            {
                int prevMatchNum = match.PreviousMatchNumbers[(int)PlayerSlot.Defender];
                String name = prevMatchNum == -1 ? "" : "Match " + prevMatchNum.ToString();

                Defender = new Player()
                {
                    Name = name
                };
            }
        }

        public int ChallengerScore()
        {
            return match.Score[(int)PlayerSlot.Challenger];
        }

        public int DefenderScore()
        {
            return match.Score[(int)PlayerSlot.Defender];
        }

        public List<IGame> GetGames()
        {
            return match.Games;
        }

        #region CRUD
        public bool Create()
        {
            throw new NotImplementedException("Not allowed to create a match from Match");
        }

        public bool Update()
        {
            throw new NotImplementedException("Not allowed to retrieve a match from Match");
        }

        public bool Retrieve()
        {
            throw new NotImplementedException("Not allowed to retrieve a match from Match");
        }

        public bool Delete()
        {
            throw new NotImplementedException("Not allowed to delete a match from Match");
        }
        #endregion

        #region ViewModel
        public void ApplyChanges()
        {

        }

        public void SetFields()
        {

        }
        #endregion
    }
}