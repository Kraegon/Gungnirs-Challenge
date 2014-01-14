using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace IPR.Control
{
    class GodController
    {
        private static MapHandler HandleMap;
        private static LocationService ServiceLocation;

        /// <summary>
        /// Initializes the project
        /// </summary>
        public static void Initialize()
        {
            HandleMap = new MapHandler();
        }

        public static MapHandler GetMapHandler()
        {
            return HandleMap;
        }

        public static async void ShowMessage(string title, string description)
        {
            var MessageDialog = new MessageDialog(description, title);
            await MessageDialog.ShowAsync();
        }

        public static async Task ShowMessageAsync(string title, string description)
        {
            var MessageDialog = new MessageDialog(description, title);
            await MessageDialog.ShowAsync();
        }
    }
}
