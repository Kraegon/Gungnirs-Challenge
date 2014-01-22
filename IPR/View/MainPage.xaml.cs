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
            /* initializes SpearHandler */
            SpearHandler.SpearLocationUpdateEvent += SpearHandler_SpearLocationUpdateEvent;

        }

        void SpearHandler_SpearLocationUpdateEvent()
        {
            DrawElements();
        }

        private async void HighscoreReader_HighscoreUpdatedEvent()
        {
            DisplayedHighscores = await HighscoreReader.SortHighestScoreFirst(await HighscoreReader.GetHighscoresAsync());
            HighscoreBox.ItemsSource = null;
            HighscoreBox.ItemsSource = DisplayedHighscores;
        }
        private async void HighscoreInit()
        {
            DisplayedHighscores = await HighscoreReader.SortHighestScoreFirst(await HighscoreReader.GetHighscoresAsync());
            HighscoreBox.ItemTemplate = Resources["HighscoreTemplate"] as DataTemplate;
            HighscoreBox.ItemsSource = DisplayedHighscores;
        }

        public async void RefreshScore()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DrawElements();
                if ((SpearHandler.Gungnir != null) && !SpearHandler.Gungnir.Available)
                    YourDistanceBlock.Text = String.Empty + 10.0; //TODO: Turn into distance thrown.
                if (SpearHandler.State == GameState.Retrieving)
                    YourTimeBlock.Text = TimeSpan.Parse("10:00").ToString(); //TODO: Turn into time taken so far.
                //if (Condition for button)
                SaveButton.IsEnabled = false;
                DrawElements();
            });
        }

        private void BingMap_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            RefreshScore();
        }

        private void Locator_PositionChanged(Windows.Devices.Geolocation.Geolocator sender, Windows.Devices.Geolocation.PositionChangedEventArgs args)
        {
            RefreshScore();
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
            if (GodController.DirectionLocation != null)
            {
                Pushpin pin = new Pushpin
                {
                    Name = "Direction_Pin"
                };
                pin.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 255));
                BingMap.Children.Add(pin);
                MapLayer.SetPosition(pin, GodController.DirectionLocation);
            }
            //Spear (if available)
            if ((SpearHandler.Gungnir != null) && SpearHandler.Gungnir.Available && SpearHandler.Gungnir.Location != null)
            {
                Pushpin pin = new Pushpin()
                {
                    Text = "Spear"
                };
                pin.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255,125,125,0));
                BingMap.Children.Add(pin);
                MapLayer.SetPosition(pin, SpearHandler.Gungnir.Location);
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await HighscoreReader.SaveHighscoreObj(new HighscoreObj(NameTextBox.Text,
                                                                    float.Parse(YourDistanceBlock.Text),
                                                                    TimeSpan.Parse(YourTimeBlock.Text)));
        }
    }
}
