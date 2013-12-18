using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bing.Maps;
using Bing.Maps.Directions;
using Windows.UI.Xaml.Controls.Image;
using Windows.Devices.Geolocation;
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
        /// The current active spear
        /// </summary>
        private Spear CurrentSpear;

        /// <summary>
        /// The current active player
        /// </summary>
        private Player CurrentPlayer;

        private Geolocator Locator;
        private CancellationTokenSource Cts;

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

            var temp = GetCurrentLocationAsync();

            CurrentPlayer = new Player()
                {
                    Name = "Jelle",
                };
            WaypointCol = new WaypointCollection();
            GetCurrentLocationAsync();
        }

        private async Task GetCurrentLocationAsync()
        {
            Geoposition pos;
            try
            {
                Locator = new Geolocator();
                Cts = new CancellationTokenSource();
                pos = await Locator.GetGeopositionAsync().AsTask(Cts.Token);
                CurrentPlayer.Location = new Location(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
            }
            catch (UnauthorizedAccessException)
            {
                GodController.ShowMessage("No acces to gps location" + "/n" + "Please make sure you enabled access to your location", "Error");
            }
            catch(TaskCanceledException)
            {
                GodController.ShowMessage("Task is canceled, /n Please try to restart", "Task canceled");
            }
            finally
            {   
                Cts = null;
            }
        }

        /// <summary>
        /// Sets the map, Importent to not change, used once not necessary afterwards
        /// </summary>
        /// <param name="map"></param>
        public void SetMap(Map map)
        {
            this.Map = map;
        }

        /// <summary>
        /// Sets the current player
        /// </summary>
        /// <param name="player"></param>
        public void SetPlayer(Player player)
        {
            this.CurrentPlayer = player;
        }

        /// <summary>
        /// Adds pushpins and waypoints
        /// </summary>
        private void AddPinsAndPoints()
        {
            // WaypointsCollection needs to be cleared to get accurate data.
            WaypointCol.Clear();

            //Order in which they are added is important!
            //First the player then the spear.

            //PlayerStuff
            WaypointCol.Add(new Waypoint(CurrentPlayer.Location));
            Pushpin PlayerPin = new Pushpin() { Name = CurrentPlayer.Name };
            Map.Children.Add(PlayerPin);
            Map.Children.Add(PlayerPin);
            MapLayer.SetPosition(PlayerPin, CurrentPlayer.Location);

            // Spear Stuff
            Pushpin SpearPin;
            if (!CurrentSpear.Avainable)
            {
                WaypointCol.Add(new Waypoint(CurrentSpear.Location));
                SpearPin = new Pushpin();
                Map.Children.Add(SpearPin);
                MapLayer.SetPosition(SpearPin, CurrentSpear.Location);
            }
        }

        /// <summary>
        /// makes a list of waypoints in the directionsmanager
        /// </summary>
        /// <param name="pointsCollection"></param>
        private void AddWaypointsToDirectionsManager(WaypointCollection pointsCollection)
        {
            DirManager.Waypoints = pointsCollection;
        }

        private async void DrawRouteToSpear()
        {
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
        }

        /// <summary>
        /// Draws A straight line from the player to the Spear the "Thrown line".
        /// </summary>
        private async void DrawThrownRoute()
        {

            if (CurrentSpear.Avainable)
                return;
            try
            {
                MapPolyline routeLine = new MapPolyline();
                routeLine.Locations = new LocationCollection();
                routeLine.Color = Windows.UI.Colors.Brown;
                routeLine.Width = 5.0;

                routeLine.Locations.Add(new Location
                    {
                        Latitude = CurrentPlayer.Location.Latitude,
                        Longitude = CurrentPlayer.Location.Longitude
                    });
                routeLine.Locations.Add(new Location
                    {
                        Latitude = CurrentSpear.Location.Latitude,
                        Longitude = CurrentSpear.Location.Longitude
                    });
                MapShapeLayer shapeLayer = new MapShapeLayer();
                shapeLayer.Shapes.Add(routeLine);
                Map.ShapeLayers.Add(shapeLayer);
            }
            catch
            {
                //Message dialog, description + Title
                GodController.ShowMessage("Something went wrong with drawing the route to the spear.", "Error");
            }
        }
    }
}
