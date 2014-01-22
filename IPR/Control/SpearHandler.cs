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
    class SpearHandler
    {
        public static Location Throw(Location currentLocation, Location directionLocation, )
        {
            MapPolygon straightLine 

            throw new NotImplementedException();
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
