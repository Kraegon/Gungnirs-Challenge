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
using Windows.Devices.Geolocation.Geofencing;
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

            ClearGeofences();
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
        }

        private async void Current_GeofenceStateChanged(GeofenceMonitor sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("Entered geofence");
            var reports = sender.ReadReports();         
            await MainPage.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var item in reports)
                    {
                        var state = item.NewState;

                        if (state == GeofenceState.Entered)
                        {
                            System.Diagnostics.Debug.WriteLine("Entered the geofence event and state.Entered");
                            SpearHandler.Gungnir.Available = true;
                        }
                    }
                });
        }

        private async void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await MainPage.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (SatanController.CurrentPlayer == null)
                    //return;
                    SatanController.CurrentPlayer = new Player();
                SatanController.CurrentPlayer.Location = new Location(e.Position.Coordinate.Point.Position.Latitude,
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
            SatanController.CurrentPlayer = player;
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
        public async void DrawWalkableRouteToSpear(Location playerLocation, Location spearLocation)
        {
            if (DirManager == null)
                DirManager = Map.DirectionsManager;
            DirManager.Waypoints.Clear();

            DirManager.Waypoints.Add(new Waypoint(playerLocation));
            DirManager.Waypoints.Add(new Waypoint(spearLocation));


            DirManager.RequestOptions.RouteMode = RouteModeOption.Walking;
            DirManager.RequestOptions.Optimize = OptimizeOption.Walking;
            
            //Do something with the distance

            RouteResponse response = await DirManager.CalculateDirectionsAsync();
            SpearHandler.Distance = response.Routes[0].TravelDistance * 1000;

            DirManager.RenderOptions.WaypointPushpinOptions.Visible = false;

            if (response.HasError)
                await SatanController.ShowMessageAsync("Route error", "The route could not be calculated.");

            DirManager.ShowRoutePath(DirManager.ActiveRoute);
        } 

        public static void DrawThrownRoute()
        {
            try
            {
                MapPolyline routeLine = new MapPolyline();
                routeLine.Locations = new LocationCollection();
                routeLine.Color = Windows.UI.Colors.Red;
                routeLine.Width = 5.0;

                routeLine.Locations.Add(new Location
                {
                    Latitude = SatanController.CurrentPlayer.Location.Latitude,
                    Longitude = SatanController.CurrentPlayer.Location.Longitude
                });
                routeLine.Locations.Add(new Location
                {
                    Latitude = SpearHandler.Gungnir.Location.Latitude,
                    Longitude = SpearHandler.Gungnir.Location.Longitude
                });
                MapShapeLayer shapeLayer = new MapShapeLayer();
                shapeLayer.Shapes.Add(routeLine);
//                Map.ShapeLayers.Add(shapeLayer);
            }
            catch
            {
                //Message dialog, description + Title
                SatanController.ShowMessage("Something went wrong with drawing the route to the spear.", "Error");
            }
        }

        /// <summary>
        /// Adds a Geofence to the location of the spear
        /// </summary>
        /// <param name="spearLocation"></param>
        private void AddGeofence(Location spearLocation)
        {
            // Especially for Julian FLOAT!
            Geofence spearFence = new Geofence(
                "SpearLocation" + DateTime.Now.ToString(),
                new Geocircle(new BasicGeoposition { Latitude = spearLocation.Latitude, Longitude = spearLocation.Longitude }, (double)30.0f),
                MonitoredGeofenceStates.Entered,
                true,
                TimeSpan.FromSeconds(2)); 
        }

        public void ClearMap()
        {
            if (Map.Children.Count > 0)
                Map.Children.Clear();
        }

        public void ClearGeofences()
        {
            if (GeofenceMonitor.Current.Geofences.Count > 0)
                GeofenceMonitor.Current.Geofences.Clear();
        }
        
        /// <summary>
        /// Overrides the doubleTap on bing maps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var position = e.GetPosition(Map);
            Location loc;
            Map.TryPixelToLocation(position, out loc);

            SatanController.DirectionLocation = loc;
            
            if(SpearHandler.State == GameState.Idle)
                SpearHandler.StartThrow();

        }
    }
}
