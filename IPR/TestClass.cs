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
            await Task.Delay(10000);
            await HighscoreReader.SaveHighscoreObj(new Model.HighscoreObj("DDD", 2000, TimeSpan.Parse("00:30")));            
        }
    }
}
