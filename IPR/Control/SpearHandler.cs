using IPR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Bing.Maps;

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
        private static int throwPower;

        public static Spear Gungnir;
        public static GameState State = GameState.Idle;
//        System.Threading.Timer timer = new System.Threading.Timer();
        public delegate void SpearLocationUpdateHandler();
        public static event SpearLocationUpdateHandler SpearLocationUpdateEvent;

        public delegate void UpdateGameStateHandler();
        public static event UpdateGameStateHandler UpdateGameStateEvent;

        /// <summary>
        /// This is the games procedure.
        /// </summary>
        /// <returns> Success </returns>
        public async static void ExecuteStateOperation()
        {
            if (Gungnir == null)
            {
                Gungnir = new Spear
                {
                    Available = true
                };
            }
            if (State > GameState.Retrieving)
                State = GameState.Idle;
            switch (State)
            {
                case GameState.PowerDetermining:
                    throwPower = DeterminePower();
                    UpdateGameStateEvent();
                    break;
                case GameState.SpearThrowing:
                    ThrowSpear(throwPower);
                    Gungnir.Available = false;
                    break;
                case GameState.RouteDrawing:
                    DrawRoute();
                    break;
                case GameState.Retrieving:
                    RetrieveSpear();
                    break;
            }
        }

        public static void StartThrow()
        {
            if (UpdateGameStateEvent == null)
                UpdateGameStateEvent += SpearHandler_UpdateGameStateEvent;
            UpdateGameStateEvent();
        }

        private static void SpearHandler_UpdateGameStateEvent()
        {
            State += 1;
            ExecuteStateOperation();
        }

        public static int DeterminePower()
        {
            int retVal = 2000;
            return retVal;
        }

        public static async void ThrowSpear(int power)
        {
            int Pow = MathCalculation.CalculateDistance(power);
            if (GodController.DirectionLocation == null || Gungnir == null)
                return;
            Gungnir.Location = GodController.CurrentPlayer.Location;
            await updateSpearLocation();
            UpdateGameStateEvent();
        }

        private async static Task updateSpearLocation()
        {
            double a = MathCalculation.Delta(GodController.CurrentPlayer.Location.Longitude, GodController.DirectionLocation.Longitude) /
                       MathCalculation.Delta(GodController.CurrentPlayer.Location.Latitude, GodController.DirectionLocation.Latitude);
            double b = GodController.CurrentPlayer.Location.Longitude - (a * GodController.CurrentPlayer.Location.Latitude);
            if (GodController.CurrentPlayer.Location.Latitude < GodController.DirectionLocation.Latitude)
            {
                for (double x = 0; x < (0.1 / throwPower); x += 0.0001)
                {
                    Gungnir.Location = new Location(
                        GodController.CurrentPlayer.Location.Latitude + x,
                        (a * (GodController.CurrentPlayer.Location.Latitude + x)) + b
                    );
                    await Task.Delay(200);
                    SpearLocationUpdateEvent();
                }
            }
            else
            {
                for (double x = 0; x > (-0.1 / throwPower); x -= 0.0001)
                {
                    Gungnir.Location = new Location(
                        GodController.CurrentPlayer.Location.Latitude + x,
                        (a * (GodController.CurrentPlayer.Location.Latitude + x)) + b
                    );
                    await Task.Delay(200);
                    SpearLocationUpdateEvent();
                }
            }
            return;
        }

        public static void DrawRoute()
        {
            //Relay the command to draw the route to Gungnir
            GodController.HandleMap.DrawWalkableRouteToSpear(GodController.CurrentPlayer.Location, Gungnir.Location);
            UpdateGameStateEvent();
        }

        public static void RetrieveSpear()
        {
            UpdateGameStateEvent();
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
