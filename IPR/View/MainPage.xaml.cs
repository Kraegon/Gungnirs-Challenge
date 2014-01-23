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
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IPR
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;

        public List<HighscoreObj> DisplayedHighscores { get; set; }

        public HelpFlyout HelpFlyout;

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        public static CoreDispatcher dispatcher;
        public MainPage()
        {
            this.InitializeComponent();
            dispatcher = this.Dispatcher;
            /* Initalises the controllers and adds the map to the maphandler */
            SatanController.HandleMap.SetMap(BingMap);
            SatanController.HandleMap.Initialize();
            SatanController.HandleMap.Locator.PositionChanged += Locator_PositionChanged;
            BingMap.DoubleTappedOverride += BingMap_DoubleTapped;
            /* Initialise the highscores and databinding to it */
            HighscoreInit();
            HighscoreReader.HighscoreUpdatedEvent += HighscoreReader_HighscoreUpdatedEvent;
            /* initializes SpearHandler */
            SpearHandler.SpearLocationUpdateEvent += SpearHandler_SpearLocationUpdateEvent;

            HelpFlyout.Show();

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

        public async void Refresh()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DrawElements();
                if ((SpearHandler.Gungnir != null) && !SpearHandler.Gungnir.Available)
                    YourDistanceBlock.Text = SpearHandler.Distance.ToString(); //TODO: Turn into distance thrown.
                if (SpearHandler.State == GameState.Retrieving)
                    YourTimeBlock.Text = TimeSpan.Parse("10:00").ToString(); //TODO: Turn into time taken so far.
                if ((SpearHandler.Gungnir == null) || SpearHandler.Gungnir.Available)
                {
                    SpearAvailableBlock.Text = "Available";
                    SpearAvailableBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else
                {

                    SpearAvailableBlock.Text = "Not available";
                    SpearAvailableBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                //geofence replica
                if ((SatanController.CurrentPlayer != null && SpearHandler.Gungnir != null) && SpearHandler.Gungnir.Location != null)
                    AntiGeofencing(SatanController.CurrentPlayer.Location, SpearHandler.Gungnir.Location);
                //if (Condition for button)
                SaveButton.IsEnabled = false;
                DrawElements();
            });
        }

        private void BingMap_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Refresh();
        }

        private void Locator_PositionChanged(Windows.Devices.Geolocation.Geolocator sender, Windows.Devices.Geolocation.PositionChangedEventArgs args)
        {
            Refresh();
        }

        public Map GetMapObject()
        {
            return BingMap;
        }

        public void DrawElements()
        {
            //Clear pins
            SatanController.HandleMap.ClearMap();
            //Player pin
            if (SatanController.CurrentPlayer != null)
            {
                Pushpin pin = new Pushpin()
                {
                    Text = "Me"
                };
                BingMap.Children.Add(pin);
                MapLayer.SetPosition(pin, SatanController.CurrentPlayer.Location);
                //Set view
                if (BingMap.ZoomLevel < 15.0f)
                    BingMap.SetView(SatanController.CurrentPlayer.Location, 15.0f);
                else
                    BingMap.SetView(SatanController.CurrentPlayer.Location, BingMap.ZoomLevel);
            }
            //Direction pin
            if (SatanController.DirectionLocation != null)
            {
                Pushpin pin = new Pushpin
                {
                    Name = "Direction_Pin"
                };
                pin.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 255));
                BingMap.Children.Add(pin);
                MapLayer.SetPosition(pin, SatanController.DirectionLocation);
            }
            //Spear (if available)
            if ((SpearHandler.Gungnir != null) && SpearHandler.Gungnir.Location != null)
            {
                Pushpin pin = new Pushpin()
                {
                    Text = "Spear"
                };
                pin.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255,125,125,0));
                BingMap.Children.Add(pin);
                MapLayer.SetPosition(pin, SpearHandler.Gungnir.Location);
                //Set view
                if (BingMap.ZoomLevel < 15.0f)
                    BingMap.SetView(SpearHandler.Gungnir.Location, 15.0f);
                else
                    BingMap.SetView(SpearHandler.Gungnir.Location, BingMap.ZoomLevel);
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

        /// <summary>
        /// Fucking square collision check insted of geofences, because Windows.Devices.Geolocation.Geofencing is horrible!
        /// </summary>
        /// <param name="playerLocation"></param>
        /// <param name="fenceLocation"></param>
        private async void AntiGeofencing(Location playerLocation, Location fenceLocation)
        {
            if ((playerLocation.Latitude < fenceLocation.Latitude + 0.0005 
                && playerLocation.Latitude > fenceLocation.Latitude - 0.0005 
                && playerLocation.Longitude < fenceLocation.Longitude + 0.0005 
                && playerLocation.Longitude > fenceLocation.Longitude - 0.0005) 
                && !SpearHandler.Gungnir.Available)
            {
                System.Diagnostics.Debug.WriteLine("Entered the geofence event and state.Entered");
                await SatanController.ShowMessageAsync("Gungnir", "Has been picked up!");
                SpearHandler.Gungnir.Available = true;

                //TODO: Remove the pushpin for the spear!
            }
        }
    }
}
