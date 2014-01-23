using IPR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Bing.Maps;
using System.Threading;
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
        private static Timer scoreTimer;
        private static int throwPower;

        public static HighscoreObj Score = new HighscoreObj();
        public static Spear Gungnir;

        public static GameState State = GameState.Idle;
//        System.Threading.Timer timer = new System.Threading.Timer();
        public delegate void PropertieUpdateHandler();
        public static event PropertieUpdateHandler PropertieUpdateEvent;

        public delegate void UpdateGameStateHandler();
        public static event UpdateGameStateHandler UpdateGameStateEvent;

        /// <summary>
        /// This is the games procedure.
        /// </summary>
        /// <returns> Success </returns>
        public static void ExecuteStateOperation()
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
                    Gungnir.Available = false;
                    ThrowSpear(throwPower);
                    break;
                case GameState.RouteDrawing:
                    DrawRoute();
                    break;
                case GameState.Retrieving:
                    RetrieveSpear();
                    StartTimeRecord();
                    break;
            }
        }

        public static void PropertyChanged()
        {
            PropertieUpdateEvent();
        }

        public static void StartTimeRecord()
        {
            scoreTimer = new Timer(new TimerCallback(timerCallBack), null, 0, 1000);
        }

        private static void timerCallBack(object e)
        {
            Score.TimeTaken = Score.TimeTaken.Add(new TimeSpan(0,0,1));
            PropertieUpdateEvent();
        }
        public static void EndTimeRecord()
        {
            scoreTimer.Dispose();
            PropertieUpdateEvent();
        }

        public static void StartThrow()
        {
            if (UpdateGameStateEvent == null)
                UpdateGameStateEvent += SpearHandler_UpdateGameStateEvent;
            UpdateGameStateEvent();
        }
        public static void EndThrow()
        {
            if (State == GameState.Retrieving)
                State = GameState.Idle;
            Gungnir.Available = true;
            PropertieUpdateEvent();
        }
        private static void SpearHandler_UpdateGameStateEvent()
        {
            State += 1;
            ExecuteStateOperation();
        }

        public static int DeterminePower()
        {
            int retVal = 50;
            return retVal;
        }

        private static async void ThrowSpear(int power)
        {
            int Pow = MathCalculation.CalculateDistance(power);
            if (SatanController.DirectionLocation == null || Gungnir == null)
                return;
            Gungnir.Location = SatanController.CurrentPlayer.Location;
            await updateSpearLocation();
            UpdateGameStateEvent();
        }

        private async static Task updateSpearLocation()
        {
            double a = MathCalculation.Delta(SatanController.CurrentPlayer.Location.Longitude, SatanController.DirectionLocation.Longitude) /
                       MathCalculation.Delta(SatanController.CurrentPlayer.Location.Latitude, SatanController.DirectionLocation.Latitude);
            double b = SatanController.CurrentPlayer.Location.Longitude - (a * SatanController.CurrentPlayer.Location.Latitude);
            if (SatanController.CurrentPlayer.Location.Latitude < SatanController.DirectionLocation.Latitude)
            {
                for (double x = 0; x < (0.1 / throwPower); x += 0.0001)
                {
                    Gungnir.Location = new Location(
                        SatanController.CurrentPlayer.Location.Latitude + x,
                        (a * (SatanController.CurrentPlayer.Location.Latitude + x)) + b
                    );
                    await Task.Delay(200);
                    PropertieUpdateEvent();
                }
            }
            else
            {
                for (double x = 0; x > (-0.1 / throwPower); x -= 0.0001)
                {
                    Gungnir.Location = new Location(
                        SatanController.CurrentPlayer.Location.Latitude + x,
                        (a * (SatanController.CurrentPlayer.Location.Latitude + x)) + b
                    );
                    await Task.Delay(200);
                    PropertieUpdateEvent();
                }
            }
            return;
        }

        private static void DrawRoute()
        {
            //Relay the command to draw the route to Gungnir
            SatanController.HandleMap.DrawWalkableRouteToSpear(SatanController.CurrentPlayer.Location, Gungnir.Location);
            System.Diagnostics.Debug.WriteLine(String.Empty + Gungnir.Location.Latitude + Gungnir.Location.Longitude);
            UpdateGameStateEvent();
        }

        private static void RetrieveSpear()
        {
            //set geofence
            SatanController.HandleMap.AddGeofence(Gungnir.Location);
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
