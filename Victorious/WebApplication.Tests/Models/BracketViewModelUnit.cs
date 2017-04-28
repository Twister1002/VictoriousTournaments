using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Models;
using Tournament.Structure;

namespace WebApplication.Tests.Models
{
    class BracketViewModelUnit
    {
        [TestMethod]
        [TestCategory("BracketView")]
        public void BracketViewModel_GetRounds_Returns_NumberOfRounds()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel();

            // Act


            // Assert
        }


        public IBracket SingleEliminationBracket()
        {
            IBracket bracket = new SingleElimBracket();

            return bracket;
        }

        public IBracket DoubleEliminationBracket()
        {
            IBracket bracket = new DoubleElimBracket();

            return bracket;
        }
    }
}
