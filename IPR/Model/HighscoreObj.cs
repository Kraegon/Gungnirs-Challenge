using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPR.Model
{
    public class HighscoreObj
    {
        public string Name { get; set; }
        public double Distance { get; set; }
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
