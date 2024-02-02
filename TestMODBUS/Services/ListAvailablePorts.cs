using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMODBUS.Models.INotifyPropertyBased;

namespace TestMODBUS.Models.Services
{
    //Дата-класс для хранения и обновления списка доступных портов
    static class ListAvailablePorts
    {
        #region Public Attributes
        public static ObservableCollection<string> AvailablePorts { get; private set; } = new ObservableCollection<string>();

        public const string NoAvaiblePortsString = "Нет доступных";

        public static bool IsAnyPortAvailable => AvailablePorts[0] != NoAvaiblePortsString;
        #endregion

        public static void UpdateAvailablePortList()
        {
            AvailablePorts.Clear();
            
            var serialPorts = SerialPort.GetPortNames();

            if (serialPorts.Length == 0)
            {
                AvailablePorts.Add(NoAvaiblePortsString);
                return;
            }

            foreach(var serialPort in serialPorts)
            {
                AvailablePorts.Add(serialPort);
            }
        }
        
        public static string GetFirstAvailablePort()
        {
            if (AvailablePorts == null)
                return null;

            if(AvailablePorts.Count == 0)
                return null;

            if (AvailablePorts[0] ==  NoAvaiblePortsString) 
                return null;

            return AvailablePorts[0];
        }
    }
}
