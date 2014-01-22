using IPR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Bing.Maps;
using IPR.Model;

namespace IPR.Control
{
    /// <summary>
    /// Class for handling a throw procedure.
    /// The throw procedure consists of:
    /// 1. Determine power
    /// 2. Move spear to location
    /// 3. Draw route
    /// 4. Retrieve spear
    /// </summary>
    public class SpearHandler
    {
        public static Spear Gungnir;
        public static GameState State;
        
        /// <summary>
        /// This is the games procedure.
        /// </summary>
        /// <returns> Success </returns>
        public static bool LetsThrow()
        {
            State = GameState.PowerDetermining;
            int power = DeterminePower();
            Task.Delay(5000);  // Await Power determining
            State = GameState.SpearThrowing;
            Task.Delay(5000);  // Await Spear throwing
            State = GameState.RouteDrawing;
            Task.Delay(5000);  // Await route drawing
            State = GameState.Retrieving;
            Task.Delay(5000);  // Await Spear retrieving
            State = GameState.Idle;
            return true;
        }

        public static int DeterminePower()
        {
            return 10;
        }

        public static void ThrowSpear(int Power)
        {
            //Set the location of TheSpear
            return;
        }

        public static void DrawRoute()
        {
            //Relay the command to draw the route to Gungnir
            return;
        }

        public static void RetrieveSpear()
        {
            //Set up the event when the spear is retrieved and handle ending procedure.
            return;
        }

        private void DrawThrownRoute()
        {

            if (GodController.CurrentSpear.Available)
                return;
            try
            {
                MapPolyline routeLine = new MapPolyline();
                routeLine.Locations = new LocationCollection();
                routeLine.Color = Windows.UI.Colors.Brown;
                routeLine.Width = 5.0;

                routeLine.Locations.Add(new Location
                {
                    Latitude = GodController.CurrentPlayer.Location.Latitude,
                    Longitude = GodController.CurrentPlayer.Location.Longitude
                });
                routeLine.Locations.Add(new Location
                {
                    Latitude = GodController.CurrentSpear.Location.Latitude,
                    Longitude = GodController.CurrentSpear.Location.Longitude
                });
                MapShapeLayer shapeLayer = new MapShapeLayer();
                shapeLayer.Shapes.Add(routeLine);
//                Map.ShapeLayers.Add(shapeLayer);
            }
            catch
            {
                //Message dialog, description + Title
                GodController.ShowMessage("Something went wrong with drawing the route to the spear.", "Error");
            }
        }
    }
    /// <summary>
    /// These states determine whats going on
    /// This way the GUI or other components know how to act.
    /// </summary>
    public enum GameState
    {
        Idle = 0,
        PowerDetermining = 1,
        SpearThrowing = 2,
        RouteDrawing = 3,
        Retrieving = 4
    }
}
