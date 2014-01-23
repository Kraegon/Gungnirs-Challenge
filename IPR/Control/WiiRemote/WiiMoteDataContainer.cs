using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage.Streams;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;

namespace IPR.Control.WiiMoteControl
{
    /// <summary>
    /// WiiMote data publically reporting its status. 
    /// </summary>
    public class WiiMote
    {
        private HidDevice device;
        public bool isAheld { get; set; }
        public bool isBheld { get; set; }
        public bool IsAheld
        {
            get { return isAheld; }
            set
            {
                if (isAheld == value) return;
                isAheld = value;
            }
        }
        public bool IsBheld
        {
            get { return isBheld; }
            set
            {
                if (isBheld == value) return;
                isBheld = value;
            }
        }
        
        //Wiimote's conventional start message
        private ushort prefix = 0xA2;
        public WiiMote(HidDevice device)
        {
            this.device = device;
            device.InputReportReceived += InputEvent;
        }

        /// <summary>
        /// InputEvents received automatically from the wiiremote.
        /// Conventionally starts with 0xA1 followed by the ID byte and then the specifics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void InputEvent(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            ParseInput(args.Report.Data);         
        }

        public void ParseInput(IBuffer data)
        {
            DataReader reader = DataReader.FromBuffer(data);
            var reportID = reader.ReadByte();
            var a = reader.ReadByte();
            var b = reader.ReadByte();
            var c = reader.ReadByte();
            byte[] dataBuffer = new byte[] { reportID, a, b, c };
            string action = "";
            switch(dataBuffer[0])
            {
                case 0x30:
                    action = "Button pressed: ";
                    switch (dataBuffer[2])
                    {
                        case 0x00:
                            action = "Button released";
                            if (IsBheld)
                            {
                                if ((SpearHandler.State == GameState.Idle) && (SatanController.CurrentPlayer != null) && (SatanController.DirectionLocation != null))
                                    SpearHandler.StartThrow();
                            }
                            IsAheld = false;
                            IsBheld = false;
                            break;
                        case 0x04:
                            if (IsBheld && !IsAheld)
                                return;
                            action += "B";
                            IsBheld = true;
                            break;
                        case 0x08:
                            if (IsAheld && !IsBheld)
                                return;
                            action += "A";
                            IsAheld = true;
                            break;
                        case 0x0C:
                            action += "A and B";
                            IsBheld = true;
                            IsAheld = true;
                            break;
                    }
                    break;
            }
            System.Diagnostics.Debug.WriteLine(action);
        }

        public void Close()
        {
            device.Dispose();
            device = null;
        }
        /// <summary>
        /// Broken. Due to an unknown glitch in the output report's data cannot operate at all.
        /// </summary>
        /// <param name="reportID">One of the Wiimotes known reportID identifiers</param>
        /// <param name="commands">A list of 1 or more command parameters</param>
        /// <returns>Succesful or not</returns>
        public async Task<bool> WriteWiiMoteAsync(WiiMoteReportID reportID, WiiMoteCommand[] commands)
        {
            //Create report
            HidOutputReport outReport = device.CreateOutputReport();
            //Create data
            var writer = new DataWriter();
            writer.WriteByte((byte)prefix);
            writer.WriteByte((byte) reportID);
            for (int i = 0; i < 20 - commands.Length; i++)
            {
                writer.WriteByte(0);
            }
            foreach (WiiMoteCommand c in commands)
            {
                writer.WriteByte((byte) c);            
            }
            outReport.Data = writer.DetachBuffer(); 
            //Send data
            try {
                uint result = await device.SendOutputReportAsync(outReport);            
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to send report");
                return false;
            }
            return true;
        }
    }
    public enum WiiMoteCommand : ushort
    {
        LED1 = 0x10,
        LED2 = 0x20
    }
    public enum WiiMoteReportID : ushort
    {
        LEDid = 0x11
    }
}
