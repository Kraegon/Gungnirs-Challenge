using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;

namespace IPR.Model
{
    class Spear
    {
        /// <summary>
        /// The weight of the spear for calculations
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Checks if the spear is avainable for usage
        /// </summary>
        public bool Avainable { get; set; }

        /// <summary>
        /// The current location of the spear
        /// </summary>
        public Location Location { get; set; }

        ///TODO: 
        ///add a BitmapImage
    }
}
