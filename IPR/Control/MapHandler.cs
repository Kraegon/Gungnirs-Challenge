using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;
using Bing.Maps.Directions;

namespace IPR.Control
{
    class MapHandler
    {
        /// <summary>
        /// The map will be stored here
        /// </summary>
        private Map Map { get; public set; }

        /// <summary>
        /// Collection of waypoints (Player and Spear)
        /// </summary>
        private WaypointCollection WaypointCol;

        /// <summary>
        /// Navigates between the waypoints
        /// </summary>
        private DirectionsManager DirManager;

        public MapHandler()
        {
            
        }

        public void Initialize()
        {

        }

        public void SetMap(Map map)
        {
            this.Map = map;
        }
    }
}
