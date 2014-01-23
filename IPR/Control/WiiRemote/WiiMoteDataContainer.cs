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
    public class WiiMote : INotifyPropertyChanged
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
                OnPropertyChanged("IsAheld");
            }
        }
        public bool IsBheld
        {
            get { return isBheld; }
            set
            {
                if (isBheld == value) return;
                isBheld = value;
                OnPropertyChanged("IsBheld");
            }
        }
        

        private ushort prefix = 0xA2;
        public WiiMote(HidDevice device)
        {
            this.device = device;
            device.InputReportReceived += InputEvent;
        }

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

        public async Task<bool> WriteWiiMoteAsync(WiiMoteReportID reportID, WiiMoteCommand[] commands)
        {
            /*
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
            }*/
            for (int i = 0; i < 3; i++)
            {
                //Create report
                HidOutputReport outReport = device.CreateOutputReport();
                //Create data
                var writer = new DataWriter();
                for (int j = 0; j < i; j++)
                {
                    writer.WriteByte(1);
                }
                try
                {
                    outReport.Data = writer.DetachBuffer();    
                    //Send data
                    uint result = await device.SendOutputReportAsync(outReport);

                    return true;
                }
                catch (ArgumentException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.ParamName);
                    System.Diagnostics.Debug.WriteLine(i + " failed to send report");
                }
                await Task.Delay(10);
            }
            return true;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
