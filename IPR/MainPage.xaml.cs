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
using Windows.UI.Core;
using IPR.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IPR
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;

        public List<HighscoreObj> DisplayedHighscores { get; set; }

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
            GodController.HandleMap.SetMap(BingMap);
            GodController.HandleMap.Initialize();
            GodController.HandleMap.Locator.PositionChanged += Locator_PositionChanged;
            BingMap.DoubleTappedOverride += BingMap_DoubleTapped;
            /* Initialise the highscores and databinding to it */
            HighscoreInit();
            HighscoreReader.HighscoreUpdatedEvent += HighscoreReader_HighscoreUpdatedEvent;
        }

        private async void HighscoreReader_HighscoreUpdatedEvent()
        {
            DisplayedHighscores = HighscoreReader.SortHighestScoreFirst(await HighscoreReader.GetHighscoresAsync());
            HighscoreBox.ItemsSource = null;
            HighscoreBox.ItemsSource = DisplayedHighscores;
        }
        private async void HighscoreInit()
        {
            DisplayedHighscores = HighscoreReader.SortHighestScoreFirst(await HighscoreReader.GetHighscoresAsync());
            HighscoreBox.ItemTemplate = Resources["HighscoreTemplate"] as DataTemplate;
            HighscoreBox.ItemsSource = DisplayedHighscores;
        }

        private async void BingMap_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, DrawElements);
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
            //Clear pins
            GodController.HandleMap.ClearMap();
            //Set view
            if(BingMap.ZoomLevel < 13.0f)
                BingMap.SetView(GodController.CurrentPlayer.Location, 13.0f);
            else
                BingMap.SetView(GodController.CurrentPlayer.Location, BingMap.ZoomLevel);
            //Player pin
            if (GodController.CurrentPlayer != null)
            {
                Pushpin pin = new Pushpin()
                {
                    Text = "Me"
                };
                BingMap.Children.Add(pin);
                MapLayer.SetPosition(pin, GodController.CurrentPlayer.Location);
            }
            //Direction pin
            if (GodController.HandleMap.DirectionLocation != null)
            {
                Pushpin pin = new Pushpin
                {
                    Name = "Direction_Pin"
                };
                BingMap.Children.Add(pin);
                MapLayer.SetPosition(pin, GodController.HandleMap.DirectionLocation);
            }
            //Spear (if available)
            if ((GodController.CurrentSpear != null) && GodController.CurrentSpear.Available)
            {
                Pushpin pin = new Pushpin()
                {
                    Text = "Spear"
                };
                BingMap.Children.Add(pin);
                MapLayer.SetPosition(pin, GodController.CurrentSpear.Location);
            }
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
