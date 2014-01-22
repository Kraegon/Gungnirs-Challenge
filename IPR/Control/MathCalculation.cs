using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Bing.Maps;

namespace IPR.Control
{
    class MathCalculation
    {
        public static double CalculateAngle(Location location, Location directionPoint)
        {
            double dLongitude = (directionPoint.Longitude - location.Longitude);
            double dLatitude = (directionPoint.Latitude - location.Latitude);

            double y = Math.Sin(dLongitude) * Math.Cos(directionPoint.Latitude);
            double x = Math.Cos(location.Latitude) * Math.Sin(directionPoint.Latitude - Math.Sin(location.Latitude * Math.Cos(directionPoint.Latitude * Math.Cos(dLongitude))));

            double angle = Math.Atan2(y, x);
            angle = (angle / Math.PI) * 180;
            angle = (angle + 360) % 360;
            angle = 360 - angle;

            return angle;
        }

        public static double Pythagoras(double aa, double bb)
        {
            double cc = (aa * aa) + (bb * bb);
            return Math.Sqrt(cc);
        }
        public static double Delta(double aa, double bb)
        {
            return bb - aa; 
        }
        /// <summary>
        /// From the pneumonic SOHCAHTOA
        /// From two points the Opposite divided by Hypoteneuse.
        /// </summary>
        /// <param name="locA">Point 1</param>
        /// <param name="locB">Point 2</param>
        /// <returns>Angle of </returns>
        public static double SOH(Location locA, Location locB)
        {
            double o = DegreesToRadian(Delta(locA.Latitude, locB.Latitude));
            double h = DegreesToRadian(Pythagoras(Delta(locA.Latitude, locB.Latitude), Delta(locA.Longitude, locB.Longitude)));
            return Math.Sin(o / h);
        }
        public static double CAH(Location locA, Location locB)
        {
            double a = DegreesToRadian(Delta(locA.Longitude, locB.Longitude));
            double h = DegreesToRadian(Pythagoras(Delta(locA.Latitude, locB.Latitude), Delta(locA.Longitude, locB.Longitude)));
            return Math.Cos(a / h);
        }

        public static int CalculateDistance(int Power)
        {
            return Power;
        }

        public static double DegreesToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }


        /// <summary>
        /// calculates the score for the xml file.
        /// calculation is the longer you take the less score you have,
        /// if the distance is low, your score will also be
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="spawn"></param>
        /// <returns>int</returns>
        public static int CalculateScore(float distance, TimeSpan spawn)
        {
            int seconds = (int)spawn.TotalSeconds;
            int dist = (int)distance;
            int ans;

            ans = ((dist / seconds) * (dist * 200)^2) / seconds;

            System.Diagnostics.Debug.WriteLine(ans.ToString());
            return ans;
        }
    }
}
