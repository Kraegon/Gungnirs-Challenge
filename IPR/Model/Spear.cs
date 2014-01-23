using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;
using Windows.UI.Xaml.Controls;


namespace IPR.Model
{
    public class Spear
    {
        /// <summary>
        /// Checks if the spear is avainable for usage
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// The current location of the spear
        /// </summary>
        public Location Location { get; set; }
    }
}
