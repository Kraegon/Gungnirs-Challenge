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

//            hoek = o/a 


            double y = Math.Sin(dLongitude) * Math.Cos(directionPoint.Latitude);
            double x = Math.Cos(location.Latitude) * Math.Sin(directionPoint.Latitude - Math.Sin(location.Latitude * Math.Cos(directionPoint.Latitude * Math.Cos(dLongitude))));

            double angle = Math.Atan2(y, x);
            angle = (angle / Math.PI) * 180;
            angle = (angle + 360) % 360;
            angle = 360 - angle;

            return angle;
        }


    }
}
