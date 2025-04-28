using ModBusTPU.Models.INotifyPropertyBased;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusTPU.Models.Mercucry
{
    public class MerucryPort : INotifyBase
    {
        //TODO: прописать класс-обертку порта для работы с Mercury

        private SerialPort _port;

        #region Public Attributes
        public string PortName
        {
            get => _port.PortName;
            set
            {
                _port.PortName = value == null ? "COM1" : value;
                OnPropertyChanged();
            }
        }

        public int PortSpeed
        {
            get => _port.BaudRate;
            private set
            {
                _port.BaudRate = value;
                OnPropertyChanged();
            }
        }

        public bool IsPortOpen
        {
            get => _port.IsOpen;
        }
        #endregion
    }
}
