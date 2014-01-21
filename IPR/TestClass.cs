using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPR.Control;

namespace IPR
{
    class TestClass
    {
        public async static void TestProcedure()
        {
            var a = await HighscoreReader.GetHighscoresAsync();
            var b = await HighscoreReader.SortHighestScoreFirstAsync();
        }
    }
}
