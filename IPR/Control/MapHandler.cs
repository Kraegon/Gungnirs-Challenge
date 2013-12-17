using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;
using Bing.Maps.Directions;
using IPR.Model;

namespace IPR.Control
{
    class MapHandler
    {
        /// <summary>
        /// The map will be stored here
        /// </summary>
        private Map Map;
        /// <summary>
        /// Collection of waypoints (Player and Spear)
        /// </summary>
        private WaypointCollection WaypointCol;

        /// <summary>
        /// Navigates between the waypoints
        /// </summary>
        private DirectionsManager DirManager;

        /// <summary>
        /// Active spear
        /// </summary>
        private Spear CurrentSpear;

        /// <summary>
        /// 
        /// </summary>
        private Player CurrentPlayer;

        public MapHandler()
        {
            
        }

        public void Initialize()
        {
            CurrentSpear = new Spear()
                {
                    Avainable = true,
                    Weight = 10
                };
            CurrentPlayer = new Player()
                {
                    Name = "Jelle"
                };
        }

        public void SetMap(Map map)
        {
            this.Map = map;
        }

        public void SetPlayer(Player player)
        {
            this.CurrentPlayer = player;
        }

        /// <summary>
        /// Adds pushpins and waypoints
        /// </summary>
        private void AddPinsAndPoints()
        {
            Waypoint playerPoint;
            Waypoint spearPoint;
            if (!CurrentSpear.Avainable)
                spearPoint = new Waypoint(CurrentSpear.Location);
            playerPoint = new Waypoint(CurrentPlayer.Location);
        }
    }
}
