using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bing.Maps;
using Bing.Maps.Directions;
//using Windows.UI.Xaml.Controls.Image;
using Windows.Devices.Geolocation;
using IPR.Model;
using Windows.UI.Core;
using Windows.UI.Xaml;

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
        public Geolocator Locator;

        public void Initialize()
        {
            Locator = new Geolocator();
            WaypointCol = new WaypointCollection();
            Locator.DesiredAccuracy = PositionAccuracy.High;
            Locator.PositionChanged += Locator_PositionChanged;
        }

        private async void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await MainPage.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (GodController.CurrentPlayer == null)
                    //return;
                    GodController.CurrentPlayer = new Player();
                GodController.CurrentPlayer.Location = new Location(e.Position.Coordinate.Point.Position.Latitude,
                                                      e.Position.Coordinate.Point.Position.Longitude);
            });
        }

        /// <summary>
        /// Sets the map, Importent to not change, used once not necessary afterwards
        /// </summary>
        /// <param name="map"></param>
        public void SetMap(Map map)
        {
            this.Map = map;
            Map.DoubleTappedOverride += Map_DoubleTapped;

        }

        /// <summary>
        /// Sets the current player
        /// </summary>
        /// <param name="player"></param>
        public void SetPlayer(Player player)
        {
            GodController.CurrentPlayer = player;
        }

        /// <summary>
        /// makes a list of waypoints in the directionsmanager
        /// </summary>
        /// <param name="pointsCollection"></param>
        private void AddWaypointsToDirectionsManager(WaypointCollection pointsCollection)
        {
            DirManager.Waypoints = pointsCollection;
        }

        /// <summary>
        /// Draws the walkable route to the spear, if the spear is unavailable
        /// </summary>
/*        private async void DrawRouteToSpear()
        {
            if (GodController.CurrentSpear.Available)
                return;

            if (DirManager.Waypoints.Count > 1)
            {
                DirectionsManager manager = DirManager;
                manager.Waypoints = DirManager.Waypoints;
                RouteResponse response = await manager.CalculateDirectionsAsync();

                manager.RequestOptions.RouteMode = RouteModeOption.Walking;

                //not sure if usefull or not
                //var distance = manager.RequestOptions.DistanceUnit;

                manager.ShowRoutePath(manager.ActiveRoute);
            }
        } */

        public void ClearMap()
        {
            if (Map.Children.Count > 0)
            {
                Map.Children.Clear();
            }
        }

        void Map_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var position = e.GetPosition(Map);
            Location loc;
            Map.TryPixelToLocation(position, out loc);

            GodController.DirectionLocation = loc;
            
            if(SpearHandler.State == GameState.Idle)
                SpearHandler.StartThrow();
        }
    }
}
