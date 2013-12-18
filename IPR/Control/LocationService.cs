using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
namespace IPR.Control
{
    class LocationService
    {
        public Geolocator Locator;
        public Geoposition CurrentPosition;
        private static LocationService instance;
        public static LocationService INSTANCE
        {
            get
            {
                if (instance == null)
                    instance = new LocationService();
                return instance;
            }
        }

        private LocationService()
        {
            Locator = new Geolocator();
            Locator.DesiredAccuracy = PositionAccuracy.High;
            FindCurrentPositionAsync();
        }

        public async void FindCurrentPositionAsync()
        {
            CurrentPosition = await Locator.GetGeopositionAsync();
        }

    }
}
