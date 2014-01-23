using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage.Streams;
using System.ComponentModel;

namespace IPR.Control.WiiMoteControl
{
    public class WiiMoteManager
    {
        private static WiiMoteManager instance;

        public static WiiMoteManager INSTANCE
        {
            get {if(instance == null) 
                    instance = new WiiMoteManager();
                return instance;}
        }
        
        private WiiMote[] wiimotes { get; set; }
        
        /// <summary>
        /// All wiimote devices connected to this computer.
        /// Only the first is actively interfaced with.
        /// </summary>
        public WiiMote[] Wiimotes{ get; set; }
        /// <summary>
        /// Attempts connection to all wiimotes connected via bluetooth.
        /// </summary>
        /// <returns>True if more than 1 wiimote was connected</returns>
        public async Task<bool> ConnectionInitAsync()
        {
            if (Wiimotes != null)
                return true;
            string moteName = Windows.Devices.HumanInterfaceDevice.HidDevice.GetDeviceSelector(0x01, 0x05);
            DeviceInformationCollection moteContainer = await DeviceInformation.FindAllAsync(moteName);
            await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                Wiimotes = new WiiMote[moteContainer.Count];
                for (int i = 0; i < moteContainer.Count; i++ )
                {
                    HidDevice wiimote = await HidDevice.FromIdAsync(moteContainer[i].Id, Windows.Storage.FileAccessMode.ReadWrite);
                    if (wiimote != null)
                    {
                        Wiimotes[i] = new WiiMote(wiimote);
                    }
                }
            });
            if (Wiimotes.Length == 0)
                return false;
            foreach (WiiMote w in Wiimotes)
                if (w == null)
                    return false;
            return true;
        }

        public void CloseWiiMotes()
        {
            foreach (WiiMote w in Wiimotes)
            {
                w.Close();  
            }
            Wiimotes = null;
        }
    }
}
