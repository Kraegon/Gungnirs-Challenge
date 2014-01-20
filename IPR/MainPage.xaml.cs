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
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IPR
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        public static CoreDispatcher dispatcher;
        public MainPage()
        {
            this.InitializeComponent();
            dispatcher = this.Dispatcher;
            /* Initalizes the controllers and adds the map to the maphandler */
            GodController.GetMapHandler().SetMap(BingMap);
            GodController.GetMapHandler().Initialize();
            GodController.GetMapHandler().Locator.PositionChanged += Locator_PositionChanged;
        }

        private async void Locator_PositionChanged(Windows.Devices.Geolocation.Geolocator sender, Windows.Devices.Geolocation.PositionChangedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, DrawElements);
        }

        public Map GetMapObject()
        {
            return BingMap;
        }

        public void DrawElements()
        {
            //Set view
            BingMap.SetView(GodController.CurrentPlayer.Location, 13.0f);
            //Player pin
            Pushpin pin = new Pushpin()
            {
                Text = "Me"
            };
            BingMap.Children.Add(pin);
            MapLayer.SetPosition(pin, GodController.CurrentPlayer.Location);
            //Direction pin

            //Spear (if available)
        }

        private void MapTapEvent(object sender, RoutedEventArgs e)
        {
            if (BingMap.Children.Count > 1)
            {
                //Bigger then one, First child is the player.
                BingMap.Children.RemoveAt(1);
            }
            
        }
    }
}
