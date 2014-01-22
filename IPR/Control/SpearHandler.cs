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
        public static Spear Gungnir = new Spear();
        public static GameState State;
//        System.Threading.Timer timer = new System.Threading.Timer();
        public delegate void SpearLocationUpdateHandler();
        public static event SpearLocationUpdateHandler SpearLocationUpdateEvent;

        
        /// <summary>
        /// This is the games procedure.
        /// </summary>
        /// <returns> Success </returns>
        public async static void LetsThrow()
        {
            Gungnir = new Spear
            {
                Available = true
            };
            State = GameState.PowerDetermining;
            int power = DeterminePower();
            await Task.Delay(5000);  // Await Power determining
            System.Diagnostics.Debug.WriteLine("SpearThrowing");
            State = GameState.SpearThrowing;
            ThrowSpear(power);
            await Task.Delay(5000);  // Await Spear throwing
            System.Diagnostics.Debug.WriteLine("route Drawing");

            State = GameState.RouteDrawing;
            await Task.Delay(5000);  // Await route drawing
            System.Diagnostics.Debug.WriteLine("Retrieving");
            State = GameState.Retrieving;
            await Task.Delay(5000);  // Await Spear retrieving
            State = GameState.Idle;
        }

        public static int DeterminePower()
        {
            return 100;
        }

        public static async void ThrowSpear(int power)
        {
            int Pow = MathCalculation.CalculateDistance(power);

            if (GodController.DirectionLocation == null || Gungnir == null)
                return;

            while(Pow > 0)
            {
                updateSpearLocation();

                System.Diagnostics.Debug.WriteLine(Pow.ToString());
                Pow--;
                await Task.Delay(100);
            }


            Gungnir.Available = false;
        }

        private static void updateSpearLocation()
        {
            Gungnir.Location = new Location(
                GodController.CurrentPlayer.Location.Longitude + 0.000001 * Math.Cos(GodController.DirectionLocation.Longitude),
                GodController.CurrentPlayer.Location.Latitude + 0.000001 * Math.Sin(GodController.DirectionLocation.Latitude));
            SpearLocationUpdateEvent();
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
