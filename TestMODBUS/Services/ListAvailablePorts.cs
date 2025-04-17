using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModBusTPU.Models.INotifyPropertyBased;

namespace ModBusTPU.Models.Services
{
    //Дата-класс для хранения и обновления списка доступных портов
    public class ListAvailablePorts
    {
        #region Public Attributes
        public ObservableCollection<string> AvailablePorts { get; private set; } = new ObservableCollection<string>();

        public const string NoAvaiblePortsString = "Нет доступных";

        public bool IsAnyPortAvailable => AvailablePorts[0] != NoAvaiblePortsString;
        #endregion

        public void UpdateAvailablePortList()
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
            var serialPorts = SerialPort.GetPortNames();

            if (serialPorts == null)
                return null;

            if(serialPorts.Length == 0)
                return null;

            if (serialPorts[0] ==  NoAvaiblePortsString) 
                return null;

            return serialPorts[0];
        }
    }
}
