using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;

namespace IPR.Model
{
    class Player
    {
        /// <summary>
        /// The Current name of the player
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current location of the player
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The Score the player gets by throwing the spear.
        /// </summary>
        public double Score { get; set; }
    }
}
