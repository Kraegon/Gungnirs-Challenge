using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using IPR.Model;

namespace IPR.Control
{
    class GodController
    {
        /// <summary>
        /// The current active spear
        /// </summary>
        public static Spear CurrentSpear { get; set; }

        /// <summary>
        /// The current active player
        /// </summary>
        public static Player CurrentPlayer { get; set; }
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
