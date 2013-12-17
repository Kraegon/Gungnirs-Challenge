using IPR.Common;
using IPR.Control;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Bing.Maps;
using IPR.Control;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IPR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }       

        public MainPage()
        {
            this.InitializeComponent();

            /* Initalizes the controllers and adds the map to the maphandler */
            GodController.GetMapHandler().SetMap(BingMap);
            GodController.GetMapHandler().Initialize();
        }

        public async void CenterPosition()
        {
            while (LocationService.INSTANCE.CurrentPosition == null)
            {
                var location = new Bing.Maps.Location(LocationService.INSTANCE.CurrentPosition.Coordinate.Latitude,
                                                       LocationService.INSTANCE.CurrentPosition.Coordinate.Longitude);
                BingMap.SetView(location);
            }
        }

        public Map GetMapObject()
        {
            return BingMap;
        }

        public void DrawCurrentLocation(Location loc)
        {
            BingMap.SetView(loc, 9.0f);

            Pushpin pin = new Pushpin()
            {
                Text = "Me"
            };

            BingMap.Children.Add(pin);
            MapLayer.SetPosition(pin, loc);
        }
    }
}
