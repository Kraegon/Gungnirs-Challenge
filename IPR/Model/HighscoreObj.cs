using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPR.Model
{
    class HighscoreObj
    {
        public string Name { get; set; }
        public float Distance { get; set; }
        public TimeSpan TimeTaken { get; set; }

        public HighscoreObj()
        {
            Name = string.Empty;
            Distance = 0;
            TimeTaken = new TimeSpan();
        }

        public HighscoreObj(string name, float distance, TimeSpan timeTaken)
        {
            Name = name;
            Distance = distance;
            TimeTaken = timeTaken; 
        }
    }
}
