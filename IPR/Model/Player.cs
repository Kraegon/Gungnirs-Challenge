using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;

namespace IPR.Model
{
    /// <summary>
    /// Contains player metadata
    /// This could be expanded with more metadata therefor received its own class.
    /// </summary>
    public class Player
    {

        /// <summary>
        /// The current location of the player
        /// </summary>
        public Location Location { get; set; }


        public Player()
        {
            Location = null;
        }
        public Player(string name)
        {
            Location = null;
        }
        public Player(string name, Location location, double score)
        {
            Location = location;
        }
    }
}
