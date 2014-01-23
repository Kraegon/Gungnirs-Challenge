using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using IPR.Model;
using Bing.Maps;

namespace IPR.Control
{
    /// <summary>
    /// Serves (served?) as focal point of the app.
    /// Would be a lone singleton containing and/or initialising other singleton classes.
    /// Contains data that isn't bound to any specific element but is used by each module.
    /// </summary>
    class SatanController
    {
        public static Player CurrentPlayer { get; set; }
        public static Location DirectionLocation { get; set; }
        private static MapHandler handleMap;
        

        public static MapHandler HandleMap
        {
            get
            {
                if (handleMap == null)
                    handleMap = new MapHandler();
                return handleMap;
            }
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
